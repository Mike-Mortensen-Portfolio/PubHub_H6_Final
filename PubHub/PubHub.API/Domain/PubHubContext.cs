using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Identity;
using System.Reflection.Emit;

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

                account.TypeToPluralTableName();
            });

            builder.Entity<IdentityUserRole<int>>(accountRole =>
            {
                accountRole.ToTable("AccountRoles");
            });

            builder.Entity<IdentityRole<int>>(role =>
            {
                role.ToTable("Roles");
            });

            builder.Entity<IdentityRoleClaim<int>>(roleClaim =>
            {
                roleClaim.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserClaim<int>>(accountClaim =>
            {
                accountClaim.ToTable("AccountClaims");
            });

            builder.Entity<IdentityUserLogin<int>>(accountLogin =>
            {
                accountLogin.Property(al => al.UserId)
                .HasColumnName("AccountId");

                accountLogin.ToTable("AccountLogins");
            });

            builder.Entity<IdentityUserToken<int>>(accountToken =>
            {
                accountToken.Property(at => at.UserId)
                .HasColumnName("AccountId");

                accountToken.ToTable("AccountTokens");
            });
            #endregion

            #region Entities
            builder.Entity<AccountType>(accountType =>
            {
                accountType.ConfigureId();

                accountType.TypeToPluralTableName();
            });

            builder.Entity<Author>(author =>
            {
                author.ConfigureId();

                author.HasMany(a => a.Books)
                    .WithMany(b => b.Authors)
                    .UsingEntity(ab => ab.ToTable("AuthorBooks"));

                author.TypeToPluralTableName();
            });

            builder.Entity<Book>(book =>
            {
                book.ConfigureId();
                book.HasOne(b => b.Publisher)
                    .WithMany(p => p.Books);
                book.HasOne(b => b.ContentType)
                    .WithMany();
                book.HasMany(b => b.Genres)
                    .WithMany(g => g.Books)
                    .UsingEntity(bg => bg.ToTable("BookGenres"));

                book.TypeToPluralTableName();
            });

            builder.Entity<Operator>(@operator =>
            {
                @operator.ConfigureId();

                @operator.HasOne(o => o.Account)
                    .WithOne();

                @operator.TypeToPluralTableName();
            });

            builder.Entity<Publisher>(publisher =>
            {
                publisher.ConfigureId();

                publisher.HasOne(p => p.Account)
                    .WithOne();

                publisher.TypeToPluralTableName();
            });

            builder.Entity<User>(user =>
            {
                user.ConfigureId();

                user.HasOne(u => u.Account)
                    .WithOne();

                user.TypeToPluralTableName();
            });

            builder.Entity<UserBook>(userBook =>
            {
                userBook.HasKey(ub => new { ub.BookId, ub.UserId, ub.AccessTypeId });

                userBook.HasOne(ub => ub.User)
                    .WithMany(u => u.UserBooks);
                userBook.HasOne(ub => ub.Book)
                    .WithMany(b => b.UserBooks);
                userBook.HasOne(ub => ub.AccessType)
                    .WithMany();

                userBook.TypeToPluralTableName();
            });

            builder.Entity<ContentType>(book =>
            {
                book.ConfigureId();

                book.TypeToPluralTableName();
            });

            builder.Entity<Genre>(book =>
            {
                book.ConfigureId();

                book.TypeToPluralTableName();
            });

            builder.Entity<AccessType>(accesstype =>
            {
                accesstype.ConfigureId();

                accesstype.TypeToPluralTableName();
            });
            #endregion
        }
    }
}
