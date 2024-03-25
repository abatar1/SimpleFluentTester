namespace SimpleFluentTester.Validators.Core;

public interface IValidationUnpacker
{
    ValidationUnpacked Unpack(IValidated validated);
}