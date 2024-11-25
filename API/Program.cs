using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
import Models;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDataContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Prova A1");

//ENDPOINTS DE CATEGORIA
//GET: http://localhost:5273/api/categoria/listar
app.MapGet("/api/categoria/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Categorias.Any())
    {
        return Results.Ok(ctx.Categorias.ToList());
    }
    return Results.NotFound("Nenhuma categoria encontrada");
});

//POST: http://localhost:5273/api/categoria/cadastrar
app.MapPost("/api/categoria/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Categoria categoria) =>
{
    ctx.Categorias.Add(categoria);
    ctx.SaveChanges();
    return Results.Created("", categoria);
});

//ENDPOINTS DE TAREFA
//GET: http://localhost:5273/api/tarefas/listar
app.MapGet("/api/tarefas/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Tarefas.Any())
    {
        return Results.Ok(ctx.Tarefas.Include(x => x.Categoria).ToList());
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//POST: http://localhost:5273/api/tarefas/cadastrar
app.MapPost("/api/tarefas/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Tarefa tarefa) =>
{
    Categoria? categoria = ctx.Categorias.Find(tarefa.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria não encontrada");
    }
    tarefa.Categoria = categoria;
    ctx.Tarefas.Add(tarefa);
    ctx.SaveChanges();
    return Results.Created("", tarefa);
});

//PUT: http://localhost:5273/tarefas/alterar/{id}
app.MapPut("/api/tarefas/alterar/{id}", ([FromServices] AppDataContext ctx, [FromRoute] string id) =>
{
    app.MapPatch("/api/tarefa/alterar/{id}", async (AppDataContext db, int id) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);
    if (tarefa == null) return Results.NotFound();

    tarefa.Status = tarefa.Status switch
    {
        "Não iniciada" => "Em andamento",
        "Em andamento" => "Concluída",
        _ => tarefa.Status
    };

    await db.SaveChangesAsync();
    return Results.Ok(tarefa);
});

});

//GET: http://localhost:5273/tarefas/naoconcluidas
app.MapGet("/api/tarefas/naoconcluidas", ([FromServices] AppDataContext ctx) =>
{
    app.MapGet("/api/tarefa/naoconcluidas", async (AppDbContext db) =>
{
    var tarefas = await db.Tarefas
        .Where(t => t.Status == "Não iniciada" || t.Status == "Em andamento")
        .ToListAsync();
    return Results.Ok(tarefas);
});

});

//GET: http://localhost:5273/tarefas/concluidas
app.MapGet("/api/tarefa/concluidas", async (AppDataContext db) =>
{
    var tarefas = await db.Tarefas
        .Where(t => t.Status == "Concluída")
        .ToListAsync();
    return Results.Ok(tarefas);
});

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapTarefaNaoConcluidasEndpoints();

app.MapPost("/api/tarefa/cadastrar", async (AppDbContext db, Tarefa tarefa) =>
{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();
    return Results.Ok(tarefa);
});

app.Run();



app.Run();
