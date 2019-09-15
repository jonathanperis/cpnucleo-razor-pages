﻿using Cpnucleo.Application.Interfaces;
using Cpnucleo.Application.ViewModels;
using Cpnucleo.RazorPages.Pages.Tarefa;
using Moq;
using SparkyTestHelpers.AspNetMvc.Core;
using SparkyTestHelpers.DataAnnotations;
using System;
using System.Collections.Generic;
using Xunit;

namespace Cpnucleo.RazorPages.Test.Pages.Tarefa
{
    public class IncluirTest
    {
        private readonly Mock<ITarefaAppService> _tarefaAppService;
        private readonly Mock<IAppService<ProjetoViewModel>> _projetoAppService;
        private readonly Mock<IAppService<SistemaViewModel>> _sistemaAppService;
        private readonly Mock<IWorkflowAppService> _workflowAppService;
        private readonly Mock<IAppService<TipoTarefaViewModel>> _tipoTarefaAppService;

        public IncluirTest()
        {
            _tarefaAppService = new Mock<ITarefaAppService>();
            _projetoAppService = new Mock<IAppService<ProjetoViewModel>>();
            _sistemaAppService = new Mock<IAppService<SistemaViewModel>>();
            _workflowAppService = new Mock<IWorkflowAppService>();
            _tipoTarefaAppService = new Mock<IAppService<TipoTarefaViewModel>>();
        }

        [Fact]
        public void Test_OnGet()
        {
            // Arrange
            var listaProjetosMock = new List<ProjetoViewModel> { };
            var listaSistemasMock = new List<SistemaViewModel> { };
            var listaWorkflowMock = new List<WorkflowViewModel> { };
            var listaTipoTarefaMock = new List<TipoTarefaViewModel> { };

            _projetoAppService.Setup(x => x.Listar()).Returns(listaProjetosMock);
            _sistemaAppService.Setup(x => x.Listar()).Returns(listaSistemasMock);
            _workflowAppService.Setup(x => x.Listar()).Returns(listaWorkflowMock);
            _tipoTarefaAppService.Setup(x => x.Listar()).Returns(listaTipoTarefaMock);

            var pageModel = new IncluirModel(_tarefaAppService.Object, _projetoAppService.Object, _sistemaAppService.Object, _workflowAppService.Object, _tipoTarefaAppService.Object);

            var pageTester = new PageModelTester<IncluirModel>(pageModel);

            // Act
            pageTester
                .Action(x => x.OnGet)

                // Assert
                .TestPage();
        }

        [Theory]
        [InlineData("Tarefa de Teste", 1)]
        public void Test_OnPost(string nome, Guid idProjeto)
        {
            // Arrange
            var tarefaMock = new TarefaViewModel { Nome = nome, IdProjeto = idProjeto };

            _tarefaAppService.Setup(x => x.Incluir(tarefaMock));

            var pageModel = new IncluirModel(_tarefaAppService.Object, _projetoAppService.Object, _sistemaAppService.Object, _workflowAppService.Object, _tipoTarefaAppService.Object);

            var pageTester = new PageModelTester<IncluirModel>(pageModel);

            // Act
            pageTester
                .Action(x => x.OnPost)

                // Assert
                .WhenModelStateIsValidEquals(false)
                .TestPage();

            // Act
            pageTester
                .Action(x => x.OnPost)

                // Assert
                .WhenModelStateIsValidEquals(true)
                .TestRedirectToPage("Listar");

            // Assert
            Validation.For(tarefaMock).ShouldReturn.NoErrors();
        }
    }
}
