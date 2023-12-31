﻿using Cpnucleo.RazorPages.Services;
using Cpnucleo.Shared.Commands.CreateApontamento;
using Cpnucleo.Shared.Queries.ListApontamentoByRecurso;
using Cpnucleo.Shared.Queries.ListTarefaByRecurso;
using System.Security.Claims;

namespace Cpnucleo.RazorPages.Pages.Apontamento;

[Authorize]
public class ListarModel : PageModel
{
    private readonly ICpnucleoApiClient _cpnucleoApiClient;

    public ListarModel(ICpnucleoApiClient cpnucleoApiClient)
    {
        _cpnucleoApiClient = cpnucleoApiClient;
    }

    [BindProperty]
    public ApontamentoDto Apontamento { get; set; }

    public IEnumerable<ApontamentoDto> Lista { get; set; }

    public IEnumerable<TarefaDto> ListaTarefas { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            await CarregarDados();

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
                await CarregarDados();

                return Page();
            }

            var result = await _cpnucleoApiClient.ExecuteAsync<OperationResult>("Apontamento", "CreateApontamento", new CreateApontamentoCommand(Apontamento.Descricao, Apontamento.DataApontamento, Apontamento.QtdHoras, Apontamento.IdTarefa, Apontamento.IdRecurso));

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

    private async Task CarregarDados()
    {
        var retorno = ClaimsService.ReadClaimsPrincipal(HttpContext.User, ClaimTypes.PrimarySid);
        Guid idRecurso = new(retorno);

        var result = await _cpnucleoApiClient.ExecuteAsync<ListApontamentoByRecursoViewModel>("Apontamento", "GetApontamentoByRecurso", new ListApontamentoByRecursoQuery(idRecurso));

        if (result.OperationResult == OperationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
            return;
        }

        Lista = result.Apontamentos;

        var result2 = await _cpnucleoApiClient.ExecuteAsync<ListTarefaByRecursoViewModel>("Tarefa", "GetTarefaByRecurso", new ListTarefaByRecursoQuery(idRecurso));

        if (result.OperationResult == OperationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível processar a solicitação no momento.");
            return;
        }

        ListaTarefas = result2.Tarefas;
    }
}
