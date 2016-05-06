# PI-AF-SDK-Limit-Calculator
A sample application for calculating dynamic control limits using event frames and PI AF SDK

## Build the project
First build the LimitCalculatorSDK projects, this creates a .dll that is then used by the other two other projects.

The Limit Calculator project is a GUI to create preference files for the calculations. The preference file (LimitCalculatorSetting.json) is stored in the document user of the current user.

The EventFrameAnalysis is a command line project that once run will read the preference file and starts writing to the data archive.

## Limitations
EventFrameAnalysis does not create tags and attributes, tags need to be created before the console app is runned. Attribute creations should be done using templates.

## Requirements
  1. PI AF 2016
  2. PI Server 2015
  3. Visual Studio 2015
