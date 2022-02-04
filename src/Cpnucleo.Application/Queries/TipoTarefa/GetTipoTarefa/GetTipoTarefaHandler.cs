﻿namespace Cpnucleo.Application.Queries.TipoTarefa.GetTipoTarefa;

public class GetTipoTarefaHandler : IRequestHandler<GetTipoTarefaQuery, GetTipoTarefaViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTipoTarefaHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetTipoTarefaViewModel> Handle(GetTipoTarefaQuery request, CancellationToken cancellationToken)
    {
        var tipoTarefa = await _unitOfWork.TipoTarefaRepository.GetAsync(request.Id);

        if (tipoTarefa == null)
        {
            return new GetTipoTarefaViewModel { OperationResult = OperationResult.NotFound };
        }

        TipoTarefaDTO result = _mapper.Map<TipoTarefaDTO>(tipoTarefa);

        return new GetTipoTarefaViewModel { TipoTarefa = result, OperationResult = OperationResult.Success };
    }
}
