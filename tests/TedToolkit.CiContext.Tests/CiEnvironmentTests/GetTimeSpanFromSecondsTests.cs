// -----------------------------------------------------------------------
// <copyright file="GetTimeSpanFromSecondsTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetTimeSpanFromSecondsTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_SECONDS";

    /// <summary>
    /// Verifies signed integer-second duration conversion.
    /// </summary>
    [Test]
    [Arguments("90", 90)]
    [Arguments("-90", -90)]
    [Arguments("0", 0)]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_parse_a_signed_integer_number_of_seconds(string value, int expectedSeconds)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, value);

        await Assert.That(CiEnvironment.GetTimeSpanFromSeconds(VARIABLE_NAME))
            .IsEqualTo(TimeSpan.FromSeconds(expectedSeconds));
    }

    /// <summary>
    /// Verifies that an absent seconds duration returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_seconds_value_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetTimeSpanFromSeconds(VARIABLE_NAME)).IsNull();
    }

    /// <summary>
    /// Verifies malformed and out-of-range second counts are rejected without disclosure.
    /// </summary>
    [Test]
    [Arguments("1.5")]
    [Arguments("922337203686")]
    [Arguments("-922337203686")]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_an_invalid_second_count_without_disclosing_it(string invalidValue)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, invalidValue);

        var exception = await Assert.That(() => CiEnvironment.GetTimeSpanFromSeconds(VARIABLE_NAME))
            .Throws<FormatException>();

        await Assert.That(exception!.Message.Contains(invalidValue, StringComparison.Ordinal)).IsFalse();
    }
}