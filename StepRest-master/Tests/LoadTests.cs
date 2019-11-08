using NUnit.Framework;
using StepRest;
using StepRest.Reporter;
using StepRest.Runner;
using System.Collections.Generic;

namespace StepRest.Test
{
    public class LoadTests
    {
        #region Setup
        IRunner runner;
        IReporter reporter;

        List<string> bkGET = new List<string>()
            {
                {"Feature: Testing"},
                {"Background:"},
                    {"Given a base uri https://postman-echo.com"},
                    {"Given a base path get"},
                    {"Given a base port 80"},
                    {"Given a query of arg1=val1"},
                    {"Given queries:"},
                        {"|arg2|val2|"},
                        {"|arg3|val3|"},
                    {"Given a header of head1=val1"},
                    {"Given headers:"},
                        {"|head2|val2|"},
                        {"|head3|val3|"}
        };
        List<string> bkPOST = new List<string>()
            {
                {"Feature: Testing"},
                {"Background:"},
                    {"Given a base uri https://postman-echo.com"},
                    {"Given a base path post"},
                    {"Given a base port 80"},
                    {"Given a query of arg1=val1"},
                    {"Given queries:"},
                        {"|arg2|val2|"},
                        {"|arg3|val3|"},
                    {"Given a header of head1=val1"},
                    {"Given headers:"},
                        {"|head2|val2|"},
                        {"|head3|val3|"},
                    {"Given a parameter of param1=val1"},
                    {"Given parameters:"},
                        {"|param2|val2|"},
                        {"|param3|val3|"}
        };

        [SetUp]
        public void Setup()
        {
            runner = new DefaultAPIRunner();
            reporter = new ConsoleReporter();
        }
        #endregion

        /// <summary>
        /// If this fails, all further tests will fail and shows something is 
        /// wrong, probably with API endpoints used in tests.
        /// </summary>
        [Test]
        public void BakgroundsAreValid()
        {
            new Feature(bkGET, runner, null);
            new Feature(bkPOST, runner, null);
        }

        [Test]
        public void ValidGET()
        {
            List<string> lst = new List<string>(bkGET)
            {
                {"Scenario: Valid GET Test"},
                {"When the system loads 2 threads for 30 sec" },
                    {"And the system requests GET"},
                {"Then the total successfull load requests is over 50"},
                    {"And the total failed load requests is under 5"},
                    {"And the success percentage is over 95%"},
                    {"And the average load time is under 500 ms"},
                    {"And the max load time is under 2sec"},
                    {"And the min load time is under 300 milliseconds"}
            };

            Feature feature = new Feature(lst, runner, null);
        }
    }
}