using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CCT
{
    public abstract class HttpClientBase
    {
        protected static Dictionary<string, string> ObterHeaders(Token token) => new Dictionary<string, string>
            {
                {"Authorization", token.SetToken},
                {"x-csrf-token", token.CsrfToken}
            };

        /// <summary>
        /// Valida o certificado remoto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected static bool RemoteCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
    }
}
