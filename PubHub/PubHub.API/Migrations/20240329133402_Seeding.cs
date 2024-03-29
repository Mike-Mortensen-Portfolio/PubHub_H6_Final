using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PubHub.API.Migrations
{
    /// <inheritdoc />
    public partial class Seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AccessTypes",
                columns: new[] { "AccessTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-d9e1-735f-ab01-2adc4c4bd70b"), "Owner" },
                    { new Guid("018e8a6a-d9e1-7360-b1fb-d3becfa8731b"), "Subscriber" },
                    { new Guid("018e8a6a-d9e1-7361-88a2-c3c754683ddb"), "Borrower" },
                    { new Guid("018e8a6a-d9e1-7362-8bc3-e805a4177d36"), "Expired" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-d9e1-7363-82b7-76610d269524"), "User" },
                    { new Guid("018e8a6a-d9e1-7364-a112-d8d7a64a8511"), "Publisher" },
                    { new Guid("018e8a6a-d9e1-7365-b794-3efe0f1e5346"), "Operator" },
                    { new Guid("018e8a6a-d9e1-7366-a987-ff6a35e3f6d8"), "Suspended" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-da88-74f7-8db7-20069106f82a"), "Jhon Doe" },
                    { new Guid("018e8a6a-da88-74f8-98f4-48d2ebf98324"), "Jane Doe" },
                    { new Guid("018e8a6a-da88-74f9-838c-3a88d053658b"), "Dan Turéll" }
                });

            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "ContentTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-da88-74fa-9a53-5420e5d69571"), "AudioBook" },
                    { new Guid("018e8a6a-da88-74fb-9355-40f9f93f77ac"), "EBook" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-da88-74fe-9879-a81d7007f654"), "Romance" },
                    { new Guid("018e8a6a-da88-74ff-99b9-f8b24c3c48bb"), "Horror" },
                    { new Guid("018e8a6a-da88-7500-93c1-82c354f2c864"), "History" },
                    { new Guid("018e8a6a-da88-7501-887d-d37324d27da0"), "Science-Fiction" },
                    { new Guid("018e8a6a-da88-7502-ab63-e1557e28397c"), "Fiction" },
                    { new Guid("018e8a6a-da88-7503-a9e9-2099a4468197"), "Novel" },
                    { new Guid("018e8a6a-da88-7504-845f-a931fc40e008"), "Fantasy" },
                    { new Guid("018e8a6a-da88-7505-9d3b-732a40fdfec1"), "Biography" },
                    { new Guid("018e8a6a-da88-7506-a648-8f201acd5e18"), "True crime" },
                    { new Guid("018e8a6a-da88-7507-a84c-0e332fee277b"), "Thriller" },
                    { new Guid("018e8a6a-da88-7508-be0c-06e71ff86374"), "Young adult" },
                    { new Guid("018e8a6a-da88-7509-b5c8-5d68b927e7d6"), "Mystery" },
                    { new Guid("018e8a6a-da88-750a-bb41-47334a12f1e5"), "Satire" },
                    { new Guid("018e8a6a-da88-750b-aedd-e785e69f50c3"), "Non-Fiction" },
                    { new Guid("018e8a6a-da88-750c-8ccf-b0f3ac3d6fce"), "Self-help" },
                    { new Guid("018e8a6a-da88-750d-8f47-e84c9407219a"), "Poetry" },
                    { new Guid("018e8a6a-da88-750e-aab8-184ebd0af4f0"), "Humor" },
                    { new Guid("018e8a6a-da88-750f-9cd8-2784fe895f4b"), "Action" },
                    { new Guid("018e8a6a-da88-7510-abc2-d0d551b25f8f"), "Adventure" },
                    { new Guid("018e8a6a-da88-7511-bf4e-0bcfa48f0533"), "Short story" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccessFailedCount", "AccountTypeId", "ConcurrencyStamp", "DeletedDate", "Email", "EmailConfirmed", "LastSignIn", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-d9e1-7367-9dac-954e8d92f4fd"), 0, new Guid("018e8a6a-d9e1-7363-82b7-76610d269524"), "UserSeedConcurrencyStamp", null, "User@Test.com", true, new DateTime(2024, 3, 29, 13, 34, 2, 209, DateTimeKind.Utc).AddTicks(3849), false, null, "USER@TEST.COM", "USER@TEST.COM", "AQAAAAIAAYagAAAAEOczCyB9R28Te+Fn2Xk0gH4miUlBNtXYzJeXBNKcHaCMgWULSea8qt+PgQhVM78QZg==", "4587654321", false, "UserSeedSecurityStamp", false, "User@Test.com" },
                    { new Guid("018e8a6a-d9e1-7368-8aed-591deb4151f9"), 0, new Guid("018e8a6a-d9e1-7364-a112-d8d7a64a8511"), "PublisherSeedConcurrencyStamp", null, "Publisher@Test.com", true, new DateTime(2024, 3, 29, 13, 34, 2, 209, DateTimeKind.Utc).AddTicks(3868), false, null, "PUBLISHER@TEST.COM", "PUBLISHER@TEST.COM", "AQAAAAIAAYagAAAAEFrU4hcQzruwsvlc1qdAQdgqhsq17FDXjOl4lKQpj0WMloePtOFXCyE9obO1y6Z06g==", "4576543210", false, "PublisherSeedSecurityStamp", false, "Publisher@Test.com" },
                    { new Guid("018e8a6a-d9e1-7369-b74d-235a4d92cf03"), 0, new Guid("018e8a6a-d9e1-7364-a112-d8d7a64a8511"), "Publisher2SeedConcurrencyStamp", null, "Publisher2@Test.com", true, new DateTime(2024, 3, 29, 13, 34, 2, 209, DateTimeKind.Utc).AddTicks(3878), false, null, "PUBLISHER2@TEST.COM", "PUBLISHER2@TEST.COM", "AQAAAAIAAYagAAAAEPwMKnmmEdv7m3EdgJXNFnQIiefizZjMyViAC8lz7+3S85HBP0UtM2Cbo0jwcyptcQ==", "4565432109", false, "Publisher2SeedSecurityStamp", false, "Publisher2@Test.com" },
                    { new Guid("018e8a6a-d9e1-736a-902e-35bba62af169"), 0, new Guid("018e8a6a-d9e1-7365-b794-3efe0f1e5346"), "OperatorSeedConcurrencyStamp", null, "Operator@Test.com", true, new DateTime(2024, 3, 29, 13, 34, 2, 209, DateTimeKind.Utc).AddTicks(3891), false, null, "OPERATOR@TEST.COM", "OPERATOR@TEST.COM", "AQAAAAIAAYagAAAAEISeacsTuT/TWbqyMeXgn0gATHnNbqie9YZ3+pgFW2TslQOeNqkjQeaCok5Hs2I8mA==", "4554321098", false, "OperatorSeedSecurityStamp", false, "Operator@Test.com" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "PublisherId", "AccountId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-da88-74fc-8de1-154d1a0fb022"), new Guid("018e8a6a-d9e1-7368-8aed-591deb4151f9"), "Gyldendal" },
                    { new Guid("018e8a6a-da88-74fd-be3b-f3dfa6e8c2d0"), new Guid("018e8a6a-d9e1-7369-b74d-235a4d92cf03"), "Forlaget Als" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AccountId", "Birthday", "Name", "Surname" },
                values: new object[] { new Guid("018e8a6a-da88-7514-bee2-834912336022"), new Guid("018e8a6a-d9e1-7367-9dac-954e8d92f4fd"), new DateOnly(1993, 4, 12), "Thomas", "Berlin" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "BookContent", "ContentTypeId", "CoverImage", "IsHidden", "Length", "PublicationDate", "PublisherId", "Title" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"), new byte[0], new Guid("018e8a6a-da88-74fa-9a53-5420e5d69571"), null, false, 3600.0, new DateOnly(1955, 12, 1), new Guid("018e8a6a-da88-74fc-8de1-154d1a0fb022"), "My day in the shoos of Tommy" },
                    { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new byte[0], new Guid("018e8a6a-da88-74fb-9355-40f9f93f77ac"), null, false, 123.0, new DateOnly(2023, 4, 7), new Guid("018e8a6a-da88-74fc-8de1-154d1a0fb022"), "My horse is the wildest" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-da88-74f7-8db7-20069106f82a"), new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd") },
                    { new Guid("018e8a6a-da88-74f8-98f4-48d2ebf98324"), new Guid("018e8a6a-da88-7513-9ded-074608658876") },
                    { new Guid("018e8a6a-da88-74f9-838c-3a88d053658b"), new Guid("018e8a6a-da88-7513-9ded-074608658876") }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                values: new object[,]
                {
                    { new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"), new Guid("018e8a6a-da88-74fe-9879-a81d7007f654") },
                    { new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"), new Guid("018e8a6a-da88-7500-93c1-82c354f2c864") },
                    { new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"), new Guid("018e8a6a-da88-7506-a648-8f201acd5e18") },
                    { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-74ff-99b9-f8b24c3c48bb") },
                    { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-7502-ab63-e1557e28397c") },
                    { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-7503-a9e9-2099a4468197") },
                    { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-7505-9d3b-732a40fdfec1") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8a6a-d9e1-735f-ab01-2adc4c4bd70b"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8a6a-d9e1-7360-b1fb-d3becfa8731b"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8a6a-d9e1-7361-88a2-c3c754683ddb"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8a6a-d9e1-7362-8bc3-e805a4177d36"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8a6a-d9e1-7366-a987-ff6a35e3f6d8"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8a6a-d9e1-736a-902e-35bba62af169"));

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-74f7-8db7-20069106f82a"), new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-74f8-98f4-48d2ebf98324"), new Guid("018e8a6a-da88-7513-9ded-074608658876") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-74f9-838c-3a88d053658b"), new Guid("018e8a6a-da88-7513-9ded-074608658876") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"), new Guid("018e8a6a-da88-74fe-9879-a81d7007f654") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"), new Guid("018e8a6a-da88-7500-93c1-82c354f2c864") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"), new Guid("018e8a6a-da88-7506-a648-8f201acd5e18") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-74ff-99b9-f8b24c3c48bb") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-7502-ab63-e1557e28397c") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-7503-a9e9-2099a4468197") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8a6a-da88-7513-9ded-074608658876"), new Guid("018e8a6a-da88-7505-9d3b-732a40fdfec1") });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7501-887d-d37324d27da0"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7504-845f-a931fc40e008"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7507-a84c-0e332fee277b"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7508-be0c-06e71ff86374"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7509-b5c8-5d68b927e7d6"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-750a-bb41-47334a12f1e5"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-750b-aedd-e785e69f50c3"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-750c-8ccf-b0f3ac3d6fce"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-750d-8f47-e84c9407219a"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-750e-aab8-184ebd0af4f0"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-750f-9cd8-2784fe895f4b"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7510-abc2-d0d551b25f8f"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7511-bf4e-0bcfa48f0533"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("018e8a6a-da88-74fd-be3b-f3dfa6e8c2d0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("018e8a6a-da88-7514-bee2-834912336022"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8a6a-d9e1-7365-b794-3efe0f1e5346"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8a6a-d9e1-7367-9dac-954e8d92f4fd"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8a6a-d9e1-7369-b74d-235a4d92cf03"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("018e8a6a-da88-74f7-8db7-20069106f82a"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("018e8a6a-da88-74f8-98f4-48d2ebf98324"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("018e8a6a-da88-74f9-838c-3a88d053658b"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("018e8a6a-da88-7512-81ba-74a2587a5ebd"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("018e8a6a-da88-7513-9ded-074608658876"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-74fe-9879-a81d7007f654"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-74ff-99b9-f8b24c3c48bb"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7500-93c1-82c354f2c864"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7502-ab63-e1557e28397c"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7503-a9e9-2099a4468197"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7505-9d3b-732a40fdfec1"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8a6a-da88-7506-a648-8f201acd5e18"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8a6a-d9e1-7363-82b7-76610d269524"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("018e8a6a-da88-74fa-9a53-5420e5d69571"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("018e8a6a-da88-74fb-9355-40f9f93f77ac"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("018e8a6a-da88-74fc-8de1-154d1a0fb022"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8a6a-d9e1-7368-8aed-591deb4151f9"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8a6a-d9e1-7364-a112-d8d7a64a8511"));
        }
    }
}
