using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Mustaxe.Application.Services.CEP.Queries;
using Mustaxe.Application.ViewModels.CEP;
using Mustaxe.Domain.Entities;
using Mustaxe.Infra.Configurations.MessageBroker;
using Mustaxe.Infra.Interfaces;
using Mustaxe.Integration.ViaCEP.Interfaces;
using Mustaxe.Persistence.Repository;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mustaxe.Application.Services.CEP.Handlers
{
    public class ConsultarCEPQueryHandler : IRequestHandler<ConsultarCEPQuery, ConsultarCEPViewModel>
    {
        private readonly IConsultaCEP _consultaCEP;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IMessageBrokerService<RabbitMQMessage> _brokerService;
        private readonly IRepository<ConsultaCEP> _consultaCEPRepository;
        private const string QueueName = "CONSULTA_CEP_QUEUE";
        

        public ConsultarCEPQueryHandler(IConsultaCEP consultaCEP,
            IMapper mapper,
            ILogger logger,
            IMessageBrokerService<RabbitMQMessage> brokerService,
            IRepository<ConsultaCEP> consultaCEPRepository)
        {
            _consultaCEP = consultaCEP;
            _mapper = mapper;
            _logger = logger;
            _brokerService = brokerService;
            _consultaCEPRepository = consultaCEPRepository;
        }

        public async Task<ConsultarCEPViewModel> Handle(ConsultarCEPQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Consulta para o CEP {cepProperty} realizada.", request.CEP);
           
            var entity = await _consultaCEPRepository.Search(x => x.CEP == request.CEP).FirstOrDefaultAsync();
            if (entity != null) 
            {
                _logger.Information("Registro encontrado na base de dados para o CEP {cepProperty}.", request.CEP);
                return _mapper.Map<ConsultarCEPViewModel>(entity);
            }

            var viacep = await _consultaCEP.ConsultarCEP(request.CEP);
            if (viacep.Cep is null) 
            {
                _logger.Information("Nenhum registro encontrado para o CEP {cepProperty}.", request.CEP);
                return null;
            }

            _logger.Information("Registro encontrado para o CEP {cepProperty} via api ViaCEP.", request.CEP);

            entity = await _consultaCEPRepository.CreateAsync(_mapper.Map<ConsultaCEP>(viacep));

            _logger.Information("Registro salvo na base de dados para o CEP {cepProperty}.", request.CEP);

            //Publicando mensagem ao broker
            _brokerService.SendMessage(new RabbitMQMessage
            {
                Message = JsonConvert.SerializeObject(entity),
                Queue = QueueName
            });

            return _mapper.Map<ConsultarCEPViewModel>(entity);
        }
    }
}
