using CCT.classes;
using CCT.classes.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace CCT
{
    /// <summary>
    /// Manutenção dos registros de logs Siscomex
    /// </summary>
    public class SiscomexDAO
    {
        /// <summary>
        /// Obtém uma lista dos registros com status não enviados
        /// </summary>
        /// <returns></returns>
        public static IList<SiscomexDTO> ObterListaXmlNaoEnviados()
        {
            var SQL = "";
            var parametros = new List<object>();

            SQL = BD.Banco.Equals("MSSQL")
                    ? "SELECT AUTONUM, XML, FUNCAO FROM [dbo].[TB_LOG_SISCOMEX] WHERE ISNULL(ENVIADO,0) = 0 AND ISNULL(TENTATIVAS,0) < @tentativas AND ISNULL(FUNCAO,'') <> '' ORDER BY FUNCAO"
                    : "SELECT AUTONUM, XML, FUNCAO FROM TB_LOG_SISCOMEX WHERE 0=0 AND NVL(ENVIADO,0) = 0 AND NVL(TENTATIVAS,0) < ? AND NVL(FUNCAO,'x') <> 'x' ORDER BY FUNCAO, AUTONUM ";

            if (BD.Banco.Equals("MSSQL"))
                parametros.Add(new SqlParameter("@tentativas", Parametros.MaxTentativas));
            else
                parametros.Add(new OleDbParameter(":tentativas", Parametros.MaxTentativas));

            using (var table = BD.List(SQL, parametros))
            {
                IList<SiscomexDTO> lista = new List<SiscomexDTO>();

                if (table != null)
                {
                    foreach (DataRow item in table.Rows)
                    {
                        var logObj = new SiscomexDTO
                        {
                            Id = Convert.ToInt32(item["AUTONUM"]),
                            Funcao = Convert.ToInt32(item["FUNCAO"]),
                            Xml = item["XML"].ToString()
                        };
                        lista.Add(logObj);
                    }
                }

                return lista;
            }
        }

        /// <summary>
        /// Grava a inconsistência para o registro atual
        /// </summary>
        /// <param name="idXml">Código do registro xml</param>
        /// <param name="critica">Inconsistências retornadas pela api</param>
        public static void GravarInconsistencia(int idXml, string critica)
        {
            var msgCritica = string.Empty;

            if (critica.Contains(nameof(retornoServico)))
            {
                try
                {
                    var retorno = (retornoServico)XmlHelper.DeserializarErro(critica);

                    if (retorno != null)
                    {
                        msgCritica = retorno.mensagens.mensagem.descricao ?? retorno.mensagens.mensagem.codigo;
                    }
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    var retorno = (Error)XmlHelper.DeserializarErro(critica);

                    if (retorno != null)
                    {
                        msgCritica = retorno.Detail?.Errors.Length > 0
                            ? retorno?.Message + string.Join(",", retorno.Detail.Errors.Select(x => x.Message))
                            : retorno?.Message;
                    }
                }
                catch
                {
                }
            }           

            var parametros = new List<object>();

            if (BD.Banco.Equals("MSSQL"))
            {
                parametros.Add(new SqlParameter("@autonum", idXml));
                parametros.Add(new SqlParameter("@critica", critica));
                parametros.Add(new SqlParameter("@msgcritica", msgCritica));
                BD.Executar($"UPDATE [dbo].[TB_LOG_SISCOMEX] SET TENTATIVAS = ISNULL(TENTATIVAS,0) + 1,ENVIADO = 0, CRITICA = @critica, MENSAGEM_CRITICA = @msgcritica WHERE AUTONUM = @autonum", parametros);
            }
            else
            {
                parametros.Add(new OleDbParameter(":critica", critica));
                parametros.Add(new OleDbParameter(":msgcritica", msgCritica));
                parametros.Add(new OleDbParameter(":autonum", idXml));

                BD.Executar($"UPDATE TB_LOG_SISCOMEX SET TENTATIVAS = NVL(TENTATIVAS,0) + 1, ENVIADO = 0, CRITICA = ?, MENSAGEM_CRITICA = ? WHERE AUTONUM = ?", parametros);
            }

            Log.GravarLog($"Inconsistência encontrada no registro id {idXml}: {msgCritica}");
        }

        /// <summary>
        /// Grava o registro como enviado
        /// </summary>
        /// <param name="idXml">Código do registro xml</param>
        public static void GravarRegistroEnviado(int idXml)
        {
            var parametros = new List<object>();

            if (BD.Banco.Equals("MSSQL"))
            {
                parametros.Add(new SqlParameter("@autonum", idXml));
                BD.Executar($"UPDATE [dbo].[TB_LOG_SISCOMEX] SET ENVIADO = 1, DATA_ENVIO = GETDATE(), CRITICA = ' ' WHERE AUTONUM = @autonum", parametros);
            }
            else
            {
                parametros.Add(new OleDbParameter(":autonum", idXml));
                BD.Executar($"UPDATE TB_LOG_SISCOMEX SET ENVIADO = 1, DATA_ENVIO = SYSDATE, CRITICA = NULL , MENSAGEM_CRITICA = NULL WHERE AUTONUM = ? ", parametros);
            }

            Log.GravarLog($"Registro id {idXml} enviado com sucesso.");
        }

        /// <summary>
        /// Grava campos Situação, Data Situação e Indicado Bloqueio
        /// </summary>
        /// <param name="dados">Classe DadosResumidos</param>
        /// /// <param name="idXml">Código do registro xml</param>
        public static void GravarDadosConsultaDUE(DadosResumidos dados, int idXml)
        {
            var parametros = new List<object>();
            object dataSituacaoDue = null;

            if (BD.Banco.Equals("MSSQL"))
            {
                dataSituacaoDue = dados.dataSituacaoDUE != null ? (object)Convert.ToDateTime(dados.dataSituacaoDUE) : DBNull.Value;

                parametros.Add(new SqlParameter("@situacaoDUE", dados.situacaoDUE));
                parametros.Add(new SqlParameter("@datasituacaoDUE", dataSituacaoDue));
                parametros.Add(new SqlParameter("@indicadorBloqueio", dados.indicadorBloqueio));
                parametros.Add(new SqlParameter("@autonum", idXml));
                BD.Executar($"UPDATE [dbo].[TB_LOG_SISCOMEX] SET TENTATIVAS = ISNULL(TENTATIVAS,0) + 1, DATA_ENVIO = GETDATE(), ENVIADO = 1, SITUACAODUE = @situacaoDUE, DATASITUACAODUE = @datasituacaoDUE, INDICADORBLOQUEIO = @indicadorBloqueio WHERE AUTONUM = @autonum", parametros);
            }
            else
            {
                dataSituacaoDue = dados.dataSituacaoDUE != null ? (object)Convert.ToDateTime(dados.dataSituacaoDUE) : DBNull.Value;

                parametros.Add(new OleDbParameter(":situacaoDUE", dados.situacaoDUE));
                parametros.Add(new OleDbParameter(":datasituacaoDUE", Convert.ToDateTime(dados.dataSituacaoDUE)));
                parametros.Add(new OleDbParameter(":indicadorBloqueio", dados.indicadorBloqueio));
                parametros.Add(new OleDbParameter(":autonum", idXml));

                BD.Executar($"UPDATE TB_LOG_SISCOMEX SET TENTATIVAS = NVL(TENTATIVAS,0) + 1, DATA_ENVIO = SYSDATE, ENVIADO = 1, SITUACAODUE = ?, DATASITUACAODUE = ?, INDICADORBLOQUEIO = ? WHERE AUTONUM = ?", parametros);
            }

            Log.GravarLog($"Consulta do registro id {idXml} realizada com sucesso.");
        }
    }
}
