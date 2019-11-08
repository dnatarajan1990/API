@:-api -console
Feature: Testing

Background:
	Given a base uri https://gorest.co.in
		And a base path public-api/users/123
		And a query of access-token=qpNI0-xwDrkNKmkHpuksI3snrV-9uv4uFj_S
		And a port 80
	
Scenario: Valid GET Test should return 200 in under 2 sec
	Given a query of _format=xml
	When the system requests GET
    Then the response code is 200
		And the Content-Type header contains xml
		And the response time is over 100ms
		And the response time is under 2 sec
		And the body equals (text)response.result.id=123
		And the body equals (text)response.result.first_name=Abraham

Scenario: Valid GET Test should return 200 in under 2 sec
	Given a query of _format=json
	When the system requests GET
    Then the response code is 200
		And the Content-Type header contains json
		And the response time is over 100ms
		And the response time is under 2 sec
		And the body equals (text)result.id=123
		And the body equals (text)result.first_name=Abraham