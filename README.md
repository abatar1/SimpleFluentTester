<div align="center">

# Simple Fluent Tester <br> :smirk_cat:

> Simple and user-friendly C# testing tool.

[![Latest release](https://github.com/abatar1/SimpleFluentTester/actions/workflows/main.yml/badge.svg)](https://github.com/abatar1/SimpleFluentTester/actions/workflows/main.yml)
[![Coverage Status](https://coveralls.io/repos/github/abatar1/SimpleFluentTester/badge.svg?branch=main)](https://coveralls.io/github/abatar1/SimpleFluentTester?branch=main)

</div>

## What is it for?

Let's say you are solving algorithmic problems on a platform like LeetCode or conducting intermediate testing for a complex algorithm. 

Would you start writing your own wrappers? This will take some time that could be spent more productively. 

Would you use a testing framework? That's a good option, but it requires writing a lot of unnecessary infrastructure code. 

Based on my personal experience, I have created a library that allows you to set up the entire testing environment in just a minute!

## Instruction

1. Use your preferred IDE to install the NuGet package `SimpleFluentTester`. You can also find it at this [link](https://www.nuget.org/packages/SimpleFluentTester).


2. I assume that you have a very simple function that can add two numbers and return the sum, and you want to test all possible test cases for it.
    ```csharp
    int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
    ```
   Let's start writing tests for it!
   ```csharp
    TestSuite.Setup()
        // Here we specify the method we want to test.
       .UseOperation<int>(Adder) 
        // Then we add 2 valid tests and one invalid test.
       .AddTestCase(2, 1, 1) 
       .AddTestCase(-2, -1, -1)
       .AddTestCase(-3, -1, -1)
       .Run() 
       .Report();
    ```
   And the output of this code will indicate that one out of the three tests has not passed: 
   ```
   18:36:46.431 info: SimpleFluentTester.Reporter.DefaultTestRunReporter[0]
      Executing tests for target method [Int32 Adder(Int32, Int32)]
      Total tests: 3
      Tests to execute: 3
   18:36:46.445 fail: SimpleFluentTester.Reporter.DefaultTestRunReporter[0]
      Test iteration [3] not passed
      Inputs: '-1', '-1'
      Expected: '-3'
      Output: '-2'
      Elapsed: 0,00040ms
   18:36:46.448 fail: SimpleFluentTester.Reporter.DefaultTestRunReporter[0]
      1/3 tests haven't passed!
      Failed test iterations: 3
      Elapsed total: 0,17370ms; Avg: 0,05790ms; Max: 0,12480ms [Iteration 2]
   ```
   Furthermore, for debugging purposes, it would be most convenient to select only the unsuccessful iteration:
   ```csharp
    TestSuite.Setup()
       .UseOperation<int>(Adder) 
       .AddTestCase(2, 1, 1) 
       .AddTestCase(-2, -1, -1)
       .AddTestCase(-3, -1, -1)
        // You should not comment on your test cases; just specify the iteration you want to test, every other iteration will be ignored.
       .Run(3) 
       .Report();
    ```
   Also, you can write your custom reporter that will generate a report in the format you need:
   ```csharp
   TestSuite.Custom
       .WithCustomReporterFactory<CustomReporterFactory>() 
       .Setup()
       .UseOperation<int>(CustomMethods.Adder) 
       .AddTestCase(2, 1, 1)
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


