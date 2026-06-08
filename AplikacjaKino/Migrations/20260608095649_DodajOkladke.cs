using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplikacjaKino.Migrations
{
    /// <inheritdoc />
    public partial class DodajOkladke : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Okladka",
                table: "Filmy",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Okladka",
                table: "Filmy");
        }
    }
}
