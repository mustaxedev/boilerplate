using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Mustaxe.Application.Services.CEP.Handlers;
using Mustaxe.Application.Services.CEP.Queries;
using Mustaxe.Application.ViewModels.CEP;
using Mustaxe.Domain.Entities;
using Mustaxe.Infra.Configurations.MessageBroker;
using Mustaxe.Infra.Interfaces;
using Mustaxe.Integration.ViaCEP.Interfaces;
using Mustaxe.Integration.ViaCEP.Responses;
using Mustaxe.Persistence.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mustaxe.Infra.Application.CEP.Queries
{
    public class ConsultarCEPQueryHandlerTests
    {
        private readonly Mock<IConsultaCEP> _consultaCEP;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger> _logger;
        private readonly Mock<IMessageBrokerService<RabbitMQMessage>> _messageBrokerService;
        private readonly Mock<IRepository<ConsultaCEP>> _consultaCEPRepository;

        private string _cep;
        private CancellationToken _cancellationToken;

        public ConsultarCEPQueryHandlerTests() 
        {
            _cancellationToken = new CancellationToken();

            _consultaCEP = new Mock<IConsultaCEP>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger>();
            _messageBrokerService = new Mock<IMessageBrokerService<RabbitMQMessage>>();
            _consultaCEPRepository = new Mock<IRepository<ConsultaCEP>>();

            _cep = "11111111";
        }

        [Fact]
        public async Task ConsultarCEPueryHandler_ReturnCEP_WhenCEPIsFoundInDatabase()
        {
            //Arrange
            var consultaCEP = new Faker<ConsultaCEP>()
                .RuleFor(x => x.CEP, _ => _cep)
                .RuleFor(x => x.Bairro, r => r.Address.StreetSuffix())
                .RuleFor(x => x.Complemento, r => r.Random.Int(1, 2000).ToString())
                .RuleFor(x => x.DDD, r => r.Random.Int(20, 80).ToString())
                .RuleFor(x => x.Localidade, r => r.Address.City())
                .RuleFor(x => x.Logradouro, r => r.Address.StreetAddress())
                .RuleFor(x => x.UF, r => r.Address.StateAbbr())
                .RuleFor(x => x.SIAFI, r => r.Random.Int(1000, 5000).ToString())
                .RuleFor(x => x.GIA, r => r.Random.Int(300000, 450000).ToString())
                .RuleFor(x => x.IBGE, r => r.Random.Int(1000, 50000).ToString());

            var consultaCEPResult = new List<ConsultaCEP>()
            {
                new Faker<ConsultaCEP>()
                    .RuleFor(x => x.CEP, _ => _cep)
                    .RuleFor(x => x.Bairro, r => r.Address.StreetSuffix())
                    .RuleFor(x => x.Complemento, r => r.Random.Int(1, 2000).ToString())
                    .RuleFor(x => x.DDD, r => r.Random.Int(20, 80).ToString())
                    .RuleFor(x => x.Localidade, r => r.Address.City())
                    .RuleFor(x => x.Logradouro, r => r.Address.StreetAddress())
                    .RuleFor(x => x.UF, r => r.Address.StateAbbr())
                    .RuleFor(x => x.SIAFI, r => r.Random.Int(1000, 5000).ToString())
                    .RuleFor(x => x.GIA, r => r.Random.Int(300000, 450000).ToString())
                    .RuleFor(x => x.IBGE, r => r.Random.Int(1000, 50000).ToString())
            };
            
            _consultaCEPRepository.Setup(x => x.Search(It.IsAny<Expression<Func<ConsultaCEP, bool>>>()))
                       .Returns(consultaCEPResult.AsQueryable());

            var consultaCEPViewModel = new Faker<ConsultarCEPViewModel>()
                .RuleFor(x => x.CEP, _ => _cep);

            _mapper.Setup(x => x.Map<ConsultarCEPViewModel>(It.IsAny<ConsultaCEP>()))
                   .Returns(consultaCEPViewModel);

            _logger.Setup(x => x.Information(It.IsAny<string>(), It.IsAny<string>()));

            var query = new ConsultarCEPQuery(_cep);
            var handler = new ConsultarCEPQueryHandler(_consultaCEP.Object, _mapper.Object, _logger.Object, _messageBrokerService.Object, _consultaCEPRepository.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeOfType<ConsultarCEPViewModel>();
            _logger.Verify(mock => mock.Information(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

        }

        [Fact]
        public async Task ConsultarCEPueryHandler_ReturnCEP_WhenCEPIsFoundApi() 
        {
            //Arrange
            var consultaCEPResponse = new Faker<ConsultaCEPResponse>()
                .RuleFor(x => x.Cep, _ => _cep)
                .RuleFor(x => x.Bairro, r => r.Address.StreetSuffix())
                .RuleFor(x => x.Complemento, r => r.Random.Int(1, 2000).ToString())
                .RuleFor(x => x.DDD, r => r.Random.Int(20, 80).ToString())
                .RuleFor(x => x.Localidade, r => r.Address.City())
                .RuleFor(x => x.Logradouro, r => r.Address.StreetAddress())
                .RuleFor(x => x.UF, r => r.Address.StateAbbr())
                .RuleFor(x => x.Siafi, r => r.Random.Int(1000, 5000).ToString())
                .RuleFor(x => x.GIA, r => r.Random.Int(300000, 450000).ToString())
                .RuleFor(x => x.IBGE, r => r.Random.Int(1000, 50000).ToString());

            var consultaCEPViewModel = new Faker<ConsultarCEPViewModel>()
                .RuleFor(x => x.CEP, _ => _cep);

            _consultaCEP.Setup(x => x.ConsultarCEP(_cep))
                        .ReturnsAsync(consultaCEPResponse);

            _mapper.Setup(x => x.Map<ConsultarCEPViewModel>(It.IsAny<ConsultaCEPResponse>()))
                   .Returns(consultaCEPViewModel);

            _logger.Setup(x => x.Information(It.IsAny<string>(), It.IsAny<string>()));

            var query = new ConsultarCEPQuery(_cep);
            var handler = new ConsultarCEPQueryHandler(_consultaCEP.Object, _mapper.Object, _logger.Object, _messageBrokerService.Object, _consultaCEPRepository.Object);
            
            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeOfType<ConsultarCEPViewModel>();
            _logger.Verify(mock => mock.Information(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

        }

        [Fact]
        public async Task ConsultarCEPueryHandler_ReturnNull_WhenCEPIsNotFound()
        {
            //Arrange
            _consultaCEP.Setup(x => x.ConsultarCEP(_cep))
                        .ReturnsAsync(new ConsultaCEPResponse());

            _logger.Setup(x => x.Information(It.IsAny<string>(), It.IsAny<string>()));

            var query = new ConsultarCEPQuery(_cep);
            var handler = new ConsultarCEPQueryHandler(_consultaCEP.Object, _mapper.Object, _logger.Object, _messageBrokerService.Object, _consultaCEPRepository.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeNull();
            _logger.Verify(mock => mock.Information(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
