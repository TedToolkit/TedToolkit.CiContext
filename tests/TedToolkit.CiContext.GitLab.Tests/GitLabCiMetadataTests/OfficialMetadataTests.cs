// -----------------------------------------------------------------------
// <copyright file="OfficialMetadataTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using TedToolkit.GitLab;
using TedToolkit.GitLab.Tests;

namespace TedToolkit.CiContext.GitLab.Tests.GitLabCiMetadataTests;

internal sealed class OfficialMetadataTests
{
    /// <summary>
    /// Verifies every official variable's availability, category, and integration subdivision.
    /// </summary>
    [Test]
    public async Task Should_match_the_complete_official_metadata_snapshot()
    {
        var actual = GetVariableProperties()
            .Select(CreateMetadata)
            .OrderBy(static metadata => metadata.Name, StringComparer.Ordinal)
            .ToArray();
        var expected = OfficialVariableMetadataSnapshot.All
            .OrderBy(static metadata => metadata.Name, StringComparer.Ordinal)
            .ToArray();

        await Assert.That(actual.Length).IsEqualTo(expected.Length);
        await Assert.That(actual.DistinctBy(static metadata => metadata.Name).Count()).IsEqualTo(expected.Length);
        await Assert.That(actual.SequenceEqual(expected)).IsTrue();
    }

    private static OfficialVariableMetadata CreateMetadata(PropertyInfo property)
    {
        var name = property.GetCustomAttribute<CiVariableAttribute>()!.Name;
        var category = property.GetCustomAttribute<GitLabCiVariableCategoryAttribute>()!.Category;
        var topLevelType = GetTopLevelType(property.DeclaringType!);
        var availability = topLevelType.Name switch
        {
            nameof(GitLabCi.PrePipeline) => OfficialVariableAvailability.PrePipeline,
            nameof(GitLabCi.Pipeline) => OfficialVariableAvailability.Pipeline,
            nameof(GitLabCi.JobOnly) => OfficialVariableAvailability.JobOnly,
            _ => OfficialVariableAvailability.Unspecified,
        };

        return new(name, availability, category, GetIntegration(property.DeclaringType!));
    }

    private static string? GetIntegration(Type type)
    {
        for (var current = type; current.DeclaringType is not null; current = current.DeclaringType)
        {
            if (current.DeclaringType == typeof(GitLabCi.JobOnly.Integrations))
            {
                return current.Name;
            }
        }

        return null;
    }

    private static Type GetTopLevelType(Type type)
    {
        var current = type;
        while (current.DeclaringType != typeof(GitLabCi))
        {
            current = current.DeclaringType!;
        }

        return current;
    }

    private static IEnumerable<PropertyInfo> GetVariableProperties()
    {
        return GetNestedTypes(typeof(GitLabCi))
            .SelectMany(static type =>
                type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(static property => property.IsDefined(typeof(CiVariableAttribute), false));
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