namespace WeStop.Infra.Repositories
{
    public abstract class RepositoryBase
    {
        protected WeStopDbContext _db;

        public RepositoryBase(WeStopDbContext db)
        {
            _db = db;
        }
    }
}
