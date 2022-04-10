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
        private static List<DB_Result> exceptions = new List<DB_Result>();
        private static readonly string API_HOST = "https://axeit.ru/api/1_dev/";
        private static readonly HttpClient client = new HttpClient();
        public static DB_Result DeleteItem(string entity_type, long id )
        {
            return new DB_Result();
        }

        public static void Init_DB_result()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node0 = doc.CreateElement("root");
            doc.AppendChild(node0);
            XmlNode node1 = doc.CreateElement("type");
            node1.InnerText = "SELECT";
            node0.AppendChild(node1);
            node1 = doc.CreateElement("entity_type");
            node1.InnerText = "DICT_ALL";
            node0.AppendChild(node1);
            node1 = doc.CreateElement("entity");
            XmlNode node2 = doc.CreateElement("code");
            node1.AppendChild(node2);
            node2 = doc.CreateElement("description");
            node1.AppendChild(node2);
            node0.AppendChild(node1);
            node1 = doc.CreateElement("where");
            node2 = doc.CreateElement("condition");
            XmlNode node3 = doc.CreateElement("column");
            node3.InnerText = "GROUP_CODE";
            node2.AppendChild(node3);
            node3 = doc.CreateElement("operator");
            node3.InnerText = "=";
            node2.AppendChild(node3);
            node3 = doc.CreateElement("value");
            node3.InnerText = "DB_RESULT";
            node2.AppendChild(node3);
            node1.AppendChild(node2);
            node0.AppendChild(node1);

            Debug.Print(doc.OuterXml);
            string res = Http_request(doc.OuterXml);
            Debug.Print(res);
        }

        public static DB_Result AddItem(string entity_type, object obj)
        {
            return new DB_Result();
        }

        public static DB_Result EditItem(string entity_type, object obj)
        {
            return new DB_Result();
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

            string res = Http_request(doc.OuterXml);

            XmlDocument result = new XmlDocument();
            result.LoadXml(res);

            var state = result.SelectSingleNode("/root/state").InnerText;

            if (state == "200")
            {
                var token = result.SelectSingleNode("/root/token").InnerText;
                return new DB_Result("", state, token);
            } else
            {
                return new DB_Result("", state);
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

    public class DB_Result
    {
        public string msg { get; set; }
        public string state { get; set;  }

        public object? result { get; set; }

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
}
