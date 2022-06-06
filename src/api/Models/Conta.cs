using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class Conta
    {
        [JsonProperty("id")]
        public int id;
        [JsonProperty("dataVencimento")]
        public DateTime dataVencimento;
        [JsonProperty("cedente")]
        public Cedente cedente;
        [JsonProperty("valor")]
        public decimal valor;
        [JsonProperty("nossoNumero")]
        public string nossoNumero;
        [JsonProperty("cliente")]
        public Cliente cliente;
        [JsonProperty("lancamentos")]
        public List<Lancamento> lancamentos;
        [JsonProperty("observacao")]
        public string observacao;
        [JsonProperty("isDecimoTerceiro")]
        public bool isDecimoTerceiro;
    }
}