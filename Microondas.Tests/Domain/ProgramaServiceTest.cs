using FluentAssertions;
using Microondas.Domain.Entities;
using Microondas.Domain.Interfaces;
using Microondas.Domain.Services;
using Moq;
using Microondas.Tests.TestBuilders;

namespace Microondas.Tests.Domain;

public class ProgramaServiceTest
{
    private readonly Mock<IProgramaRepository> _repositoryMock = new();
    private readonly ProgramaService _service;

    public ProgramaServiceTest()
    {
        _service = new ProgramaService(_repositoryMock.Object);
    }

    [Fact]
    public async Task Cadastrar_CaractereDuplicado_DeveRetornarErro()
    {
        // Arrange
        var programa = new ProgramaAquecimentoBuilder().ComCaractere('!').Build();
        _repositoryMock.Setup(r => r.ExisteCaractereAsync('!')).ReturnsAsync(true);

        // Act
        var result = await _service.CadastrarAsync(programa);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("já está sendo usado"));
    }

    [Fact]
    public async Task Cadastrar_CaractereReservado_DeveRetornarErro()
    {
        // Arrange
        var programa = new ProgramaAquecimentoBuilder().ComCaractere('.').Build();

        // Act
        var result = await _service.CadastrarAsync(programa);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("reservado"));
    }

    [Fact]
    public async Task Cadastrar_ProgramaValido_DeveChamarRepositorio()
    {
        // Arrange
        var programa = new ProgramaAquecimentoBuilder().ComCaractere('?').Build();
        _repositoryMock.Setup(r => r.ExisteCaractereAsync('?')).ReturnsAsync(false);

        // Act
        await _service.CadastrarAsync(programa);

        // Assert
        _repositoryMock.Verify(r => r.AdicionarAsync(programa), Times.Once);
    }

    [Fact]
    public async Task Listar_Sempre_DeveChamarRepositorio()
    {
        // Arrange
        var programas = new List<ProgramaAquecimento> { new ProgramaAquecimentoBuilder().EhPadrao().Build() };
        _repositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(programas);

        // Act
        var result = await _service.ListarAsync();

        // Assert
        result.Data.Should().BeEquivalentTo(programas);
        _repositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
    }
}
