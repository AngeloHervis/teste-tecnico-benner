using Microsoft.EntityFrameworkCore.Migrations;
using Microondas.Infrastructure.Migrations.Seeds;

#nullable disable

namespace Microondas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramasAquecimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Alimento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TempoSegundos = table.Column<int>(type: "int", nullable: false),
                    Potencia = table.Column<int>(type: "int", nullable: false),
                    CaractereAquecimento = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Instrucoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EhPadrao = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasAquecimento", x => x.Id);
                });

            // SEED DATA
            ProgramasPadraoSeed.Up(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramasAquecimento");
        }
    }
}
