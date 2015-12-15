using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UTGTesting;

namespace UTGCoverage
{
   public class CodeCoverageFactory
   {
      public static AbstractCodeCoverageFramework CreatePartCoverFramework(string binariesDirectoryPath, string consoleDirectoriesPath, string commandsDirectoryPath, AbstractTestFramework testFramework)
      {
         return new PartCoverFramework(binariesDirectoryPath, consoleDirectoriesPath, commandsDirectoryPath, testFramework);
      }

      public static AbstractCodeCoverageFramework CreateOpenCoverFramework(string binariesDirectoryPath, string consoleDirectoriesPath, string commandsDirectoryPath, AbstractTestFramework testFramework)
      {
         return new OpenCoverFramework(binariesDirectoryPath, consoleDirectoriesPath, commandsDirectoryPath, testFramework);
      }

      //Add any other frameworks for example 
      //public abstract AbstractTestFramework CreateMBUnitTestFramework();
   }
}
