using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace api.Models
{
    public class Retorno
    {
        public string html { get; set; }
        public string nossoNumero { get; set; }
        public string numeroDocumento { get; set; }
        public string codigoBeneficiario { get; set; }
        public decimal valor { get; set; }
    }
}