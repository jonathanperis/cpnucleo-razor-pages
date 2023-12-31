﻿using Cpnucleo.Shared.Queries.ListProjeto;

namespace Cpnucleo.RazorPages.Pages.Projeto;

[Authorize]
public class ListarModel : PageModel
{
    private readonly ICpnucleoApiClient _cpnucleoApiClient;

    public ListarModel(ICpnucleoApiClient cpnucleoApiClient)
    {
        _cpnucleoApiClient = cpnucleoApiClient;
    }

    public ProjetoDto Projeto { get; set; }

    public IEnumerable<ProjetoDto> Lista { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var result = await _cpnucleoApiClient.ExecuteAsync<ListProjetoViewModel>("Projeto", "ListProjeto", new ListProjetoQuery());

            if (result.OperationResult == OperationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
                return Page();
            }

            Lista = result.Projetos;

            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
