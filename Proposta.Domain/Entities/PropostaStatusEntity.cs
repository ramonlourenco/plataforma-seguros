namespace Proposta.Domain.Entities;

public sealed class PropostaStatusEntity
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = null!;

    private PropostaStatusEntity() { }

    public PropostaStatusEntity(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public override string ToString() => Nome;
}
