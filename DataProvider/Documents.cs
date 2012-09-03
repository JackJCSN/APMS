using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace DataProvider
{
    public class Documents
    {
        /// <summary>
        /// 获取唯一区别文档的编号
        /// </summary>
        public Int32 ID { get; private set; }
        /// <summary>
        /// 获取或设置文件名
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 获取当前文件数据的SHA-1校验值
        /// </summary>
        public String Hash { get { return Authentication.SHA1(buffer); } }
        /// <summary>
        /// 获取文档数据
        /// </summary>
        public Byte[] Data
        {
            get
            {
                byte[] bytes = new byte[buffer.Length];
                Array.Copy(buffer, bytes, buffer.Length);
                return bytes;
            }
        }

        private byte[] buffer;
        private string hash;

        /// <summary>
        /// 创建一个空文档
        /// </summary>
        public Documents()
        {
            ID = -1;
        }

        /// <summary>
        /// 创建一个名称为name的空文档
        /// </summary>
        /// <param name="name">文档的名称</param>
        public Documents(String name)
        {
            Name = name;
        }

        private Documents(SqlDataReader data)
        {
            ID = data.GetInt32(0);
            Name = data.GetString(1);
            hash = data.GetString(2);
            buffer = data.GetSqlBinary(3).Value;
        }

        /// <summary>
        /// 从输入流设置文档数据
        /// </summary>
        /// <param name="stream">输入流</param>
        public void SetData(Stream stream)
        {
            if (stream.Length < 10485761)
            {
                buffer = new Byte[stream.Length];
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                stream.Read(buffer, 0, buffer.Length);
                return;
            }
            throw new ArgumentException(LocalString.FileToLarger);
        }

        /// <summary>
        /// 删除当前文档记录
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <returns>是否删除成功</returns>
        public bool Delete(Authentication auth)
        {
            return Delete(this.ID, auth);
        }

        /// <summary>
        /// 删除指定ID对应的文档
        /// </summary>
        /// <param name="id">要删除的文档</param>
        /// <param name="auth">授权人员</param>
        /// <returns>是否删除成功</returns>
        public static bool Delete(int id, Authentication auth)
        {
            if (auth.CheckAllows("announcedoc", Permission.DELETE))
            {
                String SQL = @"DELETE FROM [dbo].[announcedoc] WHERE ([fid] = {0});";
                SQL = String.Format(SQL, id);
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                switch (cmd.ExecuteNonQuery())
                {
                    case 1:
                        cmd.Dispose();
                        return true;
                    default:
                        cmd.Dispose();
                        return false;
                }
            }
            throw new NoPermissionException();
        }

        public static Documents[] GetDocuments(Authentication auth)
        {
            if (auth.CheckAllows("schools", Permission.SELECT))
            {
                SqlDataReader data = null;
                try
                {
                    String SQL = @"SELECT TOP 1000 * FROM [dbo].[announcedoc];";
                    SQL = String.Format(SQL);
                    SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                    data = cmd.ExecuteReader();
                    List<Documents> d = new List<Documents>();
                    while (data.Read())
                    {
                        d.Add(new Documents(data));
                    }
                    data.Close();
                    return d.ToArray<Documents>();
                }
                finally
                {
                    if (!data.IsClosed)
                    {
                        data.Close();
                    }
                }
            }
            return new Documents[0];
        }

        /// <summary>
        /// 将当前文档插入数据库
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <returns>插入操作是否成功</returns>
        public bool Insert(Authentication auth)
        {
            if (auth.CheckAllows("announcedoc", Permission.INSERT))
            {
                String INSERTSQL = "INSERT INTO [dbo].[announcedoc](fhash,fname,fdata) VALUES (@fid,@filename,@filedata);";
                SqlTransaction transaction = auth.Connection.BeginTransaction();
                SqlCommand cmd = new SqlCommand(INSERTSQL, auth.Connection);
                cmd.Parameters.AddWithValue("@fid", Authentication.SHA1(this.buffer));
                cmd.Parameters.AddWithValue("@filename", this.Name);
                cmd.Parameters.AddWithValue("@filedata", this.Data);
                cmd.Transaction = transaction;
                try
                {
                    cmd.CommandText = INSERTSQL;
                    switch (cmd.ExecuteNonQuery())
                    {
                        case 1:
                            transaction.Commit();
                            cmd.Dispose();
                            return true;
                        case 0:
                            transaction.Rollback();
                            cmd.Dispose();
                            return false;
                        default:
                            transaction.Rollback();
                            cmd.Dispose();
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    Debuger.PrintException(ex);
                    try
                    {
                        transaction.Rollback();
                        cmd.Dispose();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        Debuger.PrintException(ex2);
                    }
                    return false;
                }
            }
            throw new NoPermissionException();
        }
    }
}
