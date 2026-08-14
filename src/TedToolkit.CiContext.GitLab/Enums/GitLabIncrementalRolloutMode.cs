// -----------------------------------------------------------------------
// <copyright file="GitLabIncrementalRolloutMode.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies the Auto DevOps incremental rollout strategy.
/// </summary>
public enum GitLabIncrementalRolloutMode
{
    /// <summary>
    /// Deployments progress through manually started jobs.
    /// </summary>
    [CiEnumValue("manual")]
    MANUAL = 0,

    /// <summary>
    /// Deployments progress through delayed jobs.
    /// </summary>
    [CiEnumValue("timed")]
    TIMED = 1,
}