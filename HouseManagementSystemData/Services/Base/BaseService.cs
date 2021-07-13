using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HMS.Data.Services.Base
{
    public interface IBaseService<T> where T : class
    {
        int Count();
        T Get<TKey>(TKey id);
        Task<T> GetAsyn<TKey>(TKey id);
        IQueryable<T> Get();
        T FirstOrDefault();
        Task<T> FirstOrDefaultAsyn();
        void Create(T entity);
        Task CreateAsyn(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsyn(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
    }
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected DbContext _dbContext;
        protected IBaseRepository<T> _repository;

        public BaseService(DbContext dbContext, IBaseRepository<T> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public int Count()
        {
            return _repository.Count();
        }

        public void Create(T entity)
        {
            _repository.Create(entity);
            Save();
        }

        public async Task CreateAsyn(T entity)
        {
            await _repository.CreateAsyn(entity);
            await SaveAsyn();
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
            Save();
        }

        public T FirstOrDefault()
        {
            return _repository.FirstOrDefault();
        }

        public async Task<T> FirstOrDefaultAsyn()
        {
            return await _repository.FirstOrDefaultAsyn();
        }

        public T Get<TKey>(TKey id)
        {
            return _repository.Get(id);
        }

        public IQueryable<T> Get()
        {
            return _repository.Get();
        }

        public async Task<T> GetAsyn<TKey>(TKey id)
        {
            return await _repository.GetAsyn(id);
        }

        public void Update(T entity)
        {
            _repository.Update(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            Save();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsyn()
        {
            await _dbContext.SaveChangesAsync();
        }
        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _repository.Get(predicate);
        }
        public async Task<T> FirstOrDefaultAsyn(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FirstOrDefaultAsyn(predicate);
        }
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
