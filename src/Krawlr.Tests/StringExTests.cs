using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Krawlr.Core.Extensions;

namespace Krawlr.Tests
{
    [TestClass]
    public class StringExTests
    {
        [TestMethod]
        public void RemoveTrailing_ShouldHandleEmpty()
        {
            string expected = "";
            string actual = "".RemoveTrailing('/');
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RemoveTrailing_ShouldHandleSingle()
        {
            string expected = "";
            string actual = "/".RemoveTrailing('/');
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RemoveTrailing_ShouldHandle_RealUrl()
        {
            string expected = "/something/awesome";
            string actual = "/something/awesome/".RemoveTrailing('/');
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RemoveTrailing_ShouldHandle_RealUrlWithoutTrailing()
        {
            string expected = "something/awesome";
            string actual = "something/awesome".RemoveTrailing('/');
            Assert.AreEqual(expected, actual);
        }
    }
}
