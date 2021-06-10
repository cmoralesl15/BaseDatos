using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseDatos.Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            string rutaArchivoOrigen;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(ConfigurationManager.AppSettings["storedProcedure"].ToString(), connection))
                {
                    SqlParameter sqlParameter = new SqlParameter("@Valor", SqlDbType.VarChar, -1);
                    sqlParameter.Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        rutaArchivoOrigen = command.Parameters["@Valor"].Value.ToString();
                    }
                }
            }
            string[] array = rutaArchivoOrigen.Split('\\');
            string rutaArchivoDestino = "C:\\Users\\Administrator\\OneDrive - Universidad Mariano Gálvez\\Backup" + array[array.Length - 2] + array[array.Length - 1];
            File.Copy(rutaArchivoOrigen,"");
        }
    }
}
