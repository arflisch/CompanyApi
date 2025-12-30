using Domain.DTO;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class CompanyDtoValidator : AbstractValidator<CreateCompanyDto>
    {
        public CompanyDtoValidator()
        {
            RuleFor(company => company.Name)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(50).WithMessage("Company name must not exceed 50 characters.");
            RuleFor(company => company.Vat)
                .NotEmpty().WithMessage("VAT number is required.")
                .MaximumLength(20).WithMessage("VAT number must not exceed 20 characters.");
        }
    }
}
