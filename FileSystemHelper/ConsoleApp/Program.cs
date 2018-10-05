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
            if (!int.TryParse(ConfigurationManager.AppSettings["numberOfDirectories"], out var numberOfDirectories))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfDirectories"]}");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["numberOfFiles"], out var numberOfFiles))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfFiles"]}");
            }

            var visitor = new FileSystemVisitor(
                new Validator(x => x.Contains("Debug")),
                new SaveManager());

            var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["nameDirectory"]);
            var expectedItemsForSave = EnvironmentBuilder.Create(numberOfDirectories, numberOfFiles);
            var listener = new Listener(visitor, expectedItemsForSave + 1, expectedItemsForSave + 1);

            visitor.Search(rootPath);

            Directory.Delete(rootPath, true);
        }
    }
}
