using System;
using System.Text;
using System.Data.SqlClient;

namespace DataProvider
{
    /// <summary>
    /// 许可权标识
    /// </summary>
    [FlagsAttribute]
    public enum Permission : byte
    {
        None = 0x00,
        /// <summary>
        /// 查询
        /// </summary>
        SELECT = 0x01,
        /// <summary>
        /// 插入和查询
        /// </summary>
        INSERT = 0x02,
        /// <summary>
        /// 更新、插入和查询
        /// </summary>
        UPDATE = 0x04,
        /// <summary>
        /// 删除、更新、插入和查询 
        /// </summary>
        DELETE = 0x08,
        /// <summary>
        /// 当前连接的最高权限
        /// </summary>
        ALL = 0xFF
    }

    /// <summary>
    /// 没有权限
    /// </summary>
    public class NoPermissionException : InvalidOperationException
    {
        public NoPermissionException()
            : base(LocalString.NoPermission)
        {
        }
    }

    /// <summary>
    /// 用户认证和授权体系
    /// </summary>
    public sealed class Authentication : IDisposable
    {
        private DateTime LatestTime;
        DBConnctor conn;
        public static readonly TimeSpan OverTimeSpan = new TimeSpan(1, 0, 0);
        public bool IsExpired { get { return ((DateTime.Now - LatestTime) > OverTimeSpan); } }
        public String Name { get; private set; }
        public String Uid { get; private set; }
        public String Type { get; private set; }
        public int TypeID { get; private set; }

        /// <summary>
        /// SHA-1哈希计算方法
        /// </summary>
        /// <param name="Org">原始字符串</param>
        /// <returns>原始字符串的SHA-1哈希值的16进制字符串表示</returns>
        internal static String SHA1(String Org)
        {
            return BitConverter.ToString(
                System.Security.Cryptography.SHA1.Create().ComputeHash(
                UTF8Encoding.UTF8.GetBytes(Org))).Replace("-", "");
        }

        /// <summary>
        /// SHA-1哈希计算方法
        /// </summary>
        /// <param name="OrgBytes">原始字节数组</param>
        /// <returns>原始字节数组的SHA-1哈希值的16进制字符串表示</returns>
        internal static String SHA1(byte[] OrgBytes)
        {
            return BitConverter.ToString(
                System.Security.Cryptography.SHA1.Create().ComputeHash(
                OrgBytes)).Replace("-", "");
        }

        /// <summary>
        /// 获取和当前授权相关联的数据库连接
        /// </summary>
        internal SqlConnection Connection
        {
            get { return conn.conn; }
        }

        /// <summary>
        /// 使用默认的数据库配置进行认证
        /// </summary>
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public Authentication()
        {
            conn = new DBConnctor("::1", "APMS");
            conn.Open();
        }

        /// <summary>
        /// 使用指定的数据库配置进行认证（Windows集成验证）
        /// </summary>
        /// <param name="Server">要连接的数据库服务器</param>
        /// <param name="DataBaseName">要连接的数据库名</param>
        public Authentication(String Server, String DataBaseName)
        {
            conn = new DBConnctor(Server, DataBaseName);
            conn.Open();
        }

        /// <summary>
        /// 使用指定的数据库配置进行认证（数据库验证）
        /// </summary>
        /// <param name="Server">要连接的数据库服务器</param>
        /// <param name="DataBaseName">要连接的数据库名</param>
        /// <param name="UserName">使用的数据库用户名</param>
        /// <param name="PassWord">使用的数据库密码</param>
        public Authentication(String Server, String DataBaseName,
            String UserName, String PassWord)
        {
            conn = new DBConnctor(Server, DataBaseName, UserName, PassWord);
            conn.Open();
        }

        /// <summary>
        /// 登陆并获取一份授权
        /// </summary>
        /// <param name="User">用户名</param>
        /// <param name="Identification"></param>
        /// <returns></returns>
        public bool SignIn(String User, String Identification)
        {
            String SQL = 
            @"SELECT TOP 1 * FROM dbo.UserView WHERE (dbo.UserView.workername = '{0}' OR dbo.UserView.uid = '{1}')
            AND dbo.UserView.authentication = '{2}';";
            SQL = String.Format(SQL, User.ToUpper(), User.ToUpper(), SHA1(Identification).ToUpper());
            SqlCommand cmd = new SqlCommand(SQL, Connection);
            SqlDataReader data = cmd.ExecuteReader();
            while (data.Read())
            {
                Uid = data.GetString(0);
                Name = data.GetString(1);
                TypeID = data.GetInt32(3);
                Type = data.GetString(4);
                data.Close();
                LatestTime = DateTime.Now;
                return true;
            }
            data.Close();
            return false;
        }

        /// <summary>
        /// <para>联合查询授权检查</para>
        /// <para>注意:在联合授权检查中，对任何一个表可获取的权限低于请求检查的最低权限时，都会导致授权失败</para>
        /// <para>警告:在当前版本中，系统只进行登录超时检查，不进行权限检测。</para>
        /// </summary>
        /// <param name="TableName">包含要使用的dbo表的名称的数组</param>
        /// <param name="request">需要的最低权限</param>
        /// <returns>是否具有该权限</returns>
        public bool CheckAllows(String[] TableName, Permission request = Permission.ALL)
        {
            if (IsExpired)
            {
                return false;
            }
            foreach (var s in TableName)
            {
                //if (!CheckAllows(s, request))
                //{
                //    return false;
                //}
            }
            LatestTime = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 授权检查
        /// <para>警告:在当前版本中，系统只进行登录超时检查，不进行权限检测。</para>
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="request">需要的最低权限</param>
        /// <returns>是否具有该权限</returns>
        public bool CheckAllows(String TableName, Permission request = Permission.ALL)
        {
            if (IsExpired)
            {
                return false;
            }
            LatestTime = DateTime.Now;
            return true;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            this.conn.Dispose();
        }

        /// <summary>
        /// 安全关闭当前授权对象，并释放数据库连接等非托管资源。
        /// </summary>
        public void Close()
        {
            this.conn.Close();
        }

        #endregion
    }
}
