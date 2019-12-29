using CSLox;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class BenchmarkTests : IDisposable
    {
        private TestWriter testWriter;
        private TestWriter testErrors;
        public BenchmarkTests(ITestOutputHelper outputHelper)
        {
            testWriter = new TestWriter(outputHelper);
            testErrors = new TestWriter(outputHelper);
            Runtime.Writer = testWriter;
            Runtime.Errors = testErrors;
        }
        public void Dispose()
        {
            testWriter.Dispose();
            testErrors.Dispose();
            Runtime.Writer = null;
            Runtime.Errors = null;
        }


        [Theory]
        [InlineData("benchmark\\binary_trees.lox")]
        [InlineData("benchmark\\equality.lox")]
        [InlineData("benchmark\\fib.lox")]
        [InlineData("benchmark\\invocation.lox")]
        [InlineData("benchmark\\method_call.lox")]
        [InlineData("benchmark\\properties.lox")]
        [InlineData("benchmark\\string_equality.lox")]
        public void Benchmarks(string file)
        {
            var path = System.Environment.CurrentDirectory;
            Runtime.RunFile(System.IO.Path.Combine(path, $"..\\..\\..\\cases\\{file}"));

            Assert.Empty(testErrors.GetWrittenStrings());
        }
    }
}
