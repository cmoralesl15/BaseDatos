using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace BaseDatos.Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            string rutaRed;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(ConfigurationManager.AppSettings["storedProcedure"], connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Fecha", System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")));
                    SqlParameter sqlParameter = new SqlParameter("@Ruta", SqlDbType.VarChar, -1);
                    sqlParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(sqlParameter);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        rutaRed = command.Parameters["@Ruta"].Value.ToString();
                    }
                }
            }
            string rutaArchivoOrigen = rutaRed.Replace("S:", $@"\\SQLCLUSTER");
            string[] array = rutaRed.Split('\\');
            string rutaArchivoDestino = $@"C:\Users\Administrador\OneDrive - Universidad Mariano Gálvez\Backup\{array[array.Length - 2]}\{array[array.Length - 1]}";
            File.Copy(rutaArchivoOrigen, rutaArchivoDestino);
            File.AppendAllText(ConfigurationManager.AppSettings["monitor"], $@"Backup {array[array.Length - 1]}");
        }
    }
}
