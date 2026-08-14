// -----------------------------------------------------------------------
// <copyright file="VariableDocumentationTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;
using System.Xml.Linq;

using TedToolkit.GitLab;
using TedToolkit.GitLab.Tests;

namespace TedToolkit.CiContext.GitLab.Tests.GitLabCiMetadataTests;

internal sealed class VariableDocumentationTests
{
    /// <summary>
    /// Verifies that every terminal variable has a meaningful summary and nullable value contract in the generated XML documentation.
    /// </summary>
    [Test]
    public async Task Should_document_every_terminal_variable_contract()
    {
        var documentPath = Path.Combine(AppContext.BaseDirectory, "TedToolkit.CiContext.GitLab.xml");
        var documentedMembers = XDocument.Load(documentPath)
            .Descendants("member")
            .ToDictionary(
                static member => member.Attribute("name")!.Value,
                StringComparer.Ordinal);
        var variableProperties = GetNestedTypes(typeof(GitLabCi))
            .SelectMany(static type => type.GetProperties(
                BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(static property => property.IsDefined(typeof(CiVariableAttribute), false))
            .ToArray();

        await Assert.That(variableProperties).Count().IsEqualTo(OfficialVariableNames.All.Length);

        foreach (var property in variableProperties)
        {
            var declaringTypeName = property.DeclaringType!.FullName!.Replace('+', '.');
            var documentationId = $"P:{declaringTypeName}.{property.Name}";
            var member = documentedMembers[documentationId];
            var summary = member.Element("summary")?.Value.Trim();

            await Assert.That(summary).IsNotNull().And.DoesNotContain("Gets the typed value");
            await Assert.That(member.Element("value")).IsNotNull();
        }
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