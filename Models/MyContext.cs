using Microsoft.EntityFrameworkCore;

namespace belt_exam.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<AnActivity> Activities { get; set; }
        public DbSet<AttendeeList> AttendeeLists { get; set; }
    }
}