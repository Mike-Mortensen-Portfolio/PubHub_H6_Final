using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubHub.API.Migrations
{
    /// <inheritdoc />
    public partial class Constraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operators_Accounts_AccountId",
                table: "Operators");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_Users_UserId",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_Operators_AccountId",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "Users",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Publishers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "Operators",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Operators",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Operators",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Genres",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ContentTypes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(258)",
                maxLength: 258,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Authors",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccountTypes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSignIn",
                table: "Accounts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1,
                columns: new[] { "DeletedDate", "LastSignIn", "PasswordHash" },
                values: new object[] { null, new DateTime(2024, 3, 29, 7, 19, 44, 421, DateTimeKind.Utc).AddTicks(6442), "AQAAAAIAAYagAAAAEFhkCwcV9e752vkTUPwum1GquIPAofTBGnmqktNUQhki/IM4u9E3iy3DTIZkFBTASg==" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                columns: new[] { "DeletedDate", "LastSignIn", "PasswordHash" },
                values: new object[] { null, new DateTime(2024, 3, 29, 7, 19, 44, 421, DateTimeKind.Utc).AddTicks(6456), "AQAAAAIAAYagAAAAEJIu5MlQOtlNwrmNGwTUapprj2drep+HthzMK9AVNahCk2jRj0vMkaeuq29Xdpsj3Q==" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3,
                columns: new[] { "DeletedDate", "LastSignIn", "PasswordHash" },
                values: new object[] { null, new DateTime(2024, 3, 29, 7, 19, 44, 421, DateTimeKind.Utc).AddTicks(6465), "AQAAAAIAAYagAAAAECPgqEFXiR5BA+asfaF7GmosbfP+LZOoknxrFxpOqZ6o0Be/dVWg/3URKfgc2fnlog==" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 4,
                columns: new[] { "DeletedDate", "LastSignIn", "PasswordHash" },
                values: new object[] { null, new DateTime(2024, 3, 29, 7, 19, 44, 421, DateTimeKind.Utc).AddTicks(6480), "AQAAAAIAAYagAAAAEKp0hFbOmQx4yMLAgemv8+ex/Zeqn0L5BIkIpMqC4Z0wV0dPESptBbd5AsI1J+AxEw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Operators_AccountId",
                table: "Operators",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Operators_Accounts_AccountId",
                table: "Operators",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_Users_UserId",
                table: "UserBooks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operators_Accounts_AccountId",
                table: "Operators");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_Users_UserId",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_Operators_AccountId",
                table: "Operators");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Email",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "Operators",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Operators",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Operators",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Genres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ContentTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(258)",
                oldMaxLength: 258);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccountTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSignIn",
                table: "Accounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1,
                columns: new[] { "IsDeleted", "LastSignIn", "PasswordHash" },
                values: new object[] { false, new DateTime(2024, 3, 27, 18, 6, 6, 470, DateTimeKind.Utc).AddTicks(7322), "AQAAAAIAAYagAAAAEFVEcf52tlH5E9OXHgL2yEz5agNzRtNJmYCkfJs9A0748BbuQy8Yav2JLGlJPLCb9Q==" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                columns: new[] { "IsDeleted", "LastSignIn", "PasswordHash" },
                values: new object[] { false, new DateTime(2024, 3, 27, 18, 6, 6, 470, DateTimeKind.Utc).AddTicks(7332), "AQAAAAIAAYagAAAAECMpPT3C0kI93KIxVFeR7zFXs/f1nunIkF1+VRUTjRJkGXg2i5VIBNI895PgFC0WnQ==" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3,
                columns: new[] { "IsDeleted", "LastSignIn", "PasswordHash" },
                values: new object[] { false, new DateTime(2024, 3, 27, 18, 6, 6, 470, DateTimeKind.Utc).AddTicks(7338), "AQAAAAIAAYagAAAAEA2yN3qdlNRY38oAVFPoih3794O/EWyH2sHf+niLPRlz8AZV2L3KngauSLGmfhqlhw==" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 4,
                columns: new[] { "IsDeleted", "LastSignIn", "PasswordHash" },
                values: new object[] { false, new DateTime(2024, 3, 27, 18, 6, 6, 470, DateTimeKind.Utc).AddTicks(7379), "AQAAAAIAAYagAAAAEAQPESejzk6Wt6Tm5a3VLuCBxTnCJDLcMkkBvoNzcD2153zZyf7z5rIS9iweuJUbPw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Operators_AccountId",
                table: "Operators",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Operators_Accounts_AccountId",
                table: "Operators",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_Users_UserId",
                table: "UserBooks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
