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
			
Scenario: Under load success rate should be over 95% and return data fast
    When the system loads 2 threads for 10 sec
		And the system requests GET
	 Then the total successfull load requests is over 20
		And the total failed load requests is under 5
		And the success percentage is more than 95%
		And the average load time is under 1000 ms
		And the max load time is under 2sec
		And the min load time is less than 500 milliseconds