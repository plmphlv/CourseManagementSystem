using Application.Common.Interfaces;
using Moq;

namespace Tests.Tests
{
    [TestFixture]
    public class Tests
    {
        private Mock<IIdentityService> identityService = null!;


        [SetUp]
        public async Task Setup()
        {
            identityService = new Mock<IIdentityService>();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [TearDown]
        public async Task TearDown()
        {

        }

    }
}
