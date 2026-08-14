// -----------------------------------------------------------------------
// <copyright file="GitHubActionsRunnerArchitecture.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitHub;

/// <summary>
/// Represents an architecture documented for a GitHub Actions runner.
/// </summary>
public enum GitHubActionsRunnerArchitecture
{
    /// <summary>
    /// The runner uses the 32-bit x86 architecture.
    /// </summary>
    [CiEnumValue("X86")]
    X86 = 0,

    /// <summary>
    /// The runner uses the 64-bit x86 architecture.
    /// </summary>
    [CiEnumValue("X64")]
    X64 = 1,

    /// <summary>
    /// The runner uses the 32-bit ARM architecture.
    /// </summary>
    [CiEnumValue("ARM")]
    ARM = 2,

    /// <summary>
    /// The runner uses the 64-bit ARM architecture.
    /// </summary>
    [CiEnumValue("ARM64")]
    ARM64 = 3,
}