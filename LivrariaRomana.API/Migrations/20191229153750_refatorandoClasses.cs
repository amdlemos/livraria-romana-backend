using Microsoft.EntityFrameworkCore.Migrations;

namespace LivrariaRomana.Migrations
{
    public partial class refatorandoClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Senha",
                table: "Usuarios",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Usuarios",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "TituloOriginal",
                table: "Livros",
                newName: "PublishingCompany");

            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "Livros",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Editora",
                table: "Livros",
                newName: "OriginalTitle");

            migrationBuilder.RenameColumn(
                name: "Autor",
                table: "Livros",
                newName: "Author");

            migrationBuilder.RenameColumn(
                name: "AnoPublicacao",
                table: "Livros",
                newName: "PublicationYear");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Usuarios",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Usuarios",
                newName: "Senha");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Livros",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "PublishingCompany",
                table: "Livros",
                newName: "TituloOriginal");

            migrationBuilder.RenameColumn(
                name: "PublicationYear",
                table: "Livros",
                newName: "AnoPublicacao");

            migrationBuilder.RenameColumn(
                name: "OriginalTitle",
                table: "Livros",
                newName: "Editora");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Livros",
                newName: "Autor");
        }
    }
}
