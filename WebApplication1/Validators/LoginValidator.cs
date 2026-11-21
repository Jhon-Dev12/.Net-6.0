using FluentValidation;
using PracticaModel.Seguridad;

namespace WebApplication1.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.userName).NotEmpty().WithMessage("El nombre de usuario es requerido");
            RuleFor(x => x.password).NotEmpty().WithMessage("La contraseña es requerida");
        }
    }
}
