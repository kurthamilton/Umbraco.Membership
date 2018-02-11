using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Models;

namespace ODK.Umbraco.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public static class MembershipTests
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

        [Test]
        public static void PermittedChildren_DoesNotReturnHiddenItems()
        {
            IPublishedContent[] children =
            {
                CreateMockContent(visible: true),
                CreateMockContent(visible: false)
            };

            IPublishedContent content = CreateMockContent(children: children);

            IPublishedContent[] result = content.PermittedChildren(null).ToArray();

            CollectionAssert.AreEqual(children.Take(1), result);
        }

        [Test]
        public static void PermittedChildren_DoesNotReturnRestrictedItems()
        {
            IPublishedContent[] children =
            {
                CreateMockContent(restricted: true),
                CreateMockContent(restricted: false)
            };

            IPublishedContent content = CreateMockContent(children: children);

            IPublishedContent[] result = content.PermittedChildren(null).ToArray();

            CollectionAssert.AreEqual(children.Skip(1), result);
        }

        private static IPublishedContent CreateMockContent(bool? restricted = null, bool visible = true, 
            IEnumerable<IPublishedContent> children = null)
        {
            Mock<IPublishedContent> mock = new Mock<IPublishedContent>();

            if (restricted.HasValue)
            {
                IPublishedProperty restrictedProperty = CreateMockProperty(restricted.Value);
                mock.Setup(x => x.GetProperty(PropertyNames.Restricted, It.IsAny<bool>())).Returns(restrictedProperty);
            }

            IPublishedProperty visibleProperty = CreateMockProperty((!visible).ToString());
            mock.Setup(x => x.GetProperty("umbracoNaviHide", It.IsAny<bool>())).Returns(visibleProperty);
            
            mock.Setup(x => x.Children).Returns(children ?? new IPublishedContent[] { });

            return mock.Object;
        }

        private static IPublishedContent CreateMockMember(bool authorised = false)
        {
            return authorised ? Mock.Of<IPublishedContent>() : null;
        }

        private static IPublishedProperty CreateMockProperty(object value)
        {
            Mock<IPublishedProperty> mock = new Mock<IPublishedProperty>();
            mock.Setup(x => x.Value).Returns(value);
            return mock.Object;
        }
    }
}