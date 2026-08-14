// -----------------------------------------------------------------------
// <copyright file="CompletenessTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

namespace TedToolkit.CiContext.GitLab.Tests.EnumMappingsTests;

internal sealed class CompletenessTests
{
    private static readonly Type[] _mappedEnumTypes =
    [
        typeof(GitLabDeploymentTier),
        typeof(GitLabEnvironmentAction),
        typeof(GitLabIncrementalRolloutMode),
        typeof(GitLabJobStatus),
        typeof(GitLabMergeRequestEventType),
        typeof(GitLabPipelineSource),
        typeof(GitLabProjectVisibility),
        typeof(GitLabServerProtocol),
    ];

    /// <summary>
    /// Verifies that every parsed GitLab enum member declares exactly one official string mapping.
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

        await Assert.That(mappings).Count().IsEqualTo(38);
        await Assert.That(allMembersHaveOneMapping).IsTrue();
    }
}