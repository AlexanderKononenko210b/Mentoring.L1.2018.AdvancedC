using System;
using System.Configuration;
using System.IO;
using FileSystemSearch.Enums;
using FileSystemSearch.Services;
using NUnit.Framework;

namespace FileSystemSearch.Test
{
    /// <summary>
    /// Represents a model of the <see cref="ValidatorTest"/> class.
    /// </summary>
    [TestFixture]
    public class ValidatorTest
    {
        /// <summary>
        /// Test directory path.
        /// </summary>
        private string _directoryTestPath;

        /// <summary>
        /// Initialize fields.
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            _directoryTestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["testPathDirectory"]);
        }
        
        /// <summary>
        /// Directory path exist.
        /// </summary>
        [Test]
        public void Exists_Path_DirectoryPathExist()
        {
            // arrange
            var validator = new Validator(path => path.Contains("Debug"));

            //act
            var result = validator.Exists(_directoryTestPath);

            //assert
            Assert.AreEqual(result, SearchItems.Directory);
        }

        /// <summary>
        /// Directory path not exist.
        /// </summary>
        [Test]
        public void Exists_Path_DirectoryPathNotExist()
        {
            // arrange
            var validator = new Validator(path => path.Contains("Debug"));
            var pathNotValid = Path.Combine(_directoryTestPath, "NotValid");

            //act
            var result = validator.Exists(pathNotValid);

            //assert
            Assert.AreEqual(result, SearchItems.Unknown);
        }

        /// <summary>
        /// Directory path filtered by predicate.
        /// </summary>
        [Test]
        public void IsFiltered_Path_DirectoryPathExist()
        {
            // arrange
            var validator = new Validator(path => path.Contains("Debug"));

            //act
            var result = validator.IsFiltered(_directoryTestPath);

            //assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Directory path not filtered by predicate.
        /// </summary>
        [Test]
        public void IsFiltered_Path_DirectoryPathNotExist()
        {
            // arrange
            var validator = new Validator(path => path.Contains("_"));

            //act
            var result = validator.IsFiltered(_directoryTestPath);

            //assert
            Assert.IsFalse(result);
        }
    }
}
