// -----------------------------------------------------------------------
// <copyright file="GitLabCiVariableAvailability.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies when GitLab makes a predefined CI/CD variable available.
/// </summary>
public enum GitLabCiVariableAvailability
{
    /// <summary>
    /// The variable is available before GitLab creates the pipeline.
    /// </summary>
    PRE_PIPELINE = 0,

    /// <summary>
    /// The variable is available while GitLab creates the pipeline.
    /// </summary>
    PIPELINE = 1,

    /// <summary>
    /// The variable is available only while a runner executes a job.
    /// </summary>
    JOB_ONLY = 2,
}