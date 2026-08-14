// -----------------------------------------------------------------------
// <copyright file="GitLabEnvironmentAction.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies a GitLab environment action.
/// </summary>
public enum GitLabEnvironmentAction
{
    /// <summary>
    /// Starts an environment.
    /// </summary>
    [CiEnumValue("start")]
    START = 0,

    /// <summary>
    /// Prepares an environment without creating a deployment.
    /// </summary>
    [CiEnumValue("prepare")]
    PREPARE = 1,

    /// <summary>
    /// Stops an environment.
    /// </summary>
    [CiEnumValue("stop")]
    STOP = 2,

    /// <summary>
    /// Verifies an environment.
    /// </summary>
    [CiEnumValue("verify")]
    VERIFY = 3,

    /// <summary>
    /// Accesses an environment without creating a deployment.
    /// </summary>
    [CiEnumValue("access")]
    ACCESS = 4,
}