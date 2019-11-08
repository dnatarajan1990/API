using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;

namespace StepRest
{
    class Program
    {
        public static Options Options { get; internal set; } = new Options();

        static int Main(string[] args)
        {
            RA.RestAssured.AddParser("xml", XmlToJObj);
            if(args.Length==0) args = new string[] { "test" };
            Options.ParseArgs(args);
            try
            {
                bool result = new Feature().Result;
                if (result) return 0;
                else return 1;
            } catch(Exception e)
            {
                Console.WriteLine("\n{0}\n{1}",e.Message,e.StackTrace.ToString());
                return 2;
            }
        }

        static public dynamic XmlToJObj(string _content)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_content);
            string jStr = JsonConvert.SerializeXmlNode(doc);
            JObject json = JObject.Parse(jStr);
            return json;
        }
    }
}
