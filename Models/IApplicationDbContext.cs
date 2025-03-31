using System.Data.Entity;

namespace YorkScjool.Models
{

    public interface IApplicationDbContext
    {
        DbSet<Lesson> Lessons { get; set; }
        DbSet<Homework> Homeworks { get; set; }
    }

}