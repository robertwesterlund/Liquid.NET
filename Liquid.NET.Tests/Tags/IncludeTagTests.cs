﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Liquid.NET.Tests.Helpers;
using NUnit.Framework;

namespace Liquid.NET.Tests.Tags
{
    [TestFixture]
    public class IncludeTagTests
    {
        [Test]
        public void It_Should_Include_A_Virtual_File()
        {
            // Arrange
            var ctx = CreateContext(new Dictionary<String,String>{{"test", "Test Snippet"}});
            
            //ctx.Define("payments", new ArrayValue(new List<IExpressionConstant>()));

            const String str = "{% include 'test' %}";

            // Act
            var result = RenderingHelper.RenderTemplate(str, ctx);

            // Assert
            Assert.That(result, Is.EqualTo("Test Snippet"));

        }

        [Test]
        public void It_Should_Include_A_Virtual_File_With_With()
        {
            // Arrange
            var ctx = CreateContext(new Dictionary<String, String>());
            //ctx.Define("payments", new ArrayValue(new List<IExpressionConstant>()));

            const String str = "{% include 'test' with 'green' %}";

            // Act
            var result = RenderingHelper.RenderTemplate(str, ctx);

            // Assert
            Assert.That(result, Is.EqualTo("Test Snippet: green"));

        }

        [Test]
        public void It_Should_Include_A_Virtual_File_With_A_Variable_Value()
        {
            // Arrange
            var ctx = CreateContext(new Dictionary<String, String>());
            //ctx.Define("payments", new ArrayValue(new List<IExpressionConstant>()));

            const String str = "{%assign colour = 'green' %} {% include 'test' with colour %}";

            // Act
            var result = RenderingHelper.RenderTemplate(str, ctx);

            // Assert
            Assert.That(result, Is.EqualTo("Test Snippet: green"));

        }

        private static ITemplateContext CreateContext(Dictionary<String, String> dict) 
        {
            return new TemplateContext().WithFileSystem(new TestFileSystem(dict));
        }
    }
}
