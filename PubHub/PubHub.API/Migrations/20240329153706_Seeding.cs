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
                    { new Guid("018e8adb-8368-70b8-9718-466effd86b3b"), "Owner" },
                    { new Guid("018e8adb-8368-70b9-a6b3-038c867c2c74"), "Subscriber" },
                    { new Guid("018e8adb-8368-70ba-9989-10b795d98bcf"), "Borrower" },
                    { new Guid("018e8adb-8368-70bb-958e-44661f184474"), "Expired" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8adb-8368-70bc-a239-436bd0b16077"), "User" },
                    { new Guid("018e8adb-8368-70bd-8393-772206d0123f"), "Publisher" },
                    { new Guid("018e8adb-8368-70be-9081-9f04ff23a4b1"), "Operator" },
                    { new Guid("018e8adb-8368-70bf-b88c-9d8337f0ed7f"), "Suspended" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8adb-840f-77a3-ac65-5f1e56d59cd3"), "Jhon Doe" },
                    { new Guid("018e8adb-840f-77a4-8274-f813dfa6df3e"), "Jane Doe" },
                    { new Guid("018e8adb-840f-77a5-a9f0-8d921238f0f2"), "Dan Turéll" }
                });

            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "ContentTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8adb-840f-77a6-8fba-a2058e995998"), "AudioBook" },
                    { new Guid("018e8adb-840f-77a7-8af8-4e9efba397ca"), "EBook" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8adb-840f-77aa-930a-83f7b2f0cb73"), "Romance" },
                    { new Guid("018e8adb-840f-77ab-8d6c-8171a947e74d"), "Horror" },
                    { new Guid("018e8adb-840f-77ac-9c19-5a943cfdf7d7"), "History" },
                    { new Guid("018e8adb-840f-77ad-bc45-ef8eb5f728d9"), "Science-Fiction" },
                    { new Guid("018e8adb-840f-77ae-801c-f7761b058c5c"), "Fiction" },
                    { new Guid("018e8adb-840f-77af-8876-7a7b3b128ac5"), "Novel" },
                    { new Guid("018e8adb-840f-77b0-9a88-6879bbf73fab"), "Fantasy" },
                    { new Guid("018e8adb-840f-77b1-b88c-bf08b77ad230"), "Biography" },
                    { new Guid("018e8adb-840f-77b2-97b2-ec9f5a55663b"), "True crime" },
                    { new Guid("018e8adb-840f-77b3-a2f5-28897b4979a8"), "Thriller" },
                    { new Guid("018e8adb-840f-77b4-9a27-8ae54fa760ff"), "Young adult" },
                    { new Guid("018e8adb-840f-77b5-8cc9-629f12944b07"), "Mystery" },
                    { new Guid("018e8adb-840f-77b6-a49e-24c3cf49f7f6"), "Satire" },
                    { new Guid("018e8adb-840f-77b7-b8fb-e33165e8ed6e"), "Non-Fiction" },
                    { new Guid("018e8adb-840f-77b8-af35-4c46b0a02646"), "Self-help" },
                    { new Guid("018e8adb-840f-77b9-aa42-02c964da8b21"), "Poetry" },
                    { new Guid("018e8adb-840f-77ba-b89b-48361e4499b8"), "Humor" },
                    { new Guid("018e8adb-840f-77bb-85f1-691c1b8cbd59"), "Action" },
                    { new Guid("018e8adb-840f-77bc-a0b5-683285074af9"), "Adventure" },
                    { new Guid("018e8adb-840f-77bd-b255-6873e01aac3a"), "Short story" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccessFailedCount", "AccountTypeId", "ConcurrencyStamp", "DeletedDate", "Email", "EmailConfirmed", "LastSignIn", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("018e8adb-8368-70c0-aa9f-b47e2880ff5c"), 0, new Guid("018e8adb-8368-70bc-a239-436bd0b16077"), "UserSeedConcurrencyStamp", null, "User@Test.com", true, new DateTime(2024, 3, 29, 15, 37, 5, 640, DateTimeKind.Utc).AddTicks(3355), false, null, "USER@TEST.COM", "USER@TEST.COM", "AQAAAAIAAYagAAAAENjQL3p2ARK3fATdbbvylm05GDvKYEthVXLcapETMnsQP8BVR45ZzT2nEGtSqWHHuA==", "4587654321", false, "UserSeedSecurityStamp", false, "User@Test.com" },
                    { new Guid("018e8adb-8368-70c1-8252-cd2b5e9bc3f1"), 0, new Guid("018e8adb-8368-70bd-8393-772206d0123f"), "PublisherSeedConcurrencyStamp", null, "Publisher@Test.com", true, new DateTime(2024, 3, 29, 15, 37, 5, 640, DateTimeKind.Utc).AddTicks(3370), false, null, "PUBLISHER@TEST.COM", "PUBLISHER@TEST.COM", "AQAAAAIAAYagAAAAELwMNDmZlgRPj6j74N/HE9qfxwZri6LGjHA/qf7v6miHSrGCs9gwO++4c1cjmwOQ3g==", "4576543210", false, "PublisherSeedSecurityStamp", false, "Publisher@Test.com" },
                    { new Guid("018e8adb-8368-70c2-94db-5b205df0edcf"), 0, new Guid("018e8adb-8368-70bd-8393-772206d0123f"), "Publisher2SeedConcurrencyStamp", null, "Publisher2@Test.com", true, new DateTime(2024, 3, 29, 15, 37, 5, 640, DateTimeKind.Utc).AddTicks(3381), false, null, "PUBLISHER2@TEST.COM", "PUBLISHER2@TEST.COM", "AQAAAAIAAYagAAAAELTI1cGKBGGGZszaBeiVAeEm7ExXYrG08dB/cx349nKUMgxZasuGJ2i2W0/qor+LuA==", "4565432109", false, "Publisher2SeedSecurityStamp", false, "Publisher2@Test.com" },
                    { new Guid("018e8adb-8368-70c3-995b-1c5ff20e5986"), 0, new Guid("018e8adb-8368-70be-9081-9f04ff23a4b1"), "OperatorSeedConcurrencyStamp", null, "Operator@Test.com", true, new DateTime(2024, 3, 29, 15, 37, 5, 640, DateTimeKind.Utc).AddTicks(3391), false, null, "OPERATOR@TEST.COM", "OPERATOR@TEST.COM", "AQAAAAIAAYagAAAAEKTzFgkQu7Xi4hVDtVMGPT4o68eZZuIgPXO2QWxHKduFZBU5NRfsTxkssVTq0VUo/g==", "4554321098", false, "OperatorSeedSecurityStamp", false, "Operator@Test.com" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "PublisherId", "AccountId", "Name" },
                values: new object[,]
                {
                    { new Guid("018e8adb-840f-77a8-8bb4-2180cf904f1d"), new Guid("018e8adb-8368-70c1-8252-cd2b5e9bc3f1"), "Gyldendal" },
                    { new Guid("018e8adb-840f-77a9-ae7a-5fde355e930f"), new Guid("018e8adb-8368-70c2-94db-5b205df0edcf"), "Forlaget Als" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AccountId", "Birthday", "Name", "Surname" },
                values: new object[] { new Guid("018e8adb-840f-77c0-aed1-9886932fcbbe"), new Guid("018e8adb-8368-70c0-aa9f-b47e2880ff5c"), new DateOnly(1993, 4, 12), "Thomas", "Berlin" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "BookContent", "ContentTypeId", "CoverImage", "IsHidden", "Length", "PublicationDate", "PublisherId", "Title" },
                values: new object[,]
                {
                    { new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"), new byte[0], new Guid("018e8adb-840f-77a6-8fba-a2058e995998"), null, false, 3600.0, new DateOnly(1955, 12, 1), new Guid("018e8adb-840f-77a8-8bb4-2180cf904f1d"), "My day in the shoos of Tommy" },
                    { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new byte[0], new Guid("018e8adb-840f-77a7-8af8-4e9efba397ca"), null, false, 123.0, new DateOnly(2023, 4, 7), new Guid("018e8adb-840f-77a9-ae7a-5fde355e930f"), "My horse is the wildest" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { new Guid("018e8adb-840f-77a3-ac65-5f1e56d59cd3"), new Guid("018e8adb-840f-77be-9537-e2fd5052b35b") },
                    { new Guid("018e8adb-840f-77a4-8274-f813dfa6df3e"), new Guid("018e8adb-840f-77bf-afe4-c86000cada32") },
                    { new Guid("018e8adb-840f-77a5-a9f0-8d921238f0f2"), new Guid("018e8adb-840f-77bf-afe4-c86000cada32") }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                values: new object[,]
                {
                    { new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"), new Guid("018e8adb-840f-77aa-930a-83f7b2f0cb73") },
                    { new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"), new Guid("018e8adb-840f-77ac-9c19-5a943cfdf7d7") },
                    { new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"), new Guid("018e8adb-840f-77b2-97b2-ec9f5a55663b") },
                    { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77ab-8d6c-8171a947e74d") },
                    { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77ae-801c-f7761b058c5c") },
                    { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77af-8876-7a7b3b128ac5") },
                    { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77b1-b88c-bf08b77ad230") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8adb-8368-70b8-9718-466effd86b3b"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8adb-8368-70b9-a6b3-038c867c2c74"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8adb-8368-70ba-9989-10b795d98bcf"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("018e8adb-8368-70bb-958e-44661f184474"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8adb-8368-70bf-b88c-9d8337f0ed7f"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8adb-8368-70c3-995b-1c5ff20e5986"));

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77a3-ac65-5f1e56d59cd3"), new Guid("018e8adb-840f-77be-9537-e2fd5052b35b") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77a4-8274-f813dfa6df3e"), new Guid("018e8adb-840f-77bf-afe4-c86000cada32") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77a5-a9f0-8d921238f0f2"), new Guid("018e8adb-840f-77bf-afe4-c86000cada32") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"), new Guid("018e8adb-840f-77aa-930a-83f7b2f0cb73") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"), new Guid("018e8adb-840f-77ac-9c19-5a943cfdf7d7") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"), new Guid("018e8adb-840f-77b2-97b2-ec9f5a55663b") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77ab-8d6c-8171a947e74d") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77ae-801c-f7761b058c5c") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77af-8876-7a7b3b128ac5") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("018e8adb-840f-77bf-afe4-c86000cada32"), new Guid("018e8adb-840f-77b1-b88c-bf08b77ad230") });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77ad-bc45-ef8eb5f728d9"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b0-9a88-6879bbf73fab"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b3-a2f5-28897b4979a8"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b4-9a27-8ae54fa760ff"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b5-8cc9-629f12944b07"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b6-a49e-24c3cf49f7f6"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b7-b8fb-e33165e8ed6e"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b8-af35-4c46b0a02646"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b9-aa42-02c964da8b21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77ba-b89b-48361e4499b8"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77bb-85f1-691c1b8cbd59"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77bc-a0b5-683285074af9"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77bd-b255-6873e01aac3a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("018e8adb-840f-77c0-aed1-9886932fcbbe"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8adb-8368-70be-9081-9f04ff23a4b1"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8adb-8368-70c0-aa9f-b47e2880ff5c"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("018e8adb-840f-77a3-ac65-5f1e56d59cd3"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("018e8adb-840f-77a4-8274-f813dfa6df3e"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("018e8adb-840f-77a5-a9f0-8d921238f0f2"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("018e8adb-840f-77be-9537-e2fd5052b35b"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("018e8adb-840f-77bf-afe4-c86000cada32"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77aa-930a-83f7b2f0cb73"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77ab-8d6c-8171a947e74d"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77ac-9c19-5a943cfdf7d7"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77ae-801c-f7761b058c5c"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77af-8876-7a7b3b128ac5"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b1-b88c-bf08b77ad230"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("018e8adb-840f-77b2-97b2-ec9f5a55663b"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8adb-8368-70bc-a239-436bd0b16077"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("018e8adb-840f-77a6-8fba-a2058e995998"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("018e8adb-840f-77a7-8af8-4e9efba397ca"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("018e8adb-840f-77a8-8bb4-2180cf904f1d"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("018e8adb-840f-77a9-ae7a-5fde355e930f"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8adb-8368-70c1-8252-cd2b5e9bc3f1"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("018e8adb-8368-70c2-94db-5b205df0edcf"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("018e8adb-8368-70bd-8393-772206d0123f"));
        }
    }
}
