using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ThinkSharp.Solutions.Infrastructure;

namespace ThinkSharp.Solutions.Test.Infrastructure
{
    [TestClass]
    public class IOHelperTest
    {
        [TestMethod]
        public void TestReplaceGuids()
        {
            var isGuidRegex = new Regex("[0-91-f]{8}-[0-91-f]{4}-[0-91-f]{4}-[0-91-f]{4}-[0-91-f]{12}");

            IOHelper.ReplaceGuids("Infrastructure");

            var lines = File.ReadAllLines("Infrastructure\\GuidTestFile.txt");

            foreach (var line in lines)
            {
                Assert.IsTrue(isGuidRegex.IsMatch(line.Trim('"')));
            }

            //00000000 - 1111 - 0000 - 1111 - 000000000000
            //00000000 - 1111 - 0000 - 1111 - 000000000001
            //00000000 - 1111 - 0000 - 1111 - 000000000000
            //00000000 - 1111 - 0000 - 1111 - 000000000001
            //00000000 - 1111 - 0000 - 1111 - 000000000002
            //00000000 - 1111 - 0000 - 1111 - 300000000000
            //"00000000-1111-0000-1111-000000000002"

            Assert.AreEqual(lines[1], lines[3]);
            Assert.AreEqual("\"" + lines[4] + "\"", lines[6]);

            var guids = lines.Select(l => l.Trim('"')).ToArray();

            Assert.AreEqual(5, guids.Distinct().Count());

            Assert.AreEqual(0, guids.Where(g => g.StartsWith("00000000")).Count());
        }
    }
}
