// -----------------------------------------------------------------------
// <copyright file="GetInt64Tests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetInt64Tests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_INT64";

    /// <summary>
    /// Verifies invariant 64-bit integer conversion.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_an_invariant_64_bit_integer()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "9223372036854775806");

        await Assert.That(CiEnvironment.GetInt64(VARIABLE_NAME)).IsEqualTo(9223372036854775806L);
    }
}