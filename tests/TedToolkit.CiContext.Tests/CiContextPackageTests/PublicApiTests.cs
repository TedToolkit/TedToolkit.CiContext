// -----------------------------------------------------------------------
// <copyright file="PublicApiTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiContextPackageTests;

internal sealed class PublicApiTests
{
    /// <summary>
    /// Verifies that the foundation exposes the approved provider-neutral public contracts.
    /// </summary>
    [Test]
    public async Task Should_expose_the_approved_provider_neutral_contracts()
    {
        var assembly = System.Reflection.Assembly.Load("TedToolkit.CiContext");
        var environmentType = assembly.GetType("TedToolkit.Ci.CiEnvironment");
        var variableAttributeType = assembly.GetType("TedToolkit.Ci.CiVariableAttribute");
        var enumValueAttributeType = assembly.GetType("TedToolkit.Ci.CiEnumValueAttribute");
        var legacyMetadataType = assembly.GetType("TedToolkit.Ci.ICiVariableMetadata");
        var secretType = assembly.GetType("TedToolkit.Ci.CiSecret`1");
        var secretMaskType = assembly.GetType("TedToolkit.Ci.CiSecretMask");
        var booleanFormatType = assembly.GetType("TedToolkit.Ci.CiBooleanFormat");

        await Assert.That(environmentType).IsNotNull();
        await Assert.That(variableAttributeType).IsNotNull();
        await Assert.That(enumValueAttributeType).IsNotNull();
        await Assert.That(legacyMetadataType).IsNull();
        await Assert.That(secretType).IsNotNull();
        await Assert.That(secretMaskType).IsNull();
        await Assert.That(booleanFormatType).IsNull();
        await Assert.That(environmentType!.GetMethod("GetRawValue", [typeof(string),])).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetInt32", [typeof(string),])).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetInt64", [typeof(string),])).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetAbsoluteUri", [typeof(string),])).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetBoolean", [typeof(string),])).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetPresenceMarker")).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetDateTimeOffset")).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetTimeSpanFromSeconds")).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetTimeSpanFromDays")).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetDirectoryPath")).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetFilePath")).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetCommaSeparatedList")).IsNotNull();
        await Assert.That(environmentType.GetMethod("GetJsonStringArray")).IsNotNull();
        await Assert.That(environmentType.GetMethods().Any(static method =>
            method.Name == "GetEnumValue" && method.IsGenericMethodDefinition)).IsTrue();
        await Assert.That(variableAttributeType!.IsAbstract).IsFalse();
        await Assert.That(variableAttributeType.IsSealed).IsTrue();
        await Assert.That(variableAttributeType.IsSubclassOf(typeof(Attribute))).IsTrue();
        await Assert.That(variableAttributeType.GetConstructor([typeof(string),])).IsNotNull();
        await Assert.That(variableAttributeType.GetProperty("Name")).IsNotNull();
        await Assert.That(enumValueAttributeType!.IsSubclassOf(typeof(Attribute))).IsTrue();
        await Assert.That(enumValueAttributeType.GetProperty("Value")).IsNotNull();
        await Assert.That(secretType!.GetMethod("Reveal")).IsNotNull();
        await Assert.That(secretType.IsValueType).IsTrue();
    }
}