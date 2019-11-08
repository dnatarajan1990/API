$(document).ready(function() {var formatter = new CucumberHTML.DOMFormatter($('.cucumber-report'));formatter.uri("file:[TestSet]_testing.feature");
formatter.feature({
  "name": "[TestSet]_testing",
  "description": "",
  "keyword": "Feature",
  "tags": [
    {
      "name": "@[TestSet]_testing"
    }
  ]
});
formatter.background({
  "name": "",
  "description": "",
  "keyword": "Background"
});
formatter.step({
  "name": "a chrome browser",
  "keyword": "Given "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.scenario({
  "name": "Testing text-bin convert",
  "description": "",
  "keyword": "Scenario",
  "tags": [
    {
      "name":"[1]"
    }
  ]
});
formatter.step({
  "name": "i go to text to binary page",
  "keyword": "When "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.step({
  "name": "i enter \".\" in text textbox",
  "keyword": "And "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.step({
  "name": "i click convert",
  "keyword": "And "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.step({
  "name": "the binary box shows \"00101110\"",
  "keyword": "Then "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.scenario({
  "name": "Testing bin-text convert",
  "description": "",
  "keyword": "Scenario",
  "tags": [
    {
      "name":"[2]"
    }
  ]
});
formatter.step({
  "name": "i go to binary to text page",
  "keyword": "When "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.step({
  "name": "i enter \"00101110\" in binary textbox",
  "keyword": "And "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.step({
  "name": "i click convert",
  "keyword": "And "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.step({
  "name": "the text box shows \".\"",
  "keyword": "Then "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
formatter.step({
  "name": "close browser",
  "keyword": "Then "
});
formatter.match({
  "location": "Runner"
});
formatter.result({
  "status": "passed"
});
});
