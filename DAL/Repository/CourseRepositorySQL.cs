//using DomainModel;
//using Interfaces.Repository;
//using Microsoft.EntityFrameworkCore;

//namespace DAL.Repository
//{
//    public class CourseRepositorySQL : IRepository<Course>
//    {
//        private DrivingSchoolContext db;

//        public CourseRepositorySQL(DrivingSchoolContext dbcontext)
//        {
//            this.db = dbcontext;
//        }
//        public async void CreateAsync(Course item)
//        {
//            await db.Courses.AddAsync(item);
//        }

//        public async Task DeleteAsync(string id)
//        {
//            Course? st = await db.Courses.FindAsync(id);
//            if (st != null)
//                db.Courses.Remove(st);
//        }

//        public Task DeleteAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<Course?> GetItemAsync(string id)
//        {
//            return await db.Courses.FindAsync(id);
//        }

//        public Task<Course?> GetItemAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public List<Course> GetList()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<List<Course>> GetListAsync()
//        {
//            return await db.Courses.ToListAsync();
//        }

//        public void Update(Course item)
//        {
//            db.Entry(item).State = EntityState.Modified;
//            db.SaveChanges();
//        }
//    }
//}

