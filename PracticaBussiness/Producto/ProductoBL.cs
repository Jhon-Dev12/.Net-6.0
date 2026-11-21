using PracticaData.ProductoDA;
using PracticaModel.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaBussiness.Producto
{
    public partial class ProductoBL
    {
        public async Task<List<ProductoResponse>> getProductos(ProductoRequest filtro,string conn)
        {
            ProductoDA prod  = new ProductoDA();
            var buscar = await prod.ListarProductosAsync(filtro, conn);
            return buscar;
        }
    }
}
