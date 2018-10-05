using System;
using System.Configuration;
using System.IO;
using EventsHelper.Services;
using FileSystemSearch.Services;
using NUnit.Framework;

namespace FileSystemSearch.Test
{
    /// <summary>
    /// Represents a model of the <see cref="SearchTest"/> class.
    /// </summary>
    [TestFixture]
    public class SearchTest
    {
        private int _numberOfDirectories;
        private int _numberOfFiles;
        private int _countItemsForCansel;
        private int _countItemsForExclude;
        private string _nameDirectory;
        private string _rootPath;


        /// <summary>
        /// Initialize fields.
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            if(!Int32.TryParse(ConfigurationManager.AppSettings["numberOfDirectories"], out _numberOfDirectories))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfDirectories"]}");
            }

            if (!Int32.TryParse(ConfigurationManager.AppSettings["numberOfFiles"], out _numberOfFiles))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfFiles"]}");
            }

            if (!Int32.TryParse(ConfigurationManager.AppSettings["countEventsForCansel"], out _countItemsForCansel))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["countEventForCansel"]}");
            }

            if (!Int32.TryParse(ConfigurationManager.AppSettings["countEventsForExclude"], out _countItemsForExclude))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["countEventForExclude"]}");
            }

            _nameDirectory = ConfigurationManager.AppSettings["nameDirectory"];
            _rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _nameDirectory);
        }

        /// <summary>
        /// Searched files and directories are exist, filtered and saved.
        /// </summary>
        [Test]
        public void Search_RootPath_AllFilesAndDirectoriesFoundAndSave()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());
            var expectedItemsForSave = EnvironmentBuilder.Create(_numberOfDirectories, _numberOfFiles);
            var listener = new Listener(fileSystemVisitor, expectedItemsForSave, expectedItemsForSave);
            
            //act
            fileSystemVisitor.Search(_rootPath);
            EnvironmentBuilder.Clear(_rootPath);

            //assert
            Assert.AreEqual(expectedItemsForSave, fileSystemVisitor.Count);
        }

        /// <summary>
        /// Cansel search.
        /// </summary>
        [Test]
        public void Search_RootPath_CanselAfterSaveFirstItem()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());
            var expectedItemsForSave = EnvironmentBuilder.Create(_numberOfDirectories, _numberOfFiles);
            var listener = new Listener(fileSystemVisitor, _countItemsForCansel, expectedItemsForSave);

            //act
            fileSystemVisitor.Search(_rootPath);
            EnvironmentBuilder.Clear(_rootPath);

            //assert
            Assert.AreEqual(_countItemsForCansel, fileSystemVisitor.Count);
        }

        /// <summary>
        /// Exclude file from saving.
        /// </summary>
        [Test]
        public void Search_RootPath_ExcludeFileFromSaving()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());
            var expectedItemsForSave = EnvironmentBuilder.Create(_numberOfDirectories, _numberOfFiles);
            var listener = new Listener(fileSystemVisitor, expectedItemsForSave, _countItemsForExclude);

            //act
            fileSystemVisitor.Search(_rootPath);
            EnvironmentBuilder.Clear(_rootPath);

            //assert
            Assert.AreEqual(_countItemsForExclude, fileSystemVisitor.Count);
        }

        /// <summary>
        /// Search directory is not exist. Expected FileNotFoundException.
        /// </summary>
        [Test]
        public void Search_RootPath_RootPathIsNotValid_Expected_FileNotFoundException()
        {
            // arrange
            var incorrectPath = Path.Combine(_rootPath, "notValidPath");

            var fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());

            //assert
            Assert.Throws<FileNotFoundException>(() => fileSystemVisitor.Search(incorrectPath));
        }

        /// <summary>
        /// Search method argument is null. Expected ArgumentException.
        /// </summary>
        [Test]
        public void Search_RootPathIsNull_Expected_ArgumentException()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());

            //assert
            Assert.Throws<ArgumentException>(() => fileSystemVisitor.Search(null));
        }

        /// <summary>
        /// Search method argument is whitespace. Expected ArgumentException.
        /// </summary>
        [Test]
        public void Search_RootPathIsWhiteSpace_Expected_ArgumentException()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());

            //assert
            Assert.Throws<ArgumentException>(() => fileSystemVisitor.Search(" "));
        }

        /// <summary>
        /// Search method argument is empty. Expected ArgumentException.
        /// </summary>
        [Test]
        public void Search_RootPathIsEmpty_Expected_ArgumentException()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());

            //assert
            Assert.Throws<ArgumentException>(() => fileSystemVisitor.Search(""));
        }
    }
}
