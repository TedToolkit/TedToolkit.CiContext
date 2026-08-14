// -----------------------------------------------------------------------
// <copyright file="ExternalPullRequestTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using TedToolkit.GitLab;

namespace TedToolkit.CiContext.GitLab.Tests.GitLabCiTests;

internal sealed class ExternalPullRequestTests
{
    /// <summary>
    /// Verifies that external pull request variables are available directly from the root without a synthetic availability phase.
    /// </summary>
    [Test]
    public async Task Should_expose_external_pull_request_without_context_specific_layer()
    {
        var branchName = typeof(GitLabCi.ExternalPullRequest.Source)
            .GetProperty(nameof(GitLabCi.ExternalPullRequest.Source.BranchName));
        var contextSpecific = typeof(GitLabCi)
            .GetNestedType("ContextSpecific", BindingFlags.Public);

        await Assert.That(branchName).IsNotNull();
        await Assert.That(contextSpecific).IsNull();
    }
}