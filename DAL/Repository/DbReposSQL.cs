//using DomainModel;
using DomainModel;
using Interfaces.DTO;
using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class DbReposSQL : IDbRepos
    {
        private DrivingSchoolContext db;
        private PracticeLessonsRepositorySQL PracticeLessonRepository;
        private UserRepositorySQL UserRepository;
        //private PaymentsRepositorySQL PaymentsRepository;
        //private PaymentsTypesRepositorySQL PaymentsTypesRepository;

        public DbReposSQL(DrivingSchoolContext _context)
        {
            db = _context;
        }

        public IRepository<Practice> PracticeLessons
        {
            get
            {
                if (PracticeLessonRepository == null)
                    PracticeLessonRepository = new PracticeLessonsRepositorySQL(db);
                return PracticeLessonRepository;
            }
        }
        public IRepository<User> Users
        {
            get
            {
                if (UserRepository == null)
                    UserRepository = new UserRepositorySQL(db);
                return UserRepository;
            }
        }

        //public IRepository<Payment> Payments
        //{
        //    get
        //    {
        //        if (PaymentsRepository == null)
        //            PaymentsRepository = new PaymentsRepositorySQL(db);
        //        return PaymentsRepository;
        //    }
        //}

        //public IRepository<PaymentsType> PaymentsTypes
        //{
        //    get
        //    {
        //        if (PaymentsTypesRepository == null)
        //            PaymentsTypesRepository = new PaymentsTypesRepositorySQL(db);
        //        return PaymentsTypesRepository;
        //    }
        //}

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }
       
    }
}
