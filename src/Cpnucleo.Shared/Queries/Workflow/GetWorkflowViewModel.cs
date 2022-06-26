﻿namespace Cpnucleo.Shared.Queries.Workflow;

public record GetWorkflowViewModel : BaseQuery
{
    public WorkflowDTO Workflow { get; set; }
    public OperationResult OperationResult { get; set; }
}
