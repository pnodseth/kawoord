

namespace Backend;

public interface IRepository<T>
{
    public Task Add(T entity);

    public Task Update(T entity);

}



