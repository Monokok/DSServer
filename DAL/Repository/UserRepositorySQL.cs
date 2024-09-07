using DomainModel;
using Interfaces.Repository;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class UserRepositorySQL : IRepository<User> //CourseInvite - регистрация студентов на курсы (Паки вождения)
    {
        private DrivingSchoolContext db;

        public UserRepositorySQL(DrivingSchoolContext dbcontext)
        {
            this.db = dbcontext;
        }

        public void CreateAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetItemAsync(int id)
        {
            throw new NotImplementedException();
        }

        public List<User> GetList()
        {
            throw new NotImplementedException();
        }

        public void Update(User item)
        {
            throw new NotImplementedException();
        }

        async Task<User?> IRepository<User>.GetItemAsync(string id)
        {
            var usrs = await db.Users.FindAsync(id);
            return usrs;
        }

        async Task<List<User>> IRepository<User>.GetListAsync()
        {
            return await db.Users.ToListAsync();
        }
    }
}
