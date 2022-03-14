using FluentAssertions;
using Mustaxe.Application.Services.CEP.Queries;
using Mustaxe.Application.Services.CEP.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mustaxe.Tests.Application.CEP.Validators
{
    public class ConsultarCEPValidatorTests
    {
        public ConsultarCEPValidatorTests()
        {}

        [Fact]
        public void ConsultarCEPValidator_ReturnNoErrors_WhenParametersOk() 
        {
            //Arrange
            var validator = new ConsultarCEPValidator();
            var query = new ConsultarCEPQuery("11111111");

            //Act
            var result = validator.Validate(query);

            //Assert
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void ConsultarCEPValidator_ReturnError_WhenCEPIsNull()
        {
            //Arrange
            var validator = new ConsultarCEPValidator();
            var query = new ConsultarCEPQuery(null);

            //Act
            var result = validator.Validate(query);

            //Assert
            result.Errors.Should().Contain(x => x.ErrorMessage == "O CEP nao pode ser vazio.");
        }

        [Fact]
        public void ConsultarCEPValidator_ReturnError_WhenCEPIsNotANumber()
        {
            //Arrange
            var validator = new ConsultarCEPValidator();
            var query = new ConsultarCEPQuery("not_a_number");

            //Act
            var result = validator.Validate(query);

            //Assert
            result.Errors.Should().Contain(x => x.ErrorMessage == "O CEP deve conter apenas numeros.");
        }

        [Fact]
        public void ConsultarCEPValidator_ReturnError_WhenCEPExceedMaximumLength()
        {
            //Arrange
            var validator = new ConsultarCEPValidator();
            var query = new ConsultarCEPQuery("111111111");

            //Act
            var result = validator.Validate(query);

            //Assert
            result.Errors.Should().Contain(x => x.ErrorMessage == "O CEP deve conter 8 digitos.");
        }

        [Fact]
        public void ConsultarCEPValidator_ReturnError_WhenCEPDoesntReachMinimunLength()
        {
            //Arrange
            var validator = new ConsultarCEPValidator();
            var query = new ConsultarCEPQuery("1111111");

            //Act
            var result = validator.Validate(query);

            //Assert
            result.Errors.Should().Contain(x => x.ErrorMessage == "O CEP deve conter 8 digitos.");
        }
    }
}
