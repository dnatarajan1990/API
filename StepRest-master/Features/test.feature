@:-api -console
Feature: Testing

Background:
	Given a base uri https://postman-echo.com
		And a base path get
		And a port 80
		And a query of arg1=val1
		And queries:
			|arg2|val2|
			|arg3|true|
		And a header of head1=val1
		And headers:
			|head2|69|
			|head3|3.14|
	
Scenario: Valid GET Test should return 200 in under 2 sec
	When the system requests GET
    Then the response code is 200
		And the response time is over 100ms
		And the response time is under 2 sec

Scenario: POST request should return 404 in under 1 sec
	When the system requests POST
	Then the response code is 404
		And the response time is less than 1000milliseconds
		
Scenario: Valid GET Test should return expected headers and body
	When the system requests GET
    Then the response code is 200
		And the Content-Type header contains json
		And the headers dont contain SOMEFAKEHEADER
		And the body equals (text)args.arg1=val1
		And the body equals (text)args.arg2=val2
		And the body starts with (text)args.arg2=va
		And the body equals (text)args.arg3=true
		And the body ends with (text)args.arg3=ue
		And the body is not (text)headers.head1=NOTHING
		And the body equals (text)headers.head2=69
		And the body starts with (text)headers.head2=6
		And the body contains (text)headers.head2=6
		And the body contains (text)headers.head3=14