using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Tests
{
    public class TestWriter : TextWriter
    {
        private readonly ITestOutputHelper outputHelper;

        private List<string> writtenStrings = new List<string>();

        public TestWriter(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }
        public override Encoding Encoding => Encoding.Default;

        public override void WriteLine(string value)
        {
            outputHelper.WriteLine(value);
            writtenStrings.Add(value);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                writtenStrings.Clear();
        }

        public string[] GetWrittenStrings() => writtenStrings.ToArray(); 
    }
}
