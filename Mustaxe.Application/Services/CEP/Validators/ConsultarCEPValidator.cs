using FluentValidation;
using Mustaxe.Application.Common.Utils;
using Mustaxe.Application.Services.CEP.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Application.Services.CEP.Validators
{
    public class ConsultarCEPValidator : AbstractValidator<ConsultarCEPQuery>
    {
        public ConsultarCEPValidator()
        {
            RuleFor(x => x.CEP)
                .NotNull()
                .WithMessage("O CEP nao pode ser vazio.");

            RuleFor(x => x.CEP)
                .NotEmpty()
                .WithMessage("O CEP nao pode ser vazio.");

            RuleFor(x => x.CEP)
                .Matches(RegexUtils.OnlyNumbersRegex)
                .WithMessage("O CEP deve conter apenas numeros.");

            RuleFor(x => x.CEP)
                .MaximumLength(8)
                .WithMessage("O CEP deve conter 8 digitos.");

            RuleFor(x => x.CEP)
                .MinimumLength(8)
                .WithMessage("O CEP deve conter 8 digitos.");
        }
    }
}
