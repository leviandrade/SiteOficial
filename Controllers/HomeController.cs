using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;

namespace SiteOficial.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private List<Models.Votacao> CriarListaVotos()
        {
            string stringConexao = @"Data Source=DESKTOP-DL249A7\SQLSERVER14;Initial Catalog=SiteOficial;user Id=sa;Password=24052716";
            string sql = "Select id,Nome,Idade,Voto from Votacao";
            List<Models.Votacao> lista = new List<Models.Votacao>();
            using (var conn = new SqlConnection(stringConexao))
            {
                var cmd = new SqlCommand(sql, conn);
                Models.Votacao p = null;
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            p = new Models.Votacao();
                            p.Id = (int)reader["id"];
                            p.Nome = reader["Nome"].ToString();
                            p.Idade = (int)reader["Idade"];
                            p.Voto = (int)reader["voto"];
                            lista.Add(p);
                        }
                    }
                }

                catch (Exception e)
                {
                    throw e;
                }
            }
            if (lista == null)
                lista = new List<Models.Votacao>();

            return lista
                .OrderBy(item => item.Id)
                .ToList();
        }

        public ActionResult Salvar(Models.Votacao entidade)
        {
            string stringConexao = @"Data Source=DESKTOP-DL249A7\SQLSERVER14;Initial Catalog=SiteOficial;user Id=sa;Password=24052716";
            using (var conn = new SqlConnection(stringConexao))
            {
                var lista = CriarListaVotos();
                var listaExistente = lista.Where(x => x.Nome == entidade.Nome);
                if (listaExistente.Count() > 0)
                    return RedirectToAction("Index",new { Sucesso = false });
                else
                {
                    entidade.Id = lista.Count == 0
                        ? 1
                        : lista.OrderByDescending(item => item.Id).FirstOrDefault().Id + 1;
                    string sql = "INSERT INTO Votacao (id,Nome,Voto,Idade) VALUES (@id,@Nome,@voto,@Idade)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", entidade.Id);
                    cmd.Parameters.AddWithValue("@Nome", entidade.Nome);
                    cmd.Parameters.AddWithValue("@voto", entidade.Voto);
                    cmd.Parameters.AddWithValue("@Idade", entidade.Idade);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    return RedirectToAction("Index");
                }

            }

        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult GerarGRaficoBarra()
        {
            var lista = CriarListaVotos();
            var chartData = new object[lista.Select(x=>x.Voto).Distinct().Count()+1];
            var grupos = lista.Select(x => x.Voto).Distinct().ToList();
            string role = string.Empty;

            int cont = -1;
            foreach(int i in grupos)
            {
                cont++;
                if (i == 0)
                    chartData[cont] = new object[] { "Otimo", lista.Where(x => x.Voto == 0).Count(), "#b87333" };
                if (i == 1)
                    chartData[cont] = new object[] {"Bom", lista.Where(x => x.Voto == 1).Count(), "silver"};
                if (i == 2)
                    chartData[cont] = new object[] {"Regular", lista.Where(x => x.Voto == 2).Count(), "silver"};
                if (i == 3)
                    chartData[cont] = new object[] {"Não", lista.Where(x => x.Voto == 3).Count(), "silver"};

            }

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult GerarGRafico()
        {
            var lista = CriarListaVotos();
            var chartData = new object[3];
            chartData[0] = new object[]{
                "Voto",
                "Contagem"
            };
            var VotoPositivo = "Avaliações Positivas";
            var VotoNegativo = "Avaliações Negativas";
            var ContVotosPositivos = 0;
            var ContVotosNegativos = 0;
            int j = 0;
            foreach (var item in lista)
            {
                if (item.Voto == 0 || item.Voto == 1)
                    ContVotosPositivos++;
                else
                    ContVotosNegativos++;
            }
            chartData[1] = new object[] { VotoPositivo, ContVotosPositivos };
            chartData[2] = new object[] { VotoNegativo, ContVotosNegativos };

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }
    }
}