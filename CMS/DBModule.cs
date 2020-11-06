using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace CMS
{
  
    class DBModule
    {
        

        public static MySqlConnection DBConnect()
        {
            string ip = "127.0.0.1";           
            int port = 6033;
            string database = "admdams";
            string username = "root";
            string password = "root";

            String connString = "Server=" + ip + ";Database=" + database + "; port=" + port + ";userid=" + username + ";password=" + password;
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            return conn;

        }

    }
}
