using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ECoupoun.Data
{
    public class DBHelper
    {
        public string connectionString { get; set; }

        public SqlConnection objConnection { get; set; }
        public SqlCommand objCommand { get; set; }
        public SqlDataReader objReader { get; set; }

        public DBHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            objConnection = dbConection;
            objCommand = null;
            objReader = null;
        }

        private SqlConnection _dbECoupoun;
        public SqlConnection dbConection
        {
            get
            {
                if (_dbECoupoun == null)
                {
                    _dbECoupoun = new SqlConnection(connectionString);
                }
                return _dbECoupoun;
            }
        }

        public void OpenConnection()
        {
            this.objConnection = objConnection;
            if (objConnection.State != ConnectionState.Open)
                objConnection.Open();
        }
    }  
}
