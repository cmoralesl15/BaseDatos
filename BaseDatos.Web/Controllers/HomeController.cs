using BaseDatos.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BaseDatos.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Validar(FormCollection form)
        {
            try
            {
                string usuario = form["Usuario"];
                string contrasena = form["Contrasena"];
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpIniciarSesion"))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Usuario", usuario));
                        command.Parameters.Add(new SqlParameter("@Usuario", contrasena));
                        command.ExecuteReader();
                        Session["Usuario"] = usuario;
                    }
                }
            }
            catch (Exception)
            {
                Session["Mensaje"] = "Ha ocurrido un error, intente nuevamente.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Transferencias");
        }

        public ActionResult Transferencias()
        {
            Cuentas cuentas = new Cuentas();
            try
            {
                //Cuentas Propias
                string usuario = Session["Usuario"].ToString();
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpCuentasPropias"))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Usuario", usuario));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cuentas.cuentaAjenas.Add(reader["Cuenta"].ToString());
                            } 
                        }
                    }
                }
                //Cuentas Ajenas
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpCuentasAjenas"))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Usuario", usuario));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cuentas.cuentaAjenas.Add(reader["Cuenta"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Session["Mensaje"] = "Ha ocurrido un error, intente nuevamente.";
                return RedirectToAction("Index");
            }
            return View(cuentas);
        }
        public ActionResult Transferir(FormCollection form)
        {
            try
            {
                string cuentaOrigen = form["CuentaOrigen"];
                string cuentaDestino = form["CuentaDestino"];

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpCuentasAjenas"))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@CuentaOrigen", cuentaOrigen));
                        command.Parameters.Add(new SqlParameter("@CuentaDestino", cuentaDestino));
                        command.ExecuteReader();
                        Response.Write("<script>alert('Transferencia exitosa.')</script>");
                    }
                }
            }
            catch (Exception)
            {
                Session["Mensaje"] = "Ha ocurrido un error, intente nuevamente.";
            }
            return RedirectToAction("Transferencias");
        }
    }
}