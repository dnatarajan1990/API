using RA.Exceptions;
using StepRest.Reporter;
using StepRest.Runner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace StepRest
{
    public class Feature
    {
        private string _name;
        public IRunner Runner;
        private IReporter Reporter;
        private bool failLock = true;
        public bool Result { get; private set; }

        internal Feature()
        {
            Reporter = Program.Options.Reporter;
            Runner = Program.Options.Runner;
            List<string> fileAsList = new List<string>(File.ReadAllLines(Program.Options.File));

            Result = Process(fileAsList);
        }

        public Feature(List<string> feature, IRunner runner, IReporter reporter)
        {
            Runner = runner;
            Reporter = reporter;

            Result = Process(feature);
        }

        private bool Process(List<string> fileAsList)
        {
            //Process File
            bool bkgdFound = false;
            for (int i = 0; i < fileAsList.Count; i++)
            {
                string L = fileAsList[i].Trim();
                string l = L.ToLower();
                if (l.Equals("") || l.StartsWith("#")) continue;

                #region Actions
                Action feature = delegate ()
                {
                    _name = string.Format("[TestSet]_" + l.Substring(l.IndexOf(":") + 1).Trim().Replace(" ", "_"));
                    if (Reporter != null) Reporter.NewFeature(_name, _name);
                };
                Action def = delegate ()
                {
                    throw new ArgumentException(message: "Invalid type value", fileAsList[i].Trim().Split(" ")[0]);
                };
                Action ParseCommands = delegate ()
                {
                    bool failSwitch = false;
                    for (; i < fileAsList.Count; i++)
                    {
                        L = fileAsList[i].Trim();
                        foreach (Match m in Regex.Matches(L, "(?:.*?)(?<g><(?<v>[^>]*)>)*(?:.*?)"))
                            for (int i = 0; i < m.Groups[@"g"].Captures.Count; i++)
                                L = L.Replace("<" + m.Groups["v"].Captures[i].Value + ">", Program.Options[m.Groups["v"].Captures[i].Value]);
                        l = L.ToLower();
                        if (l.Equals("") || l.StartsWith("#")) continue;
                        if (l.StartsWith("scenario")) { if (bkgdFound) { i--; } break; }
                        while (fileAsList.Count - 1 > i && fileAsList[i + 1].Trim().StartsWith("|"))
                        {
                            L += fileAsList[i + 1].Trim();
                            i++;
                        }
                        try
                        {
                            Runner.Run(L);
                            if(Reporter !=null) Reporter.Step(L, true);
                        }
                        catch (AssertException e)
                        {
                            if (Reporter != null) Reporter.Step(L, false, e);
                            failSwitch = true;
                        }
                    }
                    if (bkgdFound && Reporter != null) Reporter.EndScenario(!failSwitch);
                    else if (!bkgdFound && failSwitch)
                    {
                        Reporter.Error("Failed in building background case.");
                        throw new Exception("Failed during Build Steps");
                    }
                    if (failSwitch) failLock = false;
                };
                Action background = delegate ()
                {
                    Runner = Runner.Refresh();
                    if (Reporter != null) Reporter.Background();
                    i++;
                    ParseCommands();
                    i--;
                    Runner.SaveBackground();
                };
                Action scenario = delegate ()
                {
                    if (Reporter != null) Reporter.Scenario(L.Contains(":") ? L.Substring(L.IndexOf(":") + 1).Trim() : null);
                    bkgdFound = true;
                    Runner = Runner.Refresh();
                    i++;
                    ParseCommands();
                };
                Action variables = delegate ()
                {
                    i++;
                    for (; i < fileAsList.Count; i++)
                    {
                        L = fileAsList[i].Trim();
                        l = L.ToLower();
                        if (l.Equals("") || l.StartsWith("#")) continue;
                        if (l.StartsWith("scenario") || l.StartsWith("background")) { break; }
                        string[] pair = L.Split("=");
                        
                        Program.Options[pair[0]] = pair.Length == 2 
                            ? pair[1] : pair.Length == 1 
                            ? "" : throw new Exception("Invalid Format for vaiables [use \"foo-bar\"]");
                    }
                    i--;
                    Runner.SaveBackground();
                };
                #endregion

                switch (l.Split(":")[0].ToLower())
                {
                    case "":
                    case "#":
                        continue;
                    case "feature": feature(); break;
                    case "background": background(); break;
                    case "scenario": scenario(); break;
                    case "variables": variables(); break;
                    case "@": 
                        Program.Options.Modify(L.Split(":")[1].Split(" "));
                        Reporter = Program.Options.Reporter;
                        Runner = Program.Options.Runner;
                        break;
                    default: def(); break;
                }
            }
            if (Reporter != null) Reporter.EndFeature(failLock);
            return failLock;
        }
    }
}