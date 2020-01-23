using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LivrariaRomana.TestingAssistent.DBConfiguration
{
    public class DataOptionFactory
    {
        public string DefaultConnection { get; set; }
        public IDbConnection DatabaseConnection => new SqlConnection(DefaultConnection);
    }
}
