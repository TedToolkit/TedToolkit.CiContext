// -----------------------------------------------------------------------
// <copyright file="GetAbsoluteUriTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetAbsoluteUriTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_URI";

    /// <summary>
    /// Verifies absolute URI conversion and rejection of relative input.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_only_an_absolute_uri()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "https://example.test/path");

        await Assert.That(CiEnvironment.GetAbsoluteUri(VARIABLE_NAME))
            .IsEqualTo(new Uri("https://example.test/path"));

        Environment.SetEnvironmentVariable(VARIABLE_NAME, "relative/path");
        await Assert.That(() => CiEnvironment.GetAbsoluteUri(VARIABLE_NAME)).Throws<FormatException>();
    }
}