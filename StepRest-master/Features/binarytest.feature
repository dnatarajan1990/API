@:-BinaryConverterRunner -console
Feature: Testing

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

	
Scenario: Testing text-bin convert
Given a firefox browser
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