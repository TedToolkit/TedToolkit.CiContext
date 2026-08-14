// -----------------------------------------------------------------------
// <copyright file="CoverageTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using TedToolkit.GitHub.Tests;

namespace TedToolkit.CiContext.GitHubActions.Tests.GitHubActionsTests;

internal sealed class CoverageTests
{
    /// <summary>
    /// Verifies that the rendered reference and maintained Markdown snapshots agree exactly.
    /// </summary>
    [Test]
    public async Task Should_keep_both_official_source_snapshots_explicit_and_in_sync()
    {
        var rendered = OfficialVariableNames.Rendered.Order(StringComparer.Ordinal).ToArray();
        var maintained = OfficialVariableNames.Maintained.Order(StringComparer.Ordinal).ToArray();

        await Assert.That(rendered.Length).IsEqualTo(46);
        await Assert.That(maintained.Length).IsEqualTo(46);
        await Assert.That(rendered.Distinct(StringComparer.Ordinal).Count()).IsEqualTo(rendered.Length);
        await Assert.That(maintained.Distinct(StringComparer.Ordinal).Count()).IsEqualTo(maintained.Length);
        await Assert.That(rendered.SequenceEqual(maintained, StringComparer.Ordinal)).IsTrue();
    }

    /// <summary>
    /// Verifies the GitHub Actions provider assembly identity and exact official-variable coverage.
    /// </summary>
    [Test]
    public async Task Should_expose_the_exact_official_variable_set_once()
    {
        var assembly = typeof(GitHub.GitHubActions).Assembly;
        var rootType = assembly.GetType("TedToolkit.GitHub.GitHubActions");

        await Assert.That(rootType).IsNotNull();

        var variableTypes = GetNestedTypes(rootType!).ToArray();
        var metadata = variableTypes
            .SelectMany(static type => type.GetProperties(
                BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Select(static property => property.GetCustomAttribute<CiVariableAttribute>())
            .Where(static attribute => attribute is not null)
            .Select(static attribute => attribute!.Name)
            .Order<string>(StringComparer.Ordinal)
            .ToArray();
        var expected = OfficialVariableNames.All.Order(StringComparer.Ordinal).ToArray();

        await Assert.That(rootType!.IsAbstract && rootType.IsSealed).IsTrue();
        await Assert.That(rootType.GetMethod("GetRawValue", BindingFlags.Public | BindingFlags.Static)).IsNull();
        await Assert.That(assembly.GetType("TedToolkit.GitHub.GitHubActionsVariableAttribute")).IsNull();
        await Assert.That(assembly.GetName().Name).IsEqualTo("TedToolkit.CiContext.GitHubActions");
        await Assert.That(variableTypes.All(static type => type.IsAbstract && type.IsSealed)).IsTrue();
        await Assert.That(metadata.Length).IsEqualTo(expected.Length);
        await Assert.That(metadata.Distinct(StringComparer.Ordinal).Count()).IsEqualTo(expected.Length);
        await Assert.That(metadata.SequenceEqual(expected, StringComparer.Ordinal)).IsTrue();
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