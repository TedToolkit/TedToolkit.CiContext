// -----------------------------------------------------------------------
// <copyright file="GitLabCiVariableCategory.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Groups predefined GitLab CI/CD variables by their official documentation section.
/// </summary>
public enum GitLabCiVariableCategory
{
    /// <summary>
    /// A generally available predefined variable.
    /// </summary>
    PREDEFINED = 0,

    /// <summary>
    /// A variable for merge request pipelines.
    /// </summary>
    MERGE_REQUEST = 1,

    /// <summary>
    /// A variable for external pull request pipelines.
    /// </summary>
    EXTERNAL_PULL_REQUEST = 2,

    /// <summary>
    /// An Auto DevOps variable.
    /// </summary>
    AUTO_DEV_OPS = 3,

    /// <summary>
    /// A variable supplied by a GitLab integration.
    /// </summary>
    INTEGRATION = 4,
}