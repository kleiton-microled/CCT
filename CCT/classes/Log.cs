using System;
using System.IO;

namespace CCT.classes
{
    public class Log
    {
        private static StreamWriter _log;
        private static string _nomeArquivo { get; set; }

        static Log()
        {
            if (string.IsNullOrEmpty(_nomeArquivo))
                _nomeArquivo = Guid.NewGuid().ToString() + ".log";
        }

        public static void GravarLog(string Mensagem)
        {

            _log = new StreamWriter(Parametros.DiretorioLogs + @"\" + _nomeArquivo, true);
            _log.WriteLine(DateTime.Now + " - " + RemoverQuebras(Mensagem));
            _log.Close();

            Console.WriteLine(Mensagem);
        }

        private  static string RemoverQuebras(string blockOfText)
        {
            return blockOfText.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
        }
    }
}
