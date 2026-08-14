// -----------------------------------------------------------------------
// <copyright file="GitHubActionsRunnerOperatingSystem.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitHub;

/// <summary>
/// Represents an operating system documented for a GitHub Actions runner.
/// </summary>
public enum GitHubActionsRunnerOperatingSystem
{
    /// <summary>
    /// The runner uses Linux.
    /// </summary>
    [CiEnumValue("Linux")]
    LINUX = 0,

    /// <summary>
    /// The runner uses Windows.
    /// </summary>
    [CiEnumValue("Windows")]
    WINDOWS = 1,

    /// <summary>
    /// The runner uses macOS.
    /// </summary>
    [CiEnumValue("macOS")]
    MAC_OS = 2,
}