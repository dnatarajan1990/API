using RA.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace StepRest.Runner
{

    public static class IRunnerExtention
    {
        public static void Run(this IRunner source, string line)
        {
            Type type = GetType(line.Split(" ")[0].ToLower());

            string l = line.Substring(line.IndexOf(" ")+1);
            string rgxStr=null;
            Object[] methodParams=null;
            MethodInfo method;
            // find method using found type and matching regex
            try
            {
                method = source.GetType().GetMethods().Where(
                    m => m.GetCustomAttributes(type, false).Length > 0)
                   .Where(s => s.GetCustomAttributesData()
                         .Any(x => new Regex(x.ConstructorArguments.ToArray()[0].Value.ToString()).IsMatch(l)))
                   .ToArray()[0];
            } catch(Exception e)
            {
                Console.Error.WriteLine("\n{0}\n\t{1}","FAILED TO FIND STEP",line);
                throw e;
            }

            // extract that regex so we can extract data
            rgxStr = method.GetCustomAttributesData().ToArray()[0].ConstructorArguments.ToArray()[0].Value.ToString();

            // Extract data and save it to pass to the found method
            methodParams = GetMethodParameters(rgxStr, l);

            try
            {

                try
                {
                    method.Invoke(source, methodParams); // run method
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            } catch(Exception e)
            {
                throw new AssertException(e.Message);
            }
        }


        private static object[] GetMethodParameters(string rgxStr, string line) 
            => (rgxStr.Contains("<keyvaluepair>")
                ? "kvp" 
                : rgxStr.Contains("<dictionary>") 
                    ? "dict" 
                    : rgxStr.Contains("<datatable>")
                        ? "datatable"
                        : null) switch
        {
            "kvp" => GetKvpParams(rgxStr, line),
            "dict" => GetDictParams(rgxStr, line),
            "datatable" => GetTableParams(rgxStr, line),
            _ => GetNormalParams(rgxStr, line),
        };

        private static object[] GetKvpParams(string rgxStr, string line)
        {
            var kvp = new NameValueCollection();
            foreach (Match m in Regex.Matches(line, rgxStr))
                for (int i = 0; i < m.Groups[@"keyvaluepair"].Captures.Count; i++)
                    kvp.Add(m.Groups["key"].Captures[i].Value, m.Groups["value"].Captures[i].Value);
            return new object[] { kvp };
        }

        private static object[] GetDictParams(string rgxStr, string line)
        {
            var dict = new Dictionary<string, string>();
            foreach (Match m in Regex.Matches(line, rgxStr))
                for (int i = 0; i < m.Groups[@"dictionary"].Captures.Count; i++)
                    dict.Add(m.Groups["key"].Captures[i].Value, m.Groups["value"].Captures[i].Value);
            return new object[] { dict };
        }
        private static object[] GetTableParams(string rgxStr, string line)
        {
            List<string> keys = new List<string>();
            foreach(Match m in Regex.Matches(rgxStr, @"datatable>(?<table>[^<]*<(?<item>[^>]*)>)*"))
                for (int i = 0; i < m.Groups[@"table"].Captures.Count; i++)
                    keys.Add(m.Groups["item"].Captures[i].Value);
            var data = new List<Dictionary<string, string>>();
            foreach (Match m in Regex.Matches(line, rgxStr))
                for (int i = 0; i < m.Groups[@"datatable"].Captures.Count; i++)
                {
                    var inner = new Dictionary<string, string>();
                    foreach (var k in keys)
                        inner.Add(k, m.Groups[k].Captures[i].Value);
                    data.Add(inner);
                }
            return new object[] { data };
        }

        private static object[] GetNormalParams(string rgxStr, string line)
        {
            Regex rgx = new Regex(rgxStr);
            Match m = rgx.Match(line);

            GroupCollection groups = m.Groups;
            List<object> args = new List<object>();
            foreach (string groupName in rgx.GetGroupNames())
            {
                if (int.TryParse(groupName, out int num)) continue;
                string val = groups[groupName].Value;
                if (val.StartsWith("\"") && val.EndsWith("\"")) val = val.Substring(1, val.Length - 2);
                args.Add(val);
            }
            return args.ToArray();
        }


        private static Type GetType(string v) => v switch
        {
            "given" => ReturnType(typeof(GivenAttribute)),
            "when" => ReturnType(typeof(WhenAttribute)),
            "then" => ReturnType(typeof(ThenAttribute)),
            "and" => ReturnType(),
            _ => throw new ArgumentException("Invalid Feature File: \n\tCould not find " + v, paramName: nameof(v))
        };

        private static Type ReturnType(Type type)
        {
            lastType = type;
            return type;
        }

        private static Type lastType = null;
        private static Type ReturnType()
        {
            if (lastType == null)
                throw new ArgumentException("Invalid Feature File: \n\tAnd cannot be used before other types", paramName: "And");
            return lastType;
        }
    }
}