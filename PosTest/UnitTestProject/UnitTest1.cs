using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace UnitTestProject
{
    [TestClass]
    class TestProductService
    {
        [TestMethod]
        public void TestMethod2()
        {
            Mock<IProductService> prodServ = new Mock<IProductService>();
            prodServ.Setup(p => p.CreateProducts()).Returns(new List<Product>());


            Assert.AreEqual(prodServ.Object.CreateProducts().Count, 0);
        }
    }
}
