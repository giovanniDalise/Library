namespace Library.BookService.Infrastructure.Persistence.Interfaces
{
    public interface IMapper<TEntity, TDomain>
    {
        TEntity ToEntity(TDomain domain);
        TDomain ToDomain(TEntity entity);
    }
}
