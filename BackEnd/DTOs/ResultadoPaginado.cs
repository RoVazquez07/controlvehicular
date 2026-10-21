namespace ControlVehicular.DTOs;

public sealed class ResultadoPaginado<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanoPagina { get; init; }
}
