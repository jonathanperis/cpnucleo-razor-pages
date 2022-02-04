﻿namespace Cpnucleo.Application.Commands.TipoTarefa.RemoveTipoTarefa;

public class RemoveTipoTarefaCommand : IRequest<OperationResult>
{
    public Guid Id { get; }

    public RemoveTipoTarefaCommand(Guid id)
    {
        Id = id;
    }
}