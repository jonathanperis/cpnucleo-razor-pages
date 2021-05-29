﻿using Cpnucleo.Infra.CrossCutting.Util.Commands.Responses.TipoTarefa;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using MediatR;
using System.Runtime.Serialization;

namespace Cpnucleo.Infra.CrossCutting.Util.Commands.Requests.TipoTarefa
{
    [DataContract]
    public class UpdateTipoTarefaCommand : IRequest<UpdateTipoTarefaResponse>
    {
        [DataMember(Order = 1)]
        public TipoTarefaViewModel TipoTarefa { get; set; }
    }
}