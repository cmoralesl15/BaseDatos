using BaseDatos.Web.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace BaseDatos.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["PagOrigen"] = "Index";
            return View();
        }

        public ActionResult Validar(FormCollection form)
        {
            try
            {
                string usuario = form["Usuario"].ToLower();
                string contrasena = form["Contrasena"];
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpIniciarSesion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Usuario", usuario));
                        command.Parameters.Add(new SqlParameter("@contrasena", contrasena));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Session["Id_Cliente"] = reader["Id_Cliente"];
                                Session["Nombre"] = reader["Nombre"];
                                Session["Apellido"] = reader["Apellido"];
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Session["Mensaje"] = "";
                Session["MensajeError"] = e.Message;
                return RedirectToAction("Index");
            }
            Session["Mensaje"] = "";
            Session["MensajeError"] = "";
            return RedirectToAction("Transferencias");
        }

        public ActionResult Transferencias()
        {
            Cuentas cuentas = new Cuentas();
            try
            {
                //Cuentas Propias
                string idCliente = Session["Id_Cliente"].ToString();
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpCuentasPropias", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Id_Cliente", idCliente));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cuentas.cuentaPropia.Add(reader["Id_Cuenta"].ToString(), "Q" + reader["Saldo"].ToString());
                            } 
                        }
                    }
                }
                //Cuentas Ajenas
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpCuentasAjenas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Id_Cliente", idCliente));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cuentas.cuentaAjena.Add(reader["Id_Cuenta"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Session["Mensaje"] = "";
                Session["MensajeError"] = e.Message;
                return RedirectToAction("Index");
            }

            if (Session["PagOrigen"].Equals("Index"))
            {
                Session["Mensaje"] = "";
                Session["MensajeError"] = "";
                Session["PagOrigen"] = "Transferencias";
            }

            return View(cuentas);
        }
        public ActionResult Transferir(FormCollection form)
        {
            try
            {
                string cuentaOrigen = form["CuentaOrigen"];
                string cuentaDestino = form["CuentaDestino"];
                string monto = form["Monto"];

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SpTransferir", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@CuentaOrigen", cuentaOrigen));
                        command.Parameters.Add(new SqlParameter("@CuentaDestino", cuentaDestino));
                        command.Parameters.Add(new SqlParameter("@Monto", monto));
                        command.ExecuteReader();
                        Session["Mensaje"] = $@"Ha transferido Q{Double.Parse(monto)} desde la cuenta: {cuentaOrigen} hacia la cuenta: {cuentaDestino}";
                        Session["MensajeError"] = "";
                        Session["PagOrigen"] = "Transferencias";
                        try
                        {
                            System.IO.File.AppendAllText(ConfigurationManager.AppSettings["monitor"], $@"{Session["Nombre"]} {Session["Apellido"]} ha transferido Q{Double.Parse(monto)} desde la cuenta: {cuentaOrigen} hacia la cuenta: {cuentaDestino}" + "\n");
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Session["Mensaje"] = "";
                Session["MensajeError"] = e.Message;
                Session["PagOrigen"] = "Transferencias";
            }
            return RedirectToAction("Transferencias");
        }
    }
}