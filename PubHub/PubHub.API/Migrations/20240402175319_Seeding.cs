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
                    { new Guid("d41ee5cc-b8b0-8ce2-84fd-018e9ff1ac7c"), "Owner" },
                    { new Guid("6dff0958-8df7-8300-84fe-018e9ff1ac7c"), "Subscriber" },
                    { new Guid("f5aac44e-13cd-871b-84ff-018e9ff1ac7c"), "Borrower" },
                    { new Guid("782dfc41-1f08-8f10-8500-018e9ff1ac7c"), "Expired" },
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("57642934-1ddc-8bdf-8501-018e9ff1ac7c"), "User" },
                    { new Guid("7eac98d8-581b-8c57-8502-018e9ff1ac7c"), "Publisher" },
                    { new Guid("c2cd6e19-e622-87e8-8503-018e9ff1ac7c"), "Operator" },
                    { new Guid("724ce772-b3fb-857a-8504-018e9ff1ac7c"), "Suspended" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { new Guid("fe878307-34f9-889f-8076-018e9ff1ad21"), "Jhon Doe" },
                    { new Guid("d538b4fe-88d3-8bbd-8077-018e9ff1ad21"), "Jane Doe" },
                    { new Guid("079b7377-5d38-8608-8078-018e9ff1ad21"), "Dan Turéll" }
                });

            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "ContentTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("f5242130-041a-841c-8079-018e9ff1ad21"), "AudioBook" },
                    { new Guid("e120b26c-3e66-8fdc-807a-018e9ff1ad21"), "EBook" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "Name" },
                values: new object[,]
                {
                    { new Guid("3dfc3cc3-d389-87e3-807d-018e9ff1ad21"), "Romance" },
                    { new Guid("f02768e2-9ae4-85e2-807e-018e9ff1ad21"), "Horror" },
                    { new Guid("bbabc883-e7cf-8ef1-807f-018e9ff1ad21"), "History" },
                    { new Guid("60005d07-dcc6-80b5-8080-018e9ff1ad21"), "Science-Fiction" },
                    { new Guid("1604e64d-1563-896b-8081-018e9ff1ad21"), "Fiction" },
                    { new Guid("6458feef-63ec-8931-8082-018e9ff1ad21"), "Novel" },
                    { new Guid("e986e1e3-884e-86f7-8083-018e9ff1ad21"), "Fantasy" },
                    { new Guid("71d570e1-1446-84cc-8084-018e9ff1ad21"), "Biography" },
                    { new Guid("27a3f8e9-a3ab-80eb-8085-018e9ff1ad21"), "True crime" },
                    { new Guid("a0d31d81-1e13-8a93-8086-018e9ff1ad21"), "Thriller" },
                    { new Guid("708d7d44-7a12-82f0-8087-018e9ff1ad21"), "Young adult" },
                    { new Guid("494bd699-da35-8a56-8088-018e9ff1ad21"), "Mystery" },
                    { new Guid("17b7772f-92ed-886a-8089-018e9ff1ad21"), "Satire" },
                    { new Guid("9586bd66-4955-8f62-808a-018e9ff1ad21"), "Non-Fiction" },
                    { new Guid("74dfcaaf-db0b-8d06-808b-018e9ff1ad21"), "Self-help" },
                    { new Guid("726e9c3f-24e1-811e-808c-018e9ff1ad21"), "Poetry" },
                    { new Guid("fd5f2dd9-0462-8ea5-808d-018e9ff1ad21"), "Humor" },
                    { new Guid("d8e22e20-8b18-8bba-808e-018e9ff1ad21"), "Action" },
                    { new Guid("7e548879-2dcc-87f7-808f-018e9ff1ad21"), "Adventure" },
                    { new Guid("8523a619-0f1e-8137-8090-018e9ff1ad21"), "Short story" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccessFailedCount", "AccountTypeId", "ConcurrencyStamp", "DeletedDate", "Email", "EmailConfirmed", "LastSignIn", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("5b601ae1-9562-8703-8505-018e9ff1ac7c"), 0, new Guid("57642934-1ddc-8bdf-8501-018e9ff1ac7c"), "UserSeedConcurrencyStamp", null, "User@Test.com", true, new DateTime(2024, 4, 2, 17, 53, 19, 484, DateTimeKind.Utc).AddTicks(5911), false, null, "USER@TEST.COM", "USER@TEST.COM", "AQAAAAIAAYagAAAAEESD+yaT2P/re408aSa0YUXzgUVtPMwYIiwVqOpHd5dBH/c4wlf7jhe09tn4mzFgNw==", "4587654321", false, "UserSeedSecurityStamp", false, "User@Test.com" },
                    { new Guid("806b46a3-8885-8504-8506-018e9ff1ac7c"), 0, new Guid("7eac98d8-581b-8c57-8502-018e9ff1ac7c"), "PublisherSeedConcurrencyStamp", null, "Publisher@Test.com", true, new DateTime(2024, 4, 2, 17, 53, 19, 484, DateTimeKind.Utc).AddTicks(5964), false, null, "PUBLISHER@TEST.COM", "PUBLISHER@TEST.COM", "AQAAAAIAAYagAAAAEKC74TSJUR+TPzpckRHy0bQQtBu4ElSDI9/kwaMrQvzjwvm4YiKUARstCyCYxHhmEA==", "4576543210", false, "PublisherSeedSecurityStamp", false, "Publisher@Test.com" },
                    { new Guid("f12a8721-fa3e-8ed4-8507-018e9ff1ac7c"), 0, new Guid("7eac98d8-581b-8c57-8502-018e9ff1ac7c"), "Publisher2SeedConcurrencyStamp", null, "Publisher2@Test.com", true, new DateTime(2024, 4, 2, 17, 53, 19, 484, DateTimeKind.Utc).AddTicks(5975), false, null, "PUBLISHER2@TEST.COM", "PUBLISHER2@TEST.COM", "AQAAAAIAAYagAAAAEP89obEc0wZ0tujy24iQ2oEmOp1GwI1PIZfijyb8Zdd+avsZ+A9ZCGQ9yxp8f/wtGw==", "4565432109", false, "Publisher2SeedSecurityStamp", false, "Publisher2@Test.com" },
                    { new Guid("3f8a2ba3-d1a3-8da5-8508-018e9ff1ac7c"), 0, new Guid("c2cd6e19-e622-87e8-8503-018e9ff1ac7c"), "OperatorSeedConcurrencyStamp", null, "Operator@Test.com", true, new DateTime(2024, 4, 2, 17, 53, 19, 484, DateTimeKind.Utc).AddTicks(5985), false, null, "OPERATOR@TEST.COM", "OPERATOR@TEST.COM", "AQAAAAIAAYagAAAAELE3Ve/69B2S7QrOIlbUt8MmLAJsPfoel+iDKKsGxOcdBikAo26f/C0DMU47HrK2ng==", "4554321098", false, "OperatorSeedSecurityStamp", false, "Operator@Test.com" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "PublisherId", "AccountId", "Name" },
                values: new object[,]
                {
                    { new Guid("f61cd634-0e73-8c8e-807b-018e9ff1ad21"), new Guid("806b46a3-8885-8504-8506-018e9ff1ac7c"), "Gyldendal" },
                    { new Guid("359b1d72-4725-8314-807c-018e9ff1ad21"), new Guid("f12a8721-fa3e-8ed4-8507-018e9ff1ac7c"), "Forlaget Als" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AccountId", "Birthday", "Name", "Surname" },
                values: new object[] { new Guid("610ee7e5-edcb-894e-8093-018e9ff1ad21"), new Guid("5b601ae1-9562-8703-8505-018e9ff1ac7c"), new DateOnly(1993, 4, 12), "Thomas", "Berlin" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "BookContent", "ContentTypeId", "CoverImage", "IsHidden", "Length", "PublicationDate", "PublisherId", "Title" },
                values: new object[,]
                {
                    { new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"), new byte[0], new Guid("f5242130-041a-841c-8079-018e9ff1ad21"), null, false, 3600.0, new DateOnly(1955, 12, 1), new Guid("f61cd634-0e73-8c8e-807b-018e9ff1ad21"), "My day in the shoos of Tommy" },
                    { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new byte[0], new Guid("e120b26c-3e66-8fdc-807a-018e9ff1ad21"), null, false, 123.0, new DateOnly(2023, 4, 7), new Guid("359b1d72-4725-8314-807c-018e9ff1ad21"), "My horse is the wildest" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { new Guid("fe878307-34f9-889f-8076-018e9ff1ad21"), new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21") },
                    { new Guid("d538b4fe-88d3-8bbd-8077-018e9ff1ad21"), new Guid("8263152a-2732-89b2-8092-018e9ff1ad21") },
                    { new Guid("079b7377-5d38-8608-8078-018e9ff1ad21"), new Guid("8263152a-2732-89b2-8092-018e9ff1ad21") }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                values: new object[,]
                {
                    { new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"), new Guid("27a3f8e9-a3ab-80eb-8085-018e9ff1ad21") },
                    { new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"), new Guid("3dfc3cc3-d389-87e3-807d-018e9ff1ad21") },
                    { new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"), new Guid("bbabc883-e7cf-8ef1-807f-018e9ff1ad21") },
                    { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("1604e64d-1563-896b-8081-018e9ff1ad21") },
                    { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("6458feef-63ec-8931-8082-018e9ff1ad21") },
                    { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("71d570e1-1446-84cc-8084-018e9ff1ad21") },
                    { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("f02768e2-9ae4-85e2-807e-018e9ff1ad21") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("6dff0958-8df7-8300-84fe-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("782dfc41-1f08-8f10-8500-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("d41ee5cc-b8b0-8ce2-84fd-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("f5aac44e-13cd-871b-84ff-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("724ce772-b3fb-857a-8504-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("3f8a2ba3-d1a3-8da5-8508-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("079b7377-5d38-8608-8078-018e9ff1ad21"), new Guid("8263152a-2732-89b2-8092-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("d538b4fe-88d3-8bbd-8077-018e9ff1ad21"), new Guid("8263152a-2732-89b2-8092-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("fe878307-34f9-889f-8076-018e9ff1ad21"), new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("1604e64d-1563-896b-8081-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("6458feef-63ec-8931-8082-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("71d570e1-1446-84cc-8084-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"), new Guid("f02768e2-9ae4-85e2-807e-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"), new Guid("27a3f8e9-a3ab-80eb-8085-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"), new Guid("3dfc3cc3-d389-87e3-807d-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"), new Guid("bbabc883-e7cf-8ef1-807f-018e9ff1ad21") });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("17b7772f-92ed-886a-8089-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("494bd699-da35-8a56-8088-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("60005d07-dcc6-80b5-8080-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("708d7d44-7a12-82f0-8087-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("726e9c3f-24e1-811e-808c-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("74dfcaaf-db0b-8d06-808b-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("7e548879-2dcc-87f7-808f-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("8523a619-0f1e-8137-8090-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("9586bd66-4955-8f62-808a-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("a0d31d81-1e13-8a93-8086-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("d8e22e20-8b18-8bba-808e-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("e986e1e3-884e-86f7-8083-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("fd5f2dd9-0462-8ea5-808d-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("610ee7e5-edcb-894e-8093-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("c2cd6e19-e622-87e8-8503-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("5b601ae1-9562-8703-8505-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("079b7377-5d38-8608-8078-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("d538b4fe-88d3-8bbd-8077-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("fe878307-34f9-889f-8076-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("8263152a-2732-89b2-8092-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("9e9ce66c-cea1-80ee-8091-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("1604e64d-1563-896b-8081-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("27a3f8e9-a3ab-80eb-8085-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("3dfc3cc3-d389-87e3-807d-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("6458feef-63ec-8931-8082-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("71d570e1-1446-84cc-8084-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("bbabc883-e7cf-8ef1-807f-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("f02768e2-9ae4-85e2-807e-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("57642934-1ddc-8bdf-8501-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("e120b26c-3e66-8fdc-807a-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("f5242130-041a-841c-8079-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("359b1d72-4725-8314-807c-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("f61cd634-0e73-8c8e-807b-018e9ff1ad21"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("806b46a3-8885-8504-8506-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("f12a8721-fa3e-8ed4-8507-018e9ff1ac7c"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("7eac98d8-581b-8c57-8502-018e9ff1ac7c"));
        }
    }
}
