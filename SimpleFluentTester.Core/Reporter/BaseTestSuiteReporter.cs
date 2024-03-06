using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.Suite;

namespace SimpleFluentTester.Reporter
{
    /// <summary>
    /// Base class that should be used for defining your own custom reporter.
    /// </summary>
    public abstract class BaseTestRunReporter<TOutput>(TestSuiteResult<TOutput> testRunResult) : ITestSuiteReporter
    {
        protected readonly TestSuiteResult<TOutput> TestRunResult = testRunResult;

        public void Report()
        {
            try
            {
                ReportInternal();
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't report a result of TestSuite. Exception: {0}", e);
                throw;
            }
        }

        protected abstract void ReportInternal();

        protected virtual ILogger CreateLogger()
        {
            var loggerFactory = LoggerFactory.Create(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddSimpleConsole(x =>
                {
                    x.TimestampFormat = "HH:mm:ss.fff ";
                    x.IncludeScopes = true;
                    x.ColorBehavior = LoggerColorBehavior.Enabled;
                });
            });

            return loggerFactory.CreateLogger(TestRunResult.DisplayName ?? GetType().Name);
        }
    }
}