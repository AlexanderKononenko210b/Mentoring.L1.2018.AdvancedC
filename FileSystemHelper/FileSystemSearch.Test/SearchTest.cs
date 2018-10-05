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
        private int _countItemsForCancel;
        private int _countItemsForExclude;
        private string _nameDirectory;
        private string _rootPath;
        private FileSystemVisitor _fileSystemVisitor;


        /// <summary>
        /// Initialize fields.
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            if(!int.TryParse(ConfigurationManager.AppSettings["numberOfDirectories"], out _numberOfDirectories))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfDirectories"]}");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["numberOfFiles"], out _numberOfFiles))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["numberOfFiles"]}");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["countEventsForCancel"], out _countItemsForCancel))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["countEventForCancel"]}");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["countEventsForExclude"], out _countItemsForExclude))
            {
                throw new ArgumentException($"Incorrect setting value {ConfigurationManager.AppSettings["countEventForExclude"]}");
            }

            _nameDirectory = ConfigurationManager.AppSettings["nameDirectory"];
            _rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _nameDirectory);
            _fileSystemVisitor = new FileSystemVisitor(new Validator(path => path.Contains("Debug")), new SaveManager());
        }

        /// <summary>
        /// Searched files and directories are exist, filtered and saved.
        /// </summary>
        [Test]
        public void Search_RootPath_AllFilesAndDirectoriesFoundAndSave()
        {
            // arrange
            var expectedItemsForSave = EnvironmentBuilder.Create(_numberOfDirectories, _numberOfFiles);
            var listener = new Listener(_fileSystemVisitor, expectedItemsForSave, expectedItemsForSave);

            //act
            _fileSystemVisitor.Search(_rootPath);
            EnvironmentBuilder.Clear(_rootPath);

            //assert
            Assert.AreEqual(expectedItemsForSave, _fileSystemVisitor.Count);
        }

        /// <summary>
        /// Cancel search.
        /// </summary>
        [Test]
        public void Search_RootPath_CancelAfterSaveFirstItem()
        {
            // arrange
            var expectedItemsForSave = EnvironmentBuilder.Create(_numberOfDirectories, _numberOfFiles);
            var listener = new Listener(_fileSystemVisitor, _countItemsForCancel, expectedItemsForSave);

            //act
            _fileSystemVisitor.Search(_rootPath);
            EnvironmentBuilder.Clear(_rootPath);

            //assert
            Assert.AreEqual(_countItemsForCancel, _fileSystemVisitor.Count);
        }

        /// <summary>
        /// Exclude file from saving.
        /// </summary>
        [Test]
        public void Search_RootPath_ExcludeFileFromSaving()
        {
            // arrange
            var expectedItemsForSave = EnvironmentBuilder.Create(_numberOfDirectories, _numberOfFiles);
            var listener = new Listener(_fileSystemVisitor, expectedItemsForSave, _countItemsForExclude);

            //act
            _fileSystemVisitor.Search(_rootPath);
            EnvironmentBuilder.Clear(_rootPath);

            //assert
            Assert.AreEqual(_countItemsForExclude, _fileSystemVisitor.Count);
        }

        /// <summary>
        /// Search directory is not exist. Expected FileNotFoundException.
        /// </summary>
        [Test]
        public void Search_RootPath_RootPathIsNotValid_Expected_FileNotFoundException()
        {
            // arrange
            var incorrectPath = Path.Combine(_rootPath, "notValidPath");

            //assert
            Assert.Throws<FileNotFoundException>(() => _fileSystemVisitor.Search(incorrectPath));
        }

        /// <summary>
        /// Search method argument is null. Expected ArgumentException.
        /// </summary>
        [Test]
        public void Search_RootPathIsNull_Expected_ArgumentException()
        {
            //assert
            Assert.Throws<ArgumentException>(() => _fileSystemVisitor.Search(null));
        }

        /// <summary>
        /// Search method argument is whitespace. Expected ArgumentException.
        /// </summary>
        [Test]
        public void Search_RootPathIsWhiteSpace_Expected_ArgumentException()
        {
            //assert
            Assert.Throws<ArgumentException>(() => _fileSystemVisitor.Search(" "));
        }

        /// <summary>
        /// Search method argument is empty. Expected ArgumentException.
        /// </summary>
        [Test]
        public void Search_RootPathIsEmpty_Expected_ArgumentException()
        {
            //assert
            Assert.Throws<ArgumentException>(() => _fileSystemVisitor.Search(""));
        }
    }
}
