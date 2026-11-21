using PracticaModel.Producto;
using FluentValidation;
namespace WebApplication1.Validators
{
    public class ProductoValidator : AbstractValidator<ProductoRequest>
    {
        public ProductoValidator()
        {
            RuleFor(x=>x.NombreFiltro).NotEmpty().WithMessage("El nombre es requerido");
        }
    }
}
