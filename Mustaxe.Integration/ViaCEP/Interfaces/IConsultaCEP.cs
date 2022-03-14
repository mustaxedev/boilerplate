using Mustaxe.Integration.ViaCEP.Responses;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Integration.ViaCEP.Interfaces
{
    public interface IConsultaCEP
    {
        [Get("/ws/{cep}/json")]
        Task<ConsultaCEPResponse> ConsultarCEP(string cep);
    }
}
