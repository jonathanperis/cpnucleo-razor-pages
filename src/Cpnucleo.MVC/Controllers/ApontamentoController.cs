﻿using Cpnucleo.Infra.CrossCutting.Util.Commands.Requests.Apontamento;
using Cpnucleo.Infra.CrossCutting.Util.Commands.Requests.Tarefa;
using Cpnucleo.Infra.CrossCutting.Util.Queries.Requests.Apontamento;
using Cpnucleo.Infra.CrossCutting.Util.Queries.Responses.Apontamento;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Cpnucleo.MVC.Interfaces;
using Cpnucleo.MVC.Models;
using Cpnucleo.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cpnucleo.MVC.Controllers
{
    [Authorize]
    public class ApontamentoController : BaseController
    {
        private readonly IApontamentoService _apontamentoService;
        private readonly ITarefaService _tarefaService;
        private readonly IWorkflowService _workflowService;

        private ApontamentoView _apontamentoView;

        public ApontamentoController(IApontamentoService apontamentoService,
                                     ITarefaService tarefaService,
                                     IWorkflowService workflowService)
        {
            _apontamentoService = apontamentoService;
            _tarefaService = tarefaService;
            _workflowService = workflowService;
        }

        public ApontamentoView ApontamentoView
        {
            get
            {
                if (_apontamentoView == null)
                {
                    _apontamentoView = new ApontamentoView();
                }

                return _apontamentoView;
            }
            set => _apontamentoView = value;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                string retorno = ClaimsService.ReadClaimsPrincipal(HttpContext.User, ClaimTypes.PrimarySid);
                Guid idRecurso = new(retorno);

                GetByRecursoResponse response = await _apontamentoService.GetByRecursoAsync(Token, new GetByRecursoQuery { IdRecurso = idRecurso });
                ApontamentoView.Lista = response.Apontamentos;

                await CarregarTarefasByRecurso(idRecurso);

                return View(ApontamentoView);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Listar(ApontamentoView obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string retorno = ClaimsService.ReadClaimsPrincipal(HttpContext.User, ClaimTypes.PrimarySid);
                    Guid idRecurso = new(retorno);

                    GetByRecursoResponse response = await _apontamentoService.GetByRecursoAsync(Token, new GetByRecursoQuery { IdRecurso = idRecurso });
                    ApontamentoView.Lista = response.Apontamentos;

                    await CarregarTarefasByRecurso(idRecurso);

                    return View(ApontamentoView);
                }

                await _apontamentoService.AddAsync(Token, new CreateApontamentoCommand { Apontamento = obj.Apontamento });

                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Remover(Guid id)
        {
            try
            {
                GetApontamentoResponse response = await _apontamentoService.GetAsync(Token, new GetApontamentoQuery { Id = id });
                ApontamentoView.Apontamento = response.Apontamento;

                return View(ApontamentoView);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remover(ApontamentoView obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    GetApontamentoResponse response = await _apontamentoService.GetAsync(Token, new GetApontamentoQuery { Id = obj.Apontamento.Id });
                    ApontamentoView.Apontamento = response.Apontamento;

                    return View(ApontamentoView);
                }

                await _apontamentoService.RemoveAsync(Token, new RemoveApontamentoCommand { Id = obj.Apontamento.Id });

                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> FluxoTrabalho()
        {
            try
            {
                await CarregarWorkflows();
                await CarregarTarefas();

                return View(ApontamentoView);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpPost]
        public async Task<JsonResult> FluxoTrabalho(Guid idTarefa, Guid idWorkflow)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CarregarWorkflows();
                    await CarregarTarefas();

                    return Json(new { success = false, message = "", body = ApontamentoView });
                }

                Infra.CrossCutting.Util.Queries.Responses.Tarefa.GetTarefaResponse getTarefaResponse = await _tarefaService.GetAsync(Token, new Infra.CrossCutting.Util.Queries.Requests.Tarefa.GetTarefaQuery { Id = idTarefa });
                TarefaViewModel tarefa = getTarefaResponse.Tarefa;

                Infra.CrossCutting.Util.Queries.Responses.Workflow.GetWorkflowResponse getWorkflowResponse = await _workflowService.GetAsync(Token, new Infra.CrossCutting.Util.Queries.Requests.Workflow.GetWorkflowQuery { Id = idWorkflow });
                WorkflowViewModel workflow = getWorkflowResponse.Workflow;

                tarefa.IdWorkflow = workflow.Id;
                tarefa.Workflow = workflow;

                await _tarefaService.UpdateAsync(Token, new UpdateTarefaCommand { Tarefa = tarefa });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task CarregarTarefasByRecurso(Guid idRecurso)
        {
            Infra.CrossCutting.Util.Queries.Responses.Tarefa.GetByRecursoResponse response = await _tarefaService.GetByRecursoAsync(Token, new Infra.CrossCutting.Util.Queries.Requests.Tarefa.GetByRecursoQuery { IdRecurso = idRecurso });
            ApontamentoView.ListaTarefas = response.Tarefas;
        }

        private async Task CarregarWorkflows()
        {
            Infra.CrossCutting.Util.Queries.Responses.Workflow.ListWorkflowResponse response = await _workflowService.AllAsync(Token, new Infra.CrossCutting.Util.Queries.Requests.Workflow.ListWorkflowQuery { });
            ApontamentoView.ListaWorkflow = response.Workflows;
        }

        private async Task CarregarTarefas()
        {
            Infra.CrossCutting.Util.Queries.Responses.Tarefa.ListTarefaResponse response = await _tarefaService.AllAsync(Token, new Infra.CrossCutting.Util.Queries.Requests.Tarefa.ListTarefaQuery { GetDependencies = true });
            ApontamentoView.ListaTarefas = response.Tarefas;
        }
    }
}