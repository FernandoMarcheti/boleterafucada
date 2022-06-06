using api.Models;
using BoletoNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BoletoAPI.Controllers
{
    public class HomeController : ApiController
    {
        //public ActionResult Index()
        //{
        //    ViewBag.Message = "Boleto.Net.MVC";

        //    var bancos = from Bancos s in Enum.GetValues(typeof(Bancos))
        //                 select new { ID = Convert.ChangeType(s, typeof(int)), Name = s.ToString() };
        //    ViewData["bancos"] = new SelectList(bancos, "ID", "Name", Bancos.BancodoBrasil);

        //    return View();
        //}

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Teste()
        {
            Boletos boletos = new Boletos();
            return null;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Get(List<Conta> contas)
        {
            string nConvenio = "242530";
            int digitoCodigo = 0;

            Boletos boletos = new Boletos();
            string boletosHtml = "";
            var response = new HttpResponseMessage();
            List<Retorno> objsRetorno = new List<Retorno>();
            foreach (var obj in contas)
            {
                string boleto = "";
                
                Exemplos exemplos = new Exemplos((int)obj.id);
                switch ((Bancos)obj.id)
                {
                    case Bancos.Sicoob:
                        if (obj.isDecimoTerceiro)
                        {
                            if ("3188".StartsWith(obj.cedente.agencia))
                            {
                                var objRetorno = exemplos.CredcitrusDecimo(obj, boletos, nConvenio, digitoCodigo);
                                objsRetorno.Add(objRetorno);
                            }
                            else if ("3041".StartsWith(obj.cedente.agencia))
                            {
                                nConvenio = "009058";
                                digitoCodigo = 1;
                                var objRetorno = exemplos.SicoobDecimo(obj, boletos);
                                objsRetorno.Add(objRetorno);
                            }
                        }
                        else
                        {
                            if("3188".StartsWith(obj.cedente.agencia))
                            {
                                var objRetorno = exemplos.Credcitrus(obj, boletos, nConvenio, digitoCodigo);
                                objsRetorno.Add(objRetorno);
                            }
                            else if ("3041".StartsWith(obj.cedente.agencia))
                            {
                                nConvenio = "009058";
                                digitoCodigo = 1;
                                var objRetorno = exemplos.Credcitrus(obj, boletos, nConvenio, digitoCodigo);
                                objsRetorno.Add(objRetorno);
                            }
                            
                        }
                        
                        break;
                }
            }
            var c = getCedente(contas[0].cedente, nConvenio, digitoCodigo);
            DateTime d1 = DateTime.Now;
            var nomeAquivo = (d1.ToShortDateString() + d1.ToLongTimeString()).Replace("/", "").Replace(":", "");
            GerarArquivoRemessa(nConvenio, nomeAquivo, c, contas[0].id, boletos, nomeAquivo, contas[0].observacao);
            return Request.CreateResponse(HttpStatusCode.OK, objsRetorno);
        }
        
        private void GerarArquivoRemessa(string nConvenio, string nossoNumero, BoletoNet.Cedente c, int numeroBanco, Boletos boletos, string nomeAquivo, string obs)
        {
            var path = @"C:\boletos";
            path = CriaSubdiretorio(path, nossoNumero);
            path += "\\" + nomeAquivo + ".txt";
            var banco = new BoletoNet.Banco((int)numeroBanco);
            Stream st = File.OpenWrite(path);

            ArquivoRemessa arquivo = new ArquivoRemessa(TipoArquivo.CNAB240);

            arquivo.GerarArquivoRemessa(nConvenio, banco, c, boletos, st, 1);

        }            

        private string CriaSubdiretorio(string path, string subDiretorio)
        {
            string pathString = System.IO.Path.Combine(path, subDiretorio);
            System.IO.Directory.CreateDirectory(pathString);
            return pathString;
        }

        private BoletoNet.Cedente getCedente(api.Models.Cedente cedente, string nConvenio, int digitoCodigo)
        {
            BoletoNet.Cedente c = new BoletoNet.Cedente((string)cedente.cnpj, (string)cedente.razaoSocial, (string)cedente.agencia,
                (string)cedente.digitoAgencia, (string)cedente.conta, (string)cedente.digitoConta);
            c.Codigo = nConvenio;
            c.DigitoCedente = digitoCodigo;
            c.Carteira = "1";
            return c;
        }
        
    }
}
