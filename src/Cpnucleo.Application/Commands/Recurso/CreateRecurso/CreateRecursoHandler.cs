﻿using Cpnucleo.Infra.CrossCutting.Security.Interfaces;

namespace Cpnucleo.Application.Commands.Recurso.CreateRecurso;

public class CreateRecursoHandler : IRequestHandler<CreateRecursoCommand, OperationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICryptographyManager _cryptographyManager;

    public CreateRecursoHandler(IUnitOfWork unitOfWork, IMapper mapper, ICryptographyManager cryptographyManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cryptographyManager = cryptographyManager;
    }

    public async Task<OperationResult> Handle(CreateRecursoCommand request, CancellationToken cancellationToken)
    {
        _cryptographyManager.CryptPbkdf2(request.Senha, out string senhaCrypt, out string salt);

        request.Senha = senhaCrypt;
        request.Salt = salt;

        await _unitOfWork.RecursoRepository.AddAsync(_mapper.Map<Domain.Entities.Recurso>(request));

        bool success = await _unitOfWork.SaveChangesAsync();

        OperationResult result = success ? OperationResult.Success : OperationResult.Failed;

        return result;
    }
}
