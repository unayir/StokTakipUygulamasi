using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StokTakipUygulamasi.ORM
{
    public class ORMBase<T> : IORM<T>
    {
        Type MyGetType
        {
            get
            {
                return typeof(T);

            }
        }

        public DataTable List()
        {
            SqlDataAdapter adap = new SqlDataAdapter(string.Format("pr_{0}_List", MyGetType.Name), Tools.Connection);
            adap.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            adap.Fill(dt);
            return dt;
        }

        public bool Insert(T entity)
        {
            SqlCommand cmd = new SqlCommand(string.Format("pr_{0}_Insert", MyGetType.Name), Tools.Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            PropertyInfo [] propertys = MyGetType.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                string propertyName = "@" + p.Name;
                object value = p.GetValue(entity);
                cmd.Parameters.AddWithValue(propertyName, value);
            }
            return Tools.ExecuteNonQuery(cmd);
        }

        public bool Update(T entity)
        {
            SqlCommand cmd = new SqlCommand(string.Format("pr_{0}_Update", MyGetType.Name), Tools.Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            PropertyInfo[] propertys = MyGetType.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                string propertyName = "@" + p.Name;
                object value = p.GetValue(entity);
                cmd.Parameters.AddWithValue(propertyName, value);
            }
            return Tools.ExecuteNonQuery(cmd);
        }
        public bool Delete(int id)
        {
            T entity = Activator.CreateInstance<T>();
            SqlCommand cmd = new SqlCommand(string.Format("pr_{0}_Delete", MyGetType.Name), Tools.Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            PropertyInfo primaryColumn = MyGetType.GetProperty("PrimaryColumn");
            string parameterName = "@" + primaryColumn.GetValue(entity);
            cmd.Parameters.AddWithValue(parameterName, id);
            return Tools.ExecuteNonQuery(cmd);
        }
    }
}
