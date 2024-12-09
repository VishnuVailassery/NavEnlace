using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Interfaces
{
    public interface IDataUnitOfWork
    {
        public Task BeginTransactionAsync(CancellationToken cancellationToken);
        public Task CommitTransactionAsync(CancellationToken cancellationToken);
        public Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }
}
