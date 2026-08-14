// -----------------------------------------------------------------------
// <copyright file="GitLabPipelineSource.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies how GitLab created a pipeline.
/// </summary>
public enum GitLabPipelineSource
{
    /// <summary>
    /// The pipeline was created through the pipelines API.
    /// </summary>
    [CiEnumValue("api")]
    API = 0,

    /// <summary>
    /// The pipeline was created by a GitLab ChatOps command.
    /// </summary>
    [CiEnumValue("chat")]
    CHAT = 1,

    /// <summary>
    /// The pipeline uses an external CI service.
    /// </summary>
    [CiEnumValue("external")]
    EXTERNAL = 2,

    /// <summary>
    /// The pipeline was created for an external pull request.
    /// </summary>
    [CiEnumValue("external_pull_request_event")]
    EXTERNAL_PULL_REQUEST_EVENT = 3,

    /// <summary>
    /// The pipeline was created for a merge request.
    /// </summary>
    [CiEnumValue("merge_request_event")]
    MERGE_REQUEST_EVENT = 4,

    /// <summary>
    /// The pipeline was created for an on-demand DAST scan.
    /// </summary>
    [CiEnumValue("ondemand_dast_scan")]
    ON_DEMAND_DAST_SCAN = 5,

    /// <summary>
    /// The pipeline was created for on-demand DAST validation.
    /// </summary>
    [CiEnumValue("ondemand_dast_validation")]
    ON_DEMAND_DAST_VALIDATION = 6,

    /// <summary>
    /// The pipeline was created as a parent-child pipeline.
    /// </summary>
    [CiEnumValue("parent_pipeline")]
    PARENT_PIPELINE = 7,

    /// <summary>
    /// The pipeline was created by a multi-project pipeline.
    /// </summary>
    [CiEnumValue("pipeline")]
    PIPELINE = 8,

    /// <summary>
    /// The pipeline was created by a Git push or tag event.
    /// </summary>
    [CiEnumValue("push")]
    PUSH = 9,

    /// <summary>
    /// The pipeline was created by a schedule.
    /// </summary>
    [CiEnumValue("schedule")]
    SCHEDULE = 10,

    /// <summary>
    /// The pipeline was created by a security orchestration policy.
    /// </summary>
    [CiEnumValue("security_orchestration_policy")]
    SECURITY_ORCHESTRATION_POLICY = 11,

    /// <summary>
    /// The pipeline was created with a trigger token.
    /// </summary>
    [CiEnumValue("trigger")]
    TRIGGER = 12,

    /// <summary>
    /// The pipeline was created from the GitLab UI.
    /// </summary>
    [CiEnumValue("web")]
    WEB = 13,

    /// <summary>
    /// The pipeline was created from GitLab Web IDE.
    /// </summary>
    [CiEnumValue("webide")]
    WEB_IDE = 14,
}