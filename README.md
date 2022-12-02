# window-exception-validation
This repository is full of NUnit tests that validate and summarize each step of the window exception workbook. 

## Window Exception Steps 
1. Import Zone - the required changes for existing zones
1. Ops Plan - Starts with a mapping of Ops Markets to the actual zones / windows we currently have
1. Windows - The windows that need to be created for window exceptions. Each exception should have a new window
1. Zones - The zones that need to be created. We should only create zones for markets / days that otherwise would not have a zone.
1. Exceptions - Exceptions that link existing windows to their exception window. Also contains text shown to the user, and the days the exception is in effect
1. LCM - for LCM communications, rather than using their existing workbook given it is out of date.

