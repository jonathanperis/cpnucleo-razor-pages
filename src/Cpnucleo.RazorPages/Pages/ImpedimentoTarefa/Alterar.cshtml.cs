﻿using Cpnucleo.Shared.Commands.UpdateImpedimentoTarefa;
using Cpnucleo.Shared.Queries.GetImpedimentoTarefa;
using Cpnucleo.Shared.Queries.ListImpedimento;

namespace Cpnucleo.RazorPages.Pages.ImpedimentoTarefa;

[Authorize]
public class AlterarModel : PageModel
{
    private readonly ICpnucleoApiClient _cpnucleoApiClient;

    public AlterarModel(ICpnucleoApiClient cpnucleoApiClient)
    {
        _cpnucleoApiClient = cpnucleoApiClient;
    }

    [BindProperty]
    public ImpedimentoTarefaDto ImpedimentoTarefa { get; set; }

    public SelectList SelectImpedimentos { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        try
        {
            await CarregarDados(id);

            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await CarregarDados(ImpedimentoTarefa.Id);

                return Page();
            }

            var result = await _cpnucleoApiClient.ExecuteAsync<OperationResult>("ImpedimentoTarefa", "UpdateImpedimentoTarefa", new UpdateImpedimentoTarefaCommand(ImpedimentoTarefa.Id, ImpedimentoTarefa.Descricao, ImpedimentoTarefa.IdTarefa, ImpedimentoTarefa.IdImpedimento));

            if (result == OperationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
                return Page();
            }

            return RedirectToPage("Listar", new { idTarefa = ImpedimentoTarefa.IdTarefa });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    private async Task CarregarDados(Guid idImpedimentoTarefa)
    {
        var result = await _cpnucleoApiClient.ExecuteAsync<GetImpedimentoTarefaViewModel>("ImpedimentoTarefa", "GetImpedimentoTarefa", new GetImpedimentoTarefaQuery(idImpedimentoTarefa));

        if (result.OperationResult == OperationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
            return;
        }

        ImpedimentoTarefa = result.ImpedimentoTarefa;

        var result2 = await _cpnucleoApiClient.ExecuteAsync<ListImpedimentoViewModel>("Impedimento", "ListImpedimento", new ListImpedimentoQuery());

        if (result2.OperationResult == OperationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
            return;
        }

        SelectImpedimentos = new SelectList(result2.Impedimentos, "Id", "Nome");
    }
}
