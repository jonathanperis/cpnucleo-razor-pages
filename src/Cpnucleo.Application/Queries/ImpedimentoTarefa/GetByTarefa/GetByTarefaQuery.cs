﻿namespace Cpnucleo.Application.Queries.ImpedimentoTarefa.GetByTarefa;

public class GetByTarefaQuery : IRequest<GetByTarefaViewModel>
{
    public Guid IdTarefa { get; }

    public GetByTarefaQuery(Guid idTarefa)
    {
        IdTarefa = idTarefa;
    }
}
