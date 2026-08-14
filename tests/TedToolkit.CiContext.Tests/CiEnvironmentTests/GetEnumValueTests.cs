// -----------------------------------------------------------------------
// <copyright file="GetEnumValueTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetEnumValueTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_ENUM";

    /// <summary>
    /// Verifies that exact mapped text returns the corresponding enum member.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_the_member_for_an_exact_mapped_value()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "mapped-value");

        await Assert.That(CiEnvironment.GetEnumValue<MappedValue>(VARIABLE_NAME)).IsEqualTo(MappedValue.MAPPED);
    }

    /// <summary>
    /// Verifies that mapped enum comparison is ordinal and case-sensitive.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_a_differently_cased_value()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "MAPPED-VALUE");

        await Assert.That(() => CiEnvironment.GetEnumValue<MappedValue>(VARIABLE_NAME)).Throws<FormatException>();
    }

    /// <summary>
    /// Verifies that an absent mapped enum variable returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_mapped_enum_variable_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetEnumValue<MappedValue>(VARIABLE_NAME)).IsNull();
    }

    /// <summary>
    /// Verifies that an unknown enum value is rejected without disclosing its text.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_an_unknown_value_without_disclosing_it()
    {
        const string unknownValue = "sensitive-unknown-value";
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, unknownValue);

        FormatException? exception = null;
        try
        {
            _ = CiEnvironment.GetEnumValue<MappedValue>(VARIABLE_NAME);
        }
        catch (FormatException caughtException)
        {
            exception = caughtException;
        }

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message.Contains(VARIABLE_NAME, StringComparison.Ordinal)).IsTrue();
        await Assert.That(exception.Message.Contains(unknownValue, StringComparison.Ordinal)).IsFalse();
    }

    /// <summary>
    /// Verifies that an enum member without mapping metadata is rejected as a definition error.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_an_enum_with_an_unmapped_member()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "mapped");

        await Assert.That(() => CiEnvironment.GetEnumValue<PartiallyMappedValue>(VARIABLE_NAME))
            .Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that duplicate enum mapping metadata is rejected as a definition error.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_duplicate_mapped_values()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "duplicate");

        await Assert.That(() => CiEnvironment.GetEnumValue<DuplicateMappedValue>(VARIABLE_NAME))
            .Throws<InvalidOperationException>();
    }

    private enum MappedValue
    {
        [CiEnumValue("mapped-value")]
        MAPPED = 0,
    }

    private enum PartiallyMappedValue
    {
        [CiEnumValue("mapped")]
        MAPPED = 0,

        UNMAPPED = 1,
    }

    private enum DuplicateMappedValue
    {
        [CiEnumValue("duplicate")]
        FIRST = 0,

        [CiEnumValue("duplicate")]
        SECOND = 1,
    }
}