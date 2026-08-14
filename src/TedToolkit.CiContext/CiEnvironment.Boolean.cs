// -----------------------------------------------------------------------
// <copyright file="CiEnvironment.Boolean.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.Ci;

public static partial class CiEnvironment
{
    /// <summary>
    /// Gets a boolean represented by a common CI text value.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The parsed value, or <see langword="null"/> when the variable is absent.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    /// <exception cref="FormatException">The variable does not contain an accepted boolean value.</exception>
    /// <remarks>
    /// Letter tokens <c>true</c>, <c>false</c>, <c>yes</c>, and <c>no</c> are matched without regard to case.
    /// Numeric tokens must be exactly <c>1</c> or <c>0</c>. Surrounding whitespace is not accepted.
    /// </remarks>
    /// <example>
    /// <code>
    /// bool? enabled = CiEnvironment.GetBoolean("CI_FEATURE_ENABLED");
    /// </code>
    /// </example>
    public static bool? GetBoolean(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        if (value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || value == "1")
        {
            return true;
        }

        if (value.Equals("false", StringComparison.OrdinalIgnoreCase)
            || value.Equals("no", StringComparison.OrdinalIgnoreCase)
            || value == "0")
        {
            return false;
        }

        throw CreateInvalidValueException(name, "a boolean");
    }

    /// <summary>
    /// Gets whether a documented presence marker exists with its required exact value.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <param name="expectedValue">The exact marker text.</param>
    /// <returns><see langword="true"/> when present, or <see langword="null"/> when absent.</returns>
    /// <exception cref="ArgumentException"><paramref name="expectedValue"/> is empty.</exception>
    /// <exception cref="FormatException">The present variable does not contain the required marker.</exception>
    public static bool? GetPresenceMarker(string name, string expectedValue)
    {
        if (string.IsNullOrEmpty(expectedValue))
        {
            throw new ArgumentException("The expected marker cannot be null or empty.", nameof(expectedValue));
        }

        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return value == expectedValue
            ? true
            : throw CreateInvalidValueException(name, $"the documented marker '{expectedValue}'");
    }
}