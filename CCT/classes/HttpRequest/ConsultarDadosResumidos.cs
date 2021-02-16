using System.Collections.Generic;

namespace CCT
{
    /// <summary>
    /// Consultar dados resumidos
    /// </summary>
    public class ConsultarDadosResumidos : HttpClientGET
    {
        /// <summary>
        /// Realiza uma requisição GET para API Siscomex
        /// Função: consultarDadosResumidosDUE
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="token">Token autenticado</param>
        public static void Enviar(string xml, int idXml, Token token)
        {
            Enviar(idXml, $"due/api/ext/due/consultarDadosResumidosDUE?numero={xml}", token);         
        }
    }
}
