// -----------------------------------------------------------------------
// <copyright file="GetDateTimeOffsetTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetDateTimeOffsetTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_TIMESTAMP";

    /// <summary>
    /// Verifies invariant ISO 8601 timestamp conversion.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_parse_an_iso_8601_timestamp()
    {
        const string value = "2026-08-14T12:34:56.789+08:00";
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, value);
        var expected = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        await Assert.That(CiEnvironment.GetDateTimeOffset(VARIABLE_NAME)).IsEqualTo(expected);
    }

    /// <summary>
    /// Verifies that an absent timestamp returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_timestamp_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetDateTimeOffset(VARIABLE_NAME)).IsNull();
    }

    /// <summary>
    /// Verifies that malformed timestamp text is rejected without disclosure.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_a_malformed_timestamp_without_disclosing_it()
    {
        const string invalidValue = "sensitive-invalid-timestamp";
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, invalidValue);

        var exception = await Assert.That(() => CiEnvironment.GetDateTimeOffset(VARIABLE_NAME))
            .Throws<FormatException>();

        await Assert.That(exception!.Message.Contains(invalidValue, StringComparison.Ordinal)).IsFalse();
    }
}