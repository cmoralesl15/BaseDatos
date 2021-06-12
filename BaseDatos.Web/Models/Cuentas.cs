using System.Collections.Generic;

namespace BaseDatos.Web.Models
{
    public class Cuentas
    {
        public Dictionary<string, string> cuentaPropia { get; set; }
        public List<string> cuentaAjena { get; set; }

        public Cuentas()
        {
            cuentaPropia = new Dictionary<string, string>();
            cuentaAjena = new List<string>();
        }

    }
}