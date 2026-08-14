// -----------------------------------------------------------------------
// <copyright file="AvailabilityMetadataTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using TedToolkit.GitLab;

namespace TedToolkit.CiContext.GitLab.Tests.GitLabCiMetadataTests;

internal sealed class AvailabilityMetadataTests
{
    /// <summary>
    /// Verifies that GitLab's three official availability phases are metadata on their navigation classes.
    /// </summary>
    [Test]
    public async Task Should_attach_official_availability_to_phase_classes()
    {
        await AssertPhaseAvailability(
            typeof(GitLabCi.PrePipeline),
            GitLabCiVariableAvailability.PRE_PIPELINE).ConfigureAwait(false);
        await AssertPhaseAvailability(
            typeof(GitLabCi.Pipeline),
            GitLabCiVariableAvailability.PIPELINE).ConfigureAwait(false);
        await AssertPhaseAvailability(
            typeof(GitLabCi.JobOnly),
            GitLabCiVariableAvailability.JOB_ONLY).ConfigureAwait(false);
        await Assert.That(typeof(GitLabCi.ExternalPullRequest)
            .GetCustomAttribute<GitLabCiVariableAvailabilityAttribute>()).IsNull();
    }

    private static async Task AssertPhaseAvailability(Type type, GitLabCiVariableAvailability expected)
    {
        var attribute = type.GetCustomAttribute<GitLabCiVariableAvailabilityAttribute>();

        await Assert.That(attribute).IsNotNull();
        await Assert.That(attribute!.Availability).IsEqualTo(expected);
    }
}