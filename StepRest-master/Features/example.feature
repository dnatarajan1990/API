@:-api
Feature: Example

Background:
  Given a port 80
  
Scenario: JSON
  When the system requests GET http://jsonplaceholder.typicode.com/todos/2
  Then the response code is 200
  And the Content-Type header contains json
  And the response body contains:
    |userId|equals|1|int|

Scenario: JSON2
  When the system requests GET https://jsonplaceholder.typicode.com/users/1/
  Then the response code is 200
  And the Content-Type header contains json
  And the body contains:
    |name|equals|Leanne Graham|text|
    |address.city|equals|Gwenborough|text|