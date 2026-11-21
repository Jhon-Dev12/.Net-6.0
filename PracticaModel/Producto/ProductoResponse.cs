
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaModel.Producto
{


    public record ProductoRequest(string? NombreFiltro);
    public class ProductoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
    }
}
