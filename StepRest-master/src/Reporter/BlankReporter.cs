using System;
using System.Collections.Generic;
using System.Text;

namespace StepRest.Reporter
{
    class BlankReporter : IReporter
    {
        public void Background() { }

        public void EndFeature(bool failures) { }

        public void EndScenario(bool result) { }

        public void Error(string message, params string[] args) { }

        public void NewFeature(string name, string fname) { }

        public void Scenario(string name = "SCENARIO") { }

        public void Step(string message, bool result, Exception ex = null) { }

        public void Trace(string message, params string[] args) { }

        public void Warn(bool op, string message, params string[] args) { }
    }
}
