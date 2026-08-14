// -----------------------------------------------------------------------
// <copyright file="ConstructorTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnumValueAttributeTests;

internal sealed class ConstructorTests
{
    /// <summary>
    /// Verifies that enum mapping metadata exposes exact values and rejects empty input.
    /// </summary>
    [Test]
    public async Task Should_require_an_exact_non_empty_value()
    {
        var attribute = new CiEnumValueAttribute("exact-value");

        await Assert.That(attribute.Value).IsEqualTo("exact-value");
        await Assert.That(() => new CiEnumValueAttribute("")).Throws<ArgumentException>();
    }
}