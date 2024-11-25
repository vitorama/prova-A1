namespace API.Models;

public class Tarefa
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; }
    public bool Finished { get; set; } = false;
    public Usuario? Usuario { get; set; }
    public Tag? Tag { get; set; }
    public Projeto? Projeto { get; set; }
}
