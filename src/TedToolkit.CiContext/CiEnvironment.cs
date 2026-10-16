// -----------------------------------------------------------------------
// <copyright file="CiEnvironment.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace TedToolkit.Ci;

/// <summary>
/// Provides exact-name access to variables in the current process environment.
/// </summary>
public static partial class CiEnvironment
{
    /// <summary>
    /// Gets a current process environment variable without applying provider-specific conversion.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The current value, or <see langword="null"/> when the variable is absent.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    /// <example>
    /// <code>
    /// string? commitSha = CiEnvironment.GetRawValue("CI_COMMIT_SHA");
    /// </code>
    /// </example>
    public static string? GetRawValue(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The variable name cannot be null or empty.", nameof(name));
        }

        return Environment.GetEnvironmentVariable(name);
    }

    /// <summary>
    /// Gets an invariant 32-bit integer from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The parsed value, or <see langword="null"/> when the variable is absent.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    /// <exception cref="FormatException">The variable does not contain a 32-bit integer.</exception>
    public static int? GetInt32(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw CreateInvalidValueException(name, "a 32-bit integer");
    }

    /// <summary>
    /// Gets an invariant 64-bit integer from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The parsed value, or <see langword="null"/> when the variable is absent.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    /// <exception cref="FormatException">The variable does not contain a 64-bit integer.</exception>
    public static long? GetInt64(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw CreateInvalidValueException(name, "a 64-bit integer");
    }

    /// <summary>
    /// Gets an absolute URI from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The parsed URI, or <see langword="null"/> when the variable is absent.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    /// <exception cref="FormatException">The variable does not contain an absolute URI.</exception>
    public static Uri? GetAbsoluteUri(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return Uri.TryCreate(value, UriKind.Absolute, out var result)
            ? result
            : throw CreateInvalidValueException(name, "an absolute URI");
    }

    /// <summary>
    /// Gets an enum value whose exact string mapping is declared with <see cref="CiEnumValueAttribute"/>.
    /// </summary>
    /// <typeparam name="TEnum">The mapped enum type.</typeparam>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The mapped enum member, or <see langword="null"/> when the variable is absent.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    /// <exception cref="FormatException">The variable does not contain a mapped value.</exception>
    /// <exception cref="InvalidOperationException">The enum mapping is incomplete or ambiguous.</exception>
    public static TEnum? GetEnumValue<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
#endif
        TEnum>(string name)
        where TEnum : struct, Enum
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return GetEnumValues<TEnum>().TryGetValue(value, out var result)
            ? result
            : throw CreateInvalidValueException(name, $"a documented {typeof(TEnum).Name} value");
    }

    private static Dictionary<string, TEnum> GetEnumValues<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
#endif
        TEnum>()
        where TEnum : struct, Enum
    {
        try
        {
            return EnumValueMap<TEnum>.Values;
        }
        catch (TypeInitializationException exception) when (
            exception.InnerException is InvalidOperationException definitionException)
        {
            ExceptionDispatchInfo.Capture(definitionException).Throw();
            throw;
        }
    }

    private static FormatException CreateInvalidValueException(string name, string expected)
    {
        return new($"CI environment variable '{name}' must contain {expected}.");
    }

    private static class EnumValueMap<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
#endif
        TEnum>
        where TEnum : struct, Enum
    {
        internal static readonly Dictionary<string, TEnum> Values = Create();

        private static Dictionary<string, TEnum> Create()
        {
            var values = new Dictionary<string, TEnum>(StringComparer.Ordinal);
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attributes = field.GetCustomAttributes(typeof(CiEnumValueAttribute), false)
                    .Cast<CiEnumValueAttribute>()
                    .ToArray();
                if (attributes.Length == 0)
                {
                    throw new InvalidOperationException(
                        $"Enum member '{typeof(TEnum).Name}.{field.Name}' must declare at least one CiEnumValueAttribute.");
                }

                var enumValue = (TEnum)field.GetValue(null)!;
                foreach (var attribute in attributes)
                {
                    if (values.ContainsKey(attribute.Value))
                    {
                        throw new InvalidOperationException(
                            $"Enum type '{typeof(TEnum).Name}' declares the CI value '{attribute.Value}' more than once.");
                    }

                    values.Add(attribute.Value, enumValue);
                }
            }

            return values;
        }
    }
}