﻿using Cpnucleo.Shared.Commands.RemoveImpedimentoTarefa;

namespace Cpnucleo.Application.Commands.RemoveImpedimentoTarefa;

public sealed class RemoveImpedimentoTarefaHandler : IRequestHandler<RemoveImpedimentoTarefaCommand, OperationResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveImpedimentoTarefaHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult> Handle(RemoveImpedimentoTarefaCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.ImpedimentoTarefa impedimentoTarefa = await _unitOfWork.ImpedimentoTarefaRepository.Get(request.Id).FirstOrDefaultAsync(cancellationToken);

        if (impedimentoTarefa is null)
        {
            return OperationResult.NotFound;
        }

        await _unitOfWork.ImpedimentoTarefaRepository.RemoveAsync(request.Id);

        bool success = await _unitOfWork.SaveChangesAsync(cancellationToken);

        OperationResult result = success ? OperationResult.Success : OperationResult.Failed;

        return result;
    }
}