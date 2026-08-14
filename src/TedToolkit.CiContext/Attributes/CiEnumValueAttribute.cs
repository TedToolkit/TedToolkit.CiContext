// -----------------------------------------------------------------------
// <copyright file="CiEnumValueAttribute.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.Ci;

/// <summary>
/// Associates an enum member with an exact string value documented for a CI environment variable.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class CiEnumValueAttribute : Attribute
{
    /// <summary>
    /// Initializes an exact enum string mapping.
    /// </summary>
    /// <param name="value">The exact, case-sensitive environment value.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is empty.</exception>
    public CiEnumValueAttribute(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("The enum value cannot be null or empty.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Gets the exact environment value.
    /// </summary>
    public string Value { get; }
}