using Microsoft.EntityFrameworkCore.Migrations;
using Microondas.Domain.Entities;

namespace Microondas.Infrastructure.Migrations.Seeds;

public static class ProgramasPadraoSeed
{
    public static void Up(MigrationBuilder migrationBuilder)
    {
        InserirProgramasPadrao(migrationBuilder);
    }

    private static void InserirProgramasPadrao(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "ProgramasAquecimento",
            columns: ["Nome", "Alimento", "TempoSegundos", "Potencia", "CaractereAquecimento", "Instrucoes", "EhPadrao"
            ],
            values: new object[,]
            {
                { "Pipoca", "Pipoca (de micro-ondas)", 120, 7, "*", "Observar o barulho de estouros do milho, caso o tempo entre um estouro e outro seja maior que 2 segundos interrompa o aquecimento.", true },
                { "Leite", "Leite", 300, 5, "!", "Cuidado com o aquecimento de líquidos pode ocorrer fervura retardada e causar queimaduras.", true },
                { "Carnes de Boi", "Carne em pedaços ou fatias", 840, 4, "@", "Interrompa o aquecimento na metade do tempo e vire os pedaços de carne para um aquecimento uniforme.", true },
                { "Frango", "Frango em pedaços", 480, 7, "#", "Interrompa o aquecimento na metade do tempo e vire os pedaços de carne para um aquecimento uniforme.", true },
                { "Feijão", "Feijão congelado", 480, 9, "$", "Deixe o feijão em um recipiente fundo para evitar transbordamento.", true }
            });
    }
}
