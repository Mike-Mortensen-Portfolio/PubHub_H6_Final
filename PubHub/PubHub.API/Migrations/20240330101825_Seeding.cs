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
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(258)",
                oldMaxLength: 258);

            migrationBuilder.InsertData(
                table: "AccessTypes",
                columns: new[] { "AccessTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("d3a846d8-1910-87db-8211-018e8ede1ce8"), "Owner" },
                    { new Guid("f6f30f22-de53-8826-8212-018e8ede1ce8"), "Subscriber" },
                    { new Guid("9e7964a5-454e-8c03-8213-018e8ede1ce8"), "Borrower" },
                    { new Guid("de400bc3-448c-8dcc-8214-018e8ede1ce8"), "Expired" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("164b7367-a253-8642-8215-018e8ede1ce8"), "User" },
                    { new Guid("0301188b-b4e8-8994-8216-018e8ede1ce8"), "Publisher" },
                    { new Guid("bcc27f6e-e1df-8d68-8217-018e8ede1ce8"), "Operator" },
                    { new Guid("eb70cfb3-af43-8050-8218-018e8ede1ce8"), "Suspended" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { new Guid("dd63af00-f993-83f8-8570-018e8ede1d91"), "Jhon Doe" },
                    { new Guid("c9e6e5fd-2f07-8a15-8571-018e8ede1d91"), "Jane Doe" },
                    { new Guid("afbe5a10-baf7-8f84-8572-018e8ede1d91"), "Dan Turéll" }
                });

            migrationBuilder.InsertData(
                table: "ContentTypes",
                columns: new[] { "ContentTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("fb57133f-4319-8bbb-8573-018e8ede1d91"), "AudioBook" },
                    { new Guid("484f3814-65de-8400-8574-018e8ede1d91"), "EBook" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "Name" },
                values: new object[,]
                {
                    { new Guid("65d28176-c907-856e-8577-018e8ede1d91"), "Romance" },
                    { new Guid("075cc537-cb1f-80a4-8578-018e8ede1d91"), "Horror" },
                    { new Guid("95bfcbee-d141-80c3-8579-018e8ede1d91"), "History" },
                    { new Guid("509cc50f-b7d3-86f8-857a-018e8ede1d91"), "Science-Fiction" },
                    { new Guid("b58ad95a-4493-83c8-857b-018e8ede1d91"), "Fiction" },
                    { new Guid("32361c8d-0087-8b9d-857c-018e8ede1d91"), "Novel" },
                    { new Guid("cb0be81d-0ae3-8cd5-857d-018e8ede1d91"), "Fantasy" },
                    { new Guid("9187b3fb-e126-865b-857e-018e8ede1d91"), "Biography" },
                    { new Guid("67fef281-2514-84c5-857f-018e8ede1d91"), "True crime" },
                    { new Guid("db4624b4-ccc9-8d05-8580-018e8ede1d91"), "Thriller" },
                    { new Guid("46c26d0c-c613-8520-8581-018e8ede1d91"), "Young adult" },
                    { new Guid("90b2e2a9-36c2-80b5-8582-018e8ede1d91"), "Mystery" },
                    { new Guid("38c6a254-53ba-8879-8583-018e8ede1d91"), "Satire" },
                    { new Guid("7a3a0202-2438-8819-8584-018e8ede1d91"), "Non-Fiction" },
                    { new Guid("448dd5dc-c054-867f-8585-018e8ede1d91"), "Self-help" },
                    { new Guid("2f5c9884-459d-8434-8586-018e8ede1d91"), "Poetry" },
                    { new Guid("23ca4076-f3f1-8082-8587-018e8ede1d91"), "Humor" },
                    { new Guid("aeb5c93d-bf25-8683-8588-018e8ede1d91"), "Action" },
                    { new Guid("f9b2663d-9e1c-8694-8589-018e8ede1d91"), "Adventure" },
                    { new Guid("9fa1a85a-1b51-831d-858a-018e8ede1d91"), "Short story" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccessFailedCount", "AccountTypeId", "ConcurrencyStamp", "DeletedDate", "Email", "EmailConfirmed", "LastSignIn", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("bb6b1344-1429-86f4-8219-018e8ede1ce8"), 0, new Guid("164b7367-a253-8642-8215-018e8ede1ce8"), "UserSeedConcurrencyStamp", null, "User@Test.com", true, new DateTime(2024, 3, 30, 10, 18, 24, 872, DateTimeKind.Utc).AddTicks(7035), false, null, "USER@TEST.COM", "USER@TEST.COM", "AQAAAAIAAYagAAAAEAqnOr8BP0I2r+LvZ2Obf94k1LsgXpBe/bDJhbSy9VbEtT7DBwnGe6SGvGJPapZKpQ==", "4587654321", false, "UserSeedSecurityStamp", false, "User@Test.com" },
                    { new Guid("f3c1eeba-3945-8222-821a-018e8ede1ce8"), 0, new Guid("0301188b-b4e8-8994-8216-018e8ede1ce8"), "PublisherSeedConcurrencyStamp", null, "Publisher@Test.com", true, new DateTime(2024, 3, 30, 10, 18, 24, 872, DateTimeKind.Utc).AddTicks(7052), false, null, "PUBLISHER@TEST.COM", "PUBLISHER@TEST.COM", "AQAAAAIAAYagAAAAEApCq/20dsXSv+d/npYSxMaTjZmOdNCt36tEOt9FTjUxhOpp0qfBQdOid77ZliRTAQ==", "4576543210", false, "PublisherSeedSecurityStamp", false, "Publisher@Test.com" },
                    { new Guid("3ba560f7-c7b9-8dd1-821b-018e8ede1ce8"), 0, new Guid("0301188b-b4e8-8994-8216-018e8ede1ce8"), "Publisher2SeedConcurrencyStamp", null, "Publisher2@Test.com", true, new DateTime(2024, 3, 30, 10, 18, 24, 872, DateTimeKind.Utc).AddTicks(7063), false, null, "PUBLISHER2@TEST.COM", "PUBLISHER2@TEST.COM", "AQAAAAIAAYagAAAAEMFkR3SjBCvVlYs4raezqupzaTetZE2QZMSCtgLI39/vHOs4doGcBEY6wlC6KIc0fg==", "4565432109", false, "Publisher2SeedSecurityStamp", false, "Publisher2@Test.com" },
                    { new Guid("a7394e42-4a1d-8cc2-821c-018e8ede1ce8"), 0, new Guid("bcc27f6e-e1df-8d68-8217-018e8ede1ce8"), "OperatorSeedConcurrencyStamp", null, "Operator@Test.com", true, new DateTime(2024, 3, 30, 10, 18, 24, 872, DateTimeKind.Utc).AddTicks(7073), false, null, "OPERATOR@TEST.COM", "OPERATOR@TEST.COM", "AQAAAAIAAYagAAAAEJnI1edVj9ybEAW5riZhKq1+zr2l2XzOpnE5dk72g6gPBoSMfC6qG01VTDqvK1p/jw==", "4554321098", false, "OperatorSeedSecurityStamp", false, "Operator@Test.com" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "PublisherId", "AccountId", "Name" },
                values: new object[,]
                {
                    { new Guid("7bf64e8c-9a22-8299-8575-018e8ede1d91"), new Guid("f3c1eeba-3945-8222-821a-018e8ede1ce8"), "Gyldendal" },
                    { new Guid("bfbbc413-0444-8678-8576-018e8ede1d91"), new Guid("3ba560f7-c7b9-8dd1-821b-018e8ede1ce8"), "Forlaget Als" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AccountId", "Birthday", "Name", "Surname" },
                values: new object[] { new Guid("4b6a1736-b941-8c6b-858d-018e8ede1d91"), new Guid("bb6b1344-1429-86f4-8219-018e8ede1ce8"), new DateOnly(1993, 4, 12), "Thomas", "Berlin" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "BookContent", "ContentTypeId", "CoverImage", "IsHidden", "Length", "PublicationDate", "PublisherId", "Title" },
                values: new object[,]
                {
                    { new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"), new byte[0], new Guid("fb57133f-4319-8bbb-8573-018e8ede1d91"), null, false, 3600.0, new DateOnly(1955, 12, 1), new Guid("7bf64e8c-9a22-8299-8575-018e8ede1d91"), "My day in the shoos of Tommy" },
                    { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new byte[0], new Guid("484f3814-65de-8400-8574-018e8ede1d91"), null, false, 123.0, new DateOnly(2023, 4, 7), new Guid("bfbbc413-0444-8678-8576-018e8ede1d91"), "My horse is the wildest" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { new Guid("dd63af00-f993-83f8-8570-018e8ede1d91"), new Guid("d30633ce-197f-84d3-858b-018e8ede1d91") },
                    { new Guid("c9e6e5fd-2f07-8a15-8571-018e8ede1d91"), new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91") },
                    { new Guid("afbe5a10-baf7-8f84-8572-018e8ede1d91"), new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91") }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                values: new object[,]
                {
                    { new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"), new Guid("65d28176-c907-856e-8577-018e8ede1d91") },
                    { new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"), new Guid("67fef281-2514-84c5-857f-018e8ede1d91") },
                    { new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"), new Guid("95bfcbee-d141-80c3-8579-018e8ede1d91") },
                    { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("075cc537-cb1f-80a4-8578-018e8ede1d91") },
                    { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("32361c8d-0087-8b9d-857c-018e8ede1d91") },
                    { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("9187b3fb-e126-865b-857e-018e8ede1d91") },
                    { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("b58ad95a-4493-83c8-857b-018e8ede1d91") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("9e7964a5-454e-8c03-8213-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("d3a846d8-1910-87db-8211-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("de400bc3-448c-8dcc-8214-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "AccessTypes",
                keyColumn: "AccessTypeId",
                keyValue: new Guid("f6f30f22-de53-8826-8212-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("eb70cfb3-af43-8050-8218-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("a7394e42-4a1d-8cc2-821c-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("afbe5a10-baf7-8f84-8572-018e8ede1d91"), new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("c9e6e5fd-2f07-8a15-8571-018e8ede1d91"), new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { new Guid("dd63af00-f993-83f8-8570-018e8ede1d91"), new Guid("d30633ce-197f-84d3-858b-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("075cc537-cb1f-80a4-8578-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("32361c8d-0087-8b9d-857c-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("9187b3fb-e126-865b-857e-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"), new Guid("b58ad95a-4493-83c8-857b-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"), new Guid("65d28176-c907-856e-8577-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"), new Guid("67fef281-2514-84c5-857f-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"), new Guid("95bfcbee-d141-80c3-8579-018e8ede1d91") });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("23ca4076-f3f1-8082-8587-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("2f5c9884-459d-8434-8586-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("38c6a254-53ba-8879-8583-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("448dd5dc-c054-867f-8585-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("46c26d0c-c613-8520-8581-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("509cc50f-b7d3-86f8-857a-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("7a3a0202-2438-8819-8584-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("90b2e2a9-36c2-80b5-8582-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("9fa1a85a-1b51-831d-858a-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("aeb5c93d-bf25-8683-8588-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("cb0be81d-0ae3-8cd5-857d-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("db4624b4-ccc9-8d05-8580-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("f9b2663d-9e1c-8694-8589-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("4b6a1736-b941-8c6b-858d-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("bcc27f6e-e1df-8d68-8217-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("bb6b1344-1429-86f4-8219-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("afbe5a10-baf7-8f84-8572-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("c9e6e5fd-2f07-8a15-8571-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: new Guid("dd63af00-f993-83f8-8570-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("02fa7e3a-0a16-83f5-858c-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: new Guid("d30633ce-197f-84d3-858b-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("075cc537-cb1f-80a4-8578-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("32361c8d-0087-8b9d-857c-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("65d28176-c907-856e-8577-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("67fef281-2514-84c5-857f-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("9187b3fb-e126-865b-857e-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("95bfcbee-d141-80c3-8579-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: new Guid("b58ad95a-4493-83c8-857b-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("164b7367-a253-8642-8215-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("484f3814-65de-8400-8574-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "ContentTypes",
                keyColumn: "ContentTypeId",
                keyValue: new Guid("fb57133f-4319-8bbb-8573-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("7bf64e8c-9a22-8299-8575-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "PublisherId",
                keyValue: new Guid("bfbbc413-0444-8678-8576-018e8ede1d91"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("3ba560f7-c7b9-8dd1-821b-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("f3c1eeba-3945-8222-821a-018e8ede1ce8"));

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "AccountTypeId",
                keyValue: new Guid("0301188b-b4e8-8994-8216-018e8ede1ce8"));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(258)",
                maxLength: 258,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
