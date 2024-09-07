//using DomainModel;
//using Interfaces.Repository;
//using Microsoft.EntityFrameworkCore;

//namespace DAL.Repository
//{
//    public class CourseInviteRepositorySQL : IRepository<CourseInvite> //CourseInvite - регистрация студентов на курсы (Паки вождения)
//    {
//        private DrivingSchoolContext db;

//        public CourseInviteRepositorySQL(DrivingSchoolContext dbcontext)
//        {
//            this.db = dbcontext;
//        }
//        public async void CreateAsync(CourseInvite item)
//        {
//            await db.CourseInvites.AddAsync(item);
//        }

//        public async Task DeleteAsync(string id)
//        {
//            CourseInvite? st = await db.CourseInvites.FindAsync(id);
//            if (st != null)
//                db.CourseInvites.Remove(st);
//        }

//        public Task DeleteAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<CourseInvite?> GetItemAsync(string id)
//        {
//            return await db.CourseInvites.FindAsync(id);

//        }

//        public Task<CourseInvite?> GetItemAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public List<CourseInvite> GetList()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<List<CourseInvite>> GetListAsync()
//        {
//            return await db.CourseInvites.ToListAsync();
//        }

//        public void Update(CourseInvite item)
//        {
//            db.Entry(item).State = EntityState.Modified;
//            db.SaveChanges();

//        }
//    }
//}
