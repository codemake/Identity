// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using Microsoft.Framework.Logging;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public static class IdentityResultAssert
    {
        public static void IsSuccess(IdentityResult result)
        {
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        public static void IsFailure(IdentityResult result)
        {
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
        }

        public static void IsFailure(IdentityResult result, string error)
        {
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(error, result.Errors.First().Description);
        }

        public static void IsFailure(IdentityResult result, IdentityError error)
        {
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(error.Description, result.Errors.First().Description);
            Assert.Equal(error.Code, result.Errors.First().Code);
        }

        public static void VerifyUserManagerFailureLog(ILogger logger, string methodName, string userId, params IdentityError[] errors)
        {
            VerifyFailureLog(logger, "UserManager", methodName, userId, "user", errors);
        }

        public static void VerifyRoleManagerFailureLog(ILogger logger, string methodName, string roleId, params IdentityError[] errors)
        {
            VerifyFailureLog(logger, "RoleManager", methodName, roleId, "role", errors);
        }

        public static void VerifyUserManagerSuccessLog(ILogger logger, string methodName, string userId)
        {
            VerifySuccessLog(logger, "UserManager", methodName, userId, "user");

        }

        public static void VerifyRoleManagerSuccessLog(ILogger logger, string methodName, string roleId)
        {
            VerifySuccessLog(logger, "RoleManager", methodName, roleId, "role");

        }
        private static void VerifySuccessLog(ILogger logger, string className, string methodName, string id, string userOrRole = "user")
        {
            if (logger is TestFileLogger)
            {
                var fileLogger = logger as TestFileLogger;
                string expected = string.Format("{0} for {1}: {2} : Success", methodName, userOrRole, id);

                lock (TestFileLogger.FileLock)
                {
                    Assert.True(File.ReadAllText(fileLogger.FileName).Contains(expected)); 
                }
            }
            else
            {
                Assert.True(true, "No logger registered");
            }
        }

        public static void VerifyLogMessage(ILogger logger, string expectedLog)
        {
            if (logger is TestFileLogger)
            {
                var fileLogger = logger as TestFileLogger;
                lock (TestFileLogger.FileLock)
                {
                    Assert.True(File.ReadAllText(fileLogger.FileName).Contains(expectedLog));
                }
            }
            else
            {
                Assert.True(true, "No logger registered");
            }
        }

        private static void VerifyFailureLog(ILogger logger, string className, string methodName, string userId, string userOrRole = "user", params IdentityError[] errors)
        {
            if (logger is TestFileLogger)
            {
                var fileLogger = logger as TestFileLogger;
                errors = errors ?? new IdentityError[] { new IdentityError() };
                string expected = string.Format("{0} for {1}: {2} : Failed : {3}", methodName, userOrRole, userId, string.Join(",", errors.Select(x => x.Code).ToList()));

                lock (TestFileLogger.FileLock)
                {
                    Assert.True(File.ReadAllText(fileLogger.FileName).Contains(expected));
                }
            }
            else
            {
                Assert.True(true, "No logger registered");
            }
        }
    }
}