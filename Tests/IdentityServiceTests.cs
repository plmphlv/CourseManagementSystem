using Application.Common.Interfaces;
using Application.Users.Commands.Register;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace Tests
{
    [TestFixture]
    public class IdentityServiceTests
    {
        private Mock<UserManager<User>> userManager = null!;
        private Mock<IApplicationDbContext> db = null!;
        private IdentityService service = null!;
        private User fakeUser = null!;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<User>>();

            userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            db = new Mock<IApplicationDbContext>();
            fakeUser = new User { Id = "1", UserName = "john", Email = "john@x" };

            service = new IdentityService(userManager.Object, db.Object);

            userManager.Setup(u => u.Users).Returns(new[] { fakeUser }.AsQueryable());
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnsSuccess()
        {
            userManager.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(fakeUser);

            userManager.Setup(u => u.ChangePasswordAsync(fakeUser, "old", "new"))
                       .ReturnsAsync(IdentityResult.Success);

            var result = await service.ChangePasswordAsync("1", "old", "new", default);

            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task CreateUserAsync_Calls_SaveChanges()
        {
            var model = new RegisterCommand
            {
                Email = "a@x",
                UserName = "john",
                FirstName = "A",
                LastName = "B",
                Password = "Pwd!23"
            };

            userManager.Setup(u => u.CreateAsync(It.IsAny<User>(), model.Password))
                       .ReturnsAsync(IdentityResult.Success);

            userManager.Setup(u => u.AddClaimsAsync(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
                       .ReturnsAsync(IdentityResult.Success);

            db.Setup(d => d.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await service.CreateUserAsync(model, default);

            result.Succeeded.Should().BeTrue();

            db.Verify(d => d.SaveChangesAsync(default), Times.Once);
        }
    }

}
