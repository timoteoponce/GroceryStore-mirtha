using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroceryStore.Tests
{
    [TestClass]
    public class CustomerTest
    {
        [TestMethod]
        public void TestCreation()
        {
            var c = new Customer { Id = 1, FirstName = "Kevin", LastName = "Arnold", Age = 16 };
            Assert.AreEqual(16, c.Age);
        }
    }
}
