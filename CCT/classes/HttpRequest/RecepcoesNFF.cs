namespace CCT
{
    /// <summary>
    /// Recepcionar por Nota Fiscal Formulário
    /// </summary>
    public class RecepcoesNFF : HttpClientPOST
    {
        /// <summary>
        /// Realiza uma requisição POST para API Siscomex
        /// Função: RecepcoesNFF
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="token">Token autenticado</param>
        public static void Enviar(string xml, int idXml, Token token)
        {
            Enviar(xml, idXml, "cct/api/ext/carga/recepcao-nff", token);
        }
    }
}
