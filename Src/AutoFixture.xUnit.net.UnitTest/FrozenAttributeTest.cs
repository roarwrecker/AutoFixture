﻿using System;
using System.Linq;
using System.Reflection;
using Ploeh.TestTypeFoundation;
using Xunit;

namespace Ploeh.AutoFixture.Xunit.UnitTest
{
    public class FrozenAttributeTest
    {
        [Fact]
        public void SutIsAttribute()
        {
            // Fixture setup
            // Exercise system
            var sut = new FrozenAttribute();
            // Verify outcome
            Assert.IsAssignableFrom<CustomizeAttribute>(sut);
            // Teardown
        }

        [Fact]
        public void InitializeShouldSetDefaultMatchingStrategy()
        {
            // Fixture setup
            // Exercise system
            var sut = new FrozenAttribute();
            // Verify outcome
            Assert.Equal(Matching.ExactType, sut.By);
            // Teardown
        }

        [Fact]
        public void GetCustomizationFromNullParameterThrows()
        {
            // Fixture setup
            var sut = new FrozenAttribute();
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                sut.GetCustomization(null));
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithSpecificTypeShouldReturnCorrectResult()
        {
            // Fixture setup
            var registeredType = typeof(AbstractType);
#pragma warning disable 0618
            var sut = new FrozenAttribute { As = registeredType };
#pragma warning restore 0618
            var parameter = AParameter<ConcreteType>();
            // Exercise system
            var result = sut.GetCustomization(parameter);
            // Verify outcome
            var freezer = Assert.IsAssignableFrom<FreezingCustomization>(result);
            Assert.Equal(registeredType, freezer.RegisteredType);
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithIncompatibleSpecificTypeThrowsArgumentException()
        {
            // Fixture setup
            var registeredType = typeof(string);
#pragma warning disable 0618
            var sut = new FrozenAttribute { As = registeredType };
#pragma warning restore 0618
            var parameter = AParameter<ConcreteType>();
            // Exercise system and verify outcome
            Assert.Throws<ArgumentException>(() => sut.GetCustomization(parameter));
            // Teardown
        }

        private static ParameterInfo AParameter<T>()
        {
            return typeof(SingleParameterType<T>)
                .GetConstructor(new[] { typeof(T) })
                .GetParameters()
                .Single(p => p.Name == "parameter");
        }
    }
}
