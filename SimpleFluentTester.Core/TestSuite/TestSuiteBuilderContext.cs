using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Suite
{
    public sealed class TestSuiteBuilderContext<TOutput>(
        int number,
        string name,
        IEntryAssemblyProvider entryAssemblyProvider,
        IActivator activator,
        IList<TestCase<TOutput>> testCases,
        ISet<ValidationInvoker<TOutput>> validators,
        ITestSuiteReporterFactory reporterFactory,
        ValueWrapper<Delegate> operation,
        Func<TOutput?, TOutput?, bool>? comparer,
        bool shouldBeExecuted)
    {
        public int Number { get; } = number;
    
        public string Name { get; internal set; } = name;
    
        public bool ShouldBeExecuted { get; internal set; } = shouldBeExecuted;
    
        public IEntryAssemblyProvider EntryAssemblyProvider { get; } = entryAssemblyProvider;

        public IActivator Activator { get; } = activator;

        public IList<TestCase<TOutput>> TestCases { get; } = testCases;

        public ITestSuiteReporterFactory ReporterFactory { get; internal set; } = reporterFactory;

        public ValueWrapper<Delegate> Operation { get; } = operation;
    
        public Func<TOutput?, TOutput?, bool>? Comparer { get; internal set; } = comparer;

        public ISet<ValidationInvoker<TOutput>> Validators { get; } = validators;

        private bool? _isObjectOutput;
        public bool IsObjectOutput
        {
            get
            {
                if (_isObjectOutput != null)
                    return _isObjectOutput.Value;

                _isObjectOutput = typeof(TOutput) == typeof(object);
                return _isObjectOutput.Value;
            }
        }

        private Type? _outputUnderlyingType;
        public Type? OutputUnderlyingType
        {
            get
            {
                if (_outputUnderlyingType != null)
                    return _outputUnderlyingType;

                if (Operation.Value?.Method.ReturnParameter == null)
                    return null;

                _outputUnderlyingType = Nullable.GetUnderlyingType(Operation.Value.Method.ReturnParameter.ParameterType);
                return _outputUnderlyingType;
            }
        }

        private IReadOnlyCollection<ParameterInfo>? _operationParameters;
        public IReadOnlyCollection<ParameterInfo> OperationParameters
        {
            get
            {
                if (_operationParameters != null)
                    return _operationParameters;
                if (Operation.Value == null)
                    throw new InvalidOperationException("_operationParameters where accessed before _operation has been initialized, this should be the bug");
            
                _operationParameters = Operation.Value.Method.GetParameters();
                return _operationParameters;
            }
        }
    }
}