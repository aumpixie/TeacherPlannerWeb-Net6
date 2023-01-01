using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;

namespace NetCoreCalendar.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        /**
         * Adds the updated object to the database and returns it
         **/
        public async Task<T> AddAsync(T entity)
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /**
         * Finds the object that has the corresponding id in the database and removes it
         **/
        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if(entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        /**
         * Gets all the objects from the database and stores them as a List
         **/
        public async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        /**
         * Retrieves the object with the corresponding id from the database
         **/
        public async Task<T?> GetAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await context.Set<T>().FindAsync(id);
        }

        /**
         * Checks if the object with the corresponding id exists in teh database
         **/
        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        /**
         * Changes the state of the Object in the database
         **/
        public async Task UpdateAsync(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        /**
         * Adds the List of objects to the database
         **/
        public async Task AddRangeAsync(List<T> entities)
        {
            await context.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }
    }
}
