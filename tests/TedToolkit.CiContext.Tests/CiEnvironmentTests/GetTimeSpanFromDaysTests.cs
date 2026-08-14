// -----------------------------------------------------------------------
// <copyright file="GetTimeSpanFromDaysTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetTimeSpanFromDaysTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_DAYS";

    /// <summary>
    /// Verifies non-negative integer-day duration conversion.
    /// </summary>
    [Test]
    [Arguments("0", 0)]
    [Arguments("90", 90)]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_parse_a_non_negative_integer_number_of_days(string value, int expectedDays)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, value);

        await Assert.That(CiEnvironment.GetTimeSpanFromDays(VARIABLE_NAME))
            .IsEqualTo(TimeSpan.FromDays(expectedDays));
    }

    /// <summary>
    /// Verifies that an absent days duration returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_days_value_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetTimeSpanFromDays(VARIABLE_NAME)).IsNull();
    }

    /// <summary>
    /// Verifies negative, malformed, and out-of-range day counts are rejected without disclosure.
    /// </summary>
    [Test]
    [Arguments("-1")]
    [Arguments("1.5")]
    [Arguments("10675200")]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_an_invalid_day_count_without_disclosing_it(string invalidValue)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, invalidValue);

        var exception = await Assert.That(() => CiEnvironment.GetTimeSpanFromDays(VARIABLE_NAME))
            .Throws<FormatException>();

        await Assert.That(exception!.Message.Contains(invalidValue, StringComparison.Ordinal)).IsFalse();
    }
}