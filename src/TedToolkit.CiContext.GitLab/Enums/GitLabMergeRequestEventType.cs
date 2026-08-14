// -----------------------------------------------------------------------
// <copyright file="GitLabMergeRequestEventType.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies the merge request pipeline event type.
/// </summary>
public enum GitLabMergeRequestEventType
{
    /// <summary>
    /// The pipeline runs against the source branch commit.
    /// </summary>
    [CiEnumValue("detached")]
    DETACHED = 0,

    /// <summary>
    /// The pipeline runs against a simulated merge result.
    /// </summary>
    [CiEnumValue("merged_result")]
    MERGED_RESULT = 1,

    /// <summary>
    /// The pipeline runs as part of a merge train.
    /// </summary>
    [CiEnumValue("merge_train")]
    MERGE_TRAIN = 2,
}