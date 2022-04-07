using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Handler
    {
        public static void DeleteItem(string entity_type, long id )
        {

        }
    }

    public class DB_Client
    {
        public long id { get; set; }
        public string fullname { get; set; }
        public string pass { get; set; }

        public string phone { get; set; }

        public DB_Client()
        {

        }

    }

    public class DB_Cred
    {
        public long id { get; set; }
        public long collection_id { get; set; }
        public string client_name { get; set; }
        public string num { get; set; }
        public double iss_s { get; set; }
        public double s { get; set; }
        public DateTime create_date { get; set; }

        public DB_Cred()
        {

        }

    }
}
