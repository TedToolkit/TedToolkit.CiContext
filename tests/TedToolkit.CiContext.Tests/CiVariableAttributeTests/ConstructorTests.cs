// -----------------------------------------------------------------------
// <copyright file="ConstructorTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiVariableAttributeTests;

internal sealed class ConstructorTests
{
    /// <summary>
    /// Verifies that the common variable Attribute stores an exact non-empty name.
    /// </summary>
    [Test]
    public async Task Should_expose_the_exact_variable_name()
    {
        var attribute = new CiVariableAttribute("EXACT_VARIABLE");

        await Assert.That(attribute.Name).IsEqualTo("EXACT_VARIABLE");
    }

    /// <summary>
    /// Verifies that the common variable Attribute rejects an empty name.
    /// </summary>
    [Test]
    public async Task Should_reject_an_empty_variable_name()
    {
        await Assert.That(() => new CiVariableAttribute("")).Throws<ArgumentException>();
    }
}