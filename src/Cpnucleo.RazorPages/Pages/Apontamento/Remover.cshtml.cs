﻿using Cpnucleo.Shared.Commands.RemoveApontamento;
using Cpnucleo.Shared.Queries.GetApontamento;

namespace Cpnucleo.RazorPages.Pages.Apontamento;

[Authorize]
public class RemoverModel : PageModel
{
    private readonly ICpnucleoApiClient _cpnucleoApiClient;

    public RemoverModel(ICpnucleoApiClient cpnucleoApiClient)
    {
        _cpnucleoApiClient = cpnucleoApiClient;
    }

    [BindProperty]
    public ApontamentoDto Apontamento { get; set; }

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
                await CarregarDados(Apontamento.Id);

                return Page();
            }

            var result = await _cpnucleoApiClient.ExecuteAsync<OperationResult>("Apontamento", "RemoveApontamento", new RemoveApontamentoCommand(Apontamento.Id));

            if (result == OperationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
                return Page();
            }

            return RedirectToPage("Listar");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    private async Task CarregarDados(Guid idApontamento)
    {
        var result = await _cpnucleoApiClient.ExecuteAsync<GetApontamentoViewModel>("Apontamento", "GetApontamento", new GetApontamentoQuery(idApontamento));

        if (result.OperationResult == OperationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
            return;
        }

        Apontamento = result.Apontamento;
    }
}
