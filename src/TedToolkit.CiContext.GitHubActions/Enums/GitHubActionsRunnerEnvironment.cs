// -----------------------------------------------------------------------
// <copyright file="GitHubActionsRunnerEnvironment.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitHub;

/// <summary>
/// Represents whether GitHub or a workflow owner hosts the current runner.
/// </summary>
public enum GitHubActionsRunnerEnvironment
{
    /// <summary>
    /// GitHub hosts the runner.
    /// </summary>
    [CiEnumValue("github-hosted")]
    GIT_HUB_HOSTED = 0,

    /// <summary>
    /// The repository or organization owner hosts the runner.
    /// </summary>
    [CiEnumValue("self-hosted")]
    SELF_HOSTED = 1,
}