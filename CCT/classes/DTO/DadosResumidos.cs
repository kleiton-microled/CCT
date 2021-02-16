namespace CCT.classes.DTO
{
    public class DadosResumidos
    {
        public string numeroDUE { get; set; }
        public string numeroRUC { get; set; }
        public int situacaoDUE { get; set; }
        public string dataSituacaoDUE { get; set; }
        public int indicadorBloqueio { get; set; }
        public int controleAdministrativo { get; set; }
        public string uaEmbarque { get; set; }
        public string uaDespacho { get; set; }
        public object responsavelUADespacho { get; set; }
        public string codigoRecintoAduaneiroDespacho { get; set; }
        public string codigoRecintoAduaneiroEmbarque { get; set; }
        public object coordenadasDespacho { get; set; }
        public Declarante declarante { get; set; }
        public Exportador[] exportadores { get; set; }
        public int[] situacaoCarga { get; set; }
    }

    public class Declarante
    {
        public string numero { get; set; }
        public string tipo { get; set; }
    }

    public class Exportador
    {
        public string numero { get; set; }
        public string tipo { get; set; }
    }

}
