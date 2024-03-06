namespace SimpleFluentTester.Examples
{
    internal class CustomValue(int value)
    {
        public int Value { get; } = value;

        public static CustomValue FromInt(int value) => new(value);

        public override string ToString() => Value.ToString();
    }
}