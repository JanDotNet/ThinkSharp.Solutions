# ThinkSharp.Solutions

[![Build status](https://ci.appveyor.com/api/projects/status/l3aagqmbfmgxwv3t?svg=true)](https://ci.appveyor.com/project/JanDotNet/thinksharp-solutions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.TXT)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MSBFDUU5UUQZL)

## Intoduction

ThinkSharp.Solutions is a small Visual Studio Extension that allows to create a new solution from a template. The template is a prepared solution located in a git repository. It may contain place holders that are replaced with custom values after cloning.

## Installation

Download latest release: https://github.com/JanDotNet/ThinkSharp.Solutions/releases and install it.

Note that ThinkSharp.Solutions is currently available for Visual Studio 2015 only.

## Creating a Template

### Create the template
Create a new solution and replace all configurable text fragments with placeholders. "configurable text fragments" may be text within file content or file / directory names.

There is also a working sample available: 
https://github.com/JanDotNet/ThinkSharp.Solutions.ExampleTemplate

### Add a template definition file
* Create a file "TemplateDefinition.xml" on root level (same directory where the '.git' directory is)
* Define all placeholders within the template definition file as shown below:
   
```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <TemplateDefinition>
        <Placeholders>
            <Placeholder Name="MODULE"
                         Description="The module the micro service belongs to."
                         SuggestionList="Module01, Module02, Module03" />
            <Placeholder Name="MICROSERVICE" 
	                 Description="The name of the microservice" />
	    <Placeholder Name="PORT"
			 TextToReplace="999999"
	                 Description="The name of the microservice" />	
        </Placeholders>
	
	<Examples>
	    <Example Header="AssemblyName" Text="Company.Department.MODULE.MICROSERVICE.dll" />
            <Example Header="Namespace" Text="namespace Company.Department.MODULE.MICROSERVICE" />
            <Example Header="Service" Text="public class MICROSERVICEService" />
            <Example Header="URL" Text="http://localhost:999999/index.html" />
        </Examples>
    </TemplateDefinition>
```

The value of ``TextToReplace`` is the text that will be replaced. If ``TextToReplace`` is not set, the value of ``Name`` will be used instead. The ``SuggestionList`` is a comma-separated list that will be provided to the user as replacements when creating the solution from template.

The examples will be shown on the dialog where the place holders are entered. They can be used to give the user a preview of the template parts affected by the place holders.

### Define GUID Placeholders
GUID placeholders are valid GUIDs so that the template remains buildable. There are 2 types of special GUID placeholders that can be used.

**NewGuid Placeholder**

All occurencies of the GUID "00000000-1111-0000-1111-000000000000" will be replaced by a new generated GUID using `Guid.New()`. This placeholder provides a simple way to generate new guids.

**NewCachedGuid Placeholder**

All occurencies of the GUID "00000000-1111-0000-1111-[0-9a-f]{12}" will be replaced by a new generated or cached GUID. That means, that 2 occurencies of the placeholder "00000000-1111-0000-1111-000000000001" will be replaced by the same GUID. Therefore, this placeholder provides a way to use the same new generated GUID multiple times.

### Push the template to a git repository

## Usage

1) Start a new instance of Visual Studio 2015
2) Klick Tools -> Create Solution from Template
3) Enter the HTTPS URL of the git repository and the target directory into the dialog and press "Clone Template". Note that cloning with git url (e.g. git@github.com...) is not supported.

![solutionfromtemplate01](https://user-images.githubusercontent.com/21179870/38171518-81ae0a7e-359b-11e8-8a9c-9f0d641056b9.png)

The solution will be copied into the entered target directory.

4) Enter replacements and press "Replace Placeholders"

![solutionfromtemplate02](https://user-images.githubusercontent.com/21179870/38171522-8a98c598-359b-11e8-9e8c-73b6cef7facd.png)

All placeholders will be replaced in all files (file content) as well as directory and file names.
If "Open Solution" is checked, the solution will be opend.

## Logging

In case of errors, the log file is located in "%AppData%\ThinkSharp\Solutions\"

## License

ThinkSharp.Solutions is released under [The MIT license (MIT)](LICENSE.TXT)

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/JanDotNet/ThinkSharp.Solutions/tags). 

## Donation
If you like ThinkSharp.Solutions, feel free to give me a cup of coffee :) 

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MSBFDUU5UUQZL)
