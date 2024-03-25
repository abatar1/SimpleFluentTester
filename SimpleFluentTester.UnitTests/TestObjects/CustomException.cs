namespace SimpleFluentTester.UnitTests.TestObjects;

internal class CustomException : Exception;

internal class CustomWithMessageException(string? message) : Exception(message);