﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class Cliente
    {
        public string cpfCnpj;
        public string razaoSocial;
        public string endereco;
        public string bairro;
        public string cidade;
        public string cep;
        public string uf;
        public decimal valorMensalidade;
    }
}