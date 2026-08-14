// -----------------------------------------------------------------------
// <copyright file="GetCommaSeparatedListTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetCommaSeparatedListTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_COMMA_LIST";

    private static readonly string[] _expectedItems = ["alpha", "beta", "gamma",];

    /// <summary>
    /// Verifies that comma-separated items are trimmed and empty items are omitted.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_parse_trimmed_non_empty_items()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, " alpha, ,beta,, gamma ");

        await Assert.That(CiEnvironment.GetCommaSeparatedList(VARIABLE_NAME))
            .IsEquivalentTo(_expectedItems);
    }

    /// <summary>
    /// Verifies the distinction between an absent list and a present empty list.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_distinguish_an_absent_list_from_an_empty_list()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetCommaSeparatedList(VARIABLE_NAME)).IsNull();

        Environment.SetEnvironmentVariable(VARIABLE_NAME, ",, ");
        await Assert.That(CiEnvironment.GetCommaSeparatedList(VARIABLE_NAME)).IsEmpty();
    }
}