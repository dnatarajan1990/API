namespace StepRest.Reporter
{
    public interface IReporter
    {
        public abstract void Warn(bool op, string message, params string[] args);
        public abstract void Trace(string message, params string[] args);
        public abstract void Error(string message, params string[] args);
        public abstract void Scenario(string name = "SCENARIO");
        public abstract void Background();
        public abstract void Step(string message, bool result, System.Exception ex=null);
        public abstract void EndScenario(bool result);
        public abstract void NewFeature(string name, string fname);
        public abstract void EndFeature(bool failures);
    }
}