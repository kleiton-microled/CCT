namespace CCT
{
    /// <summary>
    /// Entregar Carga por DU-E/RUC
    /// </summary>
    public class EntregasDocumentoCarga : HttpClientPOST
    {
        /// <summary>
        /// Realiza uma requisição POST para API Siscomex
        /// Função: entregasDocumentoCarga
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="token">Token autenticado</param>
        public static void Enviar(string xml, int idXml, Token token)
        {
            Enviar(xml, idXml, "cct/api/ext/carga/entrega-due-ruc", token);
        }
    }
}
