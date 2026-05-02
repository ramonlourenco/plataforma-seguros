namespace Contratacao.Domain.ValueObjects;

public sealed class StatusProposta
{
    public int Id { get; }
    public string Nome { get; }

    private StatusProposta(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public static StatusProposta EmAnalise { get; } = new(1, "EmAnalise");
    public static StatusProposta Aprovada { get; } = new(2, "Aprovada");
    public static StatusProposta Rejeitada { get; } = new(3, "Rejeitada");
    public static StatusProposta Contratada { get; } = new(4, "Contratada");

    private static readonly IReadOnlyList<StatusProposta> Values = new[]
    {
        EmAnalise,
        Aprovada,
        Rejeitada,
        Contratada
    };

    public static StatusProposta FromId(int id)
        => Values.SingleOrDefault(x => x.Id == id)
            ?? throw new InvalidOperationException($"StatusProposta id '{id}' inválido.");

    public static StatusProposta FromName(string nome)
        => Values.SingleOrDefault(x => string.Equals(x.Nome, nome, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"StatusProposta nome '{nome}' inválido.");

    public override bool Equals(object? obj)
        => obj is StatusProposta other && other.Id == Id;

    public override int GetHashCode() => Id;

    public static bool operator ==(StatusProposta? left, StatusProposta? right)
        => ReferenceEquals(left, right) || (left is not null && right is not null && left.Id == right.Id);

    public static bool operator !=(StatusProposta? left, StatusProposta? right)
        => !(left == right);

    public override string ToString() => Nome;
}
