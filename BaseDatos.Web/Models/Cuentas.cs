﻿using System.Collections.Generic;

namespace BaseDatos.Web.Models
{
    public class Cuentas
    {
        public List<string> cuentaPropia { get; set; }
        public List<string> cuentaAjena { get; set; }

        public Cuentas()
        {
            cuentaPropia = new List<string>();
            cuentaAjena = new List<string>();
        }

    }
}