using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System;

namespace CCT
{
    /// <summary>
    /// Comunicação com o Banco de Dados
    /// </summary>
    public class BD
    {
        public static string Servidor { get; set; }
        public static string Usuario { get; set; }
        public static string Schema { get; set; }
        public static string Senha { get; set; }
        public static string Banco { get; set; }

        /// <summary>
        /// Executa um comando SQL (Insert, update, delete)
        /// </summary>
        /// <param name="SQL">Instrução SQL</param>
        /// <param name="parametros">Lista de parâmetros</param>
        public static void Executar(string SQL, IList<object> parametros = null)
        {
            if (Banco.Equals("MSSQL"))
            {
                using (SqlConnection Con = new SqlConnection(ConnectionString()))
                {
                    using (SqlCommand Cmd = new SqlCommand(SQL, Con))
                    {
                        if (parametros != null)
                        {
                            foreach (SqlParameter parametro in parametros)
                                Cmd.Parameters.AddWithValue(parametro.ParameterName, parametro.Value);
                        }

                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        Con.Close();
                    }
                }
            }
            else
            {
                using (OleDbConnection Con = new OleDbConnection(ConnectionString()))
                {
                    using (OleDbCommand Cmd = new OleDbCommand(SQL, Con))
                    {
                        if (parametros != null)
                        {
                            foreach (OleDbParameter parametro in parametros)
                                Cmd.Parameters.AddWithValue(parametro.ParameterName, parametro.Value);
                        }

                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        Con.Close();
                    }
                }
            }

        }

        /// <summary>
        /// Executa uma consulta
        /// </summary>
        /// <param name="SQL">Instrução SQL</param>
        /// <param name="parametros">Lista de parâmetros</param>
        /// <returns>Retorna um objeto DataTable</returns>
        public static DataTable List(string SQL, IList<object> parametros = null)
        {
            var Ds = new DataSet();

            if (Banco.Equals("MSSQL"))
            {
                using (SqlConnection Con = new SqlConnection(ConnectionString()))
                {
                    using (SqlCommand Cmd = new SqlCommand())
                    {
                        if (parametros != null)
                        {
                            foreach (SqlParameter parametro in parametros)
                                Cmd.Parameters.AddWithValue(parametro.ParameterName, parametro.Value);
                        }

                        Cmd.Connection = Con;
                        Cmd.CommandType = CommandType.Text;
                        Cmd.CommandText = SQL;

                        using (SqlDataAdapter Adp = new SqlDataAdapter(Cmd))
                        {
                            Adp.Fill(Ds);
                            return Ds.Tables[0];
                        }
                    }
                }
            }
            else
            {
                using (OleDbConnection Con = new OleDbConnection(ConnectionString()))
                {
                    using (OleDbCommand Cmd = new OleDbCommand())
                    {
                        if (parametros != null)
                        {
                            foreach (OleDbParameter parametro in parametros)
                                Cmd.Parameters.AddWithValue(parametro.ParameterName, parametro.Value);
                        }

                        Cmd.Connection = Con;
                        Cmd.CommandType = CommandType.Text;
                        Cmd.CommandText = SQL;

                        using (OleDbDataAdapter Adp = new OleDbDataAdapter(Cmd))
                        {
                            Adp.Fill(Ds);
                            return Ds.Tables[0];
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Executa uma consulta
        /// </summary>
        /// <param name="SQL">Instrução SQL</param>
        /// <param name="parametros">Lista de parâmetros</param>
        /// <returns>Retorna um objeto SqlDataReader</returns>
        public static string[] Reader(string SQL, IList<object> parametros = null)
        {
            var lista = new List<string>();

            if (Banco.Equals("MSSQL"))
            {
                using (SqlConnection Con = new SqlConnection(ConnectionString()))
                {
                    using (SqlCommand Cmd = new SqlCommand(SQL, Con))
                    {
                        if (parametros != null)
                        {
                            foreach (SqlParameter parametro in parametros)
                                Cmd.Parameters.AddWithValue(parametro.ParameterName, parametro.Value);
                        }

                        SqlDataReader dr;
                        Con.Open();
                        dr = Cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                lista.Add(dr[0].ToString());
                            }
                        }

                        dr.Close();
                        Con.Close();
                    }
                }
            }
            else
            {
                using (OleDbConnection Con = new OleDbConnection(ConnectionString()))
                {
                    using (OleDbCommand Cmd = new OleDbCommand(SQL, Con))
                    {
                        if (parametros != null)
                        {
                            foreach (OleDbParameter parametro in parametros)
                                Cmd.Parameters.AddWithValue(parametro.ParameterName, parametro.Value);
                        }

                        OleDbDataReader dr;
                        Con.Open();
                        dr = Cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                lista.Add(dr[0].ToString());
                            }
                        }

                        dr.Close();
                        Con.Close();
                    }
                }
            }

            return lista.ToArray();
        }

        /// <summary>
        /// Grava a inconsistência para o registro atual
        /// </summary>
        /// <param name="idXml">Código do registro xml</param>
        /// <param name="critica">Inconsistências retornadas pela api</param>
        public static void TestarConexao()
        {
            if (BD.Banco.Equals("MSSQL"))
            {
                BD.Executar($"SELECT GETDATE()");
            }
            else
            {
                BD.Executar($"SELECT SYSDATE FROM DUAL");
            }   
        }

        public static string ConnectionString()
        {
            if (Banco.Equals("MSSQL"))
            {
                return string.Format(@"Server={0};Initial Catalog={1};User ID={2};Password={3};", Servidor, Schema, Usuario, Senha);
            }
            else
            {
                return string.Format(@"Provider=OraOLEDB.Oracle;Data Source={0};User ID={1};Password={2};Unicode=True", Servidor, Usuario, Senha);
            }
        }
    }
}