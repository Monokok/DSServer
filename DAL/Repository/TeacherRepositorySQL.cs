//using DomainModel;
//using Interfaces.Repository;
//using Microsoft.EntityFrameworkCore;

//namespace DAL.Repository
//{
//    public class TeacherRepositorySQL : IRepository<Teacher> //Репозиторий занятий
//    {
//        private DrivingSchoolContext db;
//        public TeacherRepositorySQL(DrivingSchoolContext db) { this.db = db; }
//        public async void CreateAsync(Teacher item)
//        {
//            await db.Teachers.AddAsync(item);
//        }
//        public async Task<Teacher?> GetItemAsync(int id)
//        {
//            return await db.Teachers.FindAsync(id);
//        }

//        public async Task<List<Teacher>> GetListAsync()
//        {
//            return await db.Teachers.ToListAsync();
//        }

//        public void Update(Teacher item)
//        {
//            db.Entry(item).State = EntityState.Modified;
//        }

//        public async Task DeleteAsync(int id)
//        {
//            Teacher? st = await db.Teachers.FindAsync(id);
//            if (st != null)
//                db.Teachers.Remove(st);
//            await db.SaveChangesAsync();
//        }
//    }
//}
