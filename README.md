# dotMath

[![Build status](https://ci.appveyor.com/api/projects/status/wn66aw4c38itlrsm?svg=true)](https://ci.appveyor.com/project/bcwood/dotmath) [![NuGet](https://img.shields.io/nuget/v/dotMath)](https://www.nuget.org/packages/dotMath/)

This is a fork of the dotMath library that was originally written by Stephen Hebert and hosted on CodePlex, but hadn't been updated since 2004. The documentation found here and on the wiki is mostly pulled from the original CodePlex site.

## Overview

Welcome to dotMath, an extensible mathematical expression compiler for .NET. The library allows for variable handling, an entire function library and the ability to add your own functions.

If you need to evaluate fixed or variable expressions, dotMath is your solution.

## Install from NuGet

    Install-Package dotMath
    
## Example Usage

**Fixed expression example:**

    (sin(5) * cos(4/5)) / 3

**Variable expression example:**

    (sin(a) * cos(b/a)) / c

## Documentation

* [Getting Started - How to Use dotMath](https://github.com/bcwood/dotMath/wiki/Getting-Started)
* [Built-in Operators and Functions](https://github.com/bcwood/dotMath/wiki/Built-in-Operators-and-Functions)
