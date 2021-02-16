using System;

namespace CCT
{
    /// <summary>
    /// Informações do registro de Log XML
    /// </summary>
    public class SiscomexDTO
    {
        public int Id { get; set; }
        public string Xml { get; set; }
        public int Funcao { get; set; }
        public bool Enviado { get; set; }
        public DateTime? DataEnvio { get; set; }
        public DateTime? DataCadastro { get; set; }
        public string Critica { get; set; }
    }
}
