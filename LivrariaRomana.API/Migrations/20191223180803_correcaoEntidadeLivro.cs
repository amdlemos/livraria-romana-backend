using Microsoft.EntityFrameworkCore.Migrations;

namespace LivrariaRomana.Migrations
{
    public partial class correcaoEntidadeLivro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AnoPublicacao",
                table: "Livros",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AnoPublicacao",
                table: "Livros",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
