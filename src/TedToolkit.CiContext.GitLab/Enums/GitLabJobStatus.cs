// -----------------------------------------------------------------------
// <copyright file="GitLabJobStatus.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies the current GitLab job status.
/// </summary>
public enum GitLabJobStatus
{
    /// <summary>
    /// The job succeeded.
    /// </summary>
    [CiEnumValue("success")]
    SUCCESS = 0,

    /// <summary>
    /// The job failed.
    /// </summary>
    [CiEnumValue("failed")]
    FAILED = 1,

    /// <summary>
    /// The job was canceled.
    /// </summary>
    [CiEnumValue("canceled")]
    CANCELED = 2,
}