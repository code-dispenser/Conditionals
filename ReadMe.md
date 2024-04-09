[![.NET](https://github.com/code-dispenser/Conditionals/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/code-dispenser/Conditionals/actions/workflows/dotnet.yml) [![Coverage Status](https://coveralls.io/repos/github/code-dispenser/Conditionals/badge.svg?branch=main)](https://coveralls.io/github/code-dispenser/Conditionals?branch=main) [![Nuget download][download-image]][download-url]

[download-image]: https://img.shields.io/nuget/dt/Conditionals.Core
[download-url]: https://www.nuget.org/packages/Conditionals.Core
<h1>
<img src="https://raw.github.com/code-dispenser/Conditionals/main/Assets/icons-64.png" align="center" alt="Conditionals icon" /> Conditionals
</h1>
<!--
# ![icon](https://raw.github.com/code-dispenser/Conditionals/main/Assets/icon-64.png) Conditionals
-->
<!-- H1 for git hub, but for nuget the markdown is fine as it centers the image, uncomment as appropriate and do the same at the bottom of this file for the icon author -->

## Overview

Docs in Progress so not on nuget yet ....

Conditionals is essentially a Boolean expression-based rules engine. It operates by evaluating conditions, which can be conceptualized as "condition trees." These trees consist of left and right 
operands, combined with logical operators such as OrElse (||) or AndAlso (&&).

Conditions are evaluated using a dedicated condition evaluator, which comes in two varieties: built-in or custom-made. This allows for flexibility in handling various types of conditions within 
the rules engine.

Condition operands in this system come in three variants, all of which ultimately evaluate to either true or false:

**Lambda Expression Condition (PredicateCondition&lt;TContext&gt;):** These conditions are defined using lambda expressions, providing flexibility in defining custom logic based on the context 
of your application.

**Regular Expression Condition (RegexCondition&lt;TContext&gt;):** These conditions utilize regular expressions to match patterns within the context. This can be useful for scenarios where the 
condition can be expressed using pattern matching.

**Custom Expressions Requiring Custom Evaluators (CustomCondition&lt;TContext&gt;):** For cases where neither lambda expressions nor regular expressions suffice, custom conditions can be defined.
These require the implementation of custom evaluators tailored to your specific requirements.

## Installation

Download and install the latest version of the [Conditionals.Core](https://www.nuget.org/packages/Conditionals.Core) package from [nuget.org](https://www.nuget.org/) using your preferred client tool.

## Example usage


In conjunction with the [documentation](https://github.com/code-dispenser/Conditionals/wiki), it is recommended that you download the source code from the [Git repository](https://github.com/code-dispenser/Conditionals) and explore the scenarios within the demo project. These sample 
scenarios and their comments should answer most of your questions.

Any feedback, positive or negative, is welcome, especially surrounding scenarios/usage.

## Acknowledgments

Currently, this library uses the method "DynamicExpressionParser.ParseLambda" from the [System.Linq.Dynamic.Core project](https://www.nuget.org/packages/System.Linq.Dynamic.Core) to create the 
compiled lambda predicate from the string representation in any json rule files used. Many thanks to all of the contributors on that project for making it much easier for me to create this 
project/nuget package.


<img src="https://raw.githubusercontent.com/code-dispenser/Conditionals/main/Assets/icons-64.png" align="middle" height="32px" alt="Conditionals icon" />
<a target="_blank" href="https://icons8.com/icon/kxE6S5YOUvM6/if">If</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>
<!--
![icon](https://raw.github.com/code-dispenser/Conditionals/main/Assets/icon-48.png) Thanks also to Peerapak Takpho the icon creator, which I found on [freepik.com](https://www.freepik.com/icon/setting_7012934).
-->


