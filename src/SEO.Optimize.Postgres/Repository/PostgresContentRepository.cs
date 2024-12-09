using Microsoft.EntityFrameworkCore;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Core.Models.Jobs;
using SEO.Optimize.Core.Models.Opportunities;
using SEO.Optimize.Core.Models.Tokens;
using SEO.Optimize.Core.Models.Utils;
using SEO.Optimize.Core.Models.Webflow;
using SEO.Optimize.Postgres.Context;
using SEO.Optimize.Postgres.Model;
using PageInfo = SEO.Optimize.Postgres.Model.PageInfo;

namespace SEO.Optimize.Postgres.Repository
{
    public class PostgresContentRepository : IContentRepository
    {
        private readonly DataContext dbContext;

        public PostgresContentRepository(DataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateUserAsync(UserAuthInfo userAuthInfo)
        {
            var userInfo = new User()
            {
                ExternalId = userAuthInfo.ExternalIdentity,
                Provider = userAuthInfo.Provider,
                Email = userAuthInfo.MailId,
                PasswordHash = string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ProfileImageUrl = userAuthInfo.ProfileImageUrl,
                UserName = userAuthInfo.Name,
            };

            await dbContext.Users.AddAsync(userInfo);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<DetailedSiteInfo>> GetGeneralSiteDetailsByUserAsync(string userId)
        {
            var user = await GetUserByMailId(userId);
            if (user == null)
            {
                return Enumerable.Empty<DetailedSiteInfo>();
            }

            var res = dbContext.Sites
                .Where(o => o.UserId == user.Id)
                .Select(o => new DetailedSiteInfo()
                {
                    UserId = user.Id,
                    SiteId = o.SiteId,
                    SiteName = o.SiteName,
                    ExternalSiteId = o.ExternalSiteId,
                    SiteUrl = o.Url,
                    TotalExternalLinks = o.CrawlInfos.Any() ? o.CrawlInfos.OrderByDescending(o => o.CrawlDate).FirstOrDefault().TotalExternalLinks : 0,
                    TotalInternalLinks = o.CrawlInfos.Any() ? o.CrawlInfos.OrderByDescending(o => o.CrawlDate).FirstOrDefault().TotalInternalLinks : 0,
                    TotalOpportunities = o.CrawlInfos.Any() ? o.CrawlInfos.OrderByDescending(o => o.CrawlDate).FirstOrDefault().TotalOpportunities : 0,
                    TotalPages = o.CrawlInfos.Any() ? o.CrawlInfos.OrderByDescending(o => o.CrawlDate).FirstOrDefault().TotalPages : 0,
                });

            return await res.ToListAsync();
        }

        public async Task<UserAuthInfo?> GetUserByMailId(string mailId)
        {
            var userInfo = await dbContext.Users.FirstOrDefaultAsync(o => o.Email.Equals(mailId));
            if (userInfo == null)
            {
                return null;
            }

            return new UserAuthInfo()
            {
                Id = userInfo.UserId,
                Name = userInfo.UserName,
                ExternalIdentity = userInfo.ExternalId,
                ProfileImageUrl = userInfo.ProfileImageUrl,
                Provider = userInfo.Provider,
                MailId = userInfo.Email,
            };
        }

        public async Task<int> IntegrateNewSite(SiteIntegrationInfo siteIntegrationInfo)
        {
            var siteExists = await dbContext.Sites.FirstOrDefaultAsync(o => o.ExternalSiteId == siteIntegrationInfo.ExternalSiteId);
            if(siteExists != null)
            {
                return siteExists.SiteId;
            }

            await dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(o => o.Email == siteIntegrationInfo.UserMailId);
                if (user == null)
                {
                    throw new Exception("Invalid Exception");
                }

                var site = new Site()
                {
                    SiteName = siteIntegrationInfo.SiteName,
                    ExternalSiteId = siteIntegrationInfo.ExternalSiteId,
                    Url = siteIntegrationInfo.SiteUrl,
                    UserId = user.UserId,
                    CreatedOn = DateTime.UtcNow,
                    LastCrawledOn = null,
                };

                await dbContext.Sites.AddAsync(site);
                await dbContext.SaveChangesAsync();

                var token = new Token()
                {
                    TokenType = "access_token",
                    TokenValue = siteIntegrationInfo.AccessToken,
                    TokenExpiresAt = siteIntegrationInfo.AccessTokenExpiry,
                    SiteId = site.SiteId,
                };

                await dbContext.Tokens.AddAsync(token);
                await dbContext.SaveChangesAsync();

                await dbContext.Database.CommitTransactionAsync();
                return site.SiteId;
            }
            catch (Exception ex)
            {
                await dbContext.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task StoreTokens(List<TokenInfo> tokens)
        {
            foreach (var token in tokens)
            {
                var tokenExisting = dbContext.Tokens.FirstOrDefault(t => t.SiteId == token.SiteId && t.TokenType == token.TokenType);
                if(tokenExisting != null)
                {
                    tokenExisting.SiteId = token.SiteId;
                    tokenExisting.TokenType = token.TokenType;
                    tokenExisting.TokenValue = token.Token;
                    tokenExisting.TokenExpiresAt = token.Expiry;
                    tokenExisting.UpdatedOn = DateTime.UtcNow;
                }
                else
                {
                    await dbContext.Tokens.AddAsync(new Token()
                    {
                        TokenValue = token.Token,
                        TokenType = token.TokenType,
                        TokenExpiresAt = token.Expiry,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        SiteId = token.SiteId,
                    });
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TokenInfo>> GetTokensBySiteId(int siteId)
        {
            var tokens = await dbContext.Tokens.Where(o => o.SiteId == siteId).ToListAsync();
            return tokens.Select(o => new TokenInfo(o.TokenValue, o.TokenType, o.SiteId, o.TokenExpiresAt));
        }

        public async Task ScheduleCrawlJob(int userId, int siteId)
        {
            var job = new Job()
            {
                Name = "CrawlSite",
                UserId = userId,
                SiteId = siteId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CompletedOn = null,
                JobProperties = string.Empty,
                Status = "InQueue"
            };

            await dbContext.Jobs.AddAsync(job);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<JobInfo>> GetAllCrawlJobs()
        {
            var jobs = await dbContext.Jobs
                .Where(o => o.CompletedOn == null && o.UpdatedOn < DateTime.UtcNow.AddMinutes(-5))
                .OrderBy(o => o.Id).Take(10).ToListAsync()
                ;

            return jobs.Select( o =>
            {
                return new JobInfo()
                {
                    Name = o.Name,
                    UserId = o.UserId,
                    SiteId = o.SiteId,
                    CreatedOn = o.CreatedOn,
                    UpdatedOn = o.UpdatedOn,
                    CompletedOn = o.CompletedOn,
                    JobProperties = o.JobProperties,
                    Id = o.Id,
                    Status = o.Status,
                };
            });
        }

        public async Task<SiteInfo> GetSiteByIdAsync(int siteId)
        {
            var site = await dbContext.Sites.FirstOrDefaultAsync(o => o.SiteId == siteId);

            return new SiteInfo()
            {
                SiteId = site.SiteId,
                SiteName = site.SiteName,
                ExternalSiteId = site.ExternalSiteId,
                CreatedOn = site.CreatedOn,
                Url = site.Url,
                LastCrawledOn = site.LastCrawledOn,
                UserId = site.UserId,
            };
        }

        public async Task AddCrawlData(int userId, int siteId, Dictionary<CollectionItem, CrawlPageOutput> pageCrawlDataMapping)
        {
            var crawlInfo = new CrawlInfo()
            {
                SiteId = siteId,
                TotalPages = pageCrawlDataMapping.Count,
                TotalExternalLinks = pageCrawlDataMapping.Values.Sum(o => o.ExistingLinks.Count), // ext and inter
                TotalInternalLinks = pageCrawlDataMapping.Values.Sum(o => o.ExistingLinks.Count), // ext and inter
                CrawlDate = DateTime.UtcNow,
                TotalOpportunities = pageCrawlDataMapping.Values.Sum(o => o.LinkOpportunities.Count),
            };

            dbContext.CrawlInfos.Add(crawlInfo);
            await dbContext.SaveChangesAsync();

            foreach ((var pageItem, var crawlOutPut) in pageCrawlDataMapping)
            {
                var existing = dbContext.PageInfos.FirstOrDefault(o => o.ExternalId == pageItem.Id);
                PageInfo pageInfo;
                if (existing != null)
                {
                    pageInfo = new PageInfo();
                    pageInfo.SiteId = siteId;
                    pageInfo.Title = existing.Title;
                    pageInfo.Url = existing.Url;
                    pageInfo.CrawlId = crawlInfo.CrawlId;
                    pageInfo.ExternalId = $"{pageItem.CollectionId}##{pageItem.Id}";
                }
                else
                {
                    pageInfo = new PageInfo()
                    {
                        SiteId = siteId,
                        Title = pageItem.FieldData["name"].ToString(),
                        Url = pageItem.FieldData["slug"].ToString(),
                        CrawlId = crawlInfo.CrawlId,
                        ExternalId = $"{pageItem.CollectionId}##{pageItem.Id}"
                    };
                }

                await dbContext.PageInfos.AddAsync(pageInfo);
                await dbContext.SaveChangesAsync();

                var existingLinks = new List<LinkDetail>();
                foreach (var exLink in crawlOutPut.ExistingLinks)
                {
                    var detail = new LinkDetail()
                    {
                        AnchorTextContainingText = exLink.AnchorTextContainingLine,
                        AnchorText = exLink.AnchorText,
                        LinkUrl = exLink.TargetUrl,
                        LinkType = "INTERNAL",
                        IsOpportunity = false,
                        PageId = pageInfo.PageId,
                        NodeXPath = exLink.NodeXPath,
                        FieldKey = exLink.FieldKey,
                    };
                    existingLinks.Add(detail);
                }
                
                var opportunities = new List<LinkDetail>();
                foreach (var link in crawlOutPut.LinkOpportunities)
                {
                    //var st = o.IndexOf(link.Key.Item1) >= 0 ? o.IndexOf(link.Key.Item1) : 0;
                    //var etLength = o.IndexOf(link.Key.Item1) >= 0 ? o.IndexOf(link.Key.Item1) + link.Key.Item1.Length + 50 : o.Length;
                    var value = new LinkDetail()
                    {
                        AnchorTextContainingText = link.AnchorTextContainingLine,
                        AnchorText = link.AnchorText,
                        LinkType = "INTERNAL",
                        IsOpportunity = true,
                        PageId = pageInfo.PageId,
                        LinkUrl = link.TargetUrl ?? string.Empty,
                        NodeXPath = link.NodeXPath,
                        FieldKey = link.FieldKey
                    };

                    opportunities.Add(value);
                }

                await dbContext.LinkDetails.AddRangeAsync(existingLinks);
                await dbContext.LinkDetails.AddRangeAsync(opportunities);

                var stats = new PageStats()
                {
                    PageId = pageInfo.PageId,
                    ExternalLinkCount = existingLinks.Count(),
                    InternalLinkCount = existingLinks.Count(),
                    LinkOpportunityCount = opportunities.Count(),
                };

                await dbContext.PageStats.AddAsync(stats);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateJobAsync(int jobId, string? status = null)
        {
            var job = await dbContext.Jobs.FirstOrDefaultAsync(o => o.Id == jobId);
            if (job == null)
            {
                throw new Exception("invalid job id");
            }

            job.UpdatedOn = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(status))
            {
                job.Status = status;
                if(status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    job.CompletedOn = DateTime.UtcNow;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<SEO.Optimize.Core.Models.PageInfo>> GetPages(PageSearchParams searchParams)
        {
            var res = dbContext.PageInfos.AsQueryable();
            if(searchParams.SiteId.HasValue)
            {
                res = res.Where(o => o.SiteId == searchParams.SiteId);
            }

            if(searchParams.Title != null)
            {
                res = res.Where(o => o.Title == searchParams.Title);
            }

            if(searchParams.LinkOpportunityCountGreaterThan != null)
            {
                res = res.Where(o => o.PageStats.LinkOpportunityCount >  searchParams.LinkOpportunityCountGreaterThan);
            }

            if (searchParams.Offset.HasValue)
            {
                res = res.Skip(searchParams.Offset.Value * 10);
            }

            var pages = await res.Take(100).Include(o => o.PageStats).ToListAsync();

            var resultPageList = pages.Select(o =>
            {
                return new SEO.Optimize.Core.Models.PageInfo()
                {
                    Id = o.PageId,
                    ExternalId = o.ExternalId,
                    Title = o.Title,
                    Url = o.Url,
                    Stats = o.PageStats == null ? null : new PageStatistics()
                    {
                        ExternalLinkCount = o.PageStats.ExternalLinkCount,
                        InternalLinkCount = o.PageStats.InternalLinkCount,
                        LinkOpportunityCount = o.PageStats.LinkOpportunityCount,
                        PageId = o.PageStats.PageId,
                        StatsId = o.PageStats.StatsId,
                    }
                };
            }).ToList();

            return resultPageList;
        }

        public async Task<IEnumerable<Opportunities>> GetOpportunities(LinkSearchParams linkSearchParams)
        {
            var res = dbContext.LinkDetails.AsQueryable();

            res = res.Where(o => o.IsOpportunity && o.LinkType == "INTERNAL");

            if (linkSearchParams.SiteId.HasValue)
            {
                res = res.Where(o => o.PageInfo.SiteId == linkSearchParams.SiteId);
            }
            
            if (linkSearchParams.PageId.HasValue)
            {
                res = res.Where(o => o.PageId == linkSearchParams.PageId);
            }

            if (linkSearchParams.AnchorText != null)
            {
                res = res.Where(o => o.AnchorText == linkSearchParams.AnchorText);
            }

            if (linkSearchParams.Offset.HasValue)
            {
                res = res.Skip(linkSearchParams.Offset.Value * 10);
            }

            return await res.Select(o => 
            new Opportunities()
            {
                AnchorText = o.AnchorText,
                LinkId = o.LinkId,
                PageId = o.PageId,
                TargetLinkUrl = o.LinkUrl,
                PageUrl = o.PageInfo.Url,
                AnchorTextContainingText = o.AnchorTextContainingText,
                SiteId = o.PageInfo.SiteId,
                ExternalPageId = o.PageInfo.ExternalId
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<Opportunities>> GetOpportunitiesByIds(IEnumerable<int> ids)
        {
            var links = dbContext.LinkDetails.Where(o => ids.Contains(o.LinkId));
            return await links.Select(o =>
            new Opportunities()
            {
                AnchorText = o.AnchorText,
                LinkId = o.LinkId,
                PageId = o.PageId,
                TargetLinkUrl = o.LinkUrl,
                PageUrl = o.PageInfo.Url,
                AnchorTextContainingText = o.AnchorTextContainingText,
                SiteId = o.PageInfo.SiteId,
                ExternalPageId = o.PageInfo.ExternalId,
                FieldKey = o.FieldKey,
                IsOpportunity = true,
            })
            .ToListAsync();
        }

        public async Task ScheduleCrawlForPage(JobInfo jobInfo)
        {
            var job = new Job()
            {
                Name = jobInfo.Name,
                UserId = jobInfo.UserId,
                SiteId = jobInfo.SiteId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CompletedOn = null,
                JobProperties = jobInfo.JobProperties,
                Status = jobInfo.Status
            };

            await dbContext.Jobs.AddAsync(job);
            await dbContext.SaveChangesAsync();
        }
    }
}
