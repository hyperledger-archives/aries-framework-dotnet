using System;
using System.Collections.Generic;
using System.IO;

namespace AgentFramework.Core.Utils
{
    /// <summary>
    /// Environment utilities
    /// </summary>
    public static class EnvironmentUtils
    {
        /// <summary>
        /// Gets the current user path
        /// </summary>
        /// <returns>The current user path</returns>
        public static string GetUserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        /// <summary>
        /// Gets the path to indy home
        /// </summary>
        /// <returns>The path to indy home</returns>
        public static string GetIndyHomePath()
        {
            return Path.Combine(GetUserPath(), ".indy_client");
        }

        /// <summary>
        /// Gets the indy home path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        public static string GetIndyHomePath(params string[] paths)
        {
            var pathParts = new List<string>(paths);
            pathParts.Insert(0, GetIndyHomePath());
            return Path.Combine(pathParts.ToArray());
        }

        /// <summary>
        /// Gets the path to tails file
        /// </summary>
        /// <returns>The path to tails file</returns>
        public static string GetTailsPath()
        {
            return Path.Combine(GetIndyHomePath(), "tails").Replace("\\", "/");
        }

        /// <summary>
        /// Gets the path to tails file
        /// </summary>
        /// <returns>The path to tails file</returns>
        public static string GetTailsPath(string filename)
        {
            return Path.Combine(GetTailsPath(), filename);
        }
    }
}
