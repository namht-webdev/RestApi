using Microsoft.EntityFrameworkCore;

namespace Models.MyDbContext
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<QuestionGetManyResponse>().HasNoKey();
        }

        //public virtual DbSet<QuestionGetManyResponse> QuestionGetManyResponse { get; set; }
    }
}