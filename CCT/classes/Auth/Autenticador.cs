using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace CCT
{
    /// <summary>
    /// Autenticação API Siscomex
    /// </summary>
    public class Autenticador
    {
        /// <summary>
        /// Realiza uma autenticação na api Siscomex 
        /// </summary>
        /// <returns>Retorna um objeto com os Tokens de acesso</returns>
        public static Token Autenticar()
        {
            var token = new Token();
            
            using (var handler = new WebRequestHandler())
            {
                handler.ClientCertificates.Add(Certificado.ObterCertificado());
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Role-Type", Parametros.Perfil);

                    var request = new HttpRequestMessage(HttpMethod.Post, new Uri(Parametros.Url, "portal/api/autenticar"));
                    var response = client.SendAsync(request).Result;

                    response.EnsureSuccessStatusCode();

                    if (response.Headers.TryGetValues("set-token", out IEnumerable<string> valor))
                        token.SetToken = valor.FirstOrDefault();

                    if (response.Headers.TryGetValues("x-csrf-token", out valor))
                        token.CsrfToken = valor.FirstOrDefault();

                    if (response.Headers.TryGetValues("x-csrf-expiration", out valor))
                        token.CsrfExpiration = valor.FirstOrDefault();

                    return token;
                }
            }          
        }

        /// <summary>
        /// Valida o certificado remoto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool RemoteCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }      

    }

}
