﻿using Cpnucleo.Shared.Commands.CreateRecursoProjeto;
using Cpnucleo.Shared.Queries.GetProjeto;
using Cpnucleo.Shared.Queries.ListRecurso;

namespace Cpnucleo.RazorPages.Pages.RecursoProjeto;

[Authorize]
public class IncluirModel : PageModel
{
    private readonly ICpnucleoApiClient _cpnucleoApiClient;

    public IncluirModel(ICpnucleoApiClient cpnucleoApiClient)
    {
        _cpnucleoApiClient = cpnucleoApiClient;
    }

    [BindProperty]
    public RecursoProjetoDto RecursoProjeto { get; set; }

    public ProjetoDto Projeto { get; set; }

    public SelectList SelectRecursos { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid idProjeto)
    {
        try
        {
            await CarregarDados(idProjeto);

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
                await CarregarDados(RecursoProjeto.IdProjeto);

                return Page();
            }

            var result = await _cpnucleoApiClient.ExecuteAsync<OperationResult>("RecursoProjeto", "CreateRecursoProjeto", new CreateRecursoProjetoCommand(RecursoProjeto.IdRecurso, RecursoProjeto.IdProjeto));

            if (result == OperationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
                return Page();
            }

            return RedirectToPage("Listar", new { idProjeto = RecursoProjeto.IdProjeto });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    private async Task CarregarDados(Guid idProjeto)
    {
        var result = await _cpnucleoApiClient.ExecuteAsync<GetProjetoViewModel>("Projeto", "GetProjeto", new GetProjetoQuery(idProjeto));

        if (result.OperationResult == OperationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
            return;
        }

        Projeto = result.Projeto;

        var result2 = await _cpnucleoApiClient.ExecuteAsync<ListRecursoViewModel>("Recurso", "ListRecurso", new ListRecursoQuery());

        if (result2.OperationResult == OperationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
            return;
        }

        SelectRecursos = new SelectList(result2.Recursos, "Id", "Nome");
    }
}
