using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using PR16.Models;
using PR16.Services;
using PR16.View;

namespace UnitTestProject1
{
    [TestClass]
    public class CartPriceTests
    {
        [TestMethod]
        public void CalculateTotalNormal()
        {
            var view = new AuthorizedClientView(null);
            var items = new List<Товар>
            {
                new Товар { Цена = 1000, ДействующаяСкидка = 10 },
                new Товар { Цена = 2000, ДействующаяСкидка = 0 }
            };

            decimal result = view.CalculateTotal(items);

            Assert.AreEqual(2900, result);
        }

        [TestMethod]
        public void CalculateTotalMiddle()
        {
            var view = new AuthorizedClientView(null);

            Assert.AreEqual(0, view.CalculateTotal(null));
            Assert.AreEqual(0, view.CalculateTotal(new List<Товар>()));
        }

        [TestMethod]
        public void CalculateTotalExtreme()
        {
            var view = new AuthorizedClientView(null);
            var items = new List<Товар>
            {
                new Товар { Цена = 5000, ДействующаяСкидка = 100 },
                new Товар { Цена = 0, ДействующаяСкидка = 15 }
            };

            decimal result = view.CalculateTotal(items);

            Assert.AreEqual(0, result);
        }
    }
}
