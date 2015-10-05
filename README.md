## Web Linter

A Visual Studio extension that runs ESLint and TSLint on JavaScript and
TypeScript files

[![Build status](https://ci.appveyor.com/api/projects/status/3bc3dv4tsc34mv97?svg=true)](https://ci.appveyor.com/project/madskristensen/weblinter)

Download the extension at the
[VS Gallery](https://visualstudiogallery.msdn.microsoft.com/3b329021-cd7a-4a01-86fc-714c2d05bb6c)
or get the
[nightly build](http://vsixgallery.com/extension/36bf2130-106e-40f2-89ff-a2bdac6be879/)

See the
[changelog](https://github.com/madskristensen/WebLinter/blob/master/CHANGELOG.md)
for changes and roadmap.

### Features
- Lints JavaScript, JSX, TypeScript, CoffeeScript and CSS files using:
  - ESLint
  - CssLint
  - CoffeeLint
  - TSLint
- Error List integration
- Runs when a file is opened and when the file is saved
- Run on a single file, folder or the entire project
- Configuration files lets you select what rules to run
- Both errors and warnings are supported
- Option to turn off one or more linters

### Error List
When an error is found it will show up in the Error List in
Visual Studio as either an error or a warning.

![Error List](art/errorlist.png)

The individual rules for each linter can be configured to be
either an error or a warning.

Double-clicking an error will open the file and place the
cursor at the location of the error.

### Configuration
Each linter has their own configuration file. For instance,
**ESLint** uses a JSON file with the name of `.eslintrc` to
store the configuration in.

The configuration files are located
in your user profile. Example: `C:\Users\myname\`.

It's easy to modify the configuration files by using the
menu commands to open them:

![Tools menu](art/tools-menu.png)

If the configuration files have gotten messed up, you can reset
them to their defaults by clicking the
**Reset Configuration Files** command. It will prompt to ask
if you are sure you want to proceed and, if you click yes,
all the configuration files will be restored to their defaults.