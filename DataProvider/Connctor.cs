using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DataProvider
{
    /// <summary>
    /// 数据库连接器
    /// </summary>
    public class DBConnctor : IDisposable
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public static readonly int TimeOut = 7;
        Boolean Integrated = false;
        String Server;
        String DBName;
        String User;
        String Pass;
        /// <summary>
        /// 获取当前的连接
        /// </summary>
        internal SqlConnection conn { get; private set; }

        internal DBConnctor(String Server, String DataBaseName)
        {
            DBName = DataBaseName;
            Integrated = true;
            this.Server = Server;
        }

        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <param name="Server">要连接到的数据库服务器</param>
        /// <param name="DataBaseName">数据库名称</param>
        /// <param name="UserName">数据库访问用户名</param>
        /// <param name="PassWord">数据库访问用户密码</param>
        internal DBConnctor(String Server, String DataBaseName, String UserName, String PassWord)
        {
            DBName = DataBaseName;
            User = UserName;
            Pass = PassWord;
            this.Server = Server;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();
            sqlCSB.Add("server", "tcp:" + Server);
            sqlCSB.Add("database", DBName);
            if (this.Integrated)
            {
                sqlCSB.Add("Integrated Security", true);
            }
            else
            {
                sqlCSB.Add("uid", User);
                sqlCSB.Add("pwd", Pass);
            }
            sqlCSB.Add("Connection Timeout", TimeOut);
            //启用多个活动结果集支持(MARS)仅SQL Server 2008及更新版本
            //sqlCSB.Add("MultipleActiveResultSets", "true");
            conn = new SqlConnection(sqlCSB.ConnectionString);
            conn.Open();
        }

        /// <summary>
        /// 获取可用数据库名
        /// </summary>
        /// <param name="Server">连接到的服务器</param>
        /// <param name="UserName">连接用户名</param>
        /// <param name="PassWord">连接密码</param>
        /// <returns>可用数据库名称列表或者本地化错误信息</returns>
        public static String[] GetDBNames(String Server, String UserName = null, String PassWord = null)
        {
            List<String> names = new List<String>();
            names.Add(LocalString.NoDBNames);
            try
            {
                SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();
                sqlCSB.Add("server", "tcp:" + Server);
                if (UserName == null || PassWord == null)
                {
                    sqlCSB.Add("Integrated Security", true);
                }
                else
                {
                    sqlCSB.Add("uid", UserName);
                    sqlCSB.Add("pwd", PassWord);
                }
                sqlCSB.Add("Connection Timeout", TimeOut);
                SqlConnection sqlConnection = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnection.Open();
                String SQL = @"SELECT name FROM sys.databases WHERE "
                    + "name NOT IN ('master', 'tempdb', 'model', 'msdb','ReportServer','ReportServerTempDB');";
                SqlCommand cmd = new SqlCommand(SQL, sqlConnection);
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    names.RemoveAll((a) => { return true; });
                }
                while (data.Read())
                {
                    names.Add(data["name"].ToString());
                }
                data.Close();
                cmd.Dispose();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Debuger.PrintException(ex);
            }
            return names.ToArray();
        }



        /// <summary>
        /// 使用Windows集成进行测试连接
        /// </summary>
        /// <param name="Server">数据库服务器地址</param>
        /// <param name="DataBaseName">数据库名</param>
        /// <returns>是否可以连接到数据库</returns>
        /// <param name="Error"></param>
        public static bool TestDBConnction(String Server, String DataBaseName,out String Error)
        {
            try
            {
                SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();
                sqlCSB.Add("server", "tcp:" + Server);
                sqlCSB.Add("database", DataBaseName);
                sqlCSB.Add("Integrated Security", true);
                sqlCSB.Add("Connection Timeout", TimeOut);
                SqlConnection sqlConnection = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnection.Open();
                sqlConnection.Close();
                Error = "";
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Class < 17)
                {
                    Debuger.PrintException(ex);
                    Error = ex.Message;
                    return false;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 进行测试连接
        /// </summary>
        /// <param name="Server">数据库服务器地址</param>
        /// <param name="DataBaseName">数据库名</param>
        /// <param name="UserName">数据库用户名</param>
        /// <param name="PassWord">数据库密码</param>
        /// <returns>是否可以连接到数据库</returns>
        public static bool TestDBConnction(String Server, String DataBaseName, String UserName, String PassWord,out String Error)
        {
            try
            {
                SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();
                sqlCSB.Add("server", "tcp:" + Server);
                sqlCSB.Add("database", DataBaseName);
                sqlCSB.Add("uid", UserName);
                sqlCSB.Add("pwd", PassWord);
                sqlCSB.Add("Connection Timeout", TimeOut);
                SqlConnection sqlConnection = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnection.Open();
                sqlConnection.Close();
                Error = "";
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Class < 17)
                {
                    Debuger.PrintException(ex);
                    Error = ex.Message;
                    return false;
                }
                throw ex;
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            this.conn.Close();
        }

        public void Close()
        {
            this.conn.Close();
        }

        #endregion
    }
}
