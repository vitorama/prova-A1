using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

public static class TarefaNaoConcluidasEndpoints
{
    public static void MapTarefaNaoConcluidasEndpoints(this WebApplication app)
    {
        app.MapGet("/api/tarefa/naoconcluidas", async (AppDbContext db) =>)
        {
            var tarefasNaoConcluidas = await db.Tarefas
                .Where(t => t.Status == "Não iniciada" || t.Status == "Em andamento")
                .ToListAsync();

            return tarefasNao
        }
    }
}