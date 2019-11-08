namespace StepRest.Reporter
{
    internal abstract class HTMLReporter : IReporter
    {
        protected string Output;
        protected HTMLReporter()
        {
            Output = Program.Options.Dir("src")+"\\reporter\\output\\report.js";
        }

        protected abstract void feature(string name, string fname);
        protected abstract void background();
        protected abstract void scenario(string name);
        protected abstract void step(string message, bool result, System.Exception ex = null);
        protected abstract void write(bool result = false);


        public void Warn(bool op, string message, params string[] args)
            => new ConsoleReporter().Warn(op, message, args);
        public void Trace(string message, params string[] args)
            => new ConsoleReporter().Trace(message, args);
        public void Error(string message, params string[] args)
            => new ConsoleReporter().Error(message, args);

        public void NewFeature(string name, string fname)
            => feature(name.Replace("\"","\\\""),fname);
        public void Background()
            => background();
        public void Scenario(string name)
            => scenario(name.Replace("\"", "\\\""));
        public void Step(string name, bool result, System.Exception ex)
            => step(name.Replace("\"", "\\\""), result,ex);
        public void EndScenario(bool result) { }

        public void EndFeature() => write(false);
        public void EndFeature(bool result) => write(result);
    }
}