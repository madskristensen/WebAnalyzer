using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class TshintTest
    {
        [TestInitialize]
        public void Setup()
        {
            Telemetry.Enabled = false;
        }

        // This test fails on CI server with this error:
        // at WebLinterTest.TshintTest.<Standard>d__1.MoveNext() in C:\projects\webanalyzer\src\WebLinterTest\TslintTest.cs:line 21
        // --- End of stack trace from previous location where exception was thrown ---
        // at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
        // at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
        // at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
        [TestMethod, TestCategory("TSLint"), Ignore]
        public async Task Standard()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/tslint/a.ts");
            Assert.IsTrue(result.First().HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.First().Errors.First().FileName), "File name is empty");
            Assert.AreEqual(7, result.First().Errors.Count);
            Assert.AreEqual("if statements must be braced", result.First().Errors.First().Message);
        }

        [TestMethod, TestCategory("TSLint")]
        public async Task FileNotExist()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/tslint/doesntexist.js");
            Assert.IsTrue(result.First().HasErrors);
        }
    }
}
