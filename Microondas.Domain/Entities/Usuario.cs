using System.ComponentModel.DataAnnotations;

namespace Microondas.Domain.Entities;

public sealed class Usuario
{
    public int Id { get; init; }
    [MaxLength(300)] public required string Nome { get; init; }
    [MaxLength(300)] public required string SenhaHash { get; init; }
    public DateTime DataCriacao { get; init; } = DateTime.UtcNow;
    public bool Ativo { get; init; } = true;
}
