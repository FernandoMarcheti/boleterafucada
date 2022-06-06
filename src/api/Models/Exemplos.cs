using BoletoNet;
using BoletoNet.DemonstrativoValoresBoleto;
using BoletoNet.RelatorioValoresBoleto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace api.Models
{
    public enum Bancos
    {
        BancodoBrasil = 1,
        Banrisul = 41,
        Basa = 3,
        Bradesco = 237,
        BRB = 70,
        Caixa = 104,
        HSBC = 399,
        Itau = 341,
        Real = 356,
        Safra = 422,
        Santander = 33,
        Sicoob = 756,
        Sicred = 748,
        Sudameris = 347,
        Unibanco = 409
    }

    public class Exemplos
    {

        public Exemplos(int Banco)
        {
            boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = (short)Banco;
            
        }
        public BoletoBancario boletoBancario { get; set; }

        // **************************************************

        public Retorno Credcitrus(Conta obj, Boletos boletos, string codigo, int digitoCodigo)
        {
            string codigoCliente = codigo;

            Retorno objRetorno = new Retorno();
            DateTime vencimento = obj.dataVencimento;
            BoletoNet.Cedente c = new BoletoNet.Cedente((string)obj.cedente.cnpj, (string)obj.cedente.razaoSocial, (string)obj.cedente.agencia,
                (string)obj.cedente.digitoAgencia, (string)obj.cedente.conta, (string)obj.cedente.digitoConta);
            c.Codigo = codigoCliente;
            c.DigitoCedente = digitoCodigo;
            c.Carteira = "1"; 
            c.NumeroSequencial = 1;
            c.Endereco = new Endereco();
            c.Endereco.End = (string)obj.cliente.endereco;
            c.Endereco.CEP = (string)obj.cliente.cep;
            c.Endereco.Bairro = (string)obj.cliente.bairro;
            c.Endereco.UF = (string)obj.cliente.uf;
            c.Endereco.Cidade = (string)obj.cliente.cidade;
            c.Endereco.Numero = "";


            string nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0');
            string nDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');

            var b = new Boleto(vencimento, (decimal)obj.valor, "1", nossoNumero, c);
            
            b.NumeroParcela = 1;

            b.ModalidadeCobranca = 1;
            b.TipoModalidade = "01";
            b.NumeroParcela = 1;

            b.Remessa = new Remessa(TipoOcorrenciaRemessa.EntradaDeTitulos);
            b.DataDocumento = DateTime.Now;
            b.Banco = new Banco(756);
            b.NumeroDocumento = nDocumento;
            b.EspecieDocumento = new EspecieDocumento_Sicoob("1");
            b.Aceite = "A";
            
            b.Sacado = new Sacado((string)obj.cliente.cpfCnpj, (string)obj.cliente.razaoSocial);
            b.Sacado.Endereco.End = (string)obj.cliente.endereco;
            b.Sacado.Endereco.Bairro = (string)obj.cliente.bairro;
            b.Sacado.Endereco.Cidade = (string)obj.cliente.cidade;
            b.Sacado.Endereco.CEP = (string)obj.cliente.cep;
            b.Sacado.Endereco.UF = (string)obj.cliente.uf;

            Instrucao_Sicoob protestar = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar5DiasUteis, 5);
            if (obj.cedente.jurosMora > 0)
            {
                Instrucao_Sicoob juros = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.CobrarJuros, (double)obj.cedente.jurosMora);
                b.Instrucoes.Add(juros);
                b.PercJurosMora = obj.cedente.jurosMora;
            }

            b.Instrucoes.Add(protestar);

            if (obj.cedente.valorMulta > 0)
            {
                Instrucao_Sicoob multa = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.ValorMulta, obj.cedente.valorMulta);
                b.Instrucoes.Add(multa);
            }

            boletoBancario.Boleto = b;

            List<Lancamento> lancamentos = obj.lancamentos;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.JurosMora = obj.cedente.jurosMora;
            boletoBancario.Boleto.ValorMulta = obj.cedente.valorMulta;

            boletoBancario.Boleto.Valida();

            if (obj.valor > 0)
                boletos.Add(boletoBancario.Boleto);

            var retorno = boletoBancario.MontaHtmlEmbedded();
            retorno = retorno.Replace("<tr class=\"ct\">", "<tr class=\"ct\" style=\"height: 12px;\" >");
            retorno = formatarBoleto(retorno);

            objRetorno.html = WebUtility.HtmlDecode(retorno);
            objRetorno.nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0');
            objRetorno.numeroDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');
            objRetorno.codigoBeneficiario = codigoCliente;
            objRetorno.valor = (decimal)obj.valor;
            return objRetorno;

        }

        public Retorno CredcitrusDecimo(Conta obj, Boletos boletos, string codigo, int digitoCodigo)
        {
            string codigoCliente = codigo;

            Retorno objRetorno = new Retorno();
            DateTime vencimento = obj.dataVencimento;
            BoletoNet.Cedente c = new BoletoNet.Cedente((string)obj.cedente.cnpj, (string)obj.cedente.razaoSocial, (string)obj.cedente.agencia,
                (string)obj.cedente.digitoAgencia, (string)obj.cedente.conta, (string)obj.cedente.digitoConta);
            c.Codigo = codigoCliente;
            c.DigitoCedente = digitoCodigo;
            c.Carteira = "1";
            c.NumeroSequencial = 1;
            c.Endereco = new Endereco();
            c.Endereco.End = (string)obj.cliente.endereco;
            c.Endereco.CEP = (string)obj.cliente.cep;
            c.Endereco.Bairro = (string)obj.cliente.bairro;
            c.Endereco.UF = (string)obj.cliente.uf;
            c.Endereco.Cidade = (string)obj.cliente.cidade;
            c.Endereco.Numero = "";


            string nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0');
            string nDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');

            var b = new Boleto(vencimento, (decimal)obj.valor, "1", nossoNumero, c);

            b.NumeroParcela = 1;

            b.ModalidadeCobranca = 1;
            b.TipoModalidade = "01";
            b.NumeroParcela = 1;

            b.Remessa = new Remessa(TipoOcorrenciaRemessa.EntradaDeTitulos);
            b.DataDocumento = DateTime.Now;
            b.Banco = new Banco(756);
            b.NumeroDocumento = nDocumento;
            b.EspecieDocumento = new EspecieDocumento_Sicoob("1");
            b.Aceite = "A";

            b.Sacado = new Sacado((string)obj.cliente.cpfCnpj, (string)obj.cliente.razaoSocial);
            b.Sacado.Endereco.End = (string)obj.cliente.endereco;
            b.Sacado.Endereco.Bairro = (string)obj.cliente.bairro;
            b.Sacado.Endereco.Cidade = (string)obj.cliente.cidade;
            b.Sacado.Endereco.CEP = (string)obj.cliente.cep;
            b.Sacado.Endereco.UF = (string)obj.cliente.uf;

            Instrucao_Sicoob protestar = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar5DiasUteis, 5);
            if (obj.cedente.jurosMora > 0)
            {
                Instrucao_Sicoob juros = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.CobrarJuros, (double)obj.cedente.jurosMora);
                b.Instrucoes.Add(juros);
                b.PercJurosMora = obj.cedente.jurosMora;
            }

            b.Instrucoes.Add(protestar);

            if (obj.cedente.valorMulta > 0)
            {
                Instrucao_Sicoob multa = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.ValorMulta, obj.cedente.valorMulta);
                b.Instrucoes.Add(multa);
            }

            boletoBancario.Boleto = b;

            List<Lancamento> lancamentos = obj.lancamentos;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.JurosMora = obj.cedente.jurosMora;
            boletoBancario.Boleto.ValorMulta = obj.cedente.valorMulta;

            boletoBancario.Boleto.Valida();

            if (obj.valor > 0)
                boletos.Add(boletoBancario.Boleto);

            var retorno = boletoBancario.MontaHtmlEmbedded();
            retorno = retorno.Replace("<tr class=\"ct\">", "<tr class=\"ct\" style=\"height: 12px;\" >");
            retorno = formatarBoleto(retorno);

            objRetorno.html = WebUtility.HtmlDecode(retorno);
            objRetorno.nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0');
            objRetorno.numeroDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');
            objRetorno.codigoBeneficiario = codigoCliente;
            objRetorno.valor = (decimal)obj.valor;
            return objRetorno;

        }

        //************************************************

        public Retorno Sicoob(Conta obj, Boletos boletos)
        {
            Retorno objRetorno = new Retorno();
            DateTime vencimento = obj.dataVencimento;
            BoletoNet.Cedente c = new BoletoNet.Cedente((string)obj.cedente.cnpj, (string)obj.cedente.razaoSocial, (string)obj.cedente.agencia,
                (string)obj.cedente.digitoAgencia, (string)obj.cedente.conta, (string)obj.cedente.digitoConta);
            c.Codigo = "009058";
            c.DigitoCedente = 1;
            c.Carteira = "1"; // confirmar
                              //c.CodigoTransmissao = "1";


            string nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0');
            string nDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, (decimal)obj.valor, "1", nossoNumero, c);
            b.NumeroDocumento = nDocumento;
            b.NumeroParcela = 1;

            b.TipoModalidade = "01";
            Remessa remessa = new Remessa { CodigoOcorrencia = "01" };
            b.Remessa = remessa;

            b.Sacado = new Sacado((string)obj.cliente.cpfCnpj, (string)obj.cliente.razaoSocial);
            b.Sacado.Endereco.End = (string)obj.cliente.endereco;
            b.Sacado.Endereco.Bairro = (string)obj.cliente.bairro;
            b.Sacado.Endereco.Cidade = (string)obj.cliente.cidade;
            b.Sacado.Endereco.CEP = (string)obj.cliente.cep;
            b.Sacado.Endereco.UF = (string)obj.cliente.uf;

            Instrucao_Sicoob protestar = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar5DiasUteis, 5);
            if(obj.cedente.jurosMora > 0)
            {
                Instrucao_Sicoob juros = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.CobrarJuros, (double)obj.cedente.jurosMora);
                b.Instrucoes.Add(juros);
                b.PercJurosMora = obj.cedente.jurosMora;
            }

            b.Instrucoes.Add(protestar);

            if (obj.cedente.valorMulta > 0)
            {
                Instrucao_Sicoob multa = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.ValorMulta, obj.cedente.valorMulta);
                b.Instrucoes.Add(multa);
                b.PercMulta = obj.cedente.valorMulta;
            }

            
            boletoBancario.Boleto = b;

            List<Lancamento> lancamentos = obj.lancamentos;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.JurosMora = obj.cedente.jurosMora;
            boletoBancario.Boleto.ValorMulta = obj.cedente.valorMulta;
            
            //demonstrativo((string)obj.observacao, boletoBancario, lancamentos, obj.cliente, obj.cedente);

            boletoBancario.Boleto.Valida();
            
            if(obj.valor > 0)
                boletos.Add(boletoBancario.Boleto);
            //GerarArquivoRemessa("009058", nossoNumero, c, boletoBancario.CodigoBanco, boletoBancario.Boleto);

            var retorno = boletoBancario.MontaHtmlEmbedded();
            retorno = retorno.Replace("<tr class=\"ct\">", "<tr class=\"ct\" style=\"height: 12px;\" >");
            retorno = formatarBoleto(retorno);
            //Byte[] pdf = PdfSharpConvert(retorno);
            //GravarBoleto(retorno, nossoNumero);

            objRetorno.html = WebUtility.HtmlDecode(retorno);
            objRetorno.nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0'); 
            objRetorno.numeroDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');
            objRetorno.codigoBeneficiario = "009058";
            objRetorno.valor = (decimal)obj.valor;
            return objRetorno;

        }

        public Retorno SicoobDecimo(Conta obj, Boletos boletos)
        {
            Retorno objRetorno = new Retorno();
            DateTime vencimento = obj.dataVencimento;
            BoletoNet.Cedente c = new BoletoNet.Cedente((string)obj.cedente.cnpj, (string)obj.cedente.razaoSocial, (string)obj.cedente.agencia,
                (string)obj.cedente.digitoAgencia, (string)obj.cedente.conta, (string)obj.cedente.digitoConta);
            c.Codigo = "009058";
            c.DigitoCedente = 1;
            c.Carteira = "1"; // confirmar
                              //c.CodigoTransmissao = "1";


            string nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0');
            string nDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, (decimal)obj.valor, "1", nossoNumero, c);
            b.NumeroDocumento = nDocumento;
            b.NumeroParcela = 1;

            b.TipoModalidade = "01";
            Remessa remessa = new Remessa { CodigoOcorrencia = "01" };
            b.Remessa = remessa;

            b.Sacado = new Sacado((string)obj.cliente.cpfCnpj, (string)obj.cliente.razaoSocial);
            b.Sacado.Endereco.End = (string)obj.cliente.endereco;
            b.Sacado.Endereco.Bairro = (string)obj.cliente.bairro;
            b.Sacado.Endereco.Cidade = (string)obj.cliente.cidade;
            b.Sacado.Endereco.CEP = (string)obj.cliente.cep;
            b.Sacado.Endereco.UF = (string)obj.cliente.uf;

            Instrucao_Sicoob protestar = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar5DiasUteis, 5);
            if (obj.cedente.jurosMora > 0)
            {
                Instrucao_Sicoob juros = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.CobrarJuros, (double)obj.cedente.jurosMora);
                b.Instrucoes.Add(juros);
                b.PercJurosMora = obj.cedente.jurosMora;
            }

            b.Instrucoes.Add(protestar);

            if (obj.cedente.valorMulta > 0)
            {
                Instrucao_Sicoob multa = new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.ValorMulta, obj.cedente.valorMulta);
                b.Instrucoes.Add(multa);
                b.PercMulta = obj.cedente.valorMulta;
            }

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.JurosMora = obj.cedente.jurosMora;
            boletoBancario.Boleto.ValorMulta = obj.cedente.valorMulta;

            

            boletoBancario.Boleto.Valida();
            boletos.Add(boletoBancario.Boleto);
            //GerarArquivoRemessa("009058", nossoNumero, c, boletoBancario.CodigoBanco, boletoBancario.Boleto);

            var retorno = boletoBancario.MontaHtmlEmbedded();
            retorno = retorno.Replace("<tr class=\"ct\">", "<tr class=\"ct\" style=\"height: 12px;\" >");
            retorno = formatarBoleto(retorno);

            objRetorno.html = WebUtility.HtmlDecode(retorno);
            objRetorno.nossoNumero = obj.nossoNumero.ToString().PadLeft(7, '0');
            objRetorno.numeroDocumento = obj.nossoNumero.ToString().PadLeft(9, '0');
            objRetorno.codigoBeneficiario = "009058";
            objRetorno.valor = (decimal)obj.valor;
            return objRetorno;

        }

        private string formatarBoleto(string boleto)
        {
            var cabecalho = boleto.Substring(0, boleto.IndexOf("<body>")) + "<body>";

            var textoReplace2 = "<td class=\"pL6\">Instruções</td>";
            var rodape = boleto.Substring(boleto.IndexOf(textoReplace2));

            var textoReplace3 = "<table class=\"w666\">";
            rodape = rodape.Substring(rodape.IndexOf(textoReplace3)) + "<div style=\"margin-bottom: 150px;\"> </div>";

            var retorno = cabecalho + rodape;
            return retorno;
        }
        private string formatarBoletoDecimo(string boleto)
        {
            var textoReplace = "Recibo do Pagador</td>";
            var cabecalho = boleto.Substring(0, boleto.IndexOf(textoReplace)) + "Recibo do Pagador</td></tr></table><br />";

            var textoReplace2 = "<td class=\"pL6\">Instruções</td>";
            var rodape = boleto.Substring(boleto.IndexOf(textoReplace2));

            var textoReplace3 = "<table class=\"w666\">";
            rodape = rodape.Substring(rodape.IndexOf(textoReplace3));
            return cabecalho + rodape;
        }

        public static Byte[] PdfSharpConvert(String html)
        {
            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                //var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                //pdf.Save(ms);
                //res = ms.ToArray();
            }
            return res;
        }

        public string Santander(Conta obj)
        {
            DateTime vencimento = obj.dataVencimento;

            BoletoNet.Cedente c = new BoletoNet.Cedente((string)obj.cedente.cnpj, (string)obj.cedente.razaoSocial, (string)obj.cedente.agencia,
                (string)obj.cedente.digitoAgencia, (string)obj.cedente.conta, (string)obj.cedente.digitoConta);
            c.Codigo = "1795082";
            c.CodigoTransmissao = "1";

            string nossoNumero = obj.nossoNumero.ToString().PadLeft(12, '0');
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, (decimal)obj.valor, "101", nossoNumero, c);
            b.NumeroDocumento = nossoNumero;// "0282033";

            b.Sacado = new Sacado((string)obj.cliente.cpfCnpj, (string)obj.cliente.razaoSocial);
            b.Sacado.Endereco.End = (string)obj.cliente.endereco;
            b.Sacado.Endereco.Bairro = (string)obj.cliente.bairro;
            b.Sacado.Endereco.Cidade = (string)obj.cliente.cidade;
            b.Sacado.Endereco.CEP = (string)obj.cliente.cep;
            b.Sacado.Endereco.UF = (string)obj.cliente.uf;

            //Espécie Documento - [R] Recibo
            b.EspecieDocumento = new EspecieDocumento_Santander("17");
            Instrucao_Santander iProtestar = new Instrucao_Santander((int)EnumInstrucoes_Santander.Protestar, 5);
            b.Instrucoes.Add(iProtestar);
            boletoBancario.MostrarCodigoCarteira = true;
            boletoBancario.Boleto = b;

            //JArray json = (JArray)obj.lancamentos;
            List<Lancamento> lancamentos = obj.lancamentos;// JsonConvert.DeserializeObject<List<Lancamento>>(json.ToString());
            boletoBancario.Boleto = b;
            //demonstrativo((string)obj.observacao, boletoBancario, lancamentos, obj.cliente, obj.cedente);
            //GerarArquivoRemessa("3941418", nossoNumero, c, boletoBancario.CodigoBanco, boletoBancario.Boleto);

            boletoBancario.Boleto.Valida();

            var retorno = boletoBancario.MontaHtmlEmbedded();
            retorno = formatarBoleto(retorno);
            //GravarBoleto(retorno, nossoNumero);
            return retorno;
        }

        //private void GerarArquivoRemessa(string nConvenio, string nossoNumero, BoletoNet.Cedente c, int numeroBanco, Boleto boleto)
        //{
        //    var path = @"C:\boletos";
        //    path = CriaSubdiretorio(path, nossoNumero);
        //    path += "\\" + nossoNumero + ".txt";
        //    var banco = new BoletoNet.Banco((int)numeroBanco);
        //    Stream st = File.OpenWrite(path);
        //    Boletos boletos = new Boletos();
        //    boletos.Add(boleto);
        //    ArquivoRemessa arquivo = new ArquivoRemessa(TipoArquivo.CNAB400);

        //    int n = Convert.ToInt32(nossoNumero);

        //    arquivo.GerarArquivoRemessa(nConvenio, banco, c, boletos, st, n);

        //}

        //private void GravarBoleto(string boleto, string nossoNumero)
        //{
        //    var path = @"C:\boletos\" + nossoNumero + "\\" + nossoNumero + ".html";
        //    Stream st = File.OpenWrite(path);
        //    StreamWriter incluiLinha = new StreamWriter(st, Encoding.GetEncoding("UTF-8"));
        //    incluiLinha.WriteLine(boleto);

        //    incluiLinha.Close();
        //    incluiLinha.Dispose();
        //}

        private string CriaSubdiretorio(string path, string subDiretorio)
        {
            string pathString = System.IO.Path.Combine(path, subDiretorio);
            System.IO.Directory.CreateDirectory(pathString);
            return pathString;
        }

        //private void demonstrativo(string obs, BoletoBancario boletoBancario, List<Lancamento> lancamentos, dynamic cliente, dynamic empresa)
        //{
        //    boletoBancario.ExibirDemonstrativo = true;

        //    GrupoDemonstrativo gd = new GrupoDemonstrativo();
        //    ItemDemonstrativo id = new ItemDemonstrativo();
        //    id.Descricao = "<pre style=\"font-size: 14px;\">   Mensalidade  </pre> ";
        //    id.Referencia = cliente.valorMensalidade.ToString();
        //    id.Valor = cliente.valorMensalidade;
        //    gd.Itens.Add(id);
        //    int qtdeLancamentos = lancamentos.Count;
        //    foreach (var item in lancamentos)
        //    {id = new ItemDemonstrativo();
        //            id.Descricao = "<pre style=\"font-size: 14px;z\"> " + item.quantidade + "- " + item.produto.nome + "</pre> ";
        //            id.Referencia = item.valorUnitario.ToString();
        //            id.Valor = item.valorUnitario * item.quantidade;
        //            gd.Itens.Add(id);

        //    }

        //    if (obs != null)
        //        gd.Itens.Add(new ItemDemonstrativo { Descricao = "<pre style=\"font-size: 14px;\"> Observação: " + obs + "</pre> " });

        //    boletoBancario.Boleto.Demonstrativos.Add(gd);

        //    boletoBancario.Boleto.JurosMora = empresa.jurosMora;
        //    boletoBancario.Boleto.ValorMulta = empresa.valorMulta;

        //    List<IInstrucao> descricaoProdutos = new List<IInstrucao>();
        //    string msg = empresa.instrucaoCobranca;
        //    Instrucao instrucao = new Instrucao(1) { Descricao = msg, };
        //    descricaoProdutos.Add(instrucao);
        //    boletoBancario.Boleto.Instrucoes = descricaoProdutos;
        //}

        private List<IInstrucao> bindDescricaoDecimoTerceiro(Conta obj)
        {
            //string parcela = obj.IsParcelaUm == true ? "primeira" : "segunda";
            List<IInstrucao> descricaoProdutos = new List<IInstrucao>();
            string msg = obj.observacao;// "<pre>Referente a " + parcela + " parcela de décimo terceiro.</pre>";
            Instrucao instrucao = new Instrucao(1) { Descricao = msg };
            descricaoProdutos.Add(instrucao);
            return descricaoProdutos;
            
        }

    }
}