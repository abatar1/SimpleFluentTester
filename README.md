<div align="center">

# Simple Fluent Tester <br> :smirk_cat:

> Simple and user-friendly C# testing tool.

[![Latest release](https://github.com/abatar1/SimpleFluentTester/actions/workflows/main.yml/badge.svg)](https://github.com/abatar1/SimpleFluentTester/actions/workflows/main.yml)
[![Coverage Status](https://coveralls.io/repos/github/abatar1/SimpleFluentTester/badge.svg?branch=main)](https://coveralls.io/github/abatar1/SimpleFluentTester?branch=main)
![NuGet Version](https://img.shields.io/nuget/v/SimpleFluentTester)

</div>

## What is it for?

Let's say you are solving algorithmic problems on a platform like LeetCode or conducting intermediate testing for a complex algorithm. 

Would you start writing your own wrappers? This will take some time that could be spent more productively. 

Would you use a testing framework? That's a good option, but it requires writing a lot of unnecessary infrastructure code. 

Based on my personal experience, I have created a library that allows you to set up the entire testing environment in just a minute!

## Instruction

Use your preferred IDE or CLI to install the NuGet package `SimpleFluentTester`. You can also find the NuGet package at this [link](https://www.nuget.org/packages/SimpleFluentTester).

I assume that you have a very complex function to cover with test cases, but let's say we have a very simple one of some sort:
    
```csharp
 int Adder(int number1, int number2)
 {
     return number1 + number2;
 }
 ```

Let's start writing test cases for it!

```csharp
 TestSuite
     // Return type of your testable method should be specified.
    .WithExpectedReturnType<int>()
     // Here we specify the method we want to test.
    .UseOperation(Adder) 
     // Then we add 2 valid tests and one invalid test.
    .Expect(2).WithInput(1, 1) 
    .Expect(-2).WithInput(-1, -1)
    .Expect(-3).WithInput(-1, -1)
    .Run() 
    .Report();
 ```
    
And the output of this code will indicate that one out of the three test cases has not passed: 
   
```
18:36:46.431 info: SimpleFluentTester.Reporter.DefaultTestRunReporter[0]
   Executing tests for target method [Int32 Adder(Int32, Int32)]
   Total tests: 3
   Tests to execute: 3
18:36:46.445 fail: SimpleFluentTester.Reporter.DefaultTestRunReporter[0]
   Test case [3] not passed
   Inputs: '-1', '-1'
   Expected: '-3'
   Output: '-2'
   Elapsed: 0,00040ms
18:36:46.448 fail: SimpleFluentTester.Reporter.DefaultTestRunReporter[0]
   1/3 tests haven't passed!
   Failed test iterations: 3
   Elapsed total: 0,17370ms; Avg: 0,05790ms; Max: 0,12480ms [Iteration 2]
```

Furthermore, for debugging purposes, for the next run it would be most convenient to select only the unsuccessful test cases:
   
```csharp
 TestSuite
    .WithExpectedReturnType<int>()
    .UseOperation(Adder) 
    .Expect(2).WithInput(1, 1) 
    .Expect(-2).WithInput(-1, -1)
    .Expect(-3).WithInput(-1, -1)
     // You should not comment on your test cases; just specify the iteration you want to test, every other iteration will be ignored.
    .Run(3) 
    .Report();
 ```
Also, you can write your custom reporter that will generate a report in the format you need:
```csharp
TestSuite
    .WithExpectedReturnType<int>()
    .WithCustomReporterFactory<CustomReporterFactory>() 
    .UseOperation<int>(CustomMethods.Adder) 
    .Expect(2).WithInput(1, 1) 
    .Run()
    .Report();
```


If your project contains multiple test suites simultaneously, and you wish to debug only one of them, 
you don't need to comment out the code; simply follow these steps:
```csharp
TestSuite.Ignore // <- add Ignore here and this test run will be fully ignored.
    .WithExpectedReturnType<int>()
    .UseOperation<int>(CustomMethods.Adder) 
    .Expect(2).WithInput(1, 1) 
    .Run()
    .Report();
```

If you have any questions, you can find all these examples in [this project](/SimpleFluentTester.Examples) 
or ask me directly via [email](mailto:evgenyhalzov@gmail.com?Subject=SimpleFluentTester)!

## License

Released under [MIT](/LICENSE) by [@EvgenyHalzov](https://github.com/abatar1).

- You can freely modify and reuse.
- The _original license_ must be included with copies of this software.
- Please _link back_ to this repo if you use a significant portion the source code.


