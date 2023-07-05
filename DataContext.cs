//dbcontext using efcore

global using Microsoft.EntityFrameworkCore;

/*
/Users/cemdedetas/Projects/EduAPI/DataContext.cs(9,16): error CS1520: Method must have a return type [/Users/cemdedetas/Projects/EduAPI/EduAPI.csproj]
    0 Warning(s)
    1 Error(s)

    why is this?
*/


namespace WebApplication1.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<LiveLectureClass> LiveLectures => Set<LiveLectureClass>();
        public DbSet<UserHistory> UserHistories => Set<UserHistory>();
        public DbSet<Recording> Recordings => Set<Recording>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
       
    }
}