using MediatR;
using Mustaxe.Application.ViewModels.CEP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Application.Services.CEP.Queries
{
    public class ConsultarCEPQuery : IRequest<ConsultarCEPViewModel>
    {
        public string CEP { get; set; }

        public ConsultarCEPQuery(string cep) 
        {
            CEP = cep;
        }
    }
}
