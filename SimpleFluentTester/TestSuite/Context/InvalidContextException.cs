using System;

namespace SimpleFluentTester.TestSuite.Context;

public sealed class InvalidContextException(string message) : Exception(message);