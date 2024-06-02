using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisualEditorAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDesignRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designs_Users_UserId1",
                table: "Designs");

            migrationBuilder.DropIndex(
                name: "IX_Designs_UserId1",
                table: "Designs");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Designs");

            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Designs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Designs_UserId",
                table: "Designs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designs_Users_UserId",
                table: "Designs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designs_Users_UserId",
                table: "Designs");

            migrationBuilder.DropIndex(
                name: "IX_Designs_UserId",
                table: "Designs");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Designs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Designs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Designs_UserId1",
                table: "Designs",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Designs_Users_UserId1",
                table: "Designs",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
