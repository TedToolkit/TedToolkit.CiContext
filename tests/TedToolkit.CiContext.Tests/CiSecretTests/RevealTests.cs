// -----------------------------------------------------------------------
// <copyright file="RevealTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiSecretTests;

internal sealed class RevealTests
{
    /// <summary>
    /// Verifies that explicit reveal returns the original value for reference and value types.
    /// </summary>
    [Test]
    public async Task Should_return_the_original_typed_value()
    {
        var textSecret = new CiSecret<string>("token");
        var numericSecret = new CiSecret<int>(42);

        await Assert.That(textSecret.Reveal()).IsEqualTo("token");
        await Assert.That(numericSecret.Reveal()).IsEqualTo(42);
    }

    /// <summary>
    /// Verifies that the default value cannot masquerade as an initialized secret.
    /// </summary>
    [Test]
    public async Task Should_reject_revealing_an_uninitialized_value()
    {
        var secret = default(CiSecret<string>);

        await Assert.That(secret.Reveal).Throws<InvalidOperationException>();
    }
}