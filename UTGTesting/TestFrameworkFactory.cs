using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UTGTesting
{
   public class TestFrameworkFactory
   {
      public static AbstractTestFramework CreateNUnitTestFramework(string binariesDirectoryPath, string consoleDirectoriesPath, string commandsDirectoryPath)
      {
         return new NUnitTestFramework(binariesDirectoryPath, consoleDirectoriesPath, commandsDirectoryPath);
      }

      //Add any other frameworks for example 
      //public abstract AbstractTestFramework CreateMBUnitTestFramework();
   }
}
