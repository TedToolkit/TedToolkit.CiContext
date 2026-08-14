// -----------------------------------------------------------------------
// <copyright file="GetFilePathTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using TedToolkit.FileSystem;

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetFilePathTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_FILE";

    /// <summary>
    /// Verifies file-path conversion and missing-value behavior.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_a_file_path_or_null()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, @"C:\build\event.json");

        await Assert.That(CiEnvironment.GetFilePath(VARIABLE_NAME))
            .IsEqualTo(new FilePath(@"C:\build\event.json"));

        Environment.SetEnvironmentVariable(VARIABLE_NAME, null);
        await Assert.That(CiEnvironment.GetFilePath(VARIABLE_NAME)).IsNull();
    }
}