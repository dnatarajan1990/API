using NUnit.Framework;
using StepRest.Reporter;
using StepRest.Runner;
using System.Collections.Generic;

namespace StepRest.Test
{
    public class BasicTests
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
                    {"When the system requests GET"},
                    {"Then the response code is 200"},
                    {"And the response time is over 1ms"},
                    {"And the response time is under 10 sec"}
            };

            Feature feature = new Feature(lst, runner, null);
        }
        [Test]
        public void InvalidPOST()
        {
            List<string> lst = new List<string>(bkGET)
            {
                {"Scenario: Invalid POST Test"},
                    {"When the system requests POST"},
                    {"Then the response code is 404"},
                    {"And the response time is over 1ms"}
            };

            Feature feature = new Feature(lst, runner, null);
        }
        [Test]
        public void ValidPOST()
        {
            List<string> lst = new List<string>(bkPOST)
            {
                {"Scenario: Valid POST Test"},
                    {"When the system requests POST"},
                    {"Then the response code is 200"},
                    {"And the response time is over 1ms"},
                    {"And the response time is less than 10 sec"}
            };

            Feature feature = new Feature(lst, runner, null);
        }
        [Test]
        public void InvalidGET()
        {
            List<string> lst = new List<string>(bkPOST)
            {
                {"Scenario: Invalid GET Test"},
                    {"When the system requests GET"},
                    {"Then the response code starts with 4"},
                    {"And the response time is more than 1ms"},
                    {"And the response time is under 10 sec"}
            };

            Feature feature = new Feature(lst, runner, null);
        }

        [Test]
        public void TestBodyTypes()
        {
            List<string> lst = new List<string>()
            {
                {"Given a base uri http://jsonplaceholder.typicode.com"},
                {"And a base path todos"},
                {"And a port 80"},
                {"Scenario: Valid type testsing"},
                {"When the system requests GET 1"},
                {"Then the body equals(int)userId = 1"},
                {"And the body equals(int)id = 1"},
                {"And the body equals(text)title = delectus aut autem"},
                {"And the body equals(bool)completed = false"}
            };
            Feature feature = new Feature(lst, runner, null);
        }
    }
}