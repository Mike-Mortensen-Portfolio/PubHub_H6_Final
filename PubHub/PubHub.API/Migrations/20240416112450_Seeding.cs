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
                    { new Guid("19fe08ed-e16b-88e4-8632-018ee6a70583"), "Borrower" },
                    { new Guid("a2e4ee8e-f9fa-8503-8631-018ee6a70583"), "Subscriber" },
                    { new Guid("d8684ddb-25b4-802b-8633-018ee6a70583"), "Expired" },
                    { new Guid("ed9e0588-ec9d-8128-8630-018ee6a70583"), "Owner" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("69d8cb70-da6c-8d69-8634-018ee6a70583"), "User" },
                    { new Guid("898f9a3a-e0bd-85ab-8636-018ee6a70583"), "Operator" },
                    { new Guid("9b7de9c4-8bb4-8893-8637-018ee6a70583"), "Suspended" },
                    { new Guid("a321a671-396d-8850-8635-018ee6a70583"), "Publisher" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { new Guid("967d8335-aca6-8ed7-8632-018ee6a70661"), "Jhon Doe" },
                    { new Guid("c9050d9a-b650-80a0-8633-018ee6a70661"), "Jane Doe" },
                    { new Guid("fd94223f-4cf2-8d32-8634-018ee6a70661"), "Dan Turéll" }
                });

            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "ContentTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("3acf1aa9-9c59-88f9-8635-018ee6a70661"), "AudioBook" },
                    { new Guid("6784f6c1-53c3-8ccb-8636-018ee6a70661"), "EBook" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "Name" },
                values: new object[,]
                {
                    { new Guid("0e495e0f-3540-80a5-8642-018ee6a70661"), "Thriller" },
                    { new Guid("1911490d-6992-884b-863f-018ee6a70661"), "Fantasy" },
                    { new Guid("1b5281d7-9a59-86ae-8646-018ee6a70661"), "Non-Fiction" },
                    { new Guid("50ce0144-2508-8a0b-863a-018ee6a70661"), "Horror" },
                    { new Guid("529f0143-3da5-83be-8649-018ee6a70661"), "Humor" },
                    { new Guid("5c82852d-69f6-8c86-863b-018ee6a70661"), "History" },
                    { new Guid("5f11dac9-1096-846f-864c-018ee6a70661"), "Short story" },
                    { new Guid("603dabab-ddf7-8c54-863d-018ee6a70661"), "Fiction" },
                    { new Guid("6a9135be-d397-8a7a-864b-018ee6a70661"), "Adventure" },
                    { new Guid("749e96f4-6b3d-88fd-8639-018ee6a70661"), "Romance" },
                    { new Guid("7605691d-f950-8810-8640-018ee6a70661"), "Biography" },
                    { new Guid("89e42f8e-3e8a-8003-8647-018ee6a70661"), "Self-help" },
                    { new Guid("8e774635-8f1b-882b-8645-018ee6a70661"), "Satire" },
                    { new Guid("986a330a-e655-8719-8648-018ee6a70661"), "Poetry" },
                    { new Guid("a726940e-398d-82cb-863c-018ee6a70661"), "Science-Fiction" },
                    { new Guid("a79394b0-5e75-8dea-864a-018ee6a70661"), "Action" },
                    { new Guid("c0d0a84f-ac3e-862d-8643-018ee6a70661"), "Young adult" },
                    { new Guid("ccb45f43-ab7e-8e3e-8644-018ee6a70661"), "Mystery" },
                    { new Guid("d136e723-8382-8021-863e-018ee6a70661"), "Novel" },
                    { new Guid("ec723f94-ebc7-8cdb-8641-018ee6a70661"), "True crime" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccessFailedCount", "AccountTypeId", "ConcurrencyStamp", "DeletedDate", "Email", "EmailConfirmed", "LastSignIn", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("3953e976-a0af-81ae-863a-018ee6a70583"), 0, new Guid("a321a671-396d-8850-8635-018ee6a70583"), "Publisher2SeedConcurrencyStamp", null, "Publisher2@Test.com", true, new DateTime(2024, 4, 16, 11, 24, 49, 411, DateTimeKind.Utc).AddTicks(3826), true, null, "PUBLISHER2@TEST.COM", "PUBLISHER2@TEST.COM", "AQAAAAIAAYagAAAAEBZyFaUk1E9BUO9cZwccloPsxq5nQoizg2PH0sZM8M0SxjQaUDg+MLhaJLp6c4+KKQ==", "4565432109", false, "Publisher2SeedSecurityStamp", false, "Publisher2@Test.com" },
                    { new Guid("4abd85b6-ace5-8e0c-8639-018ee6a70583"), 0, new Guid("a321a671-396d-8850-8635-018ee6a70583"), "PublisherSeedConcurrencyStamp", null, "Publisher@Test.com", true, new DateTime(2024, 4, 16, 11, 24, 49, 411, DateTimeKind.Utc).AddTicks(3811), true, null, "PUBLISHER@TEST.COM", "PUBLISHER@TEST.COM", "AQAAAAIAAYagAAAAENfIJ9poQz89UGOOtxEPaZd0CstDxMW2wsVMVoUxEFUrU6waMOQ/+SAOc6FUJV5x7A==", "4576543210", false, "PublisherSeedSecurityStamp", false, "Publisher@Test.com" },
                    { new Guid("c3eb20d0-0611-8a21-863b-018ee6a70583"), 0, new Guid("898f9a3a-e0bd-85ab-8636-018ee6a70583"), "OperatorSeedConcurrencyStamp", null, "Operator@Test.com", true, new DateTime(2024, 4, 16, 11, 24, 49, 411, DateTimeKind.Utc).AddTicks(3840), true, null, "OPERATOR@TEST.COM", "OPERATOR@TEST.COM", "AQAAAAIAAYagAAAAEMqM+XlmsM3mBxFGvoIVezJ7P7XXVJozqgLN9SMOnGCGk6L3PkgFVjNBxAgp+5RLMw==", "4554321098", false, "OperatorSeedSecurityStamp", false, "Operator@Test.com" },
                    { new Guid("c93c0065-b9be-8b6d-8638-018ee6a70583"), 0, new Guid("69d8cb70-da6c-8d69-8634-018ee6a70583"), "UserSeedConcurrencyStamp", null, "User@Test.com", true, new DateTime(2024, 4, 16, 11, 24, 49, 411, DateTimeKind.Utc).AddTicks(3786), true, null, "USER@TEST.COM", "USER@TEST.COM", "AQAAAAIAAYagAAAAEF4ijOoxqmvHcPSApOJZXPWRRQIaIf2+cOIxfDc8c2jKpDSCbJ5FJ/ffMcwReygRQA==", "4587654321", false, "UserSeedSecurityStamp", false, "User@Test.com" }
                });

            migrationBuilder.InsertData(
                table: "Operators",
                columns: new[] { "OperatorId", "AccountId", "Name", "Surname" },
                values: new object[] { new Guid("3208b196-86e2-8bfb-8651-018ee6a70661"), new Guid("c3eb20d0-0611-8a21-863b-018ee6a70583"), "Selena", "Gomez" });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "PublisherId", "AccountId", "Name" },
                values: new object[,]
                {
                    { new Guid("1e472e88-5472-8424-8638-018ee6a70661"), new Guid("3953e976-a0af-81ae-863a-018ee6a70583"), "Forlaget Als" },
                    { new Guid("b6104f24-dc6c-8bf5-8637-018ee6a70661"), new Guid("4abd85b6-ace5-8e0c-8639-018ee6a70583"), "Gyldendal" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AccountId", "Birthday", "Name", "Surname" },
                values: new object[] { new Guid("858e5f39-fee7-8cfb-8652-018ee6a70661"), new Guid("c93c0065-b9be-8b6d-8638-018ee6a70583"), new DateOnly(1993, 4, 12), "Thomas", "Berlin" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "BookContentUri", "ContentTypeId", "CoverImageUri", "IsHidden", "Length", "PublicationDate", "PublisherId", "Summary", "Title" },
                values: new object[,]
                {
                    { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), "Domain\\Seeding\\Files\\skyggespil.epub", new Guid("6784f6c1-53c3-8ccb-8636-018ee6a70661"), "Domain\\Seeding\\Files\\horse.jpg", false, 123.0, new DateOnly(2023, 4, 7), new Guid("1e472e88-5472-8424-8638-018ee6a70661"), "Kentucky, 1850. An enslaved groom named Jarret and a bay foal forge a bond of understanding that will carry the horse to record-setting victories across the South. When the nation erupts in civil war, an itinerant young artist who has made his name on paintings of the racehorse takes up arms for the Union.", "Horse" },
                    { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), "Domain\\Seeding\\Files\\placeboeffekten.mp3", new Guid("3acf1aa9-9c59-88f9-8635-018ee6a70661"), "Domain\\Seeding\\Files\\horse.jpg", false, 123.0, new DateOnly(2023, 4, 7), new Guid("1e472e88-5472-8424-8638-018ee6a70661"), "Kentucky, 1850. An enslaved groom named Jarret and a bay foal forge a bond of understanding that will carry the horse to record-setting victories across the South. When the nation erupts in civil war, an itinerant young artist who has made his name on paintings of the racehorse takes up arms for the Union.", "Horse" },
                    { new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), "Domain\\Seeding\\Files\\placeboeffekten.mp3", new Guid("3acf1aa9-9c59-88f9-8635-018ee6a70661"), "Domain\\Seeding\\Files\\exquisite.jpeg", false, 3600.0, new DateOnly(2018, 7, 1), new Guid("b6104f24-dc6c-8bf5-8637-018ee6a70661"), "The exquisite creations embody our best artistic endeavors, and offer a glimpse of the greatness of the civilizations that produced them. Their sheer beauty and charm are enough for us to marvel at, let alone the large sum of resources, efforts and time poured into making them. In this book, we hope to follow our predecessors' footprints in the endless pursuit of exquisite beauty, and to explore the possibilities of how this style might blaze new trails in today's graphic design world.", "Exquisite" },
                    { new Guid("e0ecbef2-ff07-867e-8650-018ee6a70661"), "Domain\\Seeding\\Files\\skyggespil.epub", new Guid("6784f6c1-53c3-8ccb-8636-018ee6a70661"), "Domain\\Seeding\\Files\\skyggespil.png", false, 344.0, new DateOnly(2021, 9, 28), new Guid("b6104f24-dc6c-8bf5-8637-018ee6a70661"), "I 1600-tallets Aalborg straffes trolddom med døden, og før 14-årige Gry ved af det, sender en hekseanklage hende på bålet.\r\nMen de glubske flammer brænder hende ikke som de burde. \r\nSyttenårige Akela har accepteret dronning Soras tilbud om en plads ved hoffet.\r\nHer skal hun gennemføre skyggespillet; en række magiske dueller der afgør de Udvalgtes fremtid.\r\nKun ved at udmærke sig og opnå en plads som dronningens rådgiver kan Akela beskytte sin søster og forhindre at krigen rammer deres by. Desværre er der seks Udvalgte og blot én plads i hofrådet.\r\nMens oprørerne angriber, og bedrageriske skygger dukker op uden for skyggespillet, optrevler Akela den royale slægts hemmeligheder. Men hvem tør hun stole på når alle bekriger hinanden i dronningens spil?", "Shadow game" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { new Guid("c9050d9a-b650-80a0-8633-018ee6a70661"), new Guid("75d579a7-217e-8b28-864e-018ee6a70661") },
                    { new Guid("fd94223f-4cf2-8d32-8634-018ee6a70661"), new Guid("75d579a7-217e-8b28-864e-018ee6a70661") },
                    { new Guid("c9050d9a-b650-80a0-8633-018ee6a70661"), new Guid("7b235615-7afb-8f17-864f-018ee6a70661") },
                    { new Guid("fd94223f-4cf2-8d32-8634-018ee6a70661"), new Guid("7b235615-7afb-8f17-864f-018ee6a70661") },
                    { new Guid("967d8335-aca6-8ed7-8632-018ee6a70661"), new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661") }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                values: new object[,]
                {
                    { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("50ce0144-2508-8a0b-863a-018ee6a70661") },
                    { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("603dabab-ddf7-8c54-863d-018ee6a70661") },
                    { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("7605691d-f950-8810-8640-018ee6a70661") },
                    { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("d136e723-8382-8021-863e-018ee6a70661") },
                    { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("50ce0144-2508-8a0b-863a-018ee6a70661") },
                    { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("603dabab-ddf7-8c54-863d-018ee6a70661") },
                    { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("7605691d-f950-8810-8640-018ee6a70661") },
                    { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("d136e723-8382-8021-863e-018ee6a70661") },
                    { new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), new Guid("5c82852d-69f6-8c86-863b-018ee6a70661") },
                    { new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), new Guid("749e96f4-6b3d-88fd-8639-018ee6a70661") },
                    { new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), new Guid("ec723f94-ebc7-8cdb-8641-018ee6a70661") }
                });

            migrationBuilder.InsertData(
                table: "UserBooks",
                columns: new[] { "UserBookId", "AccessTypeId", "AcquireDate", "BookId", "ProgressInProcent", "UserId" },
                values: new object[,]
                {
                    { new Guid("0aaea5fa-5533-8d4b-8654-018ee6a70661"), new Guid("ed9e0588-ec9d-8128-8630-018ee6a70583"), new DateTime(2024, 4, 16, 11, 24, 49, 633, DateTimeKind.Utc).AddTicks(5200), new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), 0f, new Guid("858e5f39-fee7-8cfb-8652-018ee6a70661") },
                    { new Guid("86228abd-f2ac-8c3a-8653-018ee6a70661"), new Guid("ed9e0588-ec9d-8128-8630-018ee6a70583"), new DateTime(2024, 4, 16, 11, 24, 49, 633, DateTimeKind.Utc).AddTicks(5180), new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), 45.34f, new Guid("858e5f39-fee7-8cfb-8652-018ee6a70661") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("19fe08ed-e16b-88e4-8632-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("a2e4ee8e-f9fa-8503-8631-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("d8684ddb-25b4-802b-8633-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("9b7de9c4-8bb4-8893-8637-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("c9050d9a-b650-80a0-8633-018ee6a70661"), new Guid("75d579a7-217e-8b28-864e-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("fd94223f-4cf2-8d32-8634-018ee6a70661"), new Guid("75d579a7-217e-8b28-864e-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("c9050d9a-b650-80a0-8633-018ee6a70661"), new Guid("7b235615-7afb-8f17-864f-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("fd94223f-4cf2-8d32-8634-018ee6a70661"), new Guid("7b235615-7afb-8f17-864f-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("967d8335-aca6-8ed7-8632-018ee6a70661"), new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("50ce0144-2508-8a0b-863a-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("603dabab-ddf7-8c54-863d-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("7605691d-f950-8810-8640-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("75d579a7-217e-8b28-864e-018ee6a70661"), new Guid("d136e723-8382-8021-863e-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("50ce0144-2508-8a0b-863a-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("603dabab-ddf7-8c54-863d-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("7605691d-f950-8810-8640-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("7b235615-7afb-8f17-864f-018ee6a70661"), new Guid("d136e723-8382-8021-863e-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), new Guid("5c82852d-69f6-8c86-863b-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), new Guid("749e96f4-6b3d-88fd-8639-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"), new Guid("ec723f94-ebc7-8cdb-8641-018ee6a70661") });

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("e0ecbef2-ff07-867e-8650-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("0e495e0f-3540-80a5-8642-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("1911490d-6992-884b-863f-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("1b5281d7-9a59-86ae-8646-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("529f0143-3da5-83be-8649-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("5f11dac9-1096-846f-864c-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("6a9135be-d397-8a7a-864b-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("89e42f8e-3e8a-8003-8647-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("8e774635-8f1b-882b-8645-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("986a330a-e655-8719-8648-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("a726940e-398d-82cb-863c-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("a79394b0-5e75-8dea-864a-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("c0d0a84f-ac3e-862d-8643-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("ccb45f43-ab7e-8e3e-8644-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Operators",
                keyColumn: "OperatorId",
                keyValue: new Guid("3208b196-86e2-8bfb-8651-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "UserBooks",
                keyColumn: "UserBookId",
                keyValue: new Guid("0aaea5fa-5533-8d4b-8654-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "UserBooks",
                keyColumn: "UserBookId",
                keyValue: new Guid("86228abd-f2ac-8c3a-8653-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("ed9e0588-ec9d-8128-8630-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("c3eb20d0-0611-8a21-863b-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("967d8335-aca6-8ed7-8632-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("c9050d9a-b650-80a0-8633-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("fd94223f-4cf2-8d32-8634-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("75d579a7-217e-8b28-864e-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("7b235615-7afb-8f17-864f-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("81851b1b-bb47-8cb2-864d-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("50ce0144-2508-8a0b-863a-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("5c82852d-69f6-8c86-863b-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("603dabab-ddf7-8c54-863d-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("749e96f4-6b3d-88fd-8639-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("7605691d-f950-8810-8640-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("d136e723-8382-8021-863e-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("ec723f94-ebc7-8cdb-8641-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("858e5f39-fee7-8cfb-8652-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("898f9a3a-e0bd-85ab-8636-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("c93c0065-b9be-8b6d-8638-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("3acf1aa9-9c59-88f9-8635-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("6784f6c1-53c3-8ccb-8636-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("1e472e88-5472-8424-8638-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("b6104f24-dc6c-8bf5-8637-018ee6a70661"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("69d8cb70-da6c-8d69-8634-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("3953e976-a0af-81ae-863a-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("4abd85b6-ace5-8e0c-8639-018ee6a70583"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("a321a671-396d-8850-8635-018ee6a70583"));
        }
    }
}
