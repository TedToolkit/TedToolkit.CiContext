// -----------------------------------------------------------------------
// <copyright file="GitLabServerProtocol.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies the GitLab server protocol.
/// </summary>
public enum GitLabServerProtocol
{
    /// <summary>
    /// The server uses HTTP.
    /// </summary>
    [CiEnumValue("http")]
    HTTP = 0,

    /// <summary>
    /// The server uses HTTPS.
    /// </summary>
    [CiEnumValue("https")]
    HTTPS = 1,
}