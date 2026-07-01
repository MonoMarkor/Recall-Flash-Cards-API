using FluentValidation;
using System.Globalization;
using Domain.DTOs;

public class CreateNoteDtoValidator : AbstractValidator<FlashCardDto>
{
    public CreateNoteDtoValidator()
    {
        RuleFor(x => x.Header)
            .NotEmpty().WithMessage("Header is required.")
            .MaximumLength(300).WithMessage("Header cannot exceed 300 characters.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required.");

        RuleFor(x => x.LanguageCode)
            .NotEmpty().WithMessage("Language code is required.")
            .Must(BeAValidCulture).WithMessage("'{PropertyValue}' is not a valid or recognized language code.");
    }

    private bool BeAValidCulture(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName)) return false;

        try
        {
            _ = CultureInfo.GetCultureInfo(cultureName);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}