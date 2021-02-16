namespace CCT
{
    /// <summary>
    /// Unitização de Carga
    /// </summary>
    public class UnitilizarCarga : HttpClientPOST
    {
        /// <summary>
        /// Realiza uma requisição POST para API Siscomex
        /// Função: operacaoUnitizacao
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <param name="idXml">Código do registro de Log</param>
        /// <param name="token">Token autenticado</param>
        public static void Enviar(string xml, int idXml, Token token)
        {
            Enviar(xml, idXml, "cct/api/ext/unitizacao/unitizar-carga", token);
        }
    }
}
