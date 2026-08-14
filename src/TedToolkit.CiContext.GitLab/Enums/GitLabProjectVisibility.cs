// -----------------------------------------------------------------------
// <copyright file="GitLabProjectVisibility.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies GitLab project visibility.
/// </summary>
public enum GitLabProjectVisibility
{
    /// <summary>
    /// The project is visible to authenticated users.
    /// </summary>
    [CiEnumValue("internal")]
    INTERNAL = 0,

    /// <summary>
    /// The project is visible only to its members.
    /// </summary>
    [CiEnumValue("private")]
    PRIVATE = 1,

    /// <summary>
    /// The project is visible publicly.
    /// </summary>
    [CiEnumValue("public")]
    PUBLIC = 2,
}