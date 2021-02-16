using CCT.classes;
using Ionic.Zip;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CCT
{
    /// <summary>
    /// Envio SisComex
    /// </summary>
    class Program
    {
        static void Main()
        {
            ChecarInstancia();
            ArquivoMorto();

            using (var dt = new DataSet())
            {
                dt.ReadXml("BD.xml");

                for (int i = 0; i < dt.Tables.Count; i++)
                {
                    BD.Banco = dt.Tables[i].Rows[0]["Banco"].ToString();
                    BD.Servidor = dt.Tables[i].Rows[0]["Servidor"].ToString();
                    BD.Usuario = dt.Tables[i].Rows[0]["Usuario"].ToString();
                    BD.Senha = dt.Tables[i].Rows[0]["Senha"].ToString();
                    BD.Schema  = dt.Tables[i].Rows[0]["Schema"].ToString();

                    Parametros.CPFCertificado = dt.Tables[i].Rows[0]["CPFCertificado"].ToString();
                    Parametros.Url = new Uri(dt.Tables[i].Rows[0]["Url"].ToString());
                    Parametros.Perfil = dt.Tables[i].Rows[0]["Perfil"].ToString();
                    Parametros.MaxTentativas = dt.Tables[i].Rows[0]["MaxTentativas"].ToString();

                    Log.GravarLog("========================================================================");
                    Log.GravarLog($"Data: {DateTime.Now.ToShortDateString()}  - End-point: {Parametros.Url}");
                    Log.GravarLog("========================================================================");
                    Log.GravarLog("Iniciando envio para o Siscomex..." + Environment.NewLine);
                    Log.GravarLog("Usuário: " + BD.Usuario + " - CPF: " + Parametros.CPFCertificado);

                    // Testa conexão, se falhar continua para o próximo cliente
                    if (!TestarConexao())
                         continue;

                    // Testa o certificado, se falhar continua para o próximo cliente
                    if (!TestarCertificado())
                        continue;

                    EnviarXml();

                    Log.GravarLog("Concluído.");

                    Parametros.CPFCertificado = string.Empty;
                    Parametros.Url = null;
                }
            }
        }

        /// <summary>
        /// Verifica se existe uma outra instância aberta. Evitar processamentos simultâneos.
        /// </summary>
        public static void ChecarInstancia()
        {
            Process[] processos;
            string nomeModulo, nomeProcesso;
            var p = Process.GetCurrentProcess();

            nomeModulo = p.MainModule.ModuleName.ToString();
            nomeProcesso = System.IO.Path.GetFileNameWithoutExtension(nomeModulo);
            processos = Process.GetProcessesByName(nomeProcesso);

            if (processos.Length > 1)
            {
                Log.GravarLog("Já existe outro processo aberto");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Autentica na API do siscomex
        /// </summary>
        /// <returns>Retorna um novo Token de acesso</returns>
        public static Token ObterToken()
        {
            var token = new Token();

            try
            {
                token = Autenticador.Autenticar();
            }
            catch (Exception ex)
            {
                Log.GravarLog("Falha ao obter o Token de acesso - " + ex.Message);
            }

            return token;
        }

        /// <summary>
        /// Realiza o envio dos registros
        /// </summary>
        public static void EnviarXml()
        {
            try
            {
                var _token = ObterToken();

                if (_token.Valido())
                {
                    var listaXmlNaoEnviados = SiscomexDAO.ObterListaXmlNaoEnviados();

                    if (listaXmlNaoEnviados.Count == 0)
                        Log.GravarLog("Nenhum registro selecionado.");

                    foreach (var item in listaXmlNaoEnviados)
                    {
                        switch (item.Funcao)
                        {
                            case 1:
                                EntregasConteineres.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 2:
                                EntregasDocumentoCarga.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 3:
                                RecepcoesNFF.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 4:
                                RecepcoesNFE.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 5:
                                RecepcoesConteineres.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 6:
                                RecepcoesDocumentoCarga.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 7:
                                ManifestacoesExportacao.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 8:
                                ConsultarDadosResumidos.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 9:
                                UnitilizarCarga.Enviar(item.Xml, item.Id, _token);
                                break;
                            case 10:
                                CadastrarDUE.Enviar(item.Xml, item.Id, _token);
                                break;
                        }
                    }
                }
                else
                {
                    Log.GravarLog("O sistema não conseguiu autenticar na API Siscomex. O Token de acesso não foi gerado.");
                }
            }
            catch (Exception ex)
            {
                Log.GravarLog("Falha durante o processamento dos registros. " + ex.Message);
            }
        }

        /// <summary>
        /// Realiza um teste de conexão.
        /// </summary>
        /// <returns>Retorna falso caso o banco estiver offline</returns>
        public static bool TestarConexao()
        {
            try
            {
                BD.TestarConexao();
                return true;
            }
            catch (Exception ex)
            {
                Log.GravarLog("Falha na conexão - " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Realiza um teste de certificado. Verifica se existe o certificado com o CPF parametrizado.
        /// </summary>
        /// <returns>Retorna falso caso o certificado não existir</returns>
        public static bool TestarCertificado()
        {
            try
            {
                var certificado = Certificado.ObterCertificado();

                if (certificado != null)
                    return true;

                Log.GravarLog("Nenhum certificado encontrado para o CPF " + Parametros.CPFCertificado);
            }
            catch (Exception ex)
            {
                Log.GravarLog("Falha ao obter o certificado digital para o CPF " + Parametros.CPFCertificado + " - " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Gera o arquivo morto zipado de todos os arquivos de logs do mês anterior
        /// </summary>
        public static void ArquivoMorto()
        {
            var diretorio = Parametros.DiretorioLogs;
            var dataArquivo = DateTime.Now.AddMonths(-1);

            try
            {                
                var arquivos = new DirectoryInfo(diretorio).GetFiles()
                    .Where(a =>
                        a.Extension.Equals(".log") &&
                        a.LastWriteTime.Month == dataArquivo.Month &&
                        a.LastWriteTime.Year == dataArquivo.Year);

                if (arquivos.Count() == 0)
                    return;

                using (ZipFile zip = new ZipFile())
                {
                    foreach (var arquivo in arquivos)
                        zip.AddFile(arquivo.FullName, string.Empty);

                    zip.Save($"{diretorio}\\ArquivoMorto_{dataArquivo.Month}{dataArquivo.Year}.zip");
                }

                try
                {
                    foreach (var arquivo in arquivos)
                        File.Delete(arquivo.FullName);
                }
                catch (Exception ex)
                {
                    Log.GravarLog("Falha ao excluir os arquivos de logs (arquivo morto) - " + ex.Message);
                }                

            }
            catch (Exception ex)
            {
                Log.GravarLog($"Falha ao gerar o arquivo morto do mês: {dataArquivo.ToString("MM")} de {dataArquivo.ToString("yyyy")} - erro: {ex.Message}");
            }           
        }
    }
}
