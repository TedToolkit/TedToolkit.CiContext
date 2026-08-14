// -----------------------------------------------------------------------
// <copyright file="VariableDocumentationTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;
using System.Xml.Linq;

using TedToolkit.GitHub.Tests;

namespace TedToolkit.CiContext.GitHubActions.Tests.GitHubActionsMetadataTests;

internal sealed class VariableDocumentationTests
{
    /// <summary>
    /// Verifies that every terminal variable has a meaningful summary and nullable value contract.
    /// </summary>
    [Test]
    public async Task Should_document_every_terminal_variable_contract()
    {
        var documentPath = Path.Combine(AppContext.BaseDirectory, "TedToolkit.CiContext.GitHubActions.xml");
        var documentedMembers = XDocument.Load(documentPath)
            .Descendants("member")
            .ToDictionary(
                static member => member.Attribute("name")!.Value,
                StringComparer.Ordinal);
        var variableProperties = GetNestedTypes(typeof(GitHub.GitHubActions))
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

            await Assert.That(member.Element("summary")?.Value.Trim()).IsNotNull();
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