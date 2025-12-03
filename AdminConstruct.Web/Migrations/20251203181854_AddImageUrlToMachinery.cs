using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminConstruct.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToMachinery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Machineries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Machineries");
        }
    }
}
