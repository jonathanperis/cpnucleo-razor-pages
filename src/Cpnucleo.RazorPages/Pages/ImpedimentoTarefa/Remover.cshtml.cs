﻿using Cpnucleo.Shared.Commands.RemoveImpedimentoTarefa;
using Cpnucleo.Shared.Queries.GetImpedimentoTarefa;

namespace Cpnucleo.RazorPages.Pages.ImpedimentoTarefa;

[Authorize]
public class RemoverModel : PageModel
{
    private readonly ICpnucleoApiClient _cpnucleoApiClient;

    public RemoverModel(ICpnucleoApiClient cpnucleoApiClient)
    {
        _cpnucleoApiClient = cpnucleoApiClient;
    }

    [BindProperty]
    public ImpedimentoTarefaDto ImpedimentoTarefa { get; set; }

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

            var result = await _cpnucleoApiClient.ExecuteAsync<OperationResult>("ImpedimentoTarefa", "RemoveImpedimentoTarefa", new RemoveImpedimentoTarefaCommand(ImpedimentoTarefa.Id));

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
    }
}
