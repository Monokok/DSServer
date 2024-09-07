using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using DomainModel;
using DAL;

namespace DS.Data
{
    public class DsContextSeed
    {
        public static async Task SeedAsync(DrivingSchoolContext context)
        {
            try
            {
                context.Database.EnsureCreated();
                //if (context.Cathegories.Any())
                //{
                //    return;
                //}
                //var cathegories = new Cathegory[] //категории А В С
                //{
                //    new Cathegory
                //    {
                //          Id = 0,
                //           Description = "A",
                //            Name = "A",
                //    },
                //    new Cathegory
                //    {
                //          Id = 1,
                //           Description = "B",
                //            Name = "B",
                //    },
                //    new Cathegory
                //    {
                //          Id = 2,
                //           Description = "C",
                //            Name = "C",
                //    },
                //};
                //foreach (Cathegory b in cathegories)
                //{
                //    context.Cathegories.Add(b);
                //}
                //await context.SaveChangesAsync();

                //if (context.LessonsTypes.Any())
                //{
                //    return;
                //}
                //var types = new LessonsType[] //типы уроков
                //{
                //    new LessonsType
                //    {
                //          Id = 0,
                //            Name = "Назначено",
                //    },
                //    new LessonsType
                //    {
                //          Id = 1,
                //            Name = "Отменено",
                //    },
                //    new LessonsType
                //    {
                //          Id = 3,
                //            Name = "Проведено",
                //    },
                //};
                //foreach (LessonsType b in types)
                //{
                //    context.LessonsTypes.Add(b);
                //}
                //await context.SaveChangesAsync();

                //if (context.PaymentsTypes.Any()) // заполнение типов оплат - таких как "оплачено"\"возврат средств"
                //{
                //    return;
                //}
                //var PTypes = new PaymentsType[] 
                //{
                //    new PaymentsType
                //    {
                //          Id = 0,
                //            Name = "Оплачено",
                //    },
                //    new PaymentsType
                //    {
                //          Id = 1,
                //            Name = "Возврат средств",
                //    },
                //};
                //foreach (PaymentsType b in PTypes)
                //{
                //    context.PaymentsTypes.Add(b);
                //}
                //await context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
};
