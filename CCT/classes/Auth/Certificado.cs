using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CCT
{
    /// <summary>
    /// Gerenciamento de certificados digitais
    /// </summary>
    public class Certificado
    {
        /// <summary>
        /// Lista os certificados digitais instalados para o usuário da máquina
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<X509Certificate2> ListarCertificadosInstalados()
        {
            var stores = new X509Store(StoreName.My, StoreLocation.CurrentUser   );

            stores.Open(OpenFlags.ReadOnly);

            var certificadosInstalados = stores.Certificates;

            certificadosInstalados.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            certificadosInstalados.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, false);

            stores.Close();

            var certificados = new List<X509Certificate2>();

            foreach (X509Certificate2 certificado in certificadosInstalados)
                yield return certificado;
        }

        /// <summary>
        /// Devolve o certificado do cpf parametrizado
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 ObterCertificado()
        {
            return ListarCertificadosInstalados()
                .FirstOrDefault(a => a.FriendlyName.Contains(Parametros.CPFCertificado));
        }
    }
}
