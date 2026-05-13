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
    public void Iniciar_QuandoSemConfiguracao_DeveFazerInicioRapido()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();

        // Act
        maquina.Iniciar();

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(30);
        maquina.Potencia.Should().Be(10);
        maquina.Estado.Should().Be(EstadoMicroondas.EmAndamento);
    }

    [Fact]
    public void Iniciar_QuandoJaEmAndamento_DeveSomarTrintaSegundos()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        maquina.Iniciar();

        // Act
        maquina.Iniciar();

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(60);
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
    public void AdicionarTempo_QuandoProgramaPreDefinido_DeveIgnorarComando()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var programa = new ProgramaAquecimentoBuilder().ComTempo(180).ComPotencia(7).EhPadrao().Build();
        maquina.ConfigurarPrograma(programa);
        maquina.Iniciar();

        // Act
        maquina.AdicionarTempo(30);

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(180);
    }

    [Fact]
    public void AdicionarTempo_QuandoExcederLimite_DeveLimitarEmDoisMinutos()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var command = new ConfigurarMicroondasCommandBuilder().ComTempo(110).Build();
        maquina.Configurar(command);
        maquina.Iniciar();

        // Act
        maquina.AdicionarTempo(30);

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(120);
    }

    [Fact]
    public void AdicionarTempo_QuandoProgramaCustomizado_DeveIgnorarComando()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var programa = new ProgramaAquecimentoBuilder()
            .ComNome("Custom")
            .ComTempo(180)
            .EhPadrao(false)
            .Build();
        maquina.ConfigurarPrograma(programa);
        maquina.Iniciar();

        // Act
        maquina.AdicionarTempo(30);

        // Assert
        maquina.TempoRestanteSegundos.Should().Be(180);
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
    public void PausarOuCancelar_QuandoEmAndamento_DeveMudarParaPausado()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        maquina.Iniciar();

        // Act
        maquina.PausarOuCancelar();

        // Assert
        maquina.Estado.Should().Be(EstadoMicroondas.Pausado);
        maquina.TempoRestanteSegundos.Should().Be(30);
    }

    [Fact]
    public void Iniciar_QuandoPausado_DeveRetomarSemSomarTempo()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        maquina.Iniciar();
        maquina.PausarOuCancelar();

        // Act
        maquina.Iniciar();

        // Assert
        maquina.Estado.Should().Be(EstadoMicroondas.EmAndamento);
        maquina.TempoRestanteSegundos.Should().Be(30); // Não deve somar +30s na retomada
    }

    [Fact]
    public void PausarOuCancelar_QuandoPausado_DeveResetarTudo()
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        maquina.Iniciar();
        maquina.PausarOuCancelar(); // Primeiro clique (Pausa)

        // Act
        maquina.PausarOuCancelar(); // Segundo clique (Cancela)

        // Assert
        maquina.Estado.Should().Be(EstadoMicroondas.Inativo);
        maquina.TempoRestanteSegundos.Should().Be(0);
        maquina.VisorAquecimento.Should().BeEmpty();
    }

    [Theory]
    [InlineData(10, "10s")]
    [InlineData(60, "1:00")]
    [InlineData(90, "1:30")]
    [InlineData(120, "2:00")]
    public void ObterTempoFormatado_CenariosDiversos_DeveRetornarFormatoCorreto(int segundos, string esperado)
    {
        // Arrange
        var maquina = new MaquinaMicroondas();
        var command = new ConfigurarMicroondasCommandBuilder().ComTempo(segundos).Build();
        maquina.Configurar(command);

        // Act
        var resultado = maquina.ObterTempoFormatado();

        // Assert
        resultado.Should().Be(esperado);
    }
}
