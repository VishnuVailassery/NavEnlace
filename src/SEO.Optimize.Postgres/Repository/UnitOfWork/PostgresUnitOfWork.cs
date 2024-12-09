using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Postgres.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Postgres.Repository.UnitOfWork
{
    public class PostgresUnitOfWork : IDataUnitOfWork
    {
        private readonly DataContext dataContext;

        public PostgresUnitOfWork(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await dataContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await dataContext.Database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            await dataContext.Database.RollbackTransactionAsync(cancellationToken);
        }
    }
}
