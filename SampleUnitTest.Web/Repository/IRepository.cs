using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleUnitTest.Web.Repository
{
    public interface IRepository<T> where T:class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
