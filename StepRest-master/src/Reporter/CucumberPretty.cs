using System;
using System.IO;
using System.Text;

namespace StepRest.Reporter
{
    internal class CucumberPretty : HTMLReporter
    {
        private StringBuilder sb = new StringBuilder();
        private StringBuilder bksb = new StringBuilder();
        private bool firstScenario = false;
        private bool show;
        public CucumberPretty(bool show = false) { this.show = show; }
        private int count=0;
        private int Count { get { return ++count; } }
        protected override void feature(string name,string fname)
        {
            sb = new StringBuilder()
                .Append("$(document).ready(function() {var formatter = new CucumberHTML.DOMFormatter($('.cucumber-report'));formatter.uri(\"file:").Append(fname).Append(".feature\");\n");
            sb.Append("formatter.feature({\n")
                .Append("  \"name\": \"").Append(name ?? "FEATURE").Append("\",\n")
                .Append("  \"description\": \"\",\n")
                .Append("  \"keyword\": \"Feature\",\n")
                .Append("  \"tags\": [\n")
                .Append("    {\n")
                .Append("      \"name\": \"@" + fname + "\"\n")
                .Append("    }\n")
                .Append("  ]\n")
                .Append("});\n");
        }

        protected override void background()
        {
            bksb.Append("formatter.background({\n")
                .Append("  \"name\": \"\",\n")
                .Append("  \"description\": \"\",\n")
                .Append("  \"keyword\": \"Background\"\n")
                .Append("});\n");
        }

        private void result(bool result)
        {
            var a = firstScenario ? sb : bksb;
            a.Append("formatter.result({\n")
            .Append("  \"status\": \"").Append(result ? "passed" : "failed").Append("\"\n")
            .Append("});\n");
        }
        private void result(Exception ex)
        {
            var a = firstScenario ? sb : bksb;
            a.Append("formatter.result({\n")
            .Append("  \"error_message\": \"").Append(ex.Message).Append(" <p> ").Append(ex.StackTrace.Replace(Environment.NewLine,"<br>")).Append("\",\n")
            .Append("  \"status\": \"failed\"\n")
            .Append("});\n");
        }

        protected override void scenario(string name)
        {
            if(!firstScenario) sb.Append(bksb.ToString());
            firstScenario = true;
            
            sb.Append("formatter.scenario({\n")
                .Append("  \"name\": \"").Append(name).Append("\",\n")
                .Append("  \"description\": \"\",\n")
                .Append("  \"keyword\": \"Scenario\",\n")
                .Append("  \"tags\": [\n")
                .Append("    {\n")
                .Append("      \"name\":\"[").Append(Count).Append("]\"\n")
                .Append("    }\n")
                .Append("  ]\n")
                .Append("});\n");
        }

        protected override void step(string message, bool result, Exception ex = null)
        {
            var a = firstScenario ? sb : bksb;
            string kw = message.Split(" ")[0]+" ";
            string name = message.Substring(kw.Length).Trim().Replace(":", "; ").Replace("||", "| ; |");
            a.Append("formatter.step({\n")
                .Append("  \"name\": \"").Append(name).Append("\",\n")
                .Append("  \"keyword\": \"").Append(kw).Append("\"\n")
                .Append("});\n");
            this.match();
            if (ex == null) this.result(result);
            else this.result(ex);
        }

        private void match()
        {
            var a = firstScenario ? sb : bksb;
            a.Append("formatter.match({\n")
                .Append("  \"location\": \"Runner\"\n")
                .Append("});\n");
        }

        protected override void write(bool result = false)
        {
            sb.Append("});\n");

            using (StreamWriter swriter = new StreamWriter(Output))
                swriter.Write(sb.ToString());

            String filePath = string.Format("{1}Reporter{0}output{0}index.html",
                Path.DirectorySeparatorChar.ToString(), Program.Options.Dir("src"));

            new ConsoleReporter().Warn(result, "Test Results saved "+filePath);

            if (show) System.Diagnostics.Process.Start(@"cmd.exe ", @"/c " + filePath);
        }
    }
}
