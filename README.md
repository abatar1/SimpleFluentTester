<div align="center">

# Simple Fluent Tester <br> :smirk_cat:

> Simple and user-friendly C# testing tool.

[![Latest release](https://github.com/abatar1/SimpleFluentTester/actions/workflows/main.yml/badge.svg)](https://github.com/abatar1/SimpleFluentTester/actions/workflows/main.yml)
![Coveralls](https://img.shields.io/coverallsCoverage/github/abatar1/SimpleFluentTester?label=Test%20coverage&link=https%3A%2F%2Fcoveralls.io%2Fgithub%2Fabatar1%2FSimpleFluentTester)
![NuGet Version](https://img.shields.io/nuget/v/SimpleFluentTester?label=NuGet%20version&color=white&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FSimpleFluentTester)

</div>

## What is it for?

Let's say you are solving algorithmic problems on a platform like LeetCode or conducting intermediate testing for a complex algorithm. 

Would you start writing your own wrappers? This will take some time that could be spent more productively. 

Would you use a testing framework? That's a good option, but it requires writing a lot of unnecessary infrastructure code. 

Based on my personal experience, I have created a library that allows you to set up the entire testing environment in just a minute!

## Instruction

Use your preferred IDE or CLI to install the NuGet package `SimpleFluentTester`. You can also find the NuGet package at this [link](https://www.nuget.org/packages/SimpleFluentTester).

I assume that you have a very complex function to cover with test cases, but let's say we have a very simple one of some sorts:
    
```csharp
int Adder(int number1, int number2) 
{ 
    return number1 + number2;
}
 ```

Let's start writing test cases for it!

```csharp
TestSuite.Sequential
     // Here we could optionally specify how test suite result log entry will be shown in output.
    .WithDisplayName("Example of TestSuite")
     // Here we specify the method we want to test.
    .UseOperation(Adder) 
     // And then we add one test that should pass and one that should not.
    .ExpectReturn(2).WithInput(1, 1) 
    .ExpectReturn(-3).WithInput(-1, -1)
    .Run() 
    .Report();
 ```
    
And the output of this code will indicate that one out of the three test cases has not passed: 

<pre>
  <code>
     21:49:17.681 <span style="background-color:indianred">fail</span>: Example of TestSuite[1]
        Executing tests for target method [Int32 Adder(Int32, Int32)]
        Total tests: 2; Tests to execute: 2

     Test case [2] was not successful
        Reason: Test case not passed
        Inputs: '-1', '-1'
        Expected: '-3'
        Output: '-2'
        Elapsed: 0,12530ms
      
     Passed: 1, Not passed: 1, Failed: 0
     Not passed numbers: 2
     Elapsed total: 0,1823ms; Avg: 0,0911ms; Max: 0,1253ms [Number 2]; Min: 0,0570ms [Number 1];
  </code>
</pre>

Furthermore, for debugging purposes, for the next run it would be most convenient to select only the unsuccessful test cases:
```csharp
TestSuite.Sequential
    .UseOperation(Adder) 
    .ExpectReturn(2).WithInput(1, 1) 
    .ExpectReturn(-2).WithInput(-1, -1)
    .ExpectReturn(-3).WithInput(-1, -1)
     // You should not comment your test cases; just specify the iteration you want to test, every other iteration will be ignored.
    .Run(3) 
    .Report();
 ```
Also, you can write your custom reporter that will generate a report in the format you need, as well as define the rule which test cases
should be printed or define your own logger:
```csharp
TestSuite.Sequential
    .UseOperation(Adder)
    .ExpectReturn(2).WithInput(1, 1)
    .Run()
    .Report((builder, testSuiteRunResult) =>
    {
        builder
            .WithReportBuilder<CustomTestSuiteReportBuilder>()
            .WithPrintablePredicate(testCase => testCase.Assert.Status == AssertStatus.NotPassed));
    });
```

If your project contains multiple test suites simultaneously, and you wish to debug only one of them, 
you don't need to comment on the code; simply follow these steps:
```csharp
TestSuite.Sequential.Ignore // <- add Ignore here and this test run will be fully ignored.
    .UseOperation(Adder) 
    .ExpectReturn(2).WithInput(1, 1) 
    .Run()
    .Report();
```

If you use a non-standard object type in your function which is not assignable from IEquatable, you can define how the TestSuite should compare them yourself.
```csharp
TestSuite.Sequential
    // CustomValue could be assigned from IEquatable or comparer may be directly defined like this:
    .WithComparer<CustomValue>((x, y) => x.Value == y.Value)
    .UseOperation((CustomValue a, CustomValue b) => a.Value + b.Value)
    .ExpectReturn(CustomValue.FromInt(2)).WithInput(CustomValue.FromInt(1), CustomValue.FromInt(1))
    .Run()
    .Report();
```

## Advanced examples

Let's assume our function is a bit more complex (such tasks are quite common on LeetCode), and we need to check not only the return value but also how the function's parameters have changed after execution:

```csharp
int Adder(int[] seq1, int[] seq2)
{ 
    for (var i = 0; i < seq1.Length; i++) 
        seq1[i] = seq2[i]; 
    return seq1[0] + seq2[0];
}
 ```
No worries, our tester knows how to handle even that!

```csharp
TestSuite.Sequential
    .UseOperation(Adder)
    .ExpectParameter(0).ToBe([2, 4]).WithInput([0, 1], [2, 3])
    .ExpectParameter("seq1").ToBe([2, 4]).WithInput([0, 1], [2, 3])
    .Run()
    .Report();
```

But what if we need to test both the function call and the returned value at the same time? To avoid duplicating assertions, we can use the `.And` syntax.

```csharp
TestSuite.Sequential
    .UseOperation(Adder)
    .ExpectParameter(0).ToBe([3, 1, 2]).And.ExpectReturn(6).WithInput([1, 2, 3], [3, 1, 2])
    .Run()
    .Report();
```

## But what if...?

If you have any questions, you can find all these examples in [this project](/SimpleFluentTester.Examples) 
or ask me directly via [email](mailto:evgenyhalzov@gmail.com?Subject=SimpleFluentTester)!

## Contribution

Would you like to contribute? I am always completely open to ideas or bug reports, and I also have a couple of open issues. 
Please, follow the instructions below:

Each modification of code should be introduced through a PR originating from the cloned repository. 

`prerelease` branch should be selected as the target branch for a PR.

Every PR undergoes checks to ensure that tests are passed and cover all the changes. 
Test coverage must not decrease, and comprehensive unit tests should accompany any new implemented functionality. 
In the case of substantial features, comprehensive documentation should also be included if needed.

## License

Released under [MIT](/LICENSE) by [@EvgenyHalzov](https://github.com/abatar1).

- You can freely modify and reuse.
- The _original license_ must be included with copies of this software.
- Please _link back_ to this repo if you use a significant portion of the source code.


