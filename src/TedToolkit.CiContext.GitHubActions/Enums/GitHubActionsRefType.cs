// -----------------------------------------------------------------------
// <copyright file="GitHubActionsRefType.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitHub;

/// <summary>
/// Represents the documented kind of Git ref that triggered a GitHub Actions workflow.
/// </summary>
public enum GitHubActionsRefType
{
    /// <summary>
    /// The triggering ref is a branch.
    /// </summary>
    [CiEnumValue("branch")]
    BRANCH = 0,

    /// <summary>
    /// The triggering ref is a tag.
    /// </summary>
    [CiEnumValue("tag")]
    TAG = 1,
}