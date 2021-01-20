using Microsoft.EntityFrameworkCore;
using SampleUnitTest.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleUnitTest.Web.Repository
{
    public class Repository<T> : IRepository<T> where T:class
    {
        private SampleUnitTestContext _context;
        private DbSet<T> _dbSet;
        public Repository(SampleUnitTestContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task Create(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
