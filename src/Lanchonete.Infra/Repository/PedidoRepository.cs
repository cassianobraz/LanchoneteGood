using Lanchonete.Domain.Models.PedidoAggregate;
using Lanchonete.Infra.EF.DbCOntext;
using Microsoft.EntityFrameworkCore;

namespace Lanchonete.Infra.Repository;

public class PedidoRepository(LanchoneteContext context) : IPedidoRepository
{
    private readonly LanchoneteContext _context = context;

    public async Task<Pedido?> ObterPorIdAsync(int id, CancellationToken ct)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Pedido?> ObterPorIdNTAsync(int id, CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<Pedido>> ListarTodosAsync(CancellationToken ct)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .ToListAsync(ct);
    }

    public async Task CriarAsync(Pedido pedido, CancellationToken ct)
    {
        await _context.Pedidos.AddAsync(pedido, ct);
    }

    public Task AtualizarAsync(Pedido pedido, CancellationToken ct)
    {
        _context.Pedidos.Update(pedido);
        return Task.CompletedTask;
    }

    public async Task ExcluirAsync(int id, CancellationToken ct)
    {
        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        _context.Pedidos.Remove(pedido);
    }
}