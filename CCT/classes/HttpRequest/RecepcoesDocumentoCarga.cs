namespace CCT
{
    /// <summary>
    /// Recepcionar Carga por DU-E/RUC
    /// </summary>
    public class RecepcoesDocumentoCarga : HttpClientPOST
    {
        /// <summary>
        /// Realiza uma requisição POST para API Siscomex
        /// Função: RecepcoesDocumentoCarga
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="token">Token autenticado</param>
        public static void Enviar(string xml, int idXml, Token token)
        {
            Enviar(xml, idXml, "cct/api/ext/documento-transporte/recepcao-dat", token);
        }
    }
}
