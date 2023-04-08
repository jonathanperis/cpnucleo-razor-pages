﻿using Cpnucleo.Shared.Queries.GetWorkflow;

namespace Cpnucleo.Application.Queries.GetWorkflow;

public sealed class GetWorkflowQueryValidator : AbstractValidator<GetWorkflowQuery>
{
    public GetWorkflowQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}