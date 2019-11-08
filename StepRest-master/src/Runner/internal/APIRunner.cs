using RA;
using System;

namespace StepRest.Runner
{

    public abstract class APIRunner : IRunner
    {
        private static SetupContext _background = null;
        private static int count = 1;
        protected int Count { get { return count++; } }


        private SetupContext _setup;
        internal SetupContext Setup { get { return _setup; } }
        private HttpActionContext _action;
        internal HttpActionContext Action
        {
            get
            {
                if (_action == null)
                    _action = _setup.When();
                return _action;
            }
        }
        private ExecutionContext _execution;
        internal ExecutionContext Execution { get { return _execution; } set { _execution = value; } }
        private ResponseContext _response;
        internal ResponseContext Response
        {
            get
            {
                if (_response == null)
                    _response = _execution.Then();
                return _response;
            }
        }

        /// <summary>
        /// Constructs new Runner using backup if stored.
        /// </summary>
        public APIRunner() => _setup = _background switch
        {
            null => new RestAssured().Given(),
            _ => _background.Clone(),
        };

        void IRunner.SaveBackground() =>
            _background = _setup;

        protected void LoadTestUnder(string key, string num, string type = "m")
        {
            int c = type.StartsWith("m") ? int.Parse(num) : int.Parse(num) * 1000;
            string testName = LoadTestName(key, c, false);
            Response.TestLoad(testName, key, x => x < c);
            Response.Assert(testName);
        }
        protected void LoadTestOver(string key, string num, string type = "m")
        {
            int c = type.StartsWith("m") ? int.Parse(num) : int.Parse(num) * 1000;
            string testName = LoadTestName(key, c, true);
            Response.TestLoad(testName, key, x => x > c);
            Response.Assert(testName);
        }
        private string LoadTestName(string key, int val, bool over) => key switch
        {
            "total-succeeded" => string.Format("{1} {0} requests in load completed successfully", val, over ? "Over" : "Under"),
            "total-lost" => string.Format("{1} {0} requests in load failed", val, over ? "Over" : "Under"),
            "success-percent" => string.Format("The Success Percentage was {1} {0}%", val, over ? "over" : "under"),
            "average-ttl-ms" => string.Format("The Average Load Time is {1} than {0} milliseconds", val, over ? "more" : "less"),
            "maximum-ttl-ms" => string.Format("The Maximum Request time for Load Time is {1} {0} milliseconds", val, over ? "over" : "under"),
            "minimum-ttl-ms" => string.Format("The Minimum Request time for Load Time is {1} {0} milliseconds", val, over ? "over" : "under"),
            _ => throw new ArgumentException("Load key type not recognised", key),
        };


        [Then(@"^debug$")]
        public void TheResponseCodeIs()
        {
            _setup.Debug();
        }

        protected abstract IRunner Refresh();
        IRunner IRunner.Refresh() => Refresh();
    }
}
