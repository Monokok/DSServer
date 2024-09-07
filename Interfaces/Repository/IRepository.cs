using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IRepository<T> where T : class //обобщенный интерфейс репозитория для сущностей приложения 
    {
        Task<List<T>> GetListAsync(); //получение всех объектов
        Task<T?> GetItemAsync(string id); //Получение объекта по ID
        Task<T?> GetItemAsync(int id); //Получение объекта по ID
        List<T> GetList(); //получение всех объектов

        void CreateAsync(T item); //Создание объекта
        void Update(T item); //Обновление объекта
        Task DeleteAsync(string id); //Удаление объекта по ID
        Task DeleteAsync(int id); //Удаление объекта по ID


    }
}
