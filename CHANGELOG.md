# Roadmap

- [ ] Add config files to solution
- [ ] Red squiggly in the editor
- [ ] Add _Clean_ command to Error List context menu

Features that have a checkmark are complete and available for
download in the
[nightly build](http://vsixgallery.com/extension/36bf2130-106e-40f2-89ff-a2bdac6be879/).

# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 1.3

**2015-10-11**

- [x] Write `.eslintrc` JSON schema for SchemaStore.org
- [x] Write `.csslintrc` JSON schema for SchemaStore.org
- [x] Help links for JSX rules from ESLint
- [x] Fixed proxy server issue (#3)
- [x] Better default settings for linters
- [x] Lots of tweaks and improvements

## 1.2

**2015-10-08**

- [x] Running linters in parallel
- [x] Implemented async/await
- [x] Added _babel-eslint_ support
- [x] Added _eslint-plugin-filenames_ support
- [x] Don't show context menu on non-file/folder items

## 1.1

**2015-10-07**

- [x] Spin up a node.js server instead of shelling out
  - Huge performance gain by this
- [x] Use new VS2015 Error List API
- [x] Shows error codes from each linter
- [x] Link directly to rule's help page
- [x] Use Visual Studio's _node.exe_
- [x] Initialize linters when VS opens
- [x] Add popular config plugins to ESLint
- [x] Option to ignore nested files
- [x] Command to reset all settings

## 1.0

**2015-10-05**

- [x] Add sensible defaults
- [x] Options page
- [x] CSSLint support
- [x] CoffeeLint support
- [x] ESLint support
- [x] TSLint support
- [x] Ignore nested and minified files from linting
- [x] Support multiple files linted at the same time
- [x] Run from Solution Explorer
- [x] Command to clear all errors
- [x] Edit config files menu commands
- [x] Reset configuration files to defaults