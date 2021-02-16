using System;

namespace CCT
{
    /// <summary>
    /// Parâmetros de sistema
    /// </summary>
    public static class Parametros
    {
        public static Uri Url { get; set; }
        public static string Perfil { get; set; }
        public static string MaxTentativas { get; set; }
        public static string CPFCertificado { get; set; }
        public static string DiretorioLogs => Environment.CurrentDirectory + @"\logs";
    }
}
