using Microsoft.Data.SqlClient;
using PracticaModel.Producto;
using PracticaModel.Seguridad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaData.SeguridadDA
{
    public class SeguridadDa
    {
        public async Task<string> login(LoginRequest request, string conex)
        {
            // 1. Solo seleccionamos las columnas reales (id, username, clave) y filtramos ÚNICAMENTE por username
            const string query = "SELECT id, username, clave FROM Usuario WHERE username = @username";

            using (SqlConnection conn = new SqlConnection(conex))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = (object?)request.userName ?? DBNull.Value;

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        throw new Exception("Usuario o contraseña incorrectos");
                    }

                    string claveHash = reader.GetString(reader.GetOrdinal("clave"));

                    // 2. Ahora sí llega aquí y compara "1234" contra el hash "$2a$11$..."
                    bool esValida = BCrypt.Net.BCrypt.Verify(request.password, claveHash);
                    if (!esValida)
                    {
                        throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
                    }

                    // 3. Usamos los nombres exactos de las columnas de tu tabla: "id" y "username"
                    int idUsuario = reader.GetInt32(reader.GetOrdinal("id"));
                    string nombre = reader.GetString(reader.GetOrdinal("username"));

                    return generateToken(idUsuario, nombre);
                }
            }
        }


        public string generateToken(int idUsuario, string clave)
        {
            return $"sdsdsd{idUsuario},{clave}";
        }
    }
}
