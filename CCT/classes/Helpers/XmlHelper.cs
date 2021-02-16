using CCT.classes.DTO;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace CCT
{
    /// <summary>
    /// Funções para manipulação de Xml
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// Deserializa uma string Xml para um Objeto
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <returns>Retorna um objeto RetornoAPI</returns>
        public static RetornoAPI Deserializar(string xml)
        {
            var serializer = new XmlSerializer(typeof(RetornoAPI));
            var stringReader = new StringReader(xml);
            return (RetornoAPI)serializer.Deserialize(stringReader);
        }

        /// <summary>
        /// Deserializa uma string Xml de erro para um Objeto Critica
        /// </summary>
        /// <param name="xml">String xml</param>
        /// <returns>Retorna um objeto Critica</returns>
        public static object DeserializarErro(string xml)
        {
            var unescapedString = Regex.Unescape(xml);

            //using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(unescapedString)))
            //{               
            //    var xmlSerializer = new XmlSerializer(typeof(Error));
            //    var reader = new StreamReader(memoryStream, Encoding.UTF8);
            //    return (Error)xmlSerializer.Deserialize(reader);
            //}
            
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(unescapedString)))
            {
                XmlSerializer xmlSerializer;
                StreamReader reader;

                if (xml.Contains(nameof(retornoServico)))
                {
                    xmlSerializer = new XmlSerializer(typeof(retornoServico));
                    reader = new StreamReader(memoryStream, Encoding.UTF8);
                    return (retornoServico)xmlSerializer.Deserialize(reader);                    
                }

                xmlSerializer = new XmlSerializer(typeof(Error));
                reader = new StreamReader(memoryStream, Encoding.UTF8);
                return (Error)xmlSerializer.Deserialize(reader);
            }            
        }

        /// <summary>
        /// Serializa um objeto para string Xml
        /// </summary>
        /// <param name="objeto">Objeto do tipo RetornoAPI</param>
        /// <returns>Retorna um Xml com as informações do objeto</returns>
        public static string Serializar(RetornoAPI objeto)
        {
            var xsSubmit = new XmlSerializer(typeof(RetornoAPI));            
            
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, objeto);
                    return sww.ToString(); 
                }
            }
        }
    }
}
