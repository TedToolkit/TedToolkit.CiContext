// -----------------------------------------------------------------------
// <copyright file="GetJsonStringArrayTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetJsonStringArrayTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_JSON_ARRAY";

    private static readonly string[] _expectedItems = ["linux", "docker",];

    private static readonly string[] _expectedEscapedItems = ["quoted \"value\"", "line\nbreak", "☺",];

    /// <summary>
    /// Verifies JSON string-array conversion.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_parse_a_json_string_array()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "[\"linux\",\"docker\"]");

        await Assert.That(CiEnvironment.GetJsonStringArray(VARIABLE_NAME))
            .IsEquivalentTo(_expectedItems);
    }

    /// <summary>
    /// Verifies that JSON string escapes are decoded without changing the item contract.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_decode_json_string_escapes()
    {
        using var environment = new EnvironmentVariableScope(
            VARIABLE_NAME,
            "[\"quoted \\\"value\\\"\",\"line\\nbreak\",\"\\u263a\"]");

        await Assert.That(CiEnvironment.GetJsonStringArray(VARIABLE_NAME))
            .IsEquivalentTo(_expectedEscapedItems);
    }

    /// <summary>
    /// Verifies that an absent JSON array returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_json_array_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetJsonStringArray(VARIABLE_NAME)).IsNull();
    }

    /// <summary>
    /// Verifies that non-array and non-string JSON content is rejected without revealing its value.
    /// </summary>
    [Test]
    [Arguments("{\"secret\":\"value\"}")]
    [Arguments("[1]")]
    [Arguments("[true]")]
    [Arguments("[\"value\",null]")]
    [Arguments("[{}]")]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_non_string_json_content_without_disclosing_it(string invalidValue)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, invalidValue);

        var exception = await Assert.That(() => CiEnvironment.GetJsonStringArray(VARIABLE_NAME))
            .Throws<FormatException>();

        await Assert.That(exception!.Message.Contains(invalidValue, StringComparison.Ordinal)).IsFalse();
    }
}