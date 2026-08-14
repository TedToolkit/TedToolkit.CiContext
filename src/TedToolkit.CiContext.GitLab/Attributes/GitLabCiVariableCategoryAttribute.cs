// -----------------------------------------------------------------------
// <copyright file="GitLabCiVariableCategoryAttribute.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab;

/// <summary>
/// Represents the official documentation category of a GitLab CI/CD variable property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class GitLabCiVariableCategoryAttribute : Attribute
{
    /// <summary>
    /// Initializes category metadata for a GitLab CI/CD variable.
    /// </summary>
    /// <param name="category">The official documentation category containing the variable.</param>
    /// <example>
    /// <code>
    /// [GitLabCiVariableCategoryAttribute(GitLabCiVariableCategory.Predefined)]
    /// </code>
    /// </example>
    public GitLabCiVariableCategoryAttribute(GitLabCiVariableCategory category)
    {
        Category = category;
    }

    /// <summary>
    /// Gets the official documentation category.
    /// </summary>
    public GitLabCiVariableCategory Category { get; }
}