// -----------------------------------------------------------------------
// <copyright file="CiSecret.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TedToolkit.Ci;

/// <summary>
/// Represents a non-null value whose ordinary string representation is protected.
/// </summary>
/// <typeparam name="T">The wrapped value type.</typeparam>
/// <remarks>
/// This type prevents accidental disclosure through <see cref="ToString"/>. It does not encrypt the value or define serializer and structured-logging policies.
/// </remarks>
[DebuggerDisplay("{ToString(),nq}")]
[SuppressMessage(
    "Performance",
    "CA1815:Override equals and operator equals on value types",
    Justification = "A secret wrapper intentionally does not expose value equality.")]
public readonly struct CiSecret<T>
    where T : notnull
{
    private const string REDACTED_TEXT = "[REDACTED]";

    private const int VISIBLE_SUFFIX_LENGTH = 4;

    private const int MINIMUM_PARTIALLY_VISIBLE_LENGTH = 12;

    private readonly bool _hasValue;

    private readonly T _value;

    /// <summary>
    /// Initializes a fully redacted wrapper for a non-null value.
    /// </summary>
    /// <param name="value">The value to protect from ordinary string formatting.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public CiSecret(T value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
#endif

        _value = value;
        _hasValue = true;
    }

    /// <summary>
    /// Converts a non-null value to a fully redacted secret wrapper.
    /// </summary>
    /// <param name="value">The value to protect.</param>
    [SuppressMessage(
        "Usage",
        "CA2225:Operator overloads have named alternates",
        Justification = "The public constructor is the explicit alternative to the convenience conversion.")]
    public static implicit operator CiSecret<T>(T value)
    {
        return new(value);
    }

    /// <summary>
    /// Returns the original wrapped value explicitly.
    /// </summary>
    /// <returns>The original value supplied to the constructor.</returns>
    /// <exception cref="InvalidOperationException">This instance is the uninitialized default value.</exception>
    public T Reveal()
    {
        return _hasValue
            ? _value
            : throw new InvalidOperationException("The CI secret has not been initialized.");
    }

    /// <summary>
    /// Returns a representation that is fully redacted when short and otherwise shows only the final four characters.
    /// </summary>
    /// <returns>A representation that preserves at least eight hidden characters.</returns>
    public override string ToString()
    {
        if (!_hasValue)
        {
            return REDACTED_TEXT;
        }

        var text = _value.ToString() ?? "";
        if (text.Length < MINIMUM_PARTIALLY_VISIBLE_LENGTH)
        {
            return REDACTED_TEXT;
        }

#if NET6_0_OR_GREATER
        return string.Concat(
            REDACTED_TEXT.AsSpan(),
            text.AsSpan(text.Length - VISIBLE_SUFFIX_LENGTH));
#else
        return REDACTED_TEXT + text.Substring(text.Length - VISIBLE_SUFFIX_LENGTH);
#endif
    }
}