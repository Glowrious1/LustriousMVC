using MySql.Data.MySqlClient;

namespace Lustrious.Data
{
    public class DataBase
    {
        private readonly string connectionString = "server=localhost;port=3306;database=dbilumina;user=root;password=12345678;";

        public MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
