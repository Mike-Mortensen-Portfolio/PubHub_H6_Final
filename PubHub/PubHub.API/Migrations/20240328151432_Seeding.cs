using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1861 // Avoid constant arrays as arguments
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable CA1825 // Avoid zero-length array allocations

namespace PubHub.API.Migrations
{
    /// <inheritdoc />
    public partial class Seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSignIn",
                table: "Accounts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");



            migrationBuilder.InsertData(
                table: "AccessTypes",
                columns: new[] { "AccessTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Owner" },
                    { 2, "Subscriber" },
                    { 3, "Borrower" },
                    { 4, "Expired" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "User" },
                    { 2, "Publisher" },
                    { 3, "Operator" },
                    { 4, "Suspended" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { 1, "Jhon Doe" },
                    { 2, "Jane Doe" },
                    { 3, "Dan Turéll" }
                });

            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "ContentTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "AudioBook" },
                    { 2, "EBook" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "Name" },
                values: new object[,]
                {
                    { 1, "Romance" },
                    { 2, "Horror" },
                    { 3, "History" },
                    { 4, "Science-Fiction" },
                    { 5, "Fiction" },
                    { 6, "Novel" },
                    { 7, "Fantasy" },
                    { 8, "Biography" },
                    { 9, "True crime" },
                    { 10, "Thriller" },
                    { 11, "Young adult" },
                    { 12, "Mystery" },
                    { 13, "Satire" },
                    { 14, "Non-Fiction" },
                    { 15, "Self-help" },
                    { 16, "Poetry" },
                    { 17, "Humor" },
                    { 18, "Action" },
                    { 19, "Adventure" },
                    { 20, "Short story" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccessFailedCount", "AccountTypeId", "ConcurrencyStamp", "Email", "EmailConfirmed", "IsDeleted", "LastSignIn", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, 1, "UserSeedConcurrencyStamp", "User@Test.com", true, false, new DateTime(2024, 3, 28, 15, 14, 32, 308, DateTimeKind.Utc).AddTicks(5670), false, null, "USER@TEST.COM", "USER@TEST.COM", "AQAAAAIAAYagAAAAEFTnCpa3UBbfSLQ7VTnq9vy9DboDapVTyqXbdlucPfePa76mbJ6ltqYS4FqN/Ejt0w==", "4587654321", false, "UserSeedSecurityStamp", false, "User@Test.com" },
                    { 2, 0, 2, "PublisherSeedConcurrencyStamp", "Publisher@Test.com", true, false, new DateTime(2024, 3, 28, 15, 14, 32, 308, DateTimeKind.Utc).AddTicks(5684), false, null, "PUBLISHER@TEST.COM", "PUBLISHER@TEST.COM", "AQAAAAIAAYagAAAAEGAK65ejJJAiS+OtBpBSyqhJJhYmhJZMNJtuX1MYR+sDg35Vyr7EzYzzT+RYQmSg5w==", "4576543210", false, "PublisherSeedSecurityStamp", false, "Publisher@Test.com" },
                    { 3, 0, 2, "Publisher2SeedConcurrencyStamp", "Publisher2@Test.com", true, false, new DateTime(2024, 3, 28, 15, 14, 32, 308, DateTimeKind.Utc).AddTicks(5690), false, null, "PUBLISHER2@TEST.COM", "PUBLISHER2@TEST.COM", "AQAAAAIAAYagAAAAEKAZ13+dGi5AHBdKGEhAA8/3Z/rYjAvj4bCARhutk9TpEyzpzgPqZocJn43Oq8JYmQ==", "4565432109", false, "Publisher2SeedSecurityStamp", false, "Publisher2@Test.com" },
                    { 4, 0, 3, "OperatorSeedConcurrencyStamp", "Operator@Test.com", true, false, new DateTime(2024, 3, 28, 15, 14, 32, 308, DateTimeKind.Utc).AddTicks(5704), false, null, "OPERATOR@TEST.COM", "OPERATOR@TEST.COM", "AQAAAAIAAYagAAAAEAly7TGzLipT1MZa8cJSpNoaeYleeW4KtDNdJEFPl4lfAhTOUbPyTaKx9oyeBZ3qLw==", "4554321098", false, "OperatorSeedSecurityStamp", false, "Operator@Test.com" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "PublisherId", "AccountId", "Name" },
                values: new object[,]
                {
                    { 1, 2, "Gyldendal" },
                    { 2, 3, "Forlaget Als" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AccountId", "Birthday", "Name", "Surname" },
                values: new object[] { 1, 1, new DateOnly(1993, 4, 12), "Thomas", "Berlin" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "BookContent", "ContentTypeId", "CoverImage", "IsHidden", "Length", "PublicationDate", "PublisherId", "Title" },
                values: new object[,]
                {
                    { 1, new byte[0], 1, null, false, 3600.0, new DateOnly(1955, 12, 1), 1, "My day in the shoos of Tommy" },
                    { 2, new byte[0], 2, null, false, 123.0, new DateOnly(2023, 4, 7), 2, "My horse is the wildest" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 1, 9 },
                    { 2, 2 },
                    { 2, 5 },
                    { 2, 6 },
                    { 2, 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 1, 9 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 6 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 8 });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSignIn",
                table: "Accounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
