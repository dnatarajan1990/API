using System;
using System.Collections.Generic;
using System.Text;

namespace StepRest.Reporter
{
    public class ConsoleReporter : IReporter
    {
        public void Warn(string message, params string[] args) => Warn(false, message, args);
        public void Warn(bool b, string message, params string[] args)
        {
            Console.BackgroundColor = b ? ConsoleColor.Green : ConsoleColor.Red;
            if (b) Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("\n"+message, args);
            Console.ResetColor();
        }

        public void Trace(string message, params string[] args)
        {
            Console.Write("\n"+message, args);
        }

        public void Error(string message, params string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("\n"+message, args);
            Console.ResetColor();
        }

        public void NewFeature(string name, string fname)
        {
            Console.Title = string.Format("Test case: {0}", name ?? fname ?? "FEATURE");
        }

        public void Scenario(string name)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write("\n" + name ?? "SCENARIO:");
            Console.ResetColor();
        }

        public void Step(string step, bool result, Exception ex = null)
        {
            Console.BackgroundColor = result ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("\n\t{0}",result?"PASS":"FAIL");
            Console.ResetColor();
            Console.Write("\t{0}{1}", step.StartsWith("And") ? "\t":"", step.Replace("||","|\n\t\t\t|")); ;
        }

        public void EndScenario(bool result)
        {
            Console.BackgroundColor = result ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("\n\t --- --- {0} --- ---", result ? "PASS" : "FAIL");
            Console.ResetColor();
        }

        public void Background()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write("\n BACKGROUND:");
            Console.ResetColor();
        }

        public void Write() { /*Not needed for console.*/ }

        public void EndFeature(bool failures)
        {
            Console.BackgroundColor = !failures ? ConsoleColor.Red : ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("\n\n --- --- {0} --- ---", !failures ? "SOME FAIL" : "ALL PASS");
            Console.ResetColor();
        }
    }
}
