using CCT.classes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CCT
{
    /// <summary>
    /// Cliente Http Base 
    /// </summary>
    public abstract class HttpClientPOST : HttpClientBase, IDisposable
    {
        /// <summary>
        /// Cria um novo Request
        /// </summary>
        /// <param name="url">End-point da Api</param>
        /// <param name="headers">Conjunto de headers da requisição</param>
        /// <param name="xml">String xml</param>
        /// <returns></returns>
        private static HttpResponseMessage CriarRequest(string url, IDictionary<string, string> headers, string xml)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.ClientCertificates.Add(Certificado.ObterCertificado());
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;

                using (var client = new HttpClient(handler))
                {

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);

                    xml = xml.Replace("\r\n", string.Empty);

                    using (var stringContent = new StringContent(xml, Encoding.UTF8, "application/xml"))
                    {
                        return client.PostAsync(new Uri(Parametros.Url, url), stringContent).Result;
                    }
                }
            }
        }       

        /// <summary>
        /// Realiza uma nova requisição para um determinado end-point
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="url">End-point da Api</param>
        /// <param name="token">Token autenticado.</param>
        public static void Enviar(string xml, int idXml, string url, Token token)
        {
            var xmlRetorno = string.Empty;

            var headers = ObterHeaders(token);

            var response = CriarRequest(url, headers, xml);

            if (response == null)
                return;

            xmlRetorno = response.Content.ReadAsStringAsync().Result;

            Log.GravarLog($"Registro id: {idXml} - cpf: {Parametros.CPFCertificado} - Statuscode: {response.StatusCode.ToString()}");

            if (response.StatusCode != HttpStatusCode.OK)
                SiscomexDAO.GravarInconsistencia(idXml, xmlRetorno);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var retorno = XmlHelper.Deserializar(response.Content.ReadAsStringAsync().Result);

                if (retorno.Operacao.Mensagens.Count > 0)
                    SiscomexDAO.GravarRegistroEnviado(idXml);
            }
        }            

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
