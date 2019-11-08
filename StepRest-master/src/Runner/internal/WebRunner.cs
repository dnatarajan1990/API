using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace StepRest.Runner
{
    public abstract class WebRunner : IRunner
    {
        public struct Background
        {
            public IWebDriver driver;
            
            public void CloneTo(WebRunner wr)
            {
                wr.Driver = driver;
            }
        }
        private static Background background;
        internal IWebDriver Driver { get; set; }
        internal string DriverExtension { get; set; } = "win64";

        public WebRunner()
        {
            //Console.WriteLine("WebRunner created {0} background", background.Equals(default(Background)) ? "without" : "with");
            if (!background.Equals(default(Background))) 
                background.CloneTo(this);
        }

        protected abstract IRunner Refresh();
        IRunner IRunner.Refresh() => Refresh();

        void IRunner.SaveBackground()
        {
            background = new Background();
            background.driver = Driver;
        }
    }
}