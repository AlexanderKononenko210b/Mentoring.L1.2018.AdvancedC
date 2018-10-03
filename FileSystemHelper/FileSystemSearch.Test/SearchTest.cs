using System;
using System.Configuration;
using System.IO;
using FileSystemSearch.Enums;
using FileSystemSearch.Interfaces;
using FileSystemSearch.Services;
using FileSystemSearch.Test.Helpers;
using Moq;
using NUnit.Framework;

namespace FileSystemSearch.Test
{
    /// <summary>
    /// Represents a model of the <see cref="SearchTest"/> class.
    /// </summary>
    [TestFixture]
    public class SearchTest
    {
        private Mock<IValidator> _mockValidator;
        private string _directoryTestPath;
        private string _fileTestPath;

        /// <summary>
        /// Initialize fields.
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            _mockValidator = new Mock<IValidator>();
            _directoryTestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["testPathDirectory"]);
            _fileTestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["testPathFile"]);
        }

        /// <summary>
        /// Search file is exist and filtered.
        /// </summary>
        [Test]
        public void Search_RootPath_FileFoundedAndSave()
        {
            // arrange
            _mockValidator.Setup(mock => mock.Exists(It.IsAny<string>()))
                .Returns(() => SearchItems.File);

            _mockValidator.Setup(mock => mock.IsFiltered(It.IsAny<string>()))
                .Returns(() => true);

            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());
            var listener = new Listener(fileSystemVisitor);

            //act
            fileSystemVisitor.Search(_fileTestPath);

            //assert
            Assert.AreEqual(1, fileSystemVisitor.Count);
            Assert.AreEqual(_fileTestPath, fileSystemVisitor[0]);

            Assert.AreEqual(4, listener.Count);
            Assert.AreEqual(EventTypes.Start, listener[0].EventType);
            Assert.AreEqual(EventTypes.Founded, listener[1].EventType);
            Assert.AreEqual(SearchItems.File, listener[1].ItemType);
            Assert.AreEqual(_fileTestPath, listener[1].Path);
            Assert.AreEqual(EventTypes.Filtered, listener[2].EventType);
            Assert.AreEqual(SearchItems.File, listener[2].ItemType);
            Assert.AreEqual(_fileTestPath, listener[2].Path);
            Assert.AreEqual(EventTypes.Finish, listener[3].EventType);
        }

        /// <summary>
        /// Search directory is exist and filtered.
        /// </summary>
        [Test]
        public void Search_RootPath_DirectoryFoundedAndSave()
        {
            // arrange
            _mockValidator.Setup(mock => mock.Exists(It.IsAny<string>()))
                .Returns(() => SearchItems.Directory);

            _mockValidator.Setup(mock => mock.IsFiltered(It.IsAny<string>()))
                .Returns(() => true);

            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());
            var listener = new Listener(fileSystemVisitor);

            //act
            fileSystemVisitor.Search(_directoryTestPath);

            //assert
            Assert.AreEqual(2, fileSystemVisitor.Count);
            Assert.AreEqual(_directoryTestPath, fileSystemVisitor[0]);
            Assert.AreEqual(_fileTestPath, fileSystemVisitor[1]);

            Assert.AreEqual(6, listener.Count);
            Assert.AreEqual(EventTypes.Start, listener[0].EventType);
            Assert.AreEqual(EventTypes.Founded, listener[1].EventType);
            Assert.AreEqual(SearchItems.Directory, listener[1].ItemType);
            Assert.AreEqual(_directoryTestPath, listener[1].Path);
            Assert.AreEqual(EventTypes.Filtered, listener[2].EventType);
            Assert.AreEqual(SearchItems.Directory, listener[2].ItemType);
            Assert.AreEqual(_directoryTestPath, listener[2].Path);
            Assert.AreEqual(EventTypes.Founded, listener[3].EventType);
            Assert.AreEqual(SearchItems.File, listener[3].ItemType);
            Assert.AreEqual(_fileTestPath, listener[3].Path);
            Assert.AreEqual(EventTypes.Filtered, listener[4].EventType);
            Assert.AreEqual(SearchItems.File, listener[4].ItemType);
            Assert.AreEqual(_fileTestPath, listener[4].Path);
            Assert.AreEqual(EventTypes.Finish, listener[5].EventType);
        }

        /// <summary>
        /// Cansel search after founded, filtered success and save first item.
        /// </summary>
        [Test]
        public void Search_RootPath_CanselAfterSaveFirstItem()
        {
            // arrange
            _mockValidator.SetupSequence(mock => mock.Exists(It.IsAny<string>()))
                .Returns(() => SearchItems.Directory)
                .Returns(() => SearchItems.File);

            _mockValidator.SetupSequence(mock => mock.IsFiltered(It.IsAny<string>()))
                .Returns(() => true)
                .Returns(() => true);

            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());
            var listener = new Listener(fileSystemVisitor);
            listener._countForCansel = 2;

            //act
            fileSystemVisitor.Search(_directoryTestPath);

            //assert
            Assert.AreEqual(1, fileSystemVisitor.Count);
            Assert.AreEqual(_directoryTestPath, fileSystemVisitor[0]);

            Assert.AreEqual(4, listener.Count);
            Assert.AreEqual(EventTypes.Start, listener[0].EventType);
            Assert.AreEqual(EventTypes.Founded, listener[1].EventType);
            Assert.AreEqual(EventTypes.Filtered, listener[2].EventType);
            Assert.AreEqual(EventTypes.Finish, listener[3].EventType);
        }

        /// <summary>
        /// Exclude file from saving.
        /// </summary>
        [Test]
        public void Search_RootPath_ExcludeFileFromSaving()
        {
            // arrange
            _mockValidator.SetupSequence(mock => mock.Exists(It.IsAny<string>()))
                .Returns(() => SearchItems.Directory)
                .Returns(() => SearchItems.File);

            _mockValidator.SetupSequence(mock => mock.IsFiltered(It.IsAny<string>()))
                .Returns(() => true)
                .Returns(() => true);

            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());
            var listener = new Listener(fileSystemVisitor);
            listener._countForExclude = 3;

            //act
            fileSystemVisitor.Search(_directoryTestPath);

            //assert
            Assert.AreEqual(1, fileSystemVisitor.Count);
            Assert.AreEqual(_directoryTestPath, fileSystemVisitor[0]);

            Assert.AreEqual(6, listener.Count);
            Assert.AreEqual(EventTypes.Start, listener[0].EventType);
            Assert.AreEqual(EventTypes.Founded, listener[1].EventType);
            Assert.AreEqual(EventTypes.Filtered, listener[2].EventType);
            Assert.AreEqual(EventTypes.Founded, listener[3].EventType);
            Assert.AreEqual(EventTypes.Filtered, listener[4].EventType);
            Assert.AreEqual(EventTypes.Finish, listener[5].EventType);
        }

        /// <summary>
        /// Search directory is not exist. Expected FileNotFoundException.
        /// </summary>
        [Test]
        public void Search_RootPath_RootPathIsNotValid_Expected_FileNotFoundException()
        {
            // arrange
            _mockValidator.Setup(mock => mock.Exists(It.IsAny<string>()))
                .Returns(() => SearchItems.Unknown);

            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());

            //assert
            Assert.Throws<FileNotFoundException>(() => fileSystemVisitor.Search(_directoryTestPath));
        }

        /// <summary>
        /// Search method argument is null. Expected ArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void Search_RootPathIsNull_Expected_NullReferenceException()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());

            //assert
            Assert.Throws<NullReferenceException>(() => fileSystemVisitor.Search(null));
        }

        /// <summary>
        /// Search method argument is whitespace. Expected ArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void Search_RootPathIsWhiteSpace_Expected_ArgumentOutOfRangeException()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());

            //assert
            Assert.Throws<ArgumentOutOfRangeException>(() => fileSystemVisitor.Search(" "));
        }

        /// <summary>
        /// Search method argument is empty. Expected ArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void Search_RootPathIsEmpty_Expected_ArgumentOutOfRangeException()
        {
            // arrange
            var fileSystemVisitor = new FileSystemVisitor(_mockValidator.Object, new SaveManager());

            //assert
            Assert.Throws<ArgumentOutOfRangeException>(() => fileSystemVisitor.Search(""));
        }
    }
}
