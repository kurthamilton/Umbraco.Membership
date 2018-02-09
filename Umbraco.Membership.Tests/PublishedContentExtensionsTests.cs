using Moq;
using NUnit.Framework;
using Umbraco.Core.Models;

namespace Umbraco.Membership.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public static class PublishedContentExtensionsTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void IsRestricted_ContentDoesNotHaveProperty_AlwaysReturnsFalse(bool authorised)
        {
            IPublishedContent member = CreateMockMember(authorised);
            IPublishedContent content = CreateMockContent();

            bool result = content.IsRestricted(member);

            Assert.IsFalse(result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void IsRestricted_ContentIsNotRestricted_AlwaysReturnsFalse(bool authorised)
        {
            IPublishedContent member = CreateMockMember(authorised);
            IPublishedContent content = CreateMockContent(false);

            bool result = content.IsRestricted(member);

            Assert.IsFalse(result);
        }

        [TestCase(true, ExpectedResult = false)]
        [TestCase(false, ExpectedResult = true)]
        public static bool IsRestricted_ContentIsRestricted_ReturnsInverseOfAuthorised(bool authorised)
        {
            IPublishedContent member = CreateMockMember(authorised);
            IPublishedContent content = CreateMockContent(true);

            bool result = content.IsRestricted(member);

            return result;
        }

        private static IPublishedContent CreateMockContent(bool? restricted = null)
        {
            Mock<IPublishedContent> mock = new Mock<IPublishedContent>();

            if (restricted.HasValue)
            {
                Mock<IPublishedProperty> property = new Mock<IPublishedProperty>();
                property.Setup(x => x.Value).Returns(restricted.Value.ToString());

                mock.Setup(x => x.GetProperty("restricted")).Returns(property.Object);
            }

            return mock.Object;
        }

        private static IPublishedContent CreateMockMember(bool authorised = false)
        {
            return authorised ? Mock.Of<IPublishedContent>() : null;
        }
    }
}