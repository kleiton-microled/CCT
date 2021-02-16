using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CCT
{
    [Serializable, XmlRoot("retornoServico")]
    public class RetornoAPI
    {
        [XmlElement("operacao")]
        public Operacao Operacao { get; set; }             
    }

    public class Operacao
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlArray(ElementName = "mensagens")]
        [XmlArrayItem(ElementName = "mensagem")]
        public List<Mensagem> Mensagens { get; set; }
    }

    public class Mensagem
    {
        [XmlElement("codigo")]
        public string Codigo { get; set; }
        [XmlElement("descricao")]
        public string Descricao { get; set; }
    }
}
