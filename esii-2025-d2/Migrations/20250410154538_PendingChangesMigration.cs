using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esii2025d2.Migrations
{
    /// <inheritdoc />
    public partial class PendingChangesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Talents_TalentCategories_TalentCategoryId",
                table: "Talents");

            migrationBuilder.AlterColumn<int>(
                name: "TalentCategoryId",
                table: "Talents",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Talents_TalentCategories_TalentCategoryId",
                table: "Talents",
                column: "TalentCategoryId",
                principalTable: "TalentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Talents_TalentCategories_TalentCategoryId",
                table: "Talents");

            migrationBuilder.AlterColumn<int>(
                name: "TalentCategoryId",
                table: "Talents",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Talents_TalentCategories_TalentCategoryId",
                table: "Talents",
                column: "TalentCategoryId",
                principalTable: "TalentCategories",
                principalColumn: "Id");
        }
    }
}
