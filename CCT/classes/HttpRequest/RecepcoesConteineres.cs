namespace CCT
{
    /// <summary>
    /// Recepcionar por Contêiner
    /// </summary>
    public class RecepcoesConteineres : HttpClientPOST
    {
        /// <summary>
        /// Realiza uma requisição POST para API Siscomex
        /// Função: RecepcoesConteineres
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="token">Token autenticado</param>
        public static void Enviar(string xml, int idXml, Token token)
        {
            Enviar(xml, idXml, "cct/api/ext/carga/recepcao-conteiner", token);
        }
    }
}
