using DomainModel;
using Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class PracticeLessonsRepositorySQL : IRepository<Practice> //Репозиторий занятий
    {
        private DrivingSchoolContext db;
        public PracticeLessonsRepositorySQL(DrivingSchoolContext db) { this.db = db; }
        public async void CreateAsync(Practice item)
        {
            await db.PracticeLessons.AddAsync(item);
        }
        public async Task<Practice?> GetItemAsync(string id)
        {
            return await db.PracticeLessons.FindAsync(id);
        }

        public async Task<List<Practice>> GetListAsync()
        {
            return await db.PracticeLessons.ToListAsync();
        }

        public void Update(Practice item)
        {
            // установка состояния сущности как "измененное"
            db.Entry(item).State = EntityState.Modified;
            // сохранение изменения в бд
            db.SaveChanges();
        }

        public async Task DeleteAsync(string id)
        {
            Practice? st = await db.PracticeLessons.FindAsync(id);
            if (st != null)
                db.PracticeLessons.Remove(st);
            await db.SaveChangesAsync();
        }

        public async Task<Practice?> GetItemAsync(int id)
        {
            Practice? st = await db.PracticeLessons.FindAsync(id);
            if (st != null) return st;
            else throw new Exception("Элемент не найден!");

        }

        public async Task DeleteAsync(int id)
        {
            Practice? st = await db.PracticeLessons.FindAsync(id);
            if (st != null)
                db.PracticeLessons.Remove(st);
            await db.SaveChangesAsync();
        }

        public List<Practice> GetList()
        {
            return db.PracticeLessons.ToList();
        }
    }
}
