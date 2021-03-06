﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ploeh.AutoFixture.DataAnnotations;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixtureUnitTest.Kernel;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.AutoFixtureUnitTest.DataAnnotations
{
    public class StringLengthAttributeRelayTest
    {
        [Fact]
        public void SutIsSpecimenBuilder()
        {
            // Fixture setup
            // Exercise system
            var sut = new StringLengthAttributeRelay();
            // Verify outcome
            Assert.IsAssignableFrom<ISpecimenBuilder>(sut);
            // Teardown
        }

        [Fact]
        public void CreateWithNullRequestReturnsCorrectResult()
        {
            // Fixture setup
            var sut = new StringLengthAttributeRelay();
            // Exercise system
            var dummyContext = new DelegatingSpecimenContext();
            var result = sut.Create(null, dummyContext);
            // Verify outcome
            Assert.Equal(new NoSpecimen(), result);
            // Teardown
        }

        [Fact]
        public void CreateWithNullContextThrows()
        {
            // Fixture setup
            var sut = new StringLengthAttributeRelay();
            var dummyRequest = new object();
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                sut.Create(dummyRequest, null));
            // Teardown
        }

        [Fact]
        public void CreateWithAnonymousRequestReturnsCorrectResult()
        {
            // Fixture setup
            var sut = new StringLengthAttributeRelay();
            var dummyRequest = new object();
            // Exercise system
            var dummyContainer = new DelegatingSpecimenContext();
            var result = sut.Create(dummyRequest, dummyContainer);
            // Verify outcome
#pragma warning disable 618
            var expectedResult = new NoSpecimen(dummyRequest);
#pragma warning restore 618
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(1)]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(Version))]
        public void CreateWithNonConstrainedStringRequestReturnsCorrectResult(object request)
        {
            // Fixture setup
            var sut = new StringLengthAttributeRelay();
            // Exercise system
            var dummyContext = new DelegatingSpecimenContext();
            var result = sut.Create(request, dummyContext);
            // Verify outcome
#pragma warning disable 618
            var expectedResult = new NoSpecimen(request);
#pragma warning restore 618
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CreateWithConstrainedStringRequestReturnsCorrectResult(int maximum)
        {
            // Fixture setup
            var stringLengthAttribute = new StringLengthAttribute(maximum);
            var providedAttribute = new ProvidedAttribute(stringLengthAttribute, true);
            ICustomAttributeProvider request = new FakeCustomAttributeProvider(providedAttribute);
            var expectedRequest = new ConstrainedStringRequest(stringLengthAttribute.MaximumLength);
            var expectedResult = new object();
            var context = new DelegatingSpecimenContext
            {
#pragma warning disable 618
                OnResolve = r => expectedRequest.Equals(r) ? expectedResult : new NoSpecimen(r)
#pragma warning restore 618
            };
            var sut = new StringLengthAttributeRelay();
            // Exercise system
            var result = sut.Create(request, context);
            // Verify outcome
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CreateWithStringRequestConstrainedbyMinimunLengthReturnsCorrectResult(int maximum)
        {
            // Fixture setup
            var stringLengthAttribute = new StringLengthAttribute(maximum) { MinimumLength = 1 };
            var providedAttribute = new ProvidedAttribute(stringLengthAttribute, true);
            ICustomAttributeProvider request = new FakeCustomAttributeProvider(providedAttribute);
            var expectedRequest = new ConstrainedStringRequest(stringLengthAttribute.MinimumLength, stringLengthAttribute.MaximumLength);
            var expectedResult = new object();
            var context = new DelegatingSpecimenContext
            {
                OnResolve = r => expectedRequest.Equals(r) ? expectedResult : new NoSpecimen()
            };
            var sut = new StringLengthAttributeRelay();
            // Exercise system
            var result = sut.Create(request, context);
            // Verify outcome
            Assert.Equal(expectedResult, result);
            // Teardown
        }
    }
}
