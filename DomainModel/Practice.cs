using System;
using System.Collections.Generic;

namespace DomainModel;

public enum LessonStatus
{
    Assigned = 0,  // Назначено
    Canceled = 1,  // Отменено
    Completed = 2  // Проведено
}

public partial class Practice
{
    public int Id { get; set; }
    public string? StudentId { get; set; } // #nullable - перезапись на занятие или повторное назначение учителя ?
    public string? TeacherId { get; set; }
    public string Description {  get; set; } = string.Empty; // Какое-либо описание
    public string Tittle { get; set; } = string.Empty; // Тема/заглавие/шапка. Н-р: маневрирование в пределах перекрёстка
    public DateTime Date { get; set; } = DateTime.Now;
    public float? Mark { get; set; } // nullable - ещё не присвоенная оценка
    public LessonStatus Status { get; set; }

    // Навигационные свойства
    public virtual User Teacher { get; set; } = null!;
    public virtual User Student { get; set; } = null!;
}
