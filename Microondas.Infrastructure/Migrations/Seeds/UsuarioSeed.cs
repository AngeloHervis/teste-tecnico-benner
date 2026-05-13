using Microsoft.EntityFrameworkCore.Migrations;

namespace Microondas.Infrastructure.Migrations.Seeds;

public static class UsuarioSeed
{
    public static void Up(MigrationBuilder migrationBuilder)
    {
        InserirUsuarioAdmin(migrationBuilder);
    }

    private static void InserirUsuarioAdmin(MigrationBuilder migrationBuilder)
    {
        const string senhaHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92";

        migrationBuilder.InsertData(
            table: "Usuarios",
            columns: ["Nome", "SenhaHash", "DataCriacao", "Ativo"],
            values: new object[,]
            {
                { "admin", senhaHash, DateTime.UtcNow, true }
            });
    }
}
