namespace SimpleFluentTester.UnitTests.Helpers.TestObjects;

internal class CustomException : Exception;

internal class CustomWithMessageException(string? message) : Exception(message);