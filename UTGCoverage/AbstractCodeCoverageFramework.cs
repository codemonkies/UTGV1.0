using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UTGManagerAndExaminor;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using UTGHelper;
using UTGTesting;
using UTGTesting.ProcessTypes;

namespace UTGCoverage
{
   public abstract class AbstractCodeCoverageFramework
   {
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      public static extern int GetShortPathName(
              [MarshalAs(UnmanagedType.LPTStr)]
              string path,
              [MarshalAs(UnmanagedType.LPTStr)]
              StringBuilder shortPath,
              int shortPathLength
              );

      protected const int exitWaitPeriod = 180000;

      //While there are similarities  between these properties and those found in AbstractTestFramework for the time being
      //I am choosing to isolate them (not extending from a base). This is because I don't yet know if all of these will be needed
      //The more code coverage frameworks are explored the more it concrete we can get with this set

      //Delegates and Events
      public delegate void CoverageExaminationStartedDelegate();
      public delegate void CoverageExaminationEndedDelegate(
      Project project,
      string message
      );
      public delegate void BinariesLocationChangeDelegate(string newLocation);
      public delegate void ConsoleLocationChangeDelegate(string newLocation);
      public delegate void CommandsLocationChangeDelegate(string newLocation);

      public abstract event CoverageExaminationStartedDelegate CoverageExaminationStarted;
      public abstract event CoverageExaminationEndedDelegate CoverageExaminationEnded;
      public abstract event BinariesLocationChangeDelegate BinariesLocationChanged;
      public abstract event ConsoleLocationChangeDelegate ConsoleLocationChanged;
      public abstract event CommandsLocationChangeDelegate CommandsLocationChanged;

      protected string binariesDirectory, consoleDirectory, commandsDirectory, commandName, consoleName, configurationXml;
      protected AbstractTestFramework testFramework;

      public abstract List<string> FrameworkDlls { get; }
      public abstract string BinariesDirectory { get; set; }
      public abstract string ConsoleDirectory { get; set; }
      public abstract string CommandsDirectory { get; set; }
      public abstract string CommandName { get; }
      public abstract string ConsoleName { get; }
      public abstract string ConfigurationXml { get; }

      public abstract bool Examine(TestFixturOption testFixturOption, DTE2 applicationObject);
      protected abstract string FormatFrameworkArgument(TestFixturOption runOption);

      protected static string ToShortPathName(string longName)
      {
         StringBuilder shortNameBuffer = new StringBuilder(256);

         int bufferSize = shortNameBuffer.Capacity;

         int result = GetShortPathName(longName, shortNameBuffer, bufferSize);

         return shortNameBuffer.ToString();
      }

      protected string GetProjectOutputPath(TestFixturOption testFixtureOptions)
      {
         //Need to get output path and output filename here
         Properties properties = testFixtureOptions.Project.ConfigurationManager.ActiveConfiguration.Properties;

         foreach (Property property in properties)
         {
            if (property.Name.Equals("OutputPath"))
            {
               string pName = property.Name;
               string value = property.Value as string;

               if (value != null)
               {
                  return ToShortPathName(ProjectExaminar.GetProjectPath(testFixtureOptions.Project)) + "\\" + value + testFixtureOptions.Project.Name + ".dll";
               }
            }
         }

         return string.Empty;
      }
   }
}
