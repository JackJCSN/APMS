using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DataProvider
{
    /// <summary>
    /// 院系接口
    /// </summary>
    public class Department
    {
        public Int32 ID { get; private set; }
        public String Name { get; set; }
        public Int32 ParentSchoolID { get; set; }

        /// <summary>
        /// 初始化默认实例
        /// </summary>
        public Department()
        {
            ID = -1;
            Name = LocalString.NoDepartment;
            ParentSchoolID = -1;
        }

        /// <summary>
        /// 在院校ID为ParentSchoolID的院校下，创建一个名为Name的院系实例
        /// </summary>
        /// <param name="Name">新院系实例的名称</param>
        /// <param name="ParentSchoolID">新院系所属院校的ID</param>
        public Department(String Name, Int32 ParentSchoolID)
        {
            this.ID = -1;
            this.Name = Name;
            this.ParentSchoolID = ParentSchoolID;
        }

        private Department(SqlDataReader data)
        {
            ID = data.GetInt32(0);
            Name = data.GetString(1);
            ParentSchoolID = data.GetInt32(2);
        }

        /// <summary>
        /// 删除当前院系
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>是否删除成功</returns>
        public bool Delete(Authentication auth)
        {
            return Delete(this.ID, auth);
        }

        /// <summary>
        /// 删除一个院系
        /// </summary>
        /// <param name="id">将被删除的院系编号</param>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>是否删除成功</returns>        
        public static bool Delete(int id, Authentication auth)
        {
            if (auth.CheckAllows("departments", Permission.DELETE))
            {
                String SQL = @"DELETE FROM [dbo].[departments] WHERE ([departid] = {0});";
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
        /// 将当前实例作为新纪录添加到相关数据表
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>操作是否成功</returns>
        public bool Insert(Authentication auth)
        {
            if (auth.CheckAllows("departments", Permission.INSERT))
            {
                String INSERTSQL = "INSERT INTO [dbo].[departments] ([departname][schoolid]) VALUES (N'{0}',{1});";
                String SELECTSQL = "SELECT * FROM [dbo].[departments] WHERE departname = N'{0}' AND schoolid = {1};";
                INSERTSQL = String.Format(INSERTSQL, this.Name, this.ParentSchoolID);
                SELECTSQL = String.Format(SELECTSQL, this.Name, this.ParentSchoolID);
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
        /// 在数据库中更新当前实例
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>操作是否成功</returns>/// <returns></returns>
        public bool Update(Authentication auth)
        {
            if (auth.CheckAllows("departments", Permission.UPDATE))
            {
                String SQL = "UPDATE departments SET departname = N'{1}',schoolid = {2} WHERE departid = {0};";
                SQL = String.Format(SQL, this.ID, this.Name, this.ParentSchoolID);
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
        /// 通过ID查找院系
        /// </summary>
        /// <param name="id">院系ID</param>
        /// <param name="auth">授权人员</param>
        /// <returns>查找到的院系实例或者查找失败返回默认实例</returns>
        public static Department Search(Int32 id, Authentication auth)
        {
            if (auth.CheckAllows("departments", Permission.SELECT))
            {
                String SQL = @"SELECT TOP 1 * FROM [dbo].[schools]  WHERE [schoolid] = {0};";
                SQL = String.Format(SQL, id);
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                Department s = null;
                using (SqlDataReader data = cmd.ExecuteReader())
                {

                    if (data.Read())
                    {
                        s = new Department(data);
                    }
                    else
                    {
                        s = new Department();
                    }
                }
                return s;
            }
            throw new NoPermissionException();
        }

        /// <summary>
        /// 获取院校列表
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <returns>院校信息列表</returns>
        public static Department[] GetDepartments(School school, Authentication auth)
        {
            if (auth.CheckAllows("departments", Permission.SELECT))
            {
                List<Department> departments = new List<Department>();
                String SQL = @"SELECT TOP 1000  * FROM [dbo].[departments] WHERE [schoolid] = {0};";
                SQL = String.Format(SQL, school.ID);
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                using (SqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        departments.Add(new Department(data));
                    }
                }
                return departments.ToArray();
            }
            return new Department[0];
        }
    }

    public class Major
    {
        public Int32 ID { get; private set; }
        public String Name { get; set; }
        public Int32 ParentSchoolID { get; set; }
        public Int32 ParentDepartmentID { get; set; }
        public String ParentDepartment { get; private set; }
        public String ParentSchool { get; private set; }
        
        public Major()
        {
            ID = -1;
            Name = LocalString.NoMajor;
            ParentSchoolID = -1;
            ParentDepartmentID = -1;
            ParentDepartment = LocalString.NoDepartment;
            ParentSchool = LocalString.NoSchoolFind;
        }

        public Major(String name, Department Parent, School school)
        {
            ID = -1;
            Name = name;
            ParentSchoolID = Parent.ParentSchoolID;
            ParentDepartmentID = Parent.ID;
            ParentDepartment = Parent.Name;
            ParentSchool = school.Name;
        }

        private Major(SqlDataReader data)
        {
            ID = data.GetInt32(0);
            Name = data.GetString(1);
            ParentDepartmentID = data.GetInt32(2);
            ParentDepartment = data.GetString(3);
            ParentSchoolID = data.GetInt32(4);
            ParentSchool = data.GetString(5);
        }

        private void ctor(SqlDataReader data)
        {
            ID = data.GetInt32(0);
            Name = data.GetString(1);
            ParentDepartmentID = data.GetInt32(2);
            ParentDepartment = data.GetString(3);
            ParentSchoolID = data.GetInt32(4);
            ParentSchool = data.GetString(5);
        }

        /// <summary>
        /// 获取所有专业列表
        /// </summary>
        /// <param name="auth">授权</param>
        /// <returns>全部专业列表</returns>
        public static Major[] GetMajor(Authentication auth)
        {
            if (auth.CheckAllows("MajorView", Permission.SELECT))
            {
                List<Major> majors = new List<Major>();
                String SQL = @"SELECT  * FROM [dbo].[MajorView];";
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                using (SqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        majors.Add(new Major(data));
                    }
                }
                return majors.ToArray();
            }
            return new Major[0];
        }

        /// <summary>
        /// 删除当前专业
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>是否删除成功</returns>
        public bool Delete(Authentication auth)
        {
            return Delete(this.ID, auth);
        }

        /// <summary>
        /// 删除一个专业
        /// </summary>
        /// <param name="id">将被删除的专业编号</param>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>是否删除成功</returns>        
        public static bool Delete(int id, Authentication auth)
        {
            if (auth.CheckAllows("Major", Permission.DELETE))
            {
                String SQL = @"DELETE FROM [dbo].[major] WHERE ([majorid] = {0});";
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
        /// 获取指定院校下的专业列表
        /// </summary>
        /// <param name="school">所属院校</param>
        /// <param name="auth">授权人员</param>
        /// <returns>指定院校下的专业列表</returns>
        public static Major[] GetMajor(School school, Authentication auth)
        {
            if (auth.CheckAllows("MajorView", Permission.SELECT))
            {
                List<Major> majors = new List<Major>();
                String SQL = @"SELECT  * FROM [dbo].[MajorView] WHERE [schoolid] = {0};";
                SQL = String.Format(SQL, school.ID);
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                using (SqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        majors.Add(new Major(data));
                    }
                }
                return majors.ToArray();
            }
            return new Major[0];
        }

        /// <summary>
        /// 将当前实例作为新纪录添加到相关数据表
        /// </summary>
        /// <param name="auth">授权人员</param>
        /// <exception cref="NoPermissionException">NoPermissionException</exception>
        /// <returns>操作是否成功</returns>
        public bool Insert(Authentication auth)
        {
            if (auth.CheckAllows("major", Permission.INSERT))
            {
                String INSERTSQL = "INSERT INTO [dbo].[major] ([majorname][departid]) VALUES (N'{0}',{1});";
                String SELECTSQL = "SELECT TOP 1 * FROM [dbo].[MajorView] WHERE majorname = N'{0}' AND departid = {1};";
                INSERTSQL = String.Format(INSERTSQL, this.Name, this.ParentDepartment);
                SELECTSQL = String.Format(SELECTSQL, this.Name, this.ParentDepartment);
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
                            data = cmd.ExecuteReader();
                            if (data.Read())
                            {
                                ctor(data);
                                data.Close();
                            }
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
        /// 获取指定院系下的所有专业
        /// </summary>
        /// <param name="depart">所属院系</param>
        /// <param name="auth">授权人员</param>
        /// <returns>指定院系下的专业列表</returns>
        public static Major[] GetMajor(Department depart, Authentication auth)
        {
            if (auth.CheckAllows("MajorView", Permission.SELECT))
            {
                List<Major> majors = new List<Major>();
                String SQL = @"SELECT  * FROM [dbo].[MajorView] WHERE [departid] = {0};";
                SQL = String.Format(SQL, depart.ID);
                SqlCommand cmd = new SqlCommand(SQL, auth.Connection);
                using (SqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        majors.Add(new Major(data));
                    }
                }
                return majors.ToArray();
            }
            return new Major[0];
        }
    }
}
