// -----------------------------------------------------------------------
// <copyright file="CiEnvironment.Collections.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace TedToolkit.Ci;

public static partial class CiEnvironment
{
    /// <summary>
    /// Gets a trimmed comma-separated list from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The non-empty items, or <see langword="null"/> when absent.</returns>
    public static IReadOnlyList<string>? GetCommaSeparatedList(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return Array.AsReadOnly(value.Split(',')
            .Select(static item => item.Trim())
            .Where(static item => item.Length > 0)
            .ToArray());
    }

    /// <summary>
    /// Gets a JSON string array from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The string items, or <see langword="null"/> when absent.</returns>
    /// <exception cref="FormatException">The variable is not a JSON string array.</exception>
    public static IReadOnlyList<string>? GetJsonStringArray(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        try
        {
            using var reader = JsonReaderWriterFactory.CreateJsonReader(
                Encoding.UTF8.GetBytes(value),
                XmlDictionaryReaderQuotas.Max);
            if (reader.MoveToContent() != XmlNodeType.Element
                || reader.LocalName != "root"
                || reader.GetAttribute("type") != "array")
            {
                throw CreateInvalidValueException(name, "a JSON string array");
            }

            reader.ReadStartElement();
            var items = new List<string>();
            while (reader.MoveToContent() == XmlNodeType.Element)
            {
                if (reader.LocalName != "item" || reader.GetAttribute("type") != "string")
                {
                    throw CreateInvalidValueException(name, "a JSON string array");
                }

                items.Add(reader.ReadElementContentAsString());
            }

            reader.ReadEndElement();
            return Array.AsReadOnly(items.ToArray());
        }
        catch (XmlException exception)
        {
            throw new FormatException(
                $"CI environment variable '{name}' must contain a JSON string array.",
                exception);
        }
    }
}