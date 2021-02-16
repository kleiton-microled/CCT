using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CCT.classes.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.Linq;

namespace CCT.Testes
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DeserializarXML()
        {

            var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><retornoServico xmlns=""http://www.pucomex.serpro.gov.br/cct""><mensagens><mensagem><codigo>PUCX-ER0010</codigo><descricao>XML não atende as especificações definidas no XSD (Regras Verificadas: obrigatoriedade, tamanho, formato e domínio dos campos).</descricao></mensagem></mensagens><mensagensXSD><mensagem>[lin:1,col:485] cvc-complex-type.2.4.a: Invalid content was found starting with element 'CPF'. One of '{""http://www.pucomex.serpro.gov.br/cct"":cnpj, ""http://www.pucomex.serpro.gov.br/cct"":cpf, ""http://www.pucomex.serpro.gov.br/cct"":nomeEstrangeiro}' is expected.</mensagem><mensagem>[lin:1,col:485] elemento inesperado (uri:""http://www.pucomex.serpro.gov.br/cct"", local:""CPF""). Os elementos esperados são &lt;{http://www.pucomex.serpro.gov.br/cct}nomeEstrangeiro&gt;,&lt;{http://www.pucomex.serpro.gov.br/cct}viaTransporte&gt;,&lt;{http://www.pucomex.serpro.gov.br/cct}cnpj&gt;,&lt;{http://www.pucomex.serpro.gov.br/cct}cpf&gt;,&lt;{http://www.pucomex.serpro.gov.br/cct}baldeacaoOuTransbordo&gt;,&lt;{http://www.pucomex.serpro.gov.br/cct}transitoSimplificado&gt;</mensagem></mensagensXSD></retornoServico>";

            var unescapedString = Regex.Unescape(xml);

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(unescapedString)))
            {
                XmlSerializer xmlSerializer;
                StreamReader reader;
                var msgCritica = string.Empty;

                if (xml.Contains("retornoServico"))
                {
                    xmlSerializer = new XmlSerializer(typeof(retornoServico));
                    reader = new StreamReader(memoryStream, Encoding.UTF8);
                    var retorno = (retornoServico)xmlSerializer.Deserialize(reader);

                    if (retorno != null)
                    {
                        msgCritica = retorno.mensagens.mensagem.descricao;
                    }
                }
                else
                {
                    xmlSerializer = new XmlSerializer(typeof(Error));
                    reader = new StreamReader(memoryStream, Encoding.UTF8);
                    var retorno = (Error)xmlSerializer.Deserialize(reader);

                    if (retorno != null)
                    {
                        msgCritica = retorno.Detail?.Errors.Length > 0
                            ? retorno?.Message + string.Join(",", retorno.Detail.Errors.Select(x => x.Message))
                            : retorno?.Message;
                    }
                }



                Assert.AreEqual(3, 1);
            }

        }
    }
}
