using Microsoft.EntityFrameworkCore.Migrations;

namespace LivrariaRomana.Infrastructure.Migrations
{
    public partial class alteracaoNomePropriedadesId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Usuarios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LivroID",
                table: "Livros",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuarios",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Livros",
                newName: "LivroID");
        }
    }
}
