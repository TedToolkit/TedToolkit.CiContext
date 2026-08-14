// -----------------------------------------------------------------------
// <copyright file="CompletenessTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

namespace TedToolkit.CiContext.GitHubActions.Tests.EnumMappingsTests;

internal sealed class CompletenessTests
{
    private static readonly Type[] _mappedEnumTypes =
    [
        typeof(GitHubActionsRefType),
        typeof(GitHubActionsRunnerArchitecture),
        typeof(GitHubActionsRunnerEnvironment),
        typeof(GitHubActionsRunnerOperatingSystem),
    ];

    /// <summary>
    /// Verifies that every parsed GitHub Actions enum member declares exactly one official string mapping.
    /// </summary>
    [Test]
    public async Task Should_map_every_parsed_enum_member_once()
    {
        var fields = _mappedEnumTypes
            .SelectMany(static type => type.GetFields(BindingFlags.Public | BindingFlags.Static))
            .ToArray();
        var mappings = fields.SelectMany(static field => field.GetCustomAttributes<CiEnumValueAttribute>()).ToArray();
        var allMembersHaveOneMapping = fields
            .All(static field => field.GetCustomAttributes<CiEnumValueAttribute>().Count() == 1);

        await Assert.That(mappings).Count().IsEqualTo(11);
        await Assert.That(allMembersHaveOneMapping).IsTrue();
    }
}