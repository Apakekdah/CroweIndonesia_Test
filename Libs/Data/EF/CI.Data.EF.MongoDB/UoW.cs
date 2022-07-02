using Hero.Core.Commons;
using MongoDB.Driver;
using CI.Data.EF.MongoDB.Interfaces;
using System;
using System.Threading.Tasks;

namespace CI.Data.EF.MongoDB
{
    class UoW : Disposable, IUnitOfWorkMongo
    {
        protected IDbFactoryES DatabaseFactory { get; set; }

        protected IClientSessionHandle Transaction { get; set; }

        protected bool IsTransaction { get; set; }

        protected IMongoContext DataContext
        { get { return DatabaseFactory.Get(); } }

        public UoW(IDbFactoryES dbFactory)
        {
            DatabaseFactory = dbFactory;
        }

        public async Task Begin()
        {
            if (!DataContext.SupportReplicate()) return;
            else if (IsTransaction) return;

            try
            {
                Transaction = await DataContext.CreateTransaction();

                Transaction.StartTransaction();
                IsTransaction = true;
            }
            catch (Exception)
            {
                // handle
            }
        }

        public Task<int> Commit()
        {
            return Commit(true);
        }

        public async Task<int> Commit(bool postToDb)
        {
            int changes;

            try
            {
                changes = await DataContext.SaveChanges();

                if (postToDb)
                    await CommitTransaction();
                else
                    await Rollback();
            }
            catch (Exception)
            {
                ClearTransaction(true);
                throw;
            }
            finally
            {
                // Belum tau buat apa.
            }

            return changes;
        }

        public async Task Rollback()
        {
            await DataContext.ClearChanges();

            //if (!IsTransaction)
            //    await Task.FromResult(0);

            ClearTransaction(true);
        }

        private async Task CommitTransaction()
        {
            if (!IsTransaction)
                return;

            try
            {
                await Transaction.CommitTransactionAsync();
            }
            finally
            {
                ClearTransaction();
            }
        }

        private void ClearTransaction(bool abort = false)
        {
            if (!IsTransaction) return;

            try
            {
                IsTransaction = false;
                if (Transaction != null)
                {
                    if (Transaction.IsInTransaction)
                    {
                        try
                        {
                            if (abort)
                                Transaction.AbortTransaction();
                        }
                        finally
                        {
                            //
                        }
                    }

                    DataContext.DestroyTransaction();

                    Transaction?.Dispose();
                }
            }
            finally
            {
                Transaction = null;
            }
        }

        protected override void DisposeCore()
        {
            DatabaseFactory.Dispose();
            DataContext.Dispose();
        }
    }
}