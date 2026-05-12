using Microondas.Domain.Entities;
using Microondas.Domain.Exceptions;
using Xunit;
using FluentAssertions;
using Microondas.Tests.TestBuilders;

namespace Microondas.Tests.Domain;

public class ProgramaAquecimentoTest
{
    [Fact]
    public void Criar_ProgramaValido_DeveInstanciarComSucesso()
    {
        // Arrange & Act
        var programa = new ProgramaAquecimentoBuilder()
            .ComNome("Teste")
            .ComAlimento("Alimento Teste")
            .ComTempo(30)
            .ComPotencia(8)
            .ComCaractere('?')
            .Build();

        // Assert
        programa.Nome.Should().Be("Teste");
        programa.TempoSegundos.Should().Be(30);
        programa.Potencia.Should().Be(8);
        programa.CaractereAquecimento.Should().Be('?');
    }

    [Fact]
    public void Criar_QuandoNomeVazio_DeveLancarExcecao()
    {
        // Arrange
        var p = new ProgramaAquecimentoBuilder().Build();
        
        // Act
        var act = () => new ProgramaAquecimento("", p.Alimento, p.TempoSegundos, p.Potencia, p.CaractereAquecimento, p.Instrucoes!);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>().WithMessage("Nome é obrigatório.");
    }

    [Fact]
    public void Criar_QuandoAlimentoVazio_DeveLancarExcecao()
    {
        // Arrange
        var p = new ProgramaAquecimentoBuilder().Build();
        
        // Act
        var act = () => new ProgramaAquecimento(p.Nome, "", p.TempoSegundos, p.Potencia, p.CaractereAquecimento, p.Instrucoes!);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>().WithMessage("Alimento é obrigatório.");
    }

    [Fact]
    public void Criar_QuandoTempoAbaixoDoMinimo_DeveLancarExcecao()
    {
        // Arrange
        var p = new ProgramaAquecimentoBuilder().Build();
        
        // Act
        var act = () => new ProgramaAquecimento(p.Nome, p.Alimento, 0, p.Potencia, p.CaractereAquecimento, p.Instrucoes!);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>().WithMessage("*Tempo deve estar entre*");
    }

    [Fact]
    public void Criar_QuandoTempoAcimaDoMaximo_DeveLancarExcecao()
    {
        // Arrange
        var p = new ProgramaAquecimentoBuilder().Build();
        
        // Act
        var act = () => new ProgramaAquecimento(p.Nome, p.Alimento, 3601, p.Potencia, p.CaractereAquecimento, p.Instrucoes!);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>().WithMessage("*Tempo deve estar entre*");
    }

    [Fact]
    public void Criar_QuandoPotenciaAbaixoDoMinimo_DeveLancarExcecao()
    {
        // Arrange
        var p = new ProgramaAquecimentoBuilder().Build();
        
        // Act
        var act = () => new ProgramaAquecimento(p.Nome, p.Alimento, p.TempoSegundos, 0, p.CaractereAquecimento, p.Instrucoes!);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>().WithMessage("*Potência deve estar entre*");
    }

    [Fact]
    public void Criar_QuandoPotenciaAcimaDoMaximo_DeveLancarExcecao()
    {
        // Arrange
        var p = new ProgramaAquecimentoBuilder().Build();
        
        // Act
        var act = () => new ProgramaAquecimento(p.Nome, p.Alimento, p.TempoSegundos, 11, p.CaractereAquecimento, p.Instrucoes!);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>().WithMessage("*Potência deve estar entre*");
    }

    [Fact]
    public void Criar_QuandoCaractereReservado_DeveLancarExcecao()
    {
        // Arrange
        var p = new ProgramaAquecimentoBuilder().Build();
        
        // Act
        var act = () => new ProgramaAquecimento(p.Nome, p.Alimento, p.TempoSegundos, p.Potencia, '.', p.Instrucoes!);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>().WithMessage("*é reservado para o aquecimento padrão*");
    }
}
