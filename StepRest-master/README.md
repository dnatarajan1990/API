# StepRest
StepRest is a testing framework written in C# using .NET core. It uses a gherkin like syntax and RestAssured (for API testing) or selenium for web front end testing. It is able to write reports to the console output, file or HTML report and it built to be extendable.

## Usage
There is a built version in the build folder. You can run some example API outputs by navigating to the directory containing StepRest.exe and running `StepRest.exe -console example`. This will run the tests in features/example.feature and print results to the console.

Running a feature requires a runner, a reporter and a feature file. Settings given inside the feature file will be used first, with console options then defaults being used if they are not present.

### Runner
The runner contains the logic for processing commands. By default this will be the DefaultAPIRunner. A DefaultWebRunner exists as well, see **Selenium** below.

### Reporter
The reporter describes how the results should be displayed.
- BlankReporter *\[default]*
&nbsp;&nbsp;&nbsp;&nbsp;No output. Program will exit with code 0 if all tests passed, 1 if any failed and 2 if there was an error running test.
- ConsoleReporter
&nbsp;&nbsp;&nbsp;&nbsp;Output to console, with results colored.
- HTMLReporter
&nbsp;&nbsp;&nbsp;&nbsp;Create a HTML report.

### Feature
Feature files contain the details for your tests. This is the last argument passed to the program. For example to run the feature file in features/example.feature you would use `StepRest.exe <options> example`. Features can be segmented using folders, then using an underscore for a directory. For example to run the feature file in features/tests/foo.feature you would use `StepRest.exe <options> tests_foo`.

## Building Feature Files
Feature files are seperated into sections that must appear in order. White space before and after text, as well as blank lines are ignored and a line starting with # is ignored as a comment. Lines that start with a | continue on from the last line.

### Header Options (*optional* )
Header options give console arguments that will always be used for that feature file. This section starts with @: followed by any arguments seperated by a space. For example `@:-api -console` will use the DefaultAPIRunner and the ConsoleReporter and ignore console arguments when run. The Runner can also be given by giving the name of the Runner object (see **Building Runners** below), for example `@:-DefaultAPIRunner` would use the DefaultAPIRunner and the Reporter given either by console command or default.

### Feature Name (*optional* )
Feature name is given by `Feature: <featurename>`

### Variables (*optional* )
These are variables for the tests in that feature file to use. They take the form of:
```
Variables:
    key=value
    foo=var
```
These variables can then be used anywhere in the feature file by using `<key>` ie `<foo>` would be replaced by `var` elsewhere in the same feature file.

### Background
This section gives details for all tests in this feature file to use. For example you could give the host, port and path to an endpoint here. Any 'Given' step may be used here however (see **Steps** below). it takes the format of `Background:` followed by Given steps on seperate lines.

### Scenario (*multiple* )
A feature file can have as many scenarios as it wants. These will start with the Given steps in background, then proceed to steps in that scenario (see **Steps** below). it takes the format of `Scenario:` followed by Given steps on seperate lines.

## Steps
Steps are one of three types, differentated by the first word. And may be used in place of the first word if it is the same type as the step above. Given steps do setup tasks, When steps take actions and Then steps assert tests. See **Selenium** below if you are testing web tools.

### DefaultAPITester
#### Given Steps
|  Regex | Example  | Description  |
| ------------ | ------------ | ------------ |
| a (?:base )?uri (?<uri>.\*)  | Given a base uri https://postman-echo.com  |  Define host |
| a (?:base )?path (?<path>.\*)  | Given a base path https://postman-echo.com  |  Define path |
| a (?:base )?body (?<body>.\*)  | Given a base body {id:1}  |  Define body |
| a (?:base )?port (?<port>\d\*)  | Given a base port 8080  |  Define port |
| a query of (?<key>[^=]\*)=(?<val>.\*)  |  Given a query of foo=bar |  Define query |
| queries:?((?<keyvaluepair>\|(?<key>[^\|]\*)\|(?<value>[^\|]\*)\|)+)  |  Given queries:<br>&nbsp;&nbsp;\|arg1\|val1\|<br>&nbsp;&nbsp;\|arg2\|va21\| |  Define multiple queries |
| a parameter of (?<key>[^=]\*)=(?<val>.\*)  |  Given a parameter of foo=bar |  Define parameter |
| parameters:?((?<dictionary>\|(?<key>[^\|]\*)\|(?<value>[^\|]\*)\|)+)  |  Given parameters:<br>&nbsp;&nbsp;\|arg1\|val1\|<br>&nbsp;&nbsp;\|arg2\|va21\| |  Define multiple parameters |
| a header of (?<key>[^=]\*)=(?<val>.\*)  |  Given a header of foo=bar |  Define header |
| headers:?((?<dictionary>\|(?<key>[^\|]\*)\|(?<value>[^\|]\*)\|)+)  |  Given headers:<br>&nbsp;&nbsp;\|arg1\|val1\|<br>&nbsp;&nbsp;\|arg2\|va21\| |  Define multiple headers |

#### When Steps
|  Regex | Example  | Description  |
| ------------ | ------------ | ------------ |
| the (?:(system\|user)) requests (?<method>(GET\|POST\|PUT\|PATCH\|DELETE)) ?(?<path>.\*)?  | When the system requests GET path  |  Issue request with method and port (if supplied) |
| the system loads (?<threads>\d\*) threads for (?<time>\d\*) ?(?<op>sec\|second\|min\|minute)s?  | When the system loads 5 threads for 2 min  |  Begin Load test (see **Load Testing** below) |

#### Then Steps
|  Regex | Example  | Description  |
| ------------ | ------------ | ------------ |
| the (?:response )?code is (?<code>\d\*)  | Then the response code is 200  |  Checks the response code is as expected |
| the (?:response )?code starts with (?<code>[1,2,3,4,5]{1})  | Then the code starts with 5  | Checks the response code is of the expected type  |
| the (?:response )?time is (?:under\|less than) (?<time>\d\*) ?(?<type>(millisecond\|ms\|second\|sec)s?)  | Then the time is less than 500ms  | Checks that the response time is under the given amount.  |
| the (?:response )?time is (?:over\|more than) (?<time>\d\*) ?(?<type>(milliseconds\|ms\|seconds\|sec))  | Then the time is more than 100ms  | Checks that the response time is over the given amount. |
| the (?<key>[^ ]+) header is (?<val>.+)  |  Then the Foo header is Bar | Checks that the given header catches the given value  |
| the (?<key>[^\ ]+) header contains (?<val>.+)  | Then the Content-Type header contains json  | Checks that the given value is contained in the given header.  |
| the (?:response )?headers dont contain (?<key>.+)  | Then the response headers dont contain FAKEKEY  | Checks that the given key doesn't exist in response.  |
| the (?:response )?body (?<condition>(equals\|is not\|starts with\|ends with\|contains)) \((?<type>(text\|int\|float\|bool))\)(?<key>[^=]*)=(?<val>.*)  | Then the body equals (text)args.arg1=val1   | **Depreciated** Compares body elements.   |
| the (?:response )?body (?<condition>(starts with\|ends with\|contains)) \((?:(array))\) (?<key>[^=]\*)=(?<val>.\*)  | Then the body starts with (array)args=val1  | **Depreciated** comparing body arrays  |
| the (?:response )?body contains:?((?<datatable>\\\|(?<key>[^\\\|]*)\\\|(?<condition>[^\\\|]*)\\\|(?<value>[^\\\|]\*)\\\|(?<type>(bool\|text\|int\|float))\\\|)+)  | Then the body contains:<br>&nbsp;&nbsp;\|arg1\|starts with\|foo\|text\|<br>&nbsp;&nbsp;\|arg2\|equals\|bar\|text\|  | Comparing body values  |

##### Load Testing Then Steps
|  Regex | Example  | Description  |
| ------------ | ------------ | ------------ |
| the total successfull loads? requests? (?:is\|are) (?:under\|less than) (?<amount>\d\*)  | Then the total successfull load requests is under 1000  | Checks the total number of successfull requests in load is under given value  |
| the total successfull loads? requests? (?:is\|are) (?:over\|more than) (?<amount>\d\*)  | Then the total successfull load requests is more than 500  |  Checks the total number of successfull requests in load is over given value |
| the total failed loads? requests? (?:is\|are) (?:under\|less than) (?<amount>\d\*)  | Then the total failed load requests is under 1000  | Checks the total number of failed requests in load is under given value  |
| the total failed loads? requests? (?:is\|are) (?:over\|more than) (?<amount>\d\*)  | Then the total failed load requests is more than 500  |  Checks the total number of failed requests in load is over given value |
| the success percentage is (?:under\|less than) (?<amount>\d\*)\%  |  Then the success percentage is under 100\% |  Checks that the success percentage is under the given percentage |
| the success percentage is (?:over\|more than) (?<amount>\d\*)\%  | Then the success percentage is under 95\%  | Checks that the success percentage is over the given percentage  |
| the average load time is (?:under\|less than) (?<time>\d\*) ?(?<type>(millisecond\|ms\|second\|sec)s?)  | Then the average load time is under 1000ms  | Checks that the avergae load time is under the given amount of time.  |
| the average load time is (?:over|more than) (?<time>\d\*) ?(?<type>(millisecond\|ms\|second\|sec)s?)  | Then the average load time is over 1sec  | Checks that the avergae load time is over the given amount of time.  |
| the max(?:imum)? load time is (?:under\|less than) (?<time>\d\*) ?(?<type>(millisecond\|ms\|second\|sec)s?)  | Then the max load time is under 1 second  | Checks that the maximum load time is under the given amount of time.  |
| the max(?:imum)? load time is (?:over|more than) (?<time>\d\*) ?(?<type>(millisecond\|ms\|second\|sec)s?)  | Then the maximum load time is over 1500 seconds  | Checks that the maximum load time is over the given amount of time.  |
| the min(?:imum)? load time is (?:under\|less than) (?<time>\d\*) ?(?<type>(millisecond\|ms\|second\|sec)s?)  | Then the min load time is under 1 second  | Checks that the minimum load time is under the given amount of time.  |
| the min(?:imum)? load time is (?:over|more than) (?<time>\d\*) ?(?<type>(millisecond\|ms\|second\|sec)s?)  | Then the minimum load time is over 1500 seconds  | Checks that the minimum load time is over the given amount of time.  |

### Examples
```csharp
@:-api
Feature: Echo

  Variables:
	foo1=bar1
	foo2=bar2
	
  Background:
    Given a base uri https://postman-echo.com
		And a base path get
		And queries:
			|foo1|<foo1>|
			|foo2|<foo2>|
		And a port 80
	
  Scenario: Test
    When the system requests GET
    Then the response code is 200
		And the Content-Type header contains json
		And the response time is under 2 sec
		And the body contains:
			|args.foo1|equals|<foo1>|text|
			|args.foo2|equals|<foo2>|text|
```

### Selenium
This time running tests requires a little more coding, however StepRest does contain tools to make this easier.

    Feature: Testing
    
	@:-BinaryConverterRunner
    Background:
    	Given a chrome browser
    	
    Scenario: Testing text-bin convert
    	When i go to text to binary page
    	And i enter "." in text textbox
    	And i click convert
    	Then the binary box shows "00101110"
    
    Scenario: Testing bin-text convert
    	When i go to binary to text page
    	And i enter "00101110" in binary textbox
    	And i click convert
    	Then the text box shows "."
    Then close browser
This time a lot more of the information is hidden, so lets look at the relative step definitions:

            [When(regex: @"^i go to binary to text page$")]
            public void GoToBinToText()
                => GoToUrl("https://www.rapidtables.com/convert/number/binary-to-ascii.html");
    
            [When(regex: @"^i enter ""(?<input>[^""]+)"" in binary textbox$")]
            public void EnterIntoBinaryTextbox(string input)
                => FindBy("id", "bin").ScrollTo(Driver).SendKeys(input);
    
            [When(regex: @"^i click convert$")]
            public void ClickConvert()
                => FindBy("xpath", @"//button[starts-with(@title, 'Conver')]").ScrollTo(Driver).Click();
    
            [Then(regex: @"^the (?<op>binary|text) box shows ""(?<expected>[^""]+)""$")]
            public void OutputIs(string op, string expected)
            {
                string actual = op switch
                {
                    "binary" => FindBy("id", "bin").GetAttribute("value"),
                    "text" => FindBy("id", "txt").GetAttribute("value")
                };
                if (!actual.Equals(expected))
                    throw new AssertException(string.Format("Expected:{0}, Actual:{1}", expected, actual));
            }
    
            [Then(regex: @"^close browser$")]
            public new void Close() => base.Close();
Thes step definitions define in code what to do when a line is encountered by the reature file, using regex from the Given/When/Then line to find the matching method to use. values from named groups in the regex enter the method as data. (More advanced data can be passed into methods, see advanced below). FindBy can locate items by ID, Name, CSS selector of Xpath, and can use ScrollTo(Driver) to move that item into view on the screen.

See **Extending** below.
