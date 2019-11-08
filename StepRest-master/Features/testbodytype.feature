@:-api
Feature: Testing

Background:
	Given a base uri http://jsonplaceholder.typicode.com
		And a base path todos
		And a port 80
			
Scenario: Valid type testsing
	When the system requests GET 1
	Then the body equals (int)userId=1
		And the body equals (int)id=1
		And the body equals (text)title=delectus aut autem
		And the body equals (bool)completed=false

Scenario: Invalid type testing [ALL EXCEPT FIRST (GET) SHOULD FAIL]
	When the system requests GET 1
	Then the body equals (text)userId=1
		And the body equals (text)id=1
		And the body equals (text)completed=false