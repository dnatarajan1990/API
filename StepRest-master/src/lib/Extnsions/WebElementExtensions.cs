using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepRest.lib.Extnsions
{
    static class WebElementExtensions
    {
        public static IWebElement ScrollTo(this IWebElement source, IWebDriver driver)
        {
            string scrollElementIntoMiddle = "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);"
                                            + "var elementTop = arguments[0].getBoundingClientRect().top;"
                                            + "window.scrollBy(0, elementTop-(viewPortHeight/2));";

            ((IJavaScriptExecutor)driver).ExecuteScript(scrollElementIntoMiddle, source);
            return source;
        }
    }
}
