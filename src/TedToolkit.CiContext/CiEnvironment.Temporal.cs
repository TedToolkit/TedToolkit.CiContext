// -----------------------------------------------------------------------
// <copyright file="CiEnvironment.Temporal.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace TedToolkit.Ci;

public static partial class CiEnvironment
{
    /// <summary>
    /// Gets an invariant ISO 8601 timestamp from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The parsed value, or <see langword="null"/> when absent.</returns>
    /// <exception cref="FormatException">The variable is not an ISO 8601 timestamp.</exception>
    public static DateTimeOffset? GetDateTimeOffset(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return DateTimeOffset.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var result)
            ? result
            : throw CreateInvalidValueException(name, "an ISO 8601 timestamp");
    }

    /// <summary>
    /// Gets an invariant integer number of seconds as a duration.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The parsed duration, or <see langword="null"/> when absent.</returns>
    /// <exception cref="FormatException">The variable is not a representable integer number of seconds.</exception>
    public static TimeSpan? GetTimeSpanFromSeconds(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds)
            && seconds <= TimeSpan.MaxValue.Ticks / TimeSpan.TicksPerSecond
            && seconds >= TimeSpan.MinValue.Ticks / TimeSpan.TicksPerSecond
                ? TimeSpan.FromTicks(seconds * TimeSpan.TicksPerSecond)
                : throw CreateInvalidValueException(name, "an integer number of seconds");
    }

    /// <summary>
    /// Gets a non-negative invariant integer number of days as a duration.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The parsed duration, or <see langword="null"/> when absent.</returns>
    /// <exception cref="FormatException">The variable is not a representable non-negative integer number of days.</exception>
    public static TimeSpan? GetTimeSpanFromDays(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var days)
            && days >= 0
            && days <= TimeSpan.MaxValue.Ticks / TimeSpan.TicksPerDay
                ? TimeSpan.FromTicks(days * TimeSpan.TicksPerDay)
                : throw CreateInvalidValueException(name, "a non-negative integer number of days");
    }
}