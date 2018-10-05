using System;
using System.Configuration;
using System.IO;
using EventsHelper.Services;
using FileSystemSearch;
using FileSystemSearch.Services;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int _numberOfDirectories, _numberOfFiles;

            if (!Int32.TryParse(ConfigurationManager.AppSettings["numberOfDirectories"], out _numberOfDirectories))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfDirectories"]}");
            }

            if (!Int32.TryParse(ConfigurationManager.AppSettings["numberOfFiles"], out _numberOfFiles))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfFiles"]}");
            }

            var visitor = new FileSystemVisitor(
                new Validator(x => x.Contains("Debug")),
                new SaveManager());

            var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["nameDirectory"]);
            var expectedItemsForSave = EnvironmentBuilder.Create(_numberOfDirectories, _numberOfFiles);
            var listener = new Listener(visitor, expectedItemsForSave + 1, expectedItemsForSave + 1);

            visitor.Search(rootPath);

            Directory.Delete(rootPath, true);
        }
    }
}
