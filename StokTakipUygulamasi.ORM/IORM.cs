using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokTakipUygulamasi.ORM
{
    public interface IORM<T>
    {
        DataTable List();
        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
