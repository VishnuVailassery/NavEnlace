using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SEO.Optimize.Core.Interfaces;

namespace SEO.Optimize.Jobs.Workers
{
    public class CrawlWorker : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private const int MaximumDelayInSeconds = 3600;

        public CrawlWorker(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var runningTasks = new List<(Task, IServiceScope)>();
            TimeSpan delayBeforeNextPoll = TimeSpan.FromSeconds(10);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var workerSCope = serviceProvider.CreateScope())
                    {
                        var contentRepository = workerSCope.ServiceProvider.GetRequiredService<IContentRepository>();
                        var jobs = await contentRepository.GetAllCrawlJobs();
                        foreach (var job in jobs)
                        {
                            var scope = serviceProvider.CreateScope();
                            var handler = scope.ServiceProvider.GetRequiredService<ICrawlJobHandler>();
                            runningTasks.Add((handler.ProcessJobAsync(job), scope));
                        }
                    }

                    delayBeforeNextPoll = TimeSpan.FromSeconds(10);
                }
                catch (Exception ex)
                {
                    delayBeforeNextPoll *= 2;
                    if (delayBeforeNextPoll.TotalSeconds > MaximumDelayInSeconds)
                    {
                        delayBeforeNextPoll = TimeSpan.FromSeconds(MaximumDelayInSeconds);
                    }
                }

                try
                {
                    await Task.Delay(delayBeforeNextPoll, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Cancellation requested
                    break;
                }
            }
        }
    }
}
