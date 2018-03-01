# ThinkSharp.Solutions

[![Build status](https://ci.appveyor.com/api/projects/status/l3aagqmbfmgxwv3t?svg=true)](https://ci.appveyor.com/project/JanDotNet/thinksharp-solutions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.TXT)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MSBFDUU5UUQZL)

## Intoduction

ThinkSharp.Solutions is a small Visual Studio Extension that allows to clone a template from a git repository. 
The template may contain place holders that are replaced with custom values after cloning.

## Installation

Download last build: https://ci.appveyor.com/project/JanDotNet/thinksharp-solutions/build/artifacts and install it.

Note that ThinkSharp.Solutions is currently only available for Visual Studio 2015.

## Creating a Template

1) Create a new solution and replace all configurable text fragments with placeholders. "configurable text fragments" may text within files or file / directory names.
2) Create a file "TemplateDefinition.xml" on top level
3) Define all placeholders within the template definition file as shown below:
   
```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <TemplateDefinition>
        <Placeholders>
            <Placeholder Name="MODULE"
                         Description="The module the micro service belongs to."
                         SuggestionList="Module01, Module02, Module03" />
            <Placeholder Name="MICROSERVICE" 
	                 Description="The name of the microservice" />				 
        </Placeholders>
    </TemplateDefinition>
```

The name must be the text of the actual placeholder.
The SuggestionList is a comma-separated list that will be provided as replacements when creating the solution from template.

4) Push the template to a git repository

## Usage

1) Start a new instance of Visual Studio 2015
2) Klick Tools -> Create Solution from Template
3) Enter git repository URL, target directory into the dialog and press "Clone Template"

![thinksharp solutions01](https://user-images.githubusercontent.com/21179870/36869524-623b622a-1d9c-11e8-880f-a1359052416c.png)

The solution will be copied into the entered target directory.

4) Enter replacements and press "Replace Placeholders"

![thinksharp solutions02](https://user-images.githubusercontent.com/21179870/36869541-720c7e00-1d9c-11e8-83a1-75d48670a0d9.png)

All placeholders will be replaced in all files (file content) as well as directories and file names.
If "Open Solution" is checked, the solution will be opend.

## License

ThinkSharp.Solutions is released under [The MIT license (MIT)](LICENSE.TXT)

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/JanDotNet/ThinkSharp.Solutions/tags). 

## Donation
If you like ThinkSharp.Solutions, feel free to give me a cup of coffee :) 

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MSBFDUU5UUQZL)
