using PracticaData.SeguridadDA;
using PracticaModel.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaBussiness.Seguridad
{
    public class Login
    {
        public async Task<string> login(LoginRequest request, string conex){
            SeguridadDa seguridad = new SeguridadDa();
            return await seguridad.login(request, conex);
        }
    }
}
