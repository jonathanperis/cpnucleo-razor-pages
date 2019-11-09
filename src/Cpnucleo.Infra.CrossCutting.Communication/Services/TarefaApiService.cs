﻿using Cpnucleo.Infra.CrossCutting.Communication.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace Cpnucleo.Infra.CrossCutting.Communication.Services
{
    public class TarefaApiService : CrudApiService<TarefaViewModel>, ITarefaApiService
    {
        private const string actionRoute = "tarefa";

        public bool Incluir(string token, TarefaViewModel obj)
        {
            return Post(token, actionRoute, obj);
        }

        public IEnumerable<TarefaViewModel> Listar(string token)
        {
            return Get(token, actionRoute);
        }

        public TarefaViewModel Consultar(string token, Guid id)
        {
            return Get(token, actionRoute, id);
        }

        public bool Remover(string token, Guid id)
        {
            return Delete(token, actionRoute, id);
        }

        public bool Alterar(string token, TarefaViewModel obj)
        {
            return Put(token, actionRoute, obj.Id, obj);
        }

        public bool AlterarPorPercentualConcluido(string token, Guid idTarefa, int? percentualConcluido)
        {
            throw new NotImplementedException();
        }

        public bool AlterarPorWorkflow(string token, Guid idTarefa, Guid idWorkflow)
        {
            try
            {
                RestRequest request = new RestRequest($"api/v2/{actionRoute}/putbyworkflow/{idTarefa.ToString()}", Method.PUT);
                request.AddHeader("Authorization", token);
                request.AddJsonBody(idWorkflow);

                var response = _client.Execute(request);

                return response.StatusCode == HttpStatusCode.OK ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
