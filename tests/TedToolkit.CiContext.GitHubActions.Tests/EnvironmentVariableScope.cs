// -----------------------------------------------------------------------
// <copyright file="EnvironmentVariableScope.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.GitHubActions.Tests;

internal sealed class EnvironmentVariableScope : IDisposable
{
    private readonly IReadOnlyDictionary<string, string?> _originalValues;

    internal EnvironmentVariableScope(IReadOnlyDictionary<string, string?> values)
    {
        _originalValues = values.Keys.ToDictionary(
            static name => name,
            Environment.GetEnvironmentVariable,
            StringComparer.Ordinal);

        foreach (var pair in values)
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }
    }

    public void Dispose()
    {
        foreach (var pair in _originalValues)
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }
    }
}