using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;

namespace PubHub.API.Domain
{
    public sealed class PubHubContext : IdentityDbContext<Account, IdentityRole<int>, int>
    {
        private readonly string? _connectionString;

        public PubHubContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public PubHubContext(DbContextOptions<PubHubContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
               .EnableSensitiveDataLogging(true)
               .UseSqlServer(_connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Account>(account =>
            {
                account.Property(a => a.Id)
                .HasColumnName("AccountId");

                account.HasKey(a => a.Id);
                account.HasOne(a => a.AccountType)
                .WithMany();

                account.ToTable("Account");
            });

            builder.Entity<AccountType>(accountType =>
            {
                accountType.Property(a => a.Id)
                .HasColumnName("AccountTypeId");

                accountType.ToTable("AccountType");
            });

            builder.Entity<Author>(author =>
            {
                author.Property(a => a.Id)
                .HasColumnName("AuthorId");

                author.ToTable("Author");
            });

            builder.Entity<Book>(book =>
            {
                book.Property(b => b.Id)
                .HasColumnName("BookId");



                book.ToTable("Book");
            });
        }
    }
}
