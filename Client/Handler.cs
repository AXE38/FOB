using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Client
{
    internal class Handler
    {
        public static List<DB_Result> db_results = new List<DB_Result>();
        private static readonly string API_HOST = "https://axeit.ru/api/1_dev/";
        private static readonly HttpClient client = new HttpClient();
        public static DB_Result DeleteItem(string entity_type, long id )
        {
            return new DB_Result();
        }

        public static void Init_DB_result()
        {
            string entity_type = "DICT_ALL";
            string[] columns = { "code", "description"};
            List<DB_Where> where = new List<DB_Where>()
            {
                new DB_Where("GROUP_CODE", "=", "DB_RESULT")
            };

            var res = SelectItems(entity_type, columns, where);

            if (res.state == "200")
            {
                foreach (XmlNode n in (XmlNodeList)res.result)
                {
                    db_results.Add(new DB_Result(n["description"].InnerText, n["code"].InnerText));
                }
                Debug.Print(db_results.Count.ToString());
            }
        }

        public static DB_Result AddItem(string entity_type, object obj)
        {
            return new DB_Result();
        }

        public static DB_Result EditItem(string entity_type, object obj)
        {
            return new DB_Result();
        }

        public static DB_Result SelectItems(string entity_type, string[] columns, List<DB_Where>? where = null, int? offset = null, int? fetch = null)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node0 = doc.CreateElement("root");
            doc.AppendChild(node0);
            XmlNode node1 = doc.CreateElement("type");
            node1.InnerText = "SELECT";
            node0.AppendChild(node1);
            node1 = doc.CreateElement("entity_type");
            node1.InnerText = entity_type;
            node0.AppendChild(node1);
            node1 = doc.CreateElement("entity");
            XmlNode node2;
            foreach (string col in columns)
            {
                node2 = doc.CreateElement(col);
                node1.AppendChild(node2);
            }
            node0.AppendChild(node1);
            if (where != null)
            {
                node1 = doc.CreateElement("where");
                foreach (DB_Where w in where)
                {
                    node2 = doc.CreateElement("condition");
                    XmlNode node3 = doc.CreateElement("column");
                    node3.InnerText = w.Column;
                    node2.AppendChild(node3);
                    node3 = doc.CreateElement("operator");
                    node3.InnerText = w.Operator;
                    node2.AppendChild(node3);
                    node3 = doc.CreateElement("value");
                    node3.InnerText = w.Value;
                    node2.AppendChild(node3);
                    node1.AppendChild(node2);
                }
            }
            node0.AppendChild(node1);

            string res = Http_request(doc.OuterXml);
            XmlDocument result = new XmlDocument();
            result.LoadXml(res);

            var state = result.SelectSingleNode("/root/state").InnerText;
            if (state == "200")
            {
                var data = result.SelectSingleNode("/root/data").ChildNodes;
                return new DB_Result(state, data);
            }
            return new DB_Result(state);
        }

        public static DB_Result Auth(string login, string password)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node0 = doc.CreateElement("root");
            doc.AppendChild(node0);
            XmlNode node1 = doc.CreateElement("type");
            node1.InnerText = "AUTH";
            node0.AppendChild(node1);
            XmlNode node2 = doc.CreateElement("where");
            XmlNode node3 = doc.CreateElement("condition");
            XmlNode node4 = doc.CreateElement("column");
            node4.InnerText = "abs_login";
            node3.AppendChild(node4);
            node4 = doc.CreateElement("operator");
            node4.InnerText = "=";
            node3.AppendChild(node4);
            node4 = doc.CreateElement("value");
            node4.InnerText = login;
            node3.AppendChild(node4);
            node2.AppendChild(node3);
            
            node3 = doc.CreateElement("condition");
            node4 = doc.CreateElement("column");
            node4.InnerText = "abs_password";
            node3.AppendChild(node4);
            node4 = doc.CreateElement("operator");
            node4.InnerText = "=";
            node3.AppendChild(node4);
            node4 = doc.CreateElement("value");
            node4.InnerText = password;
            node3.AppendChild(node4);
            node2.AppendChild(node3);
            node0.AppendChild(node1);
            node0.AppendChild(node2);

            Debug.Print(doc.OuterXml);
            string res = Http_request(doc.OuterXml);

            XmlDocument result = new XmlDocument();
            result.LoadXml(res);
            Debug.Print(result.OuterXml);
            var state = result.SelectSingleNode("/root/state").InnerText;

            if (state == "200")
            {
                var token = result.SelectSingleNode("/root/token").InnerText;
                return new DB_Result(state: state, result: token);
            } else
            {
                return new DB_Result(state: state);
            }
            
        }

        private static string Http_request(string request)
        {
            var values = new Dictionary<string, string>
                {
                    { "request", request }
                };

            var content = new FormUrlEncodedContent(values);

            var response = client.PostAsync(API_HOST, content).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }
    }

    public class DB_Client
    {
        public long? id { get; set; }
        public string? fullname { get; set; }
        public string? pass { get; set; }

        public string? phone { get; set; }

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

    public class DB_Result
    {
        public string msg { get; set; }
        public string state { get; set;  }

        public object? result { get; set; }

        public DB_Result(string state, object result = null)
        {
            var db_result = Handler.db_results.Find(e => e.state == state);
            //if (db_result == null) throw new InvalidOperationException($"Экземпляр с кодом {state} не найден");
            this.msg = db_result?.msg;
            this.state = state;
            this.result = result;
        }

        public DB_Result(string msg, string state, object result = null)
        {
            this.msg = msg;
            this.state = state;
            this.result = result;
        }

        public DB_Result()
        {

        }
    }

    public class DB_Where {
        public string Column { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public DB_Where(string column, string oper, string value)
        {
            Column = column;
            Operator = oper;
            Value = value;
        }
    }

}
