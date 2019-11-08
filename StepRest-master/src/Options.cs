using StepRest.Runner;
using StepRest.Reporter;
using StepRest.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace StepRest
{
    internal class Options
    {
        private static string heading = null;

        private static readonly string Usage = string.Format(
            "Normal Usage: StepRestTest.exe <args> <test>\n\n" +
            "ARGS: full\t[short]\tdescription\n" +
            "\t-{1}\t[-{0}]\t{2}\n" +
            "\t-{4}\t[-{3}]\t{5}\n" +
            "\t-{7}\t[-{6}]\t{8}\n" +
            "\t-{10}\t[-{9}]\t{11}\n" +
            "\t-{13}\t[-{12}]\t{14}\n" +
            "\t-{14}\t[-{15}]\t{17}\n\n" +
            "TEST\n" +
            "\t{18}\n\t\t{19}\n" +
            "\t{20}\n\t\t{21}\n",
            "c", "console", "Write results to console (default)",
            "h", "html",    "Write results to html report",
            "d", "display", "Write results to html and open report on completion",
            "a", "api",     "Use default API runner (default)",
            "f", "file", "Use supplied feature file",
            "w", "web", "Use example web runner (google)",

            "If File argument not set:", "Name of file inside features directory. Ie \"example\"",
            "If File argument is set",   "Location of feature file relative to path of StepRestTest.exe"
            );

        private void Arguments(string arg)
        {
            switch (arg)
            {
                case "console":
                case "c":
                    Reporter = new ConsoleReporter();
                    break;
                case "html":
                case "h":
                    Reporter = new CucumberPretty();
                    break;
                case "display":
                case "d":
                    Reporter = new CucumberPretty(true);
                    break;
                case "api":
                case "a":
                    Runner = new DefaultAPIRunner();
                    break;
                case "file":
                case "f":
                    File = file;
                    break;
                case "web":
                case "w":
                    Runner = new DefaultWebRunner();
                    break;
                default:
                    try
                    {
                        Type[] foundClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                            String.Equals(t.Namespace, "StepRest.Runner", StringComparison.Ordinal) &&
                            String.Equals(t.Name, arg, StringComparison.Ordinal)).ToArray();


                        if (foundClasses.Length != 1)
                        {
                            Reporter.Warn(false,"Couldn't find class ["+arg+"]");
                            throw new Exception();
                        }
                        Reporter.Warn(true, "Found class [" + arg + "]");

                        ConstructorInfo ctor = foundClasses[0].GetConstructor(Type.EmptyTypes);
                        Reporter.Warn(true, "Found constructor [" + ctor.Name + "]");
                        Runner = (IRunner)ctor.Invoke(new Object[0]);
                        Reporter.Warn(true, Runner.GetType().Name);

                        break;
                    } catch
                    {
                        Console.WriteLine(Usage);
                        throw new ArgumentException("Invalid Option \"" + arg + "\"", paramName: arg);
                    }
            }
        }


        internal IRunner Runner;
        internal IReporter Reporter;
        private Dictionary<string, string> dirs = new Dictionary<string, string>();
        private Dictionary<string, string> data = new Dictionary<string, string>();
        public string File { get; internal set; }
        private string file;

        public string this[string key]
        {
            get
            {
                if (!data.ContainsKey(key))
                    return null;
                return data[key];
            }
            set => data[key] = value;
        }

        public string Dir(string key)
        {
            if (!dirs.ContainsKey(key))
                dirs.Add(key, FindFolder(key));
            return dirs[key];
        }
        public void Dir(string key,string value)
        {
            if (!dirs.ContainsKey(key))
                dirs.Add(key, value);
            else
                dirs[key] = value;
        }

        private string FindFolder(string pname)
        {
            string path = Environment.CurrentDirectory;
            while (!Directory.Exists(path + @"\" + pname))
                path = Directory.GetParent(path).FullName;
            path = path + @"\" + pname + @"\";
            return path;
        }

        internal Options()
        {
            Runner = new DefaultAPIRunner();
            Reporter = new BlankReporter();
        }

        internal void Modify(string[] args)
        {
            foreach (var arg in args)
                if (arg.StartsWith("-"))
                {
                    Reporter.Warn(true, string.Format("\tModification:{0}", arg.Substring(1)));
                    Arguments(arg.Substring(1));
                }
        }

        internal void ParseArgs(string[] args)
        {
            File = null;
            file = args[args.Length - 1];
            for (int i = 0; i < args.Length - 1; i++) args[i] = !args[i].StartsWith("-") ? "-" + args[i] : args[i];
            foreach (var arg in args)
                if (arg.StartsWith("-"))
                    Arguments(arg.Substring(1));
            string head = string.Format("Started with {0} reporting to {1}",
                Runner.GetType().ToString().SubStringToLast("."),
                Reporter.GetType().ToString().SubStringToLast("."));
            if(heading==null || !heading.Equals(head))
            {
                heading = head;
                Reporter.Warn(true, head);
            }
            if(File==null)
            {
                string path = Program.Options.Dir("Features") + file.Replace("_", Path.DirectorySeparatorChar.ToString()) + ".feature";
                if (!System.IO.File.Exists(path)) throw new FileNotFoundException("Could not locate " + file + " file", path);
                File = path;
            }
        }
    }
}
