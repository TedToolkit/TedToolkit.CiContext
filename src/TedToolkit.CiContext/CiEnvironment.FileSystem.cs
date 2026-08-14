// -----------------------------------------------------------------------
// <copyright file="CiEnvironment.FileSystem.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using TedToolkit.FileSystem;

namespace TedToolkit.Ci;

public static partial class CiEnvironment
{
    /// <summary>
    /// Gets a directory path from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The path, or <see langword="null"/> when absent.</returns>
    public static DirectoryPath? GetDirectoryPath(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return new DirectoryPath(value);
    }

    /// <summary>
    /// Gets a file path from the current process environment.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <returns>The path, or <see langword="null"/> when absent.</returns>
    public static FilePath? GetFilePath(string name)
    {
        var value = GetRawValue(name);
        if (value is null)
        {
            return null;
        }

        return new FilePath(value);
    }
}