// -----------------------------------------------------------------------
// <copyright file="CiVariableAttribute.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.Ci;

/// <summary>
/// Provides provider-neutral metadata for a property that represents one CI environment variable.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CiVariableAttribute : Attribute
{
    /// <summary>
    /// Initializes metadata for an exact CI environment variable name.
    /// </summary>
    /// <param name="name">The exact, case-sensitive environment variable name.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>
    /// <example>
    /// <code>
    /// [CiVariable("CI")]
    /// public static bool? Detected =&gt; CiEnvironment.GetBoolean("CI");
    /// </code>
    /// </example>
    public CiVariableAttribute(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The variable name cannot be null or empty.", nameof(name));
        }

        Name = name;
    }

    /// <summary>
    /// Gets the exact environment variable name.
    /// </summary>
    public string Name { get; }
}