using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
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

            #region Identity
            builder.Entity<Account>(account =>
            {
                account.ConfigureId();
                
                account.HasOne(a => a.AccountType)
                    .WithMany();

                account.ToTable(nameof(Account));
            });
            #endregion

            #region Entities
            builder.Entity<AccountType>(accountType =>
            {
                accountType.ConfigureId();

                accountType.ToTable(nameof(AccountType));
            });

            builder.Entity<Author>(author =>
            {
                author.ConfigureId();

                author.HasMany(a => a.Books)
                    .WithMany();

                author.ToTable(nameof(Author));
            });

            builder.Entity<Book>(book =>
            {
                book.ConfigureId();

                book.HasOne(b => b.Authors)
                    .WithMany();
                book.HasOne(b => b.Publisher)
                    .WithMany();

                book.ToTable(nameof(Book));
            });

            builder.Entity<ContentType>(book =>
            {
                book.ConfigureId();

                book.ToTable(nameof(ContentType));
            });

            builder.Entity<Genre>(book =>
            {
                book.ConfigureId();

                book.ToTable(nameof(Genre));
            });

            builder.Entity<Operator>(@operator =>
            {
                @operator.ConfigureId();

                @operator.HasOne(o => o.Account)
                    .WithMany();

                @operator.ToTable(nameof(Operator));
            });

            builder.Entity<Publisher>(publisher =>
            {
                publisher.ConfigureId();

                publisher.HasOne(p => p.Account)
                    .WithMany();

                publisher.ToTable(nameof(Publisher));
            });

            builder.Entity<User>(user =>
            {
                user.ConfigureId();

                user.HasOne(u => u.Account)
                    .WithMany();
                user.HasMany(u => u.Books)
                    .WithMany();

                user.ToTable(nameof(User));
            });
            #endregion
        }
    }
}
