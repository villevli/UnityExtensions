using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace VLExtensions.Tests
{
    public class CSVParser_Tests
    {
        private static string GetCallerFilePath([CallerFilePath] string path = null) => path;

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("abc;123", "abc;123")]
        [TestCase("abc,123", "\"abc,123\"")]
        [TestCase("abc\n123", "\"abc\n123\"")]
        [TestCase("abc \"123\"", "\"abc \"\"123\"\"\"")]
        public void Escape_ReturnsExpected(string s, string expected)
        {
            var result = CSVParser.Escape(s);

            Assert.AreEqual(expected, result);
        }

        [TestCase("abc;123", "\"abc;123\"")]
        [TestCase("abc,123", "abc,123")]
        public void Escape_WithSemicolon_ReturnsExpected(string s, string expected)
        {
            var result = CSVParser.Escape(s, ';');

            Assert.AreEqual(expected, result);
        }

        [TestCase('\n')]
        [TestCase('\r')]
        [TestCase('"')]
        public void Parse_InvalidSeparator_Throws(char s)
        {
            Assert.Throws<ArgumentException>(() => CSVParser.Parse("", s));
        }

        [Test]
        public void Parse_TextCsv_ReturnsExpectedData()
        {
            var csvFilePath = Path.ChangeExtension(GetCallerFilePath(), "text.csv");
            var csv = File.ReadAllText(csvFilePath);
            // normalize line endings to \r\n
            csv = csv.Replace("\r\n", "\n").Replace("\n", "\r\n");

            List<string> expectedCol1 = new()
            {
                "text",
                "comma,intext",
                "space intext",
                "quote\"intext",
                "\"quotes\"",
                "empty col >",
                "newline\r\nintext",
                "\r\nnewlines\r\n",
                "comma,space quote\"and\r\nnewline",
                "last empty >",
            };
            List<string> expectedCol2 = new()
            {
                "text",
                "comma,intext",
                "space intext",
                "quote\"intext",
                "\"quotes\"",
                "",
                "newline\r\nintext",
                "\r\nnewlines\r\n",
                "comma,space quote\"and\r\nnewline",
                "",
            };
            List<List<string>> expectedData = new()
            {
                expectedCol1,
                expectedCol2
            };

            var data = CSVParser.Parse(csv);

            Assert.AreEqual(expectedData, data);
        }
    }
}
