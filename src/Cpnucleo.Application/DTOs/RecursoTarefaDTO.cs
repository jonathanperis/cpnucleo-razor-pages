namespace Cpnucleo.Application.DTOs;

public class RecursoTarefaDTO : BaseDTO
{
    public Guid IdRecurso { get; set; }

    public Guid IdTarefa { get; set; }

    public RecursoDTO? Recurso { get; set; }

    public TarefaDTO? Tarefa { get; set; }
}