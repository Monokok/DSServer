using DomainModel;

namespace Interfaces.Repository
{
    public interface IDbRepos //интерфейс для взаимодействия с репозиториями UnitOfWork
    {
        IRepository<User> Users { get; } //Репозиторий с пользователями системы DS
        IRepository<Practice> PracticeLessons { get; } // Уроками
        //IRepository<Payment> Payments { get; } // Оплатами
        //IRepository<PaymentsType> PaymentsTypes { get; } // Типами оплат (оплачено\возврат средств)
        Task<int> Save(); //Функция сохранения
    }
}
