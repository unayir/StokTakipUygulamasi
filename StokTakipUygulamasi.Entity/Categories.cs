using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokTakipUygulamasi.Entity
{
    public class Categories : EntityBase
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public override string PrimaryColumn
        {
            get
            {
                return "CategoryID";
            }
        }
    }
}
