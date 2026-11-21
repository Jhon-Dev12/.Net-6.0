using Microsoft.Data.SqlClient;
using PracticaModel.Producto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaData.ProductoDA
{
    public class ProductoDA
    {
        public async Task<List<ProductoResponse>> ListarProductosAsync(ProductoRequest req, string cnx)
        {
            var lista = new List<ProductoResponse>();
            try
            {
                using (SqlConnection conn = new SqlConnection(cnx))
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Nombre FROM Productos WHERE (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')", conn))
                {
                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar).Value = (object?)req.NombreFiltro ?? DBNull.Value;
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            lista.Add(new ProductoResponse
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            });
                        }
                    }

                    return lista;
                }
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
