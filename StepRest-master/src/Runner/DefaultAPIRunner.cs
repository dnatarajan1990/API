using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace StepRest.Runner
{
    public class DefaultAPIRunner : APIRunner
    {
        protected override IRunner Refresh() => new DefaultAPIRunner();

        #region Givens
        [Given(regex: @"^a (?:base )?uri (?<uri>.*)$")]
        public void AUri(string uri) => Setup.Host(uri);

        [Given(regex: @"^a query of (?<key>[^=]*)=(?<val>.*)$")]
        public void AQuery(string key, string value) => Setup.Query(key, value);
        [Given(regex: @"^queries:?((?<keyvaluepair>\|(?<key>[^\|]*)\|(?<value>[^\|]*)\|)+)$")]
        public void Queries(NameValueCollection data) => Setup.Queries(data);

        [Given(regex: @"^a parameter of (?<key>[^=]*)=(?<val>.*)$")]
        public void AParam(string key, string value) => Setup.Param(key, value);

        [Given(regex: @"^parameters:?((?<dictionary>\|(?<key>[^\|]*)\|(?<value>[^\|]*)\|)+)$")]
        public void Params(Dictionary<string, string> data) => Setup.Params(data);

        [Given(regex: @"^a header of (?<key>[^=]*)=(?<val>.*)$")]
        public void AHeader(string key, string value) => Setup.Header(key, value);

        [Given(regex: @"^headers:?((?<dictionary>\|(?<key>[^\|]*)\|(?<value>[^\|]*)\|)+)$")]
        public void Headers(Dictionary<string, string> data) => Setup.Headers(data);

        [Given(regex: @"^a (?:base )?path (?<path>.*)$")]
        public void APath(string path) => Setup.Uri(path);

        [Given(regex: @"^a (?:base )?body (?<body>.*)$")]
        public void ABody(string body) => Setup.Body(body.Replace("|","\"n").Trim());

        [Given(regex: @"^a (?:base )?port (?<port>\d*)$")]
        public void APort(string port) => Setup.Port(int.Parse(port));
        #endregion

        #region Whens
        [When(@"^the (?:(system|user)) requests (?<method>(GET|POST|PUT|PATCH|DELETE)) ?(?<path>.*)?$")]
        public void TheSystemRequests(string method, string path = null) => Execution = method switch
        {
            "GET" => Action.Get(path),
            "POST" => Action.Post(path),
            "PUT" => Action.Put(path),
            "PATCH" => Action.Patch(path),
            "DELETE" => Action.Delete(path),
            _ => throw new ArgumentException("Http method not recognised [" + method + "]", nameof(method)),
        };
        [When(@"^the system loads (?<threads>\d*) threads for (?<time>\d*) ?(?<op>sec|seconds|min|minutes)$")]
        public void TheSystemLoads(string threads, string time, string op) 
            => Action.Load(int.Parse(threads), op.StartsWith("m") ? int.Parse(time) * 60 : int.Parse(time));
        #endregion

        #region Thens
        [Then(@"^the (?:response )?code is (?<code>\d*)$")]
        public void TheResponseCodeIs(string code)
        {
            Response.TestStatus("The Response Code is " + code, x => x == int.Parse(code));
            Response.Assert("The Response Code is " + code);
        }

        [Then(@"^the (?:response )?code starts with (?<code>[1,2,3,4,5]{1})$")]
        public void TheResponseCodeStartsWith(string code)
        {
            Response.TestStatus("The Response Code starts with " + code, x => x.ToString().StartsWith(code));
            Response.Assert("The Response Code starts with " + code);
        }
        
        [Then(@"^the (?:response )?time is (?:under|less than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheResponseTimeIsUnder(string time, string type)
        {
            int c = type.StartsWith("m") ? int.Parse(time) : int.Parse(time) * 1000;
            string stepname = string.Format("The Response Time is less than {0} milliseconds", c);
            Response.TestElaspedTime(stepname, x => x < c);
            Response.Assert(stepname);
        }
        [Then(@"^the (?:response )?time is (?:over|more than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheResponseTimeIsOver(string time, string type)
        {
            int c = type.StartsWith("m") ? int.Parse(time) : int.Parse(time) * 1000;
            string stepname = string.Format("The Response Time is more than {0} milliseconds", c);
            Response.TestElaspedTime(stepname, x => x > c);
            Response.Assert(stepname);
        }

        #region Load
        [Then(@"^the total successfull loads? requests? (?:is|are) (?:under|less than) (?<amount>\d*)$")]
        public void TheLoadSuccessIsUnder(string amount) => LoadTestUnder("total-succeeded", amount);

        [Then(@"^the total successfull load requests? (?:is|are) (?:over|more than) (?<amount>\d*)$")]
        public void TheLoadSuccessIsOver(string amount) => LoadTestOver("total-succeeded", amount);


        [Then(@"^the total failed load requests? (?:is|are) (?:under|less than) (?<amount>\d*)$")]
        public void TheLoadFailIsUnder(string amount) => LoadTestUnder("total-lost", amount);

        [Then(@"^the total failed load requests? (?:is|are) (?:over|more than) (?<amount>\d*)$")]
        public void TheLoadFailIsOver(string amount) => LoadTestOver("total-lost", amount);


        [Then(@"^the success percentage is (?:under|less than) (?<amount>\d*)%$")]
        public void TheSuccessPercentIsUnder(string amount) => LoadTestUnder("success-percent", amount);

        [Then(@"^the success percentage is (?:over|more than) (?<amount>\d*)%$")]
        public void TheSuccessPercentIsOver(string amount) => LoadTestOver("success-percent", amount);


        [Then(@"^the average load time is (?:under|less than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheAverageLoadTimeIsUnder(string time, string type) => LoadTestUnder("average-ttl-ms", time, type);

        [Then(@"^the average load time is (?:over|more than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheAverageLoadTimeIsOver(string time, string type) => LoadTestOver("average-ttl-ms", time, type);


        [Then(@"^the max(?:imum)? load time is (?:under|less than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheMaximumLoadTimeIsUnder(string time, string type) => LoadTestUnder("maximum-ttl-ms", time, type);

        [Then(@"^the max(?:imum)? load time is (?:over|more than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheMaximumLoadTimeIsOver(string time, string type) => LoadTestOver("maximum-ttl-ms", time, type);


        [Then(@"^the min(?:imum)? load time is (?:under|less than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheMinimumLoadTimeIsUnder(string time, string type) => LoadTestUnder("minimum-ttl-ms", time, type);

        [Then(@"^the min(?:imum)? load time is (?:over|more than) (?<time>\d*) ?(?<type>(milliseconds|ms|seconds|sec))$")]
        public void TheMinimumLoadTimeIsOver(string time, string type) => LoadTestOver("minimum-ttl-ms", time, type);
        #endregion

        [Then(@"^the (?<key>[^ ]+) header is (?<val>.+)$")]
        public void TheHeadersContain(string key, string val)
        {
            string tname = string.Format("The Headers Contains {0}={1}", key, val);
            Response.TestHeader(tname, key, x => x.Equals(val));
            Response.Assert(tname);
        }
        [Then(@"^the (?<key>[^\ ]+) header contains (?<val>.+)$")]
        public void TheHeadersContainsLike(string key, string val)
        {
            string tname = string.Format("The Headers Contains {0}={1}", key, val);
            Response.TestHeader(tname, key, x => x.Contains(val));
            Response.Assert(tname);
        }

        [Then(@"^the (?:response )?headers dont contain (?<key>.+)$")]
        public void TheHeadersDontContain(string key)
        {
            string tname = string.Format("The Headers Dont Contain {0}", key);
            Response.TestHeader(tname, key, s => string.IsNullOrEmpty(s));
        }

        [Then(@"^the (?:response )?body (?<condition>(equals|is not|starts with|ends with|contains)) \((?<type>(text|int|float|bool))\)(?<key>[^=]*)=(?<val>.*)$")]
        public void TheBodyHas(string condition, string type, string key, string val)
        {
            string tname = string.Format("The Response Body contains {0} {1} {2} of {3}",
                    key, condition, val, type);
            object expected = type switch
            {
                "text" => val,
                "bool" => bool.Parse(val),
                "int" => Int64.Parse(val),
                "float" => float.Parse(val),
                _ => throw new Exception("1")
            };
            object actual = Response.Retrieve(j => j.SelectToken(key));

            if (expected.GetType() != expected.GetType())
            {
                Response.TestBody(tname, j => false);
            }
            else
            {
                Response.TestBody(tname, j => condition switch {
                    "equals" => actual.Equals(expected),
                    "is not" => !actual.Equals(expected),
                    "starts with" => ((string)actual).StartsWith((string)expected),
                    "ends with" => actual.ToString().EndsWith(expected.ToString()),
                    "contains" => actual.ToString().Contains(expected.ToString()),
                });
            }

            Response.Assert(tname);
        }
        [Then(@"^the (?:response )?body (?<condition>(starts with|ends with|contains)) \((?:(array))\) (?<key>[^=]*)=(?<val>.*)$")]
        public void TheBodyHasArr(string condition, string key, string val)
        {
            string tname = string.Format("The Response Body contains {0} {1} {2} of array",
                    key, condition, val);

            string[] actual = Response.Retrieve(j => j.SelectToken(key)) as string[];

            Response.TestBody(tname, j => condition switch {
                "starts with" => actual[0].StartsWith(val),
                "ends with" => actual[actual.Length - 1].EndsWith(val),
                "contains" => new List<string>(actual).Contains(val),
                _ => throw new ArgumentException("Invalid condition")
            });

            Response.Assert(tname);
        }

        [Then(@"^the (?:response )?body contains:?((?<datatable>\|(?<key>[^\|]*)\|(?<condition>[^\|]*)\|(?<value>[^\|]*)\|(?<type>(bool|text|int|null|float))\|)+)$")]
        public void TheBodyContains(List<Dictionary<string,string>> data)
        {
            foreach(var item in data)
                TheBodyHas(item["condition"], item["type"], item["key"], item["value"]);
        }
        #endregion
    }
}
