// -----------------------------------------------------------------------
// <copyright file="CategoryMetadataTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using TedToolkit.GitLab;
using TedToolkit.GitLab.Tests;

namespace TedToolkit.CiContext.GitLab.Tests.GitLabCiMetadataTests;

internal sealed class CategoryMetadataTests
{
    /// <summary>
    /// Verifies that every GitLab CI/CD variable carries one provider-specific category beside its common name.
    /// </summary>
    [Test]
    public async Task Should_attach_one_category_to_every_variable()
    {
        var properties = GetNestedTypes(typeof(GitLabCi))
            .SelectMany(type => type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(property => property.IsDefined(typeof(CiVariableAttribute), false))
            .ToArray();

        await Assert.That(properties.Length).IsEqualTo(OfficialVariableNames.All.Length);
        await Assert.That(properties.All(property =>
            property.GetCustomAttributes<GitLabCiVariableCategoryAttribute>(false).Count() == 1)).IsTrue();
    }

    /// <summary>
    /// Verifies that GitLab-specific categories remain independent metadata beside common variable names.
    /// </summary>
    [Test]
    public async Task Should_attach_official_categories_to_representative_variables()
    {
        await AssertCategory(
            typeof(GitLabCi.PrePipeline.Commit),
            nameof(GitLabCi.PrePipeline.Commit.Sha),
            GitLabCiVariableCategory.PREDEFINED).ConfigureAwait(false);
        await AssertCategory(
            typeof(GitLabCi.PrePipeline.AutoDevOps),
            nameof(GitLabCi.PrePipeline.AutoDevOps.ExplicitlyEnabled),
            GitLabCiVariableCategory.AUTO_DEV_OPS).ConfigureAwait(false);
        await AssertCategory(
            typeof(GitLabCi.PrePipeline.MergeRequest),
            nameof(GitLabCi.PrePipeline.MergeRequest.Iid),
            GitLabCiVariableCategory.MERGE_REQUEST).ConfigureAwait(false);
        await AssertCategory(
            typeof(GitLabCi.JobOnly.Integrations.Harbor),
            nameof(GitLabCi.JobOnly.Integrations.Harbor.Url),
            GitLabCiVariableCategory.INTEGRATION).ConfigureAwait(false);
        await AssertCategory(
            typeof(GitLabCi.ExternalPullRequest),
            nameof(GitLabCi.ExternalPullRequest.Iid),
            GitLabCiVariableCategory.EXTERNAL_PULL_REQUEST).ConfigureAwait(false);
    }

    private static async Task AssertCategory(
        Type declaringType,
        string propertyName,
        GitLabCiVariableCategory expected)
    {
        var attribute = declaringType
            .GetProperty(propertyName)!
            .GetCustomAttribute<GitLabCiVariableCategoryAttribute>();

        await Assert.That(attribute).IsNotNull();
        await Assert.That(attribute!.Category).IsEqualTo(expected);
    }

    private static IEnumerable<Type> GetNestedTypes(Type type)
    {
        foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public))
        {
            yield return nestedType;

            foreach (var descendant in GetNestedTypes(nestedType))
            {
                yield return descendant;
            }
        }
    }
}