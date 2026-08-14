// -----------------------------------------------------------------------
// <copyright file="GitLabDeploymentTier.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies a GitLab environment deployment tier.
/// </summary>
public enum GitLabDeploymentTier
{
    /// <summary>
    /// A development environment.
    /// </summary>
    [CiEnumValue("development")]
    DEVELOPMENT = 0,

    /// <summary>
    /// A testing environment.
    /// </summary>
    [CiEnumValue("testing")]
    TESTING = 1,

    /// <summary>
    /// A staging environment.
    /// </summary>
    [CiEnumValue("staging")]
    STAGING = 2,

    /// <summary>
    /// A production environment.
    /// </summary>
    [CiEnumValue("production")]
    PRODUCTION = 3,

    /// <summary>
    /// An environment that does not match another tier.
    /// </summary>
    [CiEnumValue("other")]
    OTHER = 4,
}