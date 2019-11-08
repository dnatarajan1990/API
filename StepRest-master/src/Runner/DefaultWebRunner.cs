using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Opera;
using RA.Exceptions;
using StepRest.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StepRest.Runner
{
    class DefaultWebRunner : WebRunner
    {
        private Dictionary<string, IWebElement> _found;
        IWebElement lastFound = null;

        protected void GivenBrowser(string browser)
        {
            try
            {
                Driver = browser switch
                {
                    "chrome" => _chrome(),
                    "firefox" => _firefox(),
                    "edge" => _edge(),
                    "ie" => _ie(),
                    "opera" => _opera(),
                    "headless" => _headless(),
                    _ => throw new ArgumentException("Unsupported type [" + browser + "]", nameof(browser))
                };
            } catch (ArgumentException ae) { throw ae; }
            catch(Exception)
            {
                throw new ConfigException("Could not open Web browser\n\tCheck that the driver version matches the local installed version.");
            }
        }
        #region BrowserSettings
        protected virtual IWebDriver _chrome()
        {
            var service = ChromeDriverService.CreateDefaultService(Program.Options.Dir("drivers") + DriverExtension);
            var options = new ChromeOptions();

            options.AddArgument("--log-level=3");
            service.SuppressInitialDiagnosticInformation = true;

            return new ChromeDriver(service, options);
        }
        protected virtual IWebDriver _firefox()
        {
            var service = FirefoxDriverService.CreateDefaultService(Program.Options.Dir("drivers") + DriverExtension);
            var options = new FirefoxOptions();

            service.SuppressInitialDiagnosticInformation = true;
            options.SetLoggingPreference(LogType.Driver, LogLevel.Off);
            options.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            options.SetLoggingPreference(LogType.Client, LogLevel.Off);

            return new FirefoxDriver(service,options);
        }
        protected virtual IWebDriver _edge()
        {
            return new EdgeDriver(Program.Options.Dir("drivers") + DriverExtension);
        }
        protected virtual IWebDriver _ie()
        {
            var service = InternetExplorerDriverService.CreateDefaultService(Program.Options.Dir("drivers") + DriverExtension);
            var options = new InternetExplorerOptions();

            service.SuppressInitialDiagnosticInformation = true;
            options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;

            return new InternetExplorerDriver(service, options);
        }
        protected virtual IWebDriver _opera()
        {
            return new OperaDriver(Program.Options.Dir("drivers") + DriverExtension);
        }
        protected virtual IWebDriver _headless()
        {
            var service = ChromeDriverService.CreateDefaultService(Program.Options.Dir("drivers") + DriverExtension);
            var options = new ChromeOptions();

            service.SuppressInitialDiagnosticInformation = true;
            options.AddArgument("--log-level=3");
            options.AddArguments("headless");

            return new ChromeDriver(service, options);
        }
        #endregion

        protected void GivenOperatingSystem(string os)
            => DriverExtension = os;


        protected void GoToUrl(string url)
        {
            _found = new Dictionary<string, IWebElement>();
            Driver.Navigate().GoToUrl(url);
        }

        protected IWebElement ScrollTo(IWebElement element)
        {
            Actions actions = new Actions(Driver);
            actions.MoveToElement(element);
            actions.Perform();
            return element;
        }

        protected IWebElement FindBy(string op, string search)
        {
            if(op.Equals("id") && _found.ContainsKey(search))
            {
                lastFound = _found[search];
            } else {
                IWebElement item = Driver.FindElement(op switch
                {
                    "id" => By.Id(search),
                    "name" => By.Name(search),
                    "css" => By.CssSelector(search),
                    "xpath" => By.XPath(search),
                    _ => throw new ArgumentException("Unsupported type [" + op + "]", nameof(op))
                }); ;
                if (item == null) throw new AssertException(string.Format("Finding item {0}={1} by", op, search));
                string itemID = item.GetAttribute("id");
                if (itemID != null) _found.Add(itemID, item);
                lastFound = item;
            }
            return lastFound;
        }


        protected void Close()
        {
            Driver.Close();
        }

        protected override IRunner Refresh()
            => new DefaultWebRunner();

        
    }
}
