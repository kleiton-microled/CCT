using System;
using System.Xml.Serialization;

namespace CCT.classes.DTO
{
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.pucomex.serpro.gov.br/cct")]
    [XmlRoot(Namespace = "http://www.pucomex.serpro.gov.br/cct", IsNullable = false)]
    public partial class retornoServico
    {
        public retornoServicoMensagens mensagens { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.pucomex.serpro.gov.br/cct")]
    public partial class retornoServicoMensagens
    {
        public RetornoServicoMensagensMensagem mensagem { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.pucomex.serpro.gov.br/cct")]
    public partial class RetornoServicoMensagensMensagem
    {
        public string codigo { get; set; }
        public string descricao { get; set; }
    }
}
