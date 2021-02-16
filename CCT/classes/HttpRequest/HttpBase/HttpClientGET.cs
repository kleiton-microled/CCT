using CCT.classes;
using CCT.classes.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace CCT
{
    public abstract class HttpClientGET : HttpClientBase, IDisposable
    {
        /// <summary>
        /// Cria um novo Request GET
        /// </summary>
        /// <param name="url">End-point da Api</param>
        /// <param name="headers">Conjunto de headers da requisição</param>
        /// <param name="xml">String xml</param>
        /// <returns></returns>
        private static string CriarRequest(string url, IDictionary<string, string> headers)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.ClientCertificates.Add(Certificado.ObterCertificado());
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;

                using (var client = new HttpClient(handler))
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);

                    return client.GetStringAsync(new Uri(Parametros.Url, url)).Result;
                }
            }
        }

        /// <summary>
        /// Realiza uma nova requisição GET para um determinado end-point
        /// </summary>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="url">End-point da Api</param>
        /// <param name="token">Token autenticado</param>
        public static void Enviar(int idXml, string url, Token token)
        {            
            Log.GravarLog($"Iniciando consulta para o id: {idXml} - cpf: {Parametros.CPFCertificado}");

            var headers = ObterHeaders(token);

            var response = "";

            try
            {
                response = CriarRequest(url, headers);
            }
            catch (Exception ex)
            {
                SiscomexDAO.GravarInconsistencia(idXml, ex.InnerException.Message);
            }            

            if (string.IsNullOrEmpty(response))
                return;

            var dadosResumidos = JsonConvert.DeserializeObject<DadosResumidos>(response);

            if (dadosResumidos == null)
                return;

            Log.GravarLog($"Registro id: {idXml} - Situação DUE: {dadosResumidos.situacaoDUE}");

            SiscomexDAO.GravarDadosConsultaDUE(dadosResumidos, idXml);
        }       

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
