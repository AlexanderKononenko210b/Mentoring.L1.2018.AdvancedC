using System;
using System.Configuration;
using System.IO;

namespace EventsHelper.Services
{
    /// <summary>
    /// Represents a model of the <see cref="EnvironmentBuilder"/> class.
    /// </summary>
    public static class EnvironmentBuilder
    {
        /// <summary>
        /// Create directories and files.
        /// </summary>
        /// <param name="numberOfDirectories">The number of directories.</param>
        /// <param name="numberOfFiles">The number of files.</param>
        public static int Create(int numberOfDirectories, int numberOfFiles)
        {
            var directoryName = ConfigurationManager.AppSettings["nameDirectory"];
            var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directoryName);
            var count = 0;

            Directory.CreateDirectory(tempPath);
            count++;

            for (int i = 0; i < numberOfDirectories; i++)
            {
                var subDirectoryName = $"{directoryName}{i}";
                var subDirectoryPath = Path.Combine(tempPath, subDirectoryName);

                Directory.CreateDirectory(subDirectoryPath);
                count++;

                for (int j = 0; j < numberOfFiles; j++)
                {
                    var fileName = $"{ConfigurationManager.AppSettings["nameFile"]}{j}.txt";

                    var filePath = Path.Combine(subDirectoryPath, fileName);

                    File.Create(filePath).Close();
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Clear temp directory
        /// </summary>
        /// <param name="rootPath">The root direcotry or file for delete.</param>
        public static void Clear(string rootPath)
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
            else
            {
                if (File.Exists(rootPath))
                {
                    File.Delete(rootPath);
                }
                else
                {
                    throw new InvalidOperationException("Unknown path for delete operation.");
                }
            }
        }
    }
}
