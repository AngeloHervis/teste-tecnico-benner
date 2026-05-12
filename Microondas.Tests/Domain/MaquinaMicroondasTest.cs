using FluentAssertions;
using Microondas.Domain.Entities;
using Microondas.Domain.Enums;
using Microondas.Domain.Exceptions;
using Microondas.Tests.TestBuilders;

namespace Microondas.Tests.Domain;

public sealed class MaquinaMicroondasTest : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }

    [Fact]
    public void Configurar_QuandoTempoInvalido_DeveLancarExcecao()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var command = new ConfigurarMicroondasCommandBuilder().ComTempo(0).Build();

        // Act
        var act = () => maquina.Configurar(command);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>()
           .WithMessage("Tempo deve estar entre 1 segundo e 2 minutos");
    }

    [Fact]
    public void Configurar_QuandoPotenciaInvalida_DeveLancarExcecao()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var command = new ConfigurarMicroondasCommandBuilder().ComTempo(30).ComPotencia(11).Build();

        // Act
        var act = () => maquina.Configurar(command);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>()
           .WithMessage("Potência deve ser um valor de 1 a 10");
    }

    [Fact]
    public void Configurar_QuandoPotenciaNula_DeveAssumirDez()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var command = new ConfigurarMicroondasCommandBuilder().ComTempo(30).ComPotencia(null).Build();

        // Act
        maquina.Configurar(command);

        // Assert
        maquina.Potencia.Should().Be(10);
        maquina.TempoRestanteSegundos.Should().Be(30);
    }

    [Fact]
    public void InicioRapido_QuandoInvocado_DeveDefinirTrintaSegundosPotenciaDezEIniciar()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();

        // Act
        maquina.InicioRapido();

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(30);
        maquina.Potencia.Should().Be(10);
        maquina.Estado.Should().Be(EstadoMicroondas.EmAndamento);
    }

    [Fact]
    public void AvancarSegundo_QuandoEmAndamento_DeveDecrementarTempoEGerarVisor()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var command = new ConfigurarMicroondasCommandBuilder().ComTempo(2).ComPotencia(3).Build();
        maquina.Configurar(command);
        maquina.Iniciar();

        // Act
        maquina.AvancarSegundo();

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(1);
        maquina.VisorAquecimento.Should().Be("... ");
    }

    [Fact]
    public void AvancarSegundo_QuandoChegarAZero_DeveConcluirEAdicionarMensagem()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var command = new ConfigurarMicroondasCommandBuilder().ComTempo(1).ComPotencia(1).Build();
        maquina.Configurar(command);
        maquina.Iniciar();

        // Act
        maquina.AvancarSegundo();

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(0);
        maquina.Estado.Should().Be(EstadoMicroondas.Concluido);
        maquina.VisorAquecimento.Should().Be(". Aquecimento concluído");
    }

    [Fact]
    public void ConfigurarPrograma_DevePreencherTempoPotenciaEAtribuirPrograma()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var programa = new ProgramaAquecimentoBuilder().ComTempo(180).ComPotencia(7).EhPadrao().Build();

        // Act
        maquina.ConfigurarPrograma(programa);

        // Assert
        maquina.ProgramaAtual.Should().Be(programa);
        maquina.TempoRestanteSegundos.Should().Be(180);
        maquina.Potencia.Should().Be(7);
        maquina.Estado.Should().Be(EstadoMicroondas.Inativo);
    }

    [Fact]
    public void AdicionarTempo_QuandoProgramaPreDefinido_DeveLancarExcecao()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var programa = new ProgramaAquecimentoBuilder().ComTempo(180).ComPotencia(7).EhPadrao().Build();
        maquina.ConfigurarPrograma(programa);
        maquina.Iniciar();

        // Act
        var act = () => maquina.AdicionarTempo(30);

        // Assert
        act.Should().Throw<ValidacaoMicroondasException>()
           .WithMessage("Acréscimo de tempo não é permitido para programas pré-definidos.");
    }

    [Fact]
    public void AvancarSegundo_QuandoProgramaPreDefinido_DeveUsarCaractereDoPrograma()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var programa = new ProgramaAquecimentoBuilder().ComNome("Leite").ComTempo(300).ComPotencia(5).ComCaractere('!').EhPadrao().Build();
        maquina.ConfigurarPrograma(programa);
        maquina.Iniciar();

        // Act
        maquina.AvancarSegundo();

        // Assert
        maquina.VisorAquecimento.Should().Be("!!!!! ");
    }

    [Fact]
    public void PausarOuCancelar_QuandoCancelar_DeveLimparProgramaAtual()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var programa = new ProgramaAquecimentoBuilder().ComTempo(180).ComPotencia(7).EhPadrao().Build();
        maquina.ConfigurarPrograma(programa);
        
        // Act
        maquina.PausarOuCancelar();

        // Assert
        maquina.ProgramaAtual.Should().BeNull();
        maquina.TempoRestanteSegundos.Should().Be(0);
        maquina.Potencia.Should().Be(0);
    }
}
