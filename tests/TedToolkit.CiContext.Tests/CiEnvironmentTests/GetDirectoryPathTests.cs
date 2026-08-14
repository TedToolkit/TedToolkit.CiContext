// -----------------------------------------------------------------------
// <copyright file="GetDirectoryPathTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using TedToolkit.FileSystem;

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetDirectoryPathTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_DIRECTORY";

    /// <summary>
    /// Verifies directory-path conversion and missing-value behavior.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_a_directory_path_or_null()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, @"C:\build\repository");

        await Assert.That(CiEnvironment.GetDirectoryPath(VARIABLE_NAME))
            .IsEqualTo(new DirectoryPath(@"C:\build\repository"));

        Environment.SetEnvironmentVariable(VARIABLE_NAME, null);
        await Assert.That(CiEnvironment.GetDirectoryPath(VARIABLE_NAME)).IsNull();
    }
}