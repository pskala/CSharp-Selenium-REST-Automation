# Automation Tests for internet application

Selenium automation and REST automation tests example written in C# language. 

## Set Config file in Visual Studio

1. open solution SeleniumAndRESTtests.sln  in Visual Studio 2019
2. in menu navigate to Test -> Test Settings -> Select Test Settings File
3. navigate to folder "settings" and select one of the file according to environment what you want test it:
 - config-test-environment.runsettings (for testing on test server)
 
## Configuration

Open runsettings file in your editor

* webAppUrl - full url with test web app -  **IMPORTANT!!**
* webAppUserName - admin user login for login page
* webAppPassword - admin user password for login page
* remoteDriver - set true for remote test - for example over Docker
* remoteUrl - url where is remote driver - exmple url for Docker: http://localhost:4444/wd/hub 
* DataPathDir - path to folder for external saved data: excels, json, images etc.

## Run

In Visual Studio:
1. download all nuget packages and build project
1. open Test Explorer (in VS menu: Test -> Windows -> Test Explorer)
2. click on Run All or select specific testcase in tree -> click on right mouse button -> select Run Selected Tests / Debug Selected Tests

## Contributors

* [Petr Skala](pskala@seznam.cz)
