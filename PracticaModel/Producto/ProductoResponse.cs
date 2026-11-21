
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaModel.Producto
{


    public class ProductoRequest
    {
        public string? NombreFiltro { get; set; }
    }
    public class ProductoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
    }
}
