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