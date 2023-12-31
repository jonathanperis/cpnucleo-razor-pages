﻿using Cpnucleo.Shared.Queries.ListRecurso;

namespace Cpnucleo.RazorPages.Pages.Recurso;

[Authorize]
public class ListarModel : PageModel
{
    private readonly ICpnucleoApiClient _cpnucleoApiClient;

    public ListarModel(ICpnucleoApiClient cpnucleoApiClient)
    {
        _cpnucleoApiClient = cpnucleoApiClient;
    }

    public RecursoDto Recurso { get; set; }

    public IEnumerable<RecursoDto> Lista { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var result = await _cpnucleoApiClient.ExecuteAsync<ListRecursoViewModel>("Recurso", "ListRecurso", new ListRecursoQuery());

            if (result.OperationResult == OperationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
                return Page();
            }

            Lista = result.Recursos;

            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
