using System;
using System.Configuration;
using System.IO;
using FileSystemSearch;
using FileSystemSearch.Services;
using FileSystemSearch.Test.Helpers;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var visitor = new FileSystemVisitor(
                new Validator(x => x.Contains("Debug")),
                new SaveManager());

            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                ConfigurationManager.AppSettings["testPathDirectory"]);

            var listener = new Listener(visitor);

            visitor.Search(path);
        }
    }
}
