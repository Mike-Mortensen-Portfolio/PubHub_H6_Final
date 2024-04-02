using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Identity;
using PubHub.API.Domain.Seeding;

namespace PubHub.API.Domain
{
    public sealed class PubHubContext : IdentityDbContext<Account, IdentityRole<Guid>, Guid>
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
            const int NAME_MAX_LENGTH = 128;
            const int TYPE_NAME_MAX_LENGTH = 128;
            const int EMAIL_MAX_LENGTH = 256;
            const int USERNAME_MAX_LENGTH = 256;
            const int BOOK_TITLE_MAX_LENGTH = 256;

            base.OnModelCreating(builder);

            #region Identity
            builder.Entity<Account>(account =>
            {
                account.ConfigureId();

                account.HasOne(a => a.AccountType)
                    .WithMany()
                    .HasForeignKey(a => a.AccountTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                account.Property(a => a.Email)
                    .HasMaxLength(EMAIL_MAX_LENGTH);
                account.Property(a => a.NormalizedEmail)
                    .HasMaxLength(EMAIL_MAX_LENGTH);
                account.Property(a => a.UserName)
                    .HasMaxLength(USERNAME_MAX_LENGTH);
                account.Property(a => a.UserName)
                    .HasMaxLength(USERNAME_MAX_LENGTH);

                account.HasIndex(a => a.Email)
                    .IsUnique();

                account.HasQueryFilter(a => a.DeletedDate == null);

                account.TypeToPluralTableName();
            });

            builder.Entity<IdentityUserRole<Guid>>(accountRole =>
            {
                accountRole.ToTable("AccountRoles");
            });

            builder.Entity<IdentityRole<Guid>>(role =>
            {
                role.ToTable("Roles");
            });

            builder.Entity<IdentityRoleClaim<Guid>>(roleClaim =>
            {
                roleClaim.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserClaim<Guid>>(accountClaim =>
            {
                accountClaim.ToTable("AccountClaims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(accountLogin =>
            {
                accountLogin.Property(al => al.UserId)
                    .HasColumnName("AccountId");

                accountLogin.ToTable("AccountLogins");
            });

            builder.Entity<IdentityUserToken<Guid>>(accountToken =>
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

                accountType.Property(at => at.Name)
                    .HasMaxLength(TYPE_NAME_MAX_LENGTH);

                accountType.TypeToPluralTableName();
            });

            builder.Entity<Author>(author =>
            {
                author.ConfigureId();

                // Author has relations defined in BookAuthor.

                author.Property(a => a.Name)
                    .HasMaxLength(NAME_MAX_LENGTH);

                author.TypeToPluralTableName();
            });
            
            builder.Entity<Book>(book =>
            {
                book.ConfigureId();

                book.HasOne(b => b.Publisher)
                    .WithMany(b => b.Books)
                    .HasForeignKey(b => b.PublisherId)
                    .OnDelete(DeleteBehavior.Restrict);
                book.HasOne(b => b.ContentType)
                    .WithMany()
                    .HasForeignKey(b => b.ContentTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                // Book has relations defined in BookAuthor.
                // Book has relations defined in BookGenre.
                // Book has relations defined in UserBook.

                book.Property(b => b.Title)
                    .HasMaxLength(BOOK_TITLE_MAX_LENGTH);

                book.TypeToPluralTableName();
            });

            builder.Entity<BookAuthor>(bookAuthor =>
            {
                bookAuthor.HasKey(ba => new { ba.BookId, ba.AuthorId });

                bookAuthor.HasOne(ba => ba.Book)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(ba => ba.BookId);
                bookAuthor.HasOne(ba => ba.Author)
                    .WithMany(a => a.BookAuthors)
                    .HasForeignKey(ba => ba.AuthorId);

                bookAuthor.TypeToPluralTableName();
            });

            builder.Entity<BookGenre>(bookGenre =>
            {
                bookGenre.HasKey(bg => new { bg.BookId, bg.GenreId });

                bookGenre.HasOne(bg => bg.Book)
                    .WithMany(b => b.BookGenres)
                    .HasForeignKey(bg => bg.BookId);
                bookGenre.HasOne(bg => bg.Genre)
                    .WithMany(g => g.BookGenres)
                    .HasForeignKey(bg => bg.GenreId);

                bookGenre.TypeToPluralTableName();
            });

            builder.Entity<ContentType>(contentType =>
            {
                contentType.ConfigureId();

                contentType.Property(ct => ct.Name)
                    .HasMaxLength(TYPE_NAME_MAX_LENGTH);

                contentType.TypeToPluralTableName();
            });

            builder.Entity<Genre>(genre =>
            {
                genre.ConfigureId();

                // Genre has relations defined in BookGenre.

                genre.Property(g => g.Name)
                    .HasMaxLength(NAME_MAX_LENGTH);

                genre.TypeToPluralTableName();
            });

            builder.Entity<Operator>(@operator =>
            {
                @operator.ConfigureId();

                @operator.HasOne(o => o.Account)
                    .WithOne()
                    .HasForeignKey<Operator>(u => u.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                @operator.Property(o => o.Name)
                    .HasMaxLength(NAME_MAX_LENGTH);
                @operator.Property(o => o.Surname)
                    .HasMaxLength(NAME_MAX_LENGTH);

                @operator.HasQueryFilter(o => o.Account!.DeletedDate == null);

                @operator.TypeToPluralTableName();
            });

            builder.Entity<Publisher>(publisher =>
            {
                publisher.ConfigureId();

                publisher.HasOne(p => p.Account)
                    .WithOne()
                    .HasForeignKey<Publisher>(u => u.AccountId)
                    .OnDelete(DeleteBehavior.SetNull);

                publisher.Property(o => o.Name)
                    .HasMaxLength(NAME_MAX_LENGTH);

                publisher.TypeToPluralTableName();
            });

            builder.Entity<User>(user =>
            {
                user.ConfigureId();

                user.HasOne(u => u.Account)
                    .WithOne()
                    .HasForeignKey<User>(u => u.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
                // User has relations defined in UserBook.

                user.Property(o => o.Name)
                    .HasMaxLength(NAME_MAX_LENGTH);
                user.Property(o => o.Surname)
                    .HasMaxLength(NAME_MAX_LENGTH);

                user.HasQueryFilter(u => u.Account!.DeletedDate == null);

                user.TypeToPluralTableName();
            });

            builder.Entity<UserBook>(userBook =>
            {
                userBook.ConfigureId();

                userBook.HasOne(ub => ub.User)
                    .WithMany(u => u.UserBooks)
                    .HasForeignKey(ub => ub.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
                userBook.HasOne(ub => ub.Book)
                    .WithMany(b => b.UserBooks)
                    .HasForeignKey(ub => ub.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
                userBook.HasOne(ub => ub.AccessType)
                    .WithMany()
                    .HasForeignKey(ub => ub.AccessTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

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

            builder.Entity<BookGenre>(bookGenre =>
            {
                bookGenre.HasKey(bg => new { bg.BookId, bg.GenreId });

                bookGenre.HasOne(bg => bg.Book)
                   .WithMany(b => b.BookGenres);
                bookGenre.HasOne(bg => bg.Genre)
                    .WithMany(g => g.BookGenres);

                bookGenre.TypeToPluralTableName();
            });

            builder.Entity<AccessType>(accesstype =>
            {
                accesstype.ConfigureId();

                accesstype.TypeToPluralTableName();
            });
            #endregion

            #region Seeding
            var accessTypes = new AccessTypeSeed();
            var accountTypes = new AccountTypeSeed();
            var accounts = new AccountSeed(accountTypes);
            var authors = new AuthorSeed();
            var contentTypes = new ContentTypeSeed();
            var publishers = new PublisherSeed(accounts);
            var genres = new GenreSeed();
            var books = new BookSeed(contentTypes, publishers);
            var bookAuthors = new BookAuthorSeed(books, authors);
            var bookGenres = new BookGenreSeed(books, genres);
            var operators = new OperatorSeed(accounts);
            var users = new UserSeed(accounts);
            
            builder.ApplyConfiguration(accessTypes);
            builder.ApplyConfiguration(accountTypes);
            builder.ApplyConfiguration(accounts);
            builder.ApplyConfiguration(authors);
            builder.ApplyConfiguration(contentTypes);
            builder.ApplyConfiguration(publishers);
            builder.ApplyConfiguration(genres);
            builder.ApplyConfiguration(books);
            builder.ApplyConfiguration(bookAuthors);
            builder.ApplyConfiguration(bookGenres);
            builder.ApplyConfiguration(users);
            #endregion
        }
    }
}
