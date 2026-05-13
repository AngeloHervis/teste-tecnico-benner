using Microsoft.EntityFrameworkCore.Migrations;

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
                { "Pipoca", "Pipoca (de micro-ondas)", 180, 7, "*", "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.", true },
                { "Leite", "Leite", 300, 5, "!", "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.", true },
                { "Carnes de boi", "Carne em pedaços ou fatias", 840, 4, "@", "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.", true },
                { "Frango", "Frango (qualquer corte)", 480, 7, "#", "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.", true },
                { "Feijão", "Feijão congelado", 480, 9, "$", "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.", true }
            });
    }
}
