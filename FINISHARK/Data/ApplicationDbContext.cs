using FINISHARK.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace FINISHARK.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions) { }

        public DbSet<Stock> Stock { set; get; }

        public DbSet<Comment> Comments { set; get; }

        public DbSet<Portfolio> Portfolios { set; get; }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);
            builder.Entity<Portfolio>(x => x.HasKey(p => new { p.AppUserId, p.StockID }));
            builder.Entity<Portfolio>()
                .HasOne(x => x.User)
                .WithMany(x => x.Portfolios)
                .HasForeignKey(x => x.AppUserId);

            builder.Entity<Portfolio>()
               .HasOne(x => x.stock)
               .WithMany(x => x.Portfolios)
               .HasForeignKey(x => x.StockID);


            // Use fixed, deterministic Ids for seeded IdentityRole entries to avoid pending model changes
            var roles = new[]
            {
                new IdentityRole
                {
                    Id = "11111111-1111-1111-1111-111111111111",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "a1111111-1111-1111-1111-111111111111"
                },
                new IdentityRole
                {
                    Id = "22222222-2222-2222-2222-222222222222",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "b2222222-2222-2222-2222-222222222222"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
