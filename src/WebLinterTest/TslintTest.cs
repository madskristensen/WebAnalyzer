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

        [TestMethod, TestCategory("TSLint")]
        public async Task Standard()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/tslint/a.ts");
            Assert.IsTrue(result.First().HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.First().Errors.First().FileName), "File name is empty");
            Assert.AreEqual(7, result.First().Errors.Count);
            Assert.AreEqual("if statements must be braced", result.First().Errors.First().Message);
        }

        //[TestMethod, TestCategory("TSLint")]
        //public void Multiple()
        //{
        //    var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/tslint/b.ts", "../../artifacts/tslint/a.ts");
        //    var first = result.First();
        //    var firstErrors = first.Errors.ToArray();
        //    Assert.IsTrue(first.HasErrors);
        //    Assert.IsFalse(string.IsNullOrEmpty(firstErrors.First().FileName), "File name is empty");
        //    Assert.AreEqual(14, firstErrors.Length);
        //    Assert.AreEqual("if statements must be braced", firstErrors.First().Message);
        //}

        [TestMethod, TestCategory("TSLint")]
        public async Task FileNotExist()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/tslint/doesntexist.js");
            Assert.IsTrue(result.First().HasErrors);
        }
    }
}
