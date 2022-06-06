using BoletoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class ListaBoletos
    {
        public Boletos boletos { get; set; }

        public ListaBoletos()
        {
            boletos = new Boletos();
        }
    }
}