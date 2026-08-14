// -----------------------------------------------------------------------
// <copyright file="GitLabCiVariableAvailabilityAttribute.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Identifies the official phase represented by a GitLab CI/CD variable navigation class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GitLabCiVariableAvailabilityAttribute : Attribute
{
    /// <summary>
    /// Initializes metadata for an official GitLab CI/CD variable availability phase.
    /// </summary>
    /// <param name="availability">The phase represented by the attributed navigation class.</param>
    public GitLabCiVariableAvailabilityAttribute(GitLabCiVariableAvailability availability)
    {
        Availability = availability;
    }

    /// <summary>
    /// Gets the official GitLab CI/CD variable availability phase.
    /// </summary>
    public GitLabCiVariableAvailability Availability { get; }
}