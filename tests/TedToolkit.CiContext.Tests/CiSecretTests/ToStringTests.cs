// -----------------------------------------------------------------------
// <copyright file="ToStringTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiSecretTests;

internal sealed class ToStringTests
{
    /// <summary>
    /// Verifies that short values remain fully redacted.
    /// </summary>
    [Test]
    public async Task Should_fully_redact_a_short_value()
    {
        var secret = new CiSecret<string>("top-secret");

        await Assert.That(secret.ToString()).IsEqualTo("[REDACTED]");
#if NET6_0_OR_GREATER
        await Assert.That($"Value: {secret}").IsEqualTo("Value: [REDACTED]");
#endif
    }

    /// <summary>
    /// Verifies that generated secrets expose only the final four identifying characters.
    /// </summary>
    [Test]
    public async Task Should_show_only_the_final_four_characters_of_a_generated_secret()
    {
        CiSecret<string> secret = "glpat-1234567890";

        await Assert.That(secret.ToString()).IsEqualTo("[REDACTED]7890");
#if NET6_0_OR_GREATER
        await Assert.That($"Value: {secret}").IsEqualTo("Value: [REDACTED]7890");
#endif
    }

    /// <summary>
    /// Verifies that partial display requires at least eight hidden characters.
    /// </summary>
    [Test]
    public async Task Should_apply_the_length_boundary_safely()
    {
        CiSecret<string> elevenCharacters = "12345678901";
        CiSecret<string> twelveCharacters = "123456789012";

        await Assert.That(elevenCharacters.ToString()).IsEqualTo("[REDACTED]");
        await Assert.That(twelveCharacters.ToString()).IsEqualTo("[REDACTED]9012");
        await Assert.That(default(CiSecret<string>).ToString()).IsEqualTo("[REDACTED]");
    }
}