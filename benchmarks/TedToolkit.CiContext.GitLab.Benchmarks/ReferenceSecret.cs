// -----------------------------------------------------------------------
// <copyright file="ReferenceSecret.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.GitLab.Benchmarks;

public sealed class ReferenceSecret<T>
    where T : notnull
{
    internal ReferenceSecret(T value)
    {
        Value = value;
    }

    internal T Value { get; }
}