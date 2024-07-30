using Microsoft.Ajax.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JuntaDados
{
    public partial class _Default : Page
    {

        MySqlConnection conexaoC1 = new MySqlConnection(String.Format(ConfigurationManager.AppSettings["StrDBConnectPoint"]));
        MySqlConnection conexaoC2 = new MySqlConnection(String.Format(ConfigurationManager.AppSettings["StrDBConnectPoint"]));

        MySqlConnection conexaoR1 = new MySqlConnection(String.Format(ConfigurationManager.AppSettings["StrDbEdital"]));
        MySqlConnection conexaoR2 = new MySqlConnection(String.Format(ConfigurationManager.AppSettings["StrDbEdital"]));

        internal DataTable dtbN1 = null;
        internal DataTable dtbN2 = null;

        Boolean erro = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                dtbN1 = new DataTable();

                dtbN1 = CriaDataTableN1();

                Session["DataTableN1"] = dtbN1;

                dtbN2 = new DataTable();

                dtbN2 = CriaDataTableN2();

                Session["DataTableN2"] = dtbN2;

                conexaoR1.Open();

                string Sel = "Select ID,ID_Cidade,ID_Estado,CaminhoAnexo,CodigoLicitacao,DataAbertura,DataPublicacao,EmailContato,FonteLicitacao,Modalidade,Nome,Objeto,Observacoes,OrgaoResponsavel,TelefoneContato,TipoLocalizacao,ValorLicitacao,ValorSigiloso,DataCadastro,DataEdicao,DataExclusao from tb_edital where ValorSigiloso in(1,2) and (DataAbertura >= now() or DataAbertura is null)";

                MySqlCommand qrySelect = new MySqlCommand(Sel, conexaoR1);
                MySqlDataReader reader = qrySelect.ExecuteReader();
                int nTotalErros = 0;
                int nTotalLicitacoes = 0;
                int nTotalRegistrosProcessados = 0;
                int nTotalRegistroDbEdital = 0;
                while (reader.Read())
                {
                    nTotalLicitacoes++;

                    int ID = Convert.ToInt32(reader["ID"].ToString());
                    int quantidade = 0;

                    if (reader["ID_Cidade"].ToString().Length > 0 && reader["ID_Estado"].ToString().Length > 0)
                    {
                        conexaoC1.Open();

                        string SelG = "Select count(*) as qtde from tb_edital where ID_Cidade=@cidade and ID_Estado=@estado and CodigoLicitacao=@codigo";
                        MySqlCommand qrySelectG = new MySqlCommand(SelG, conexaoC1);
                        qrySelectG.Parameters.Add("@cidade", MySqlDbType.Int32).Value = Convert.ToInt32(reader["ID_Cidade"].ToString());
                        qrySelectG.Parameters.Add("@estado", MySqlDbType.Int32).Value = Convert.ToInt32(reader["ID_Estado"].ToString());
                        qrySelectG.Parameters.Add("@codigo", MySqlDbType.VarChar, 255).Value = reader["CodigoLicitacao"].ToString();
                        MySqlDataReader readerG = qrySelectG.ExecuteReader();

                        while (readerG.Read())
                        {
                            quantidade = Convert.ToInt32(readerG["qtde"].ToString());
                        }

                        qrySelectG.Dispose();

                        conexaoC1.Close();

                        if (quantidade == 0)
                        {
                            DataTable dataN1 = (DataTable)Session["DataTableN1"];
                            dataN1.Clear();
                            DataTable dataN2 = (DataTable)Session["DataTableN2"];
                            dataN2.Clear();

                            string objeto = reader["objeto"].ToString();

                            if (objeto.Substring(0, 1) == "[")
                            {
                                objeto = objeto.Substring(objeto.IndexOf("]") + 1, objeto.Length - (objeto.IndexOf("]") + 1)).TrimStart();
                            }

                            if (objeto.Substring(0, 3) == "* L")
                            {
                                objeto = objeto.Substring(2, objeto.Length - 2);
                                objeto = objeto.Substring(objeto.IndexOf("*") + 1, objeto.Length - (objeto.IndexOf("*") + 1)).TrimStart();
                            }

                            string[] palavras = objeto.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',', '"', '‖', '―', 'Á' }, StringSplitOptions.RemoveEmptyEntries);
                            string classificacao = "";
                            int conta = palavras.Count();
                            int nivel_1 = 0;
                            int nivel_2 = 0;
                            int n1 = 0;
                            int n2 = 0;
                            string palavran = "";
                            int peso = 0;

                            foreach (string palavra in palavras)
                            {
                                var pala = palavra.Replace("'", "");

                                //pala = ObterStringSemAcentosECaracteresEspeciais(pala);

                                pala = pala.Trim().ToUpper();

                                if (pala.Length > 1)
                                {
                                    if (pala.Substring(pala.Length - 1, 1) == "," || pala.Substring(pala.Length - 1, 1) == "." || pala.Substring(pala.Length - 1, 1) == ")" || pala.Substring(pala.Length - 1, 1) == "}" || pala.Substring(pala.Length - 1, 1) == "]" || pala.Substring(pala.Length - 1, 1) == ";" || pala.Substring(pala.Length - 1, 1) == ":" || pala.Substring(pala.Length - 1, 1) == "|")
                                    {
                                        palavran = pala.Substring(0, pala.Length - 1);

                                        if ((palavran.Substring(palavran.Length - 1, 1) == "A") && (palavran.Length > 1))
                                            palavran = palavran.Substring(0, palavran.Length - 1);

                                        if ((palavran.Substring(palavran.Length - 1, 1) == "S") && (palavran.Length > 1))
                                        {
                                            palavran = palavran.Substring(0, palavran.Length - 1);
                                            if ((palavran.Substring(palavran.Length - 1, 1) == "A") && (palavran.Length > 1))
                                                palavran = palavran.Substring(0, palavran.Length - 1);
                                        }
                                    }
                                    else
                                    {
                                        palavran = pala;

                                        if ((palavran.Substring(palavran.Length - 1, 1) == "A") && (palavran.Length > 1))
                                            palavran = palavran.Substring(0, palavran.Length - 1);

                                        if ((palavran.Substring(palavran.Length - 1, 1) == "S") && (palavran.Length > 1))
                                        {
                                            palavran = palavran.Substring(0, palavran.Length - 1);
                                            if ((palavran.Substring(palavran.Length - 1, 1) == "A") && (palavran.Length > 1))
                                                palavran = palavran.Substring(0, palavran.Length - 1);
                                        }
                                    }

                                    if (palavran.Length > 1)
                                    {
                                        if (palavran.Substring(0, 1) == "(" || palavran.Substring(0, 1) == "," || palavran.Substring(0, 1) == "." || palavran.Substring(0, 1) == "{" || palavran.Substring(0, 1) == "[" || palavran.Substring(0, 1) == ";" || palavran.Substring(0, 1) == ":")
                                        {
                                            palavran = palavran.Substring(1, palavran.Length - 1);
                                        }
                                    }
                                }

                                if (palavran.Length > 3 && (palavran != "PARA"))
                                {

                                    conexaoC2.Open();

                                    string SelN1 = "Select ID,Peso,Valor from tb_palavra where Valor like('" + palavran + "%') and Nivel=1 and Inativo=0";
                                    MySqlCommand qrySelectN1 = new MySqlCommand(SelN1, conexaoC2);
                                    MySqlDataReader readerN1 = qrySelectN1.ExecuteReader();
                                    while (readerN1.Read())
                                    {
                                        nivel_1 = Convert.ToInt32(readerN1["ID"].ToString());
                                        n1++;

                                        incluirNoDataTable((DataTable)Session["DataTableN1"], readerN1["Peso"].ToString(), 1, readerN1["ID"].ToString());
                                    }
                                    
                                    qrySelectN1.Dispose();

                                    conexaoC2.Close();

                                    conexaoC1.Open();

                                    string SelN2 = "Select ID,Peso,Valor from tb_palavra where Valor like('" + palavran + "%') and Nivel=2  and Inativo=0";
                                    MySqlCommand qrySelectN2 = new MySqlCommand(SelN2, conexaoC1);
                                    MySqlDataReader readerN2 = qrySelectN2.ExecuteReader();

                                    while (readerN2.Read())
                                    {
                                        nivel_2 = Convert.ToInt32(readerN2["ID"].ToString());
                                        peso = Convert.ToInt32(readerN2["Peso"].ToString());
                                        n2++;

                                        incluirNoDataTable((DataTable)Session["DataTableN2"], readerN2["Peso"].ToString(), 2, readerN2["ID"].ToString());
                                    }

                                    qrySelectN2.Dispose();

                                    conexaoC1.Close();
                                }
                            }

                            if (n1 > 0 && n2 > 0)
                            {
                                dataN1 = (DataTable)Session["DataTableN1"];
                                dataN1.DefaultView.Sort = "n1_peso Desc";
                                dataN1 = dataN1.DefaultView.ToTable();

                                dataN2 = (DataTable)Session["DataTableN2"];
                                dataN2.DefaultView.Sort = "n2_peso Desc";
                                dataN2 = dataN2.DefaultView.ToTable();

                                foreach (DataRow RowN1 in dataN1.Rows)
                                {
                                    nivel_1 = Convert.ToInt32(RowN1["n1_id"]);

                                    foreach (DataRow RowN2 in dataN2.Rows)
                                    {
                                        nivel_2 = Convert.ToInt32(RowN2["n2_id"]);

                                        conexaoC2.Open();

                                        string SelC = "select b.ID as codigo from rl_classificacao_palavra as a inner join tb_classificacao as b on a.Classificacao=b.ID where a.Nivel1=@nivel1 and a.Nivel2=@nivel2 and a.Inativo=0 and b.Inativo=0";
                                        MySqlCommand qrySelectC = new MySqlCommand(SelC, conexaoC2);
                                        qrySelectC.Parameters.Add("@nivel1", MySqlDbType.Int32).Value = nivel_1;
                                        qrySelectC.Parameters.Add("@nivel2", MySqlDbType.Int32).Value = nivel_2;
                                        MySqlDataReader readerC = qrySelectC.ExecuteReader();
                                        classificacao = "";

                                        while (readerC.Read())
                                        {
                                            classificacao = readerC["codigo"].ToString();
                                        }

                                        qrySelectC.Dispose();

                                        conexaoC2.Close();

                                        if (classificacao != "")
                                            break;
                                    }

                                    if (classificacao != "")
                                        break;
                                }
                            }

                            string modalidade = reader["Modalidade"].ToString();

                            string moda = reader["Modalidade"].ToString();

                            conexaoC1.Open();

                            string SelM = "select ID from tb_modalidade where CodigoEdital=@modalidade";
                            MySqlCommand qrySelectM = new MySqlCommand(SelM, conexaoC1);
                            qrySelectM.Parameters.Add("@modalidade", MySqlDbType.Int32).Value = Convert.ToInt32(modalidade);
                            MySqlDataReader readerM = qrySelectM.ExecuteReader();

                            while (readerM.Read())
                            {
                                modalidade = readerM["ID"].ToString();
                            }

                            qrySelectM.Dispose();

                            conexaoC1.Close();

                            conexaoC2.Open();

                            string codigo = reader["CodigoLicitacao"].ToString();

                            if (reader["CodigoLicitacao"].ToString().Contains("DER-MG-"))
                            {
                                codigo.Replace("-" + reader["Modalidade"].ToString() + "-", "-" + modalidade + "-");
                            }

                            erro = true;

                            string Ins = "insert into tb_edital(ID_Cidade,ID_Estado,CaminhoAnexo,CodigoLicitacao,DataAbertura,DataPublicacao,EmailContato,FonteLicitacao,Modalidade,Nome,Objeto,Observacoes,OrgaoResponsavel,TelefoneContato,TipoLocalizacao,ValorLicitacao,ValorSigiloso,DataCadastro,DataEdicao,DataExclusao,ID_Classificacao,ID_Area,ID_Subarea,DataAprovacao,Origem) values(@Cidade,@Estado,@Anexo,@Codigo,@Abertura,@Publicacao,@Email,@Licitacao,@Modalidade,@Nome,@Objeto,@Obs,@Responsavel,@Contato,@Localizacao,@Licitado,@Sigiloso,@Cadastro,@Edicao,@Exclusao,@Classificacao,@Area,@Subarea,null,@Origem)";
                            MySqlCommand qryInsert = new MySqlCommand(Ins, conexaoC2);
                            qryInsert.Parameters.Add("@Cidade", MySqlDbType.Int32).Value = Convert.ToInt32(reader["ID_Cidade"].ToString());
                            qryInsert.Parameters.Add("@Estado", MySqlDbType.Int32).Value = Convert.ToInt32(reader["ID_Estado"].ToString());
                            qryInsert.Parameters.Add("@Anexo", MySqlDbType.VarChar, 2083).Value = reader["CaminhoAnexo"].ToString();
                            qryInsert.Parameters.Add("@Codigo", MySqlDbType.VarChar, 255).Value = codigo;

                            if (reader["DataAbertura"].ToString().Length > 0)
                                qryInsert.Parameters.Add("@Abertura", MySqlDbType.DateTime).Value = Convert.ToDateTime(reader["DataAbertura"].ToString());
                            else
                                qryInsert.Parameters.Add("@Abertura", MySqlDbType.DateTime).Value = DateTime.Now;

                            if (reader["DataPublicacao"].ToString().Length > 0)
                                qryInsert.Parameters.Add("@Publicacao", MySqlDbType.DateTime).Value = Convert.ToDateTime(reader["DataPublicacao"].ToString());
                            else
                                qryInsert.Parameters.Add("@Publicacao", MySqlDbType.DateTime).Value = DateTime.Now;

                            qryInsert.Parameters.Add("@Email", MySqlDbType.VarChar, 255).Value = reader["EmailContato"].ToString();
                            qryInsert.Parameters.Add("@Licitacao", MySqlDbType.VarChar, 2083).Value = reader["FonteLicitacao"].ToString();
                            qryInsert.Parameters.Add("@Modalidade", MySqlDbType.Int32).Value = Convert.ToInt32(modalidade);
                            qryInsert.Parameters.Add("@Nome", MySqlDbType.VarChar, 255).Value = reader["Nome"].ToString();
                            qryInsert.Parameters.Add("@Objeto", MySqlDbType.Text).Value = reader["Objeto"].ToString();
                            qryInsert.Parameters.Add("@Obs", MySqlDbType.Text).Value = reader["Observacoes"].ToString();
                            qryInsert.Parameters.Add("@Responsavel", MySqlDbType.VarChar, 255).Value = reader["OrgaoResponsavel"].ToString();
                            qryInsert.Parameters.Add("@Contato", MySqlDbType.VarChar, 20).Value = reader["TelefoneContato"].ToString();
                            if (reader["TipoLocalizacao"].ToString().Length > 0)
                                qryInsert.Parameters.Add("@Localizacao", MySqlDbType.Int64).Value = Convert.ToInt64(reader["TipoLocalizacao"].ToString());
                            else
                                qryInsert.Parameters.Add("@Localizacao", MySqlDbType.Int64).Value = null;

                            if (reader["ValorLicitacao"].ToString().Length > 0)
                                qryInsert.Parameters.Add("@Licitado", MySqlDbType.Decimal).Value = Convert.ToDecimal(reader["ValorLicitacao"].ToString());
                            else
                                qryInsert.Parameters.Add("@Licitado", MySqlDbType.Decimal).Value = null;

                            qryInsert.Parameters.Add("@Sigiloso", MySqlDbType.Bit, 1).Value = Convert.ToBoolean(reader["ValorSigiloso"].ToString());

                            if (reader["DataCadastro"].ToString().Length > 0)
                                qryInsert.Parameters.Add("@Cadastro", MySqlDbType.DateTime).Value = Convert.ToDateTime(reader["DataCadastro"].ToString());
                            else
                                qryInsert.Parameters.Add("@Cadastro", MySqlDbType.DateTime).Value = null;
                            if (reader["DataEdicao"].ToString().Length > 0)
                                qryInsert.Parameters.Add("@Edicao", MySqlDbType.DateTime).Value = Convert.ToDateTime(reader["DataEdicao"].ToString());
                            else
                                qryInsert.Parameters.Add("@Edicao", MySqlDbType.DateTime).Value = null;
                            if (reader["DataExclusao"].ToString().Length > 0)
                                qryInsert.Parameters.Add("@Exclusao", MySqlDbType.DateTime).Value = Convert.ToDateTime(reader["DataExclusao"].ToString());
                            else
                                qryInsert.Parameters.Add("@Exclusao", MySqlDbType.DateTime).Value = null;
                            if (classificacao != "")
                                qryInsert.Parameters.Add("@classificacao", MySqlDbType.Int32).Value = Convert.ToInt32(classificacao);
                            else
                                qryInsert.Parameters.Add("@classificacao", MySqlDbType.Int32).Value = null;
                            qryInsert.Parameters.Add("@Area", MySqlDbType.Int16).Value = null;
                            qryInsert.Parameters.Add("@Subarea", MySqlDbType.Int16).Value = null;

                            if (Convert.ToBoolean(reader["ValorSigiloso"].ToString()) == true)
                                if (reader["CaminhoAnexo"].ToString() == "@")
                                    qryInsert.Parameters.Add("@Origem", MySqlDbType.Int16).Value = 3;
                                else
                                    qryInsert.Parameters.Add("@Origem", MySqlDbType.Int16).Value = 2;
                            else
                                qryInsert.Parameters.Add("@Origem", MySqlDbType.Int16).Value = 1;

                            try
                            {
                                qryInsert.ExecuteNonQuery();
                                erro = false;
                                nTotalRegistrosProcessados++;
                            }
                            catch (Exception ex)
                            {
                                nTotalErros++;
                                Console.WriteLine(ex.ToString());
                            }
                            finally
                            {
                                qryInsert.Dispose();

                                conexaoC2.Close();
                            }
                        }
                    }

                    conexaoR2.Open();

                    string Upd = "update tb_edital set CaminhoAnexo=@Caminho,ValorSigiloso=0 where ID=@id";
                    MySqlCommand qryUpdate = new MySqlCommand(Upd, conexaoR2);
                    qryUpdate.Parameters.Add("@id", MySqlDbType.Int32).Value = ID;
                    if (Convert.ToBoolean(reader["ValorSigiloso"].ToString()) == true)
                    {
                        if (quantidade == 0)
                        {
                            if (reader["CaminhoAnexo"].ToString() == "@")
                                qryUpdate.Parameters.Add("@Caminho", MySqlDbType.VarChar, 2083).Value = "@ - " + DateTime.Now.ToString();
                            else
                                qryUpdate.Parameters.Add("@Caminho", MySqlDbType.VarChar, 2083).Value = "# - " + DateTime.Now.ToString();
                        }
                        else
                        {
                            if (reader["CaminhoAnexo"].ToString() == "@")
                                qryUpdate.Parameters.Add("@Caminho", MySqlDbType.VarChar, 2083).Value = "@ Repetido - " + DateTime.Now.ToString();
                            else
                                qryUpdate.Parameters.Add("@Caminho", MySqlDbType.VarChar, 2083).Value = "# Repetido - " + DateTime.Now.ToString();
                        }
                    }
                    else
                    {
                        if (quantidade == 0)
                            qryUpdate.Parameters.Add("@Caminho", MySqlDbType.VarChar, 2083).Value = "* - " + DateTime.Now.ToString();
                        else
                            qryUpdate.Parameters.Add("@Caminho", MySqlDbType.VarChar, 2083).Value = "* Repetido - " + DateTime.Now.ToString();
                    }

                    try
                    {
                        qryUpdate.ExecuteNonQuery();
                        nTotalRegistroDbEdital++;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        qryUpdate.Dispose();
                        conexaoR2.Close();
                    }
                }
                qrySelect.Dispose();
                conexaoR1.Close();
                TotalLicitacoes.Text = nTotalLicitacoes.ToString();
                TotalRegistrosProcessados.Text = nTotalRegistrosProcessados.ToString();
                TotalRegistroDbEdital.Text = nTotalRegistroDbEdital.ToString();
            }

        }

        private DataTable CriaDataTableN1()
        {

            DataTable DataTableN1 = new DataTable();
            DataColumn DataColumnN1;
            DataColumnN1 = new DataColumn();
            DataColumnN1.DataType = Type.GetType("System.String");
            DataColumnN1.ColumnName = "n1_id";
            DataTableN1.Columns.Add(DataColumnN1);

            DataColumnN1 = new DataColumn();
            DataColumnN1.DataType = Type.GetType("System.String");
            DataColumnN1.ColumnName = "n1_peso";
            DataTableN1.Columns.Add(DataColumnN1);
            return DataTableN1;
        }
        private DataTable CriaDataTableN2()
        {
            DataTable DataTableN2 = new DataTable();
            DataColumn DataColumnN2;
            DataColumnN2 = new DataColumn();
            DataColumnN2.DataType = Type.GetType("System.String");
            DataColumnN2.ColumnName = "n2_id";
            DataTableN2.Columns.Add(DataColumnN2);

            DataColumnN2 = new DataColumn();
            DataColumnN2.DataType = Type.GetType("System.String");
            DataColumnN2.ColumnName = "n2_peso";
            DataTableN2.Columns.Add(DataColumnN2);
            return DataTableN2;
        }

        private void incluirNoDataTable(DataTable Tabela, string peso, int nivel, string id)
        {
            DataRow linha;

            linha = Tabela.NewRow();

            if (nivel == 1)
            {
                linha["n1_id"] = id;
                linha["n1_peso"] = peso;
            }
            else
            {
                linha["n2_id"] = id;
                linha["n2_peso"] = peso;
            }

            Tabela.Rows.Add(linha);
        }

        public static string ObterStringSemAcentosECaracteresEspeciais(string str)
        {

            /** Troca os caracteres acentuados por não acentuados **/
            
            string[] acentos = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
            string[] semAcento = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

            for (int i = 0; i < acentos.Length; i++)
            {
                str = str.Replace(acentos[i], semAcento[i]);
            }
            

            /** Troca os caracteres especiais da string por "" **/
            string[] caracteresEspeciais = { "¹", "²", "³", "£", "¢", "¬", "º", "¨", "\"", "'", ".", ",", "-", ":", "(", ")", "ª", "|", "\\\\", "°", "_", "@", "#", "!", "$", "%", "&", "*", ";", "/", "<", ">", "?", "[", "]", "{", "}", "=", "+", "§", "´", "`", "^", "~", "ÇO", "ÇÃO", "ÍPIO" };

            for (int i = 0; i < caracteresEspeciais.Length; i++)
            {
                str = str.Replace(caracteresEspeciais[i], "");
            }

            /** Troca os caracteres especiais da string por " " **/
            str = Regex.Replace(str, @"[^\w\.@-]", " ",
                                RegexOptions.None, TimeSpan.FromSeconds(1.5));

            return str.Trim();
        }

    }
}