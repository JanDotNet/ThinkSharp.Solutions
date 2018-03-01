using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThinkSharp.Solutions.TemplateDefinition;

namespace ThinkSharp.Solutions.Test.TemplateDefinition
{
    [TestClass]
    public class TemplateDefinitionHelperTest
    {
        [TestMethod]
        public void TestSplit()
        {
            var splitted = TemplateDefinitionHelper.SplitEscapedString("1,2,3", ',');

            Assert.AreEqual(3, splitted.Length);
            Assert.AreEqual("1", splitted[0]);
            Assert.AreEqual("2", splitted[1]);
            Assert.AreEqual("3", splitted[2]);


            splitted = TemplateDefinitionHelper.SplitEscapedString("1\\,2\\,3", ',');
            Assert.AreEqual(1, splitted.Length);
            Assert.AreEqual("1,2,3", splitted[0]);

            splitted = TemplateDefinitionHelper.SplitEscapedString("1 , 2,3", ',');
            Assert.AreEqual(3, splitted.Length);
            Assert.AreEqual("1", splitted[0]);
            Assert.AreEqual("2", splitted[1]);
            Assert.AreEqual("3", splitted[2]);
        }

        [TestMethod]
        public void TestParse_TestFile01()
        {
            var definition = TemplateDefinitionHelper.Parse("TemplateDefinition\\TestFile01.xml");
            Assert.IsNotNull(definition);

            Assert.AreEqual(2, definition.Placeholders.Length);

            Assert.AreEqual("MODULE1", definition.Placeholders[0].Name);
            Assert.AreEqual("Hello World", definition.Placeholders[0].Description);
            Assert.AreEqual("123,456,788", definition.Placeholders[0].SuggestionList);
            Assert.AreEqual("123", definition.Placeholders[0].DefaultValue);

            Assert.AreEqual("MODULE2", definition.Placeholders[1].Name);
            Assert.AreEqual(null, definition.Placeholders[1].Description);
            Assert.AreEqual(null, definition.Placeholders[1].SuggestionList);
            Assert.AreEqual(null, definition.Placeholders[1].Description);

        }
    }
}

