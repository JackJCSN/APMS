using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DataProvider
{
    /// <summary>
    /// 院校接口
    /// </summary>
    public class School
    {
        /// <summary>
        /// 获取当前院校的ID
        /// </summary>
        public Int32 ID { get; private set; }
        /// <summary>
        /// 获取或设置当前院校的名字
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 默认构造函数，禁止类外默认实例化
        /// </summary>
        private School()
        {
            ID = -1;
            Name = LocalString.NoSchoolFind;
        }

        /// <summary>
        /// 创建一个名为name的院校实例
        /// </summary>
        /// <param name="name">新学校实例的名称</param>
        public School(string name)
        {
            this.ID = -1;
            this.Name = name;
        }

        /// <summary>
        /// 删除当前院校
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>是否删除成功</returns>
        public bool Delete(Authentication auth)
        {
            return Delete(this.ID, auth);
        }

        /// <summary>
        /// 删除一个院校
        /// </summary>
        /// <param name="id">将被删除的院校编号</param>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>是否删除成功</returns>        
        public static bool Delete(int id, Authentication auth)
        {
            if (auth.CheckAllows("schools", Permission.DELETE))
            {
                String SQL = @"DELETE FROM [dbo].[schools] WHERE ([schoolid] = {0});";
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

        /// <summary>
        /// 新增一个院校到相关数据表
        /// </summary>
        /// <param name="name">院校名字</param>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>操作是否成功</returns>
        public static bool Insert(String name, Authentication auth)
        {
            if (auth.CheckAllows("schools", Permission.INSERT))
            {
                String INSERTSQL = "INSERT INTO [dbo].[schools] ([schoolname]) VALUES (N'{0}');";
                String SELECTSQL = "SELECT * FROM [dbo].[schools] WHERE schoolname = N'{0}';";
                INSERTSQL = String.Format(INSERTSQL, name);
                SELECTSQL = String.Format(SELECTSQL, name);
                SqlTransaction transaction = auth.Connection.BeginTransaction();
                SqlCommand cmd = new SqlCommand(SELECTSQL, auth.Connection);
                cmd.Transaction = transaction;
                SqlDataReader data = cmd.ExecuteReader();
                try
                {
                    if (data.HasRows)
                    {
                        data.Close();
                        transaction.Rollback();
                        return false;
                    }
                    data.Close();
                    cmd.CommandText = INSERTSQL;
                    switch (cmd.ExecuteNonQuery())
                    {
                        case 1:
                            transaction.Commit();
                            cmd.Dispose();
                            return true;
                        case 0:
                            transaction.Commit();
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
                    if (!data.IsClosed)
                    {
                        data.Close();
                    }
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
                finally
                {
                    if (!data.IsClosed)
                    {
                        data.Close();
                    }
                }
            }
            throw new NoPermissionException();
        }

        /// <summary>
        /// 更新对当前院校实例所做的修改
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <returns>更新是否成功</returns>
        public bool Update(Authentication auth)
        {
            if (auth.CheckAllows("schools", Permission.UPDATE))
            {
                String SQL = "UPDATE schools SET schoolname = '{1}' WHERE schoolid = {0};";
                SQL = String.Format(SQL, this.ID, this.Name);
                SqlTransaction transaction = auth.Connection.BeginTransaction();
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                cmd.Transaction = transaction;
                try
                {
                    switch (cmd.ExecuteNonQuery())
                    {
                        case 1:
                            transaction.Commit();
                            cmd.Dispose();
                            return true;
                        case 0:
                            transaction.Commit();
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

        /// <summary>
        /// 将当前实例作为新纪录添加到相关数据表
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>操作是否成功</returns>
        public bool Insert(Authentication auth)
        {
            return Insert(this.Name, auth);
        }

        private School(SqlDataReader data)
        {
            this.ID = data.GetInt32(0);
            this.Name = data.GetString(1);
        }

        /// <summary>
        /// 通过编号查找院校
        /// </summary>
        /// <param name="id">要查找的院校ID</param>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>查找到的院校实例,或者没有找到时返回默认实例。</returns>
        public static School Search(Int32 id, Authentication auth)
        {
            if (auth.CheckAllows("schools", Permission.SELECT))
            {
                String SQL = @"SELECT TOP 1 * FROM [dbo].[schools]  WHERE [schoolid] = {0};";
                SQL = String.Format(SQL, id);
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                SqlDataReader data = cmd.ExecuteReader();
                School s = null;
                if (data.Read())
                {
                    s = new School(data);
                }
                else
                {
                    s = new School();
                }
                data.Close();
                return s;
            }
            throw new NoPermissionException();
        }

        /// <summary>
        /// 获取院校列表
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <returns>院校信息列表</returns>
        public static School[] GetSchools(Authentication auth)
        {
            if (auth.CheckAllows("schools", Permission.SELECT))
            {
                List<School> schools = new List<School>();
                String SQL = @"SELECT * FROM [dbo].[schools];";
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                SqlDataReader data = cmd.ExecuteReader();
                while (data.Read())
                {
                    schools.Add(new School(data));
                }
                data.Close();
                return schools.ToArray();
            }
            return new School[0];
        }
    }
}
