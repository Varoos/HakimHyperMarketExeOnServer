using Focus.DatabaseFactory;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakimHyperMarketExeOnServer
{
    public class Clsdata
    {
        //Get Data from Focus Database Sql server 
        static string FSerName = ConfigurationManager.AppSettings["FocusServerName"];
        static string FDB = ConfigurationManager.AppSettings["FocusDBName"];
        static string FSQLUID = ConfigurationManager.AppSettings["FocusUserName"];
        static string FSQLPWD = ConfigurationManager.AppSettings["FocusPassword"];
        static string Fconnection = $"data source={FSerName};initial catalog={FDB};User ID={FSQLUID};Password={FSQLPWD};integrated security=True;MultipleActiveResultSets=True";
        SqlConnection con = new SqlConnection(Fconnection);

        public static DataSet GetData(string strSQLQry, int compid)
        {
            Clsdata.LogFile("POSSalesExeserver", "GetData Method Entered");
            DataSet ds = null;

            try
            {
                Clsdata.LogFile("POSSalesExeserver", "GetDatabase SelectQuery : " + strSQLQry);
                Database db = DatabaseWrapper.GetDatabase(compid);
                Clsdata.LogFile("POSSalesExeserver", "After GetDatabase DatabaseWrapper.GetDatabase");
                ds = db.ExecuteDataSet(CommandType.Text, strSQLQry);

            }
            catch (Exception e)
            {
                Clsdata.LogFile("POSSalesExeserver", "GetDatabase exception:" + e.Message);
            }
            finally
            {
            }
            return ds;
        }
        public static int GetExecute(string strSelQry, int CompId)
        {
            Clsdata.LogFile("POSSalesExeserver", "GetExecute Method Entered");
            try
            {
                try
                {
                    Clsdata.LogFile("POSSalesExeserver", "GetExecute SelectQuery"+strSelQry);
                    Database obj = Focus.DatabaseFactory.DatabaseWrapper.GetDatabase(CompId);
                    Clsdata.LogFile("POSSalesExeserver", "After Focus.DatabaseFactory.DatabaseWrapper.GetDatabase");
                    return (obj.ExecuteNonQuery(CommandType.Text, strSelQry));
                }
                catch (Exception e)
                {
                    Clsdata.LogFile("POSSalesExeserver", "GetExecute exception:" + e.Message);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Clsdata.LogFile("POSSalesExeserver", "GetExecute exception:" + ex.Message);
                return 0;
            }
        }

        public int GetCompanyId(string strCompanyCode)
        {
            try
            {
                string strCode = strCompanyCode;
                int iCompId = ((strCode[0] >= 'A' ? strCode[0] - 55 : strCode[0] - 48) * 36 * 36) + ((strCode[1] >= 'A' ? strCode[1] - 55 : strCode[1] - 48) * 36) + (strCode[2] - 48);
                return iCompId;
            }
            catch (Exception ex)
            {
                LogFile("StatusLog.txt", "Bad request In GetCompanyId" + ex.Message);
                return 0;
            }

        }
        public Int64 GetDateTimetoInt(DateTime dt)
        {
            Int64 val;
            val = Convert.ToInt64(dt.Year) * 8589934592 + Convert.ToInt64(dt.Month) * 33554432 + Convert.ToInt64(dt.Day) * 131072 + Convert.ToInt64(dt.Hour) * 4096 + Convert.ToInt64(dt.Minute) * 64 + Convert.ToInt64(dt.Second);
            return val;
        }
        public int GetDateToInt(DateTime dt)
        {
            int val;
            val = Convert.ToInt16(dt.Year) * 65536 + Convert.ToInt16(dt.Month) * 256 + Convert.ToInt16(dt.Day);
            return val;
        }
        public static void LogFile(string LogName, string content)
        {
            string sFilePath = AppDomain.CurrentDomain.BaseDirectory;
            string str = LogName + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            FileStream stream = new FileStream(sFilePath + str, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.BaseStream.Seek(0L, SeekOrigin.End);
            writer.WriteLine(DateTime.Now.ToString() + " - " + content);
            writer.Flush();
            writer.Close();




        }

        public static int Update(string Vouc)
        {
            Clsdata.LogFile("POSSalesExeserver", "Update Method Entered");
            int result = 0;
            using (SqlConnection connect = new SqlConnection(Fconnection))
            {
                string sql = $"{Vouc}";
                using (SqlCommand command = new SqlCommand(sql, connect))
                {
                    connect.Open();
                    result = command.ExecuteNonQuery();
                    connect.Close();
                }
            }
            Clsdata.LogFile("POSSalesExeserver", "Update Method Ended");
            return result;
        }
    }
}
