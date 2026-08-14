// -----------------------------------------------------------------------
// <copyright file="ConstructorTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiSecretTests;

internal sealed class ConstructorTests
{
    /// <summary>
    /// Verifies that the wrapper is an allocation-free value type.
    /// </summary>
    [Test]
    public async Task Should_be_a_readonly_value_type()
    {
        var type = typeof(CiSecret<string>);

        await Assert.That(type.IsValueType).IsTrue();
        await Assert.That(type.IsDefined(typeof(System.Runtime.CompilerServices.IsReadOnlyAttribute), false)).IsTrue();
    }

    /// <summary>
    /// Verifies that a null reference value cannot be wrapped as a secret.
    /// </summary>
    [Test]
    public async Task Should_reject_a_null_value()
    {
        await Assert.That(() => new CiSecret<string>(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that a protected value can be supplied without an explicit constructor call.
    /// </summary>
    [Test]
    public async Task Should_implicitly_wrap_the_source_value()
    {
        CiSecret<string> secret = "token";

        await Assert.That(secret.Reveal()).IsEqualTo("token");
    }
}