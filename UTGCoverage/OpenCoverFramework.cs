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

namespace UTGCoverage
{
   /// <summary>
   /// Under construction still....
   /// </summary>
   public class OpenCoverFramework : AbstractCodeCoverageFramework
   {
      public override event CoverageExaminationStartedDelegate CoverageExaminationStarted;
      public override event CoverageExaminationEndedDelegate CoverageExaminationEnded;
      public override event BinariesLocationChangeDelegate BinariesLocationChanged;
      public override event ConsoleLocationChangeDelegate ConsoleLocationChanged;
      public override event CommandsLocationChangeDelegate CommandsLocationChanged;

      public override List<string> FrameworkDlls
      {
         get
         {
            return null;
         }
      }

      public override string BinariesDirectory
      {
         get
         {
            return this.binariesDirectory;
         }
         set
         {
            this.binariesDirectory = value;

            if (BinariesLocationChanged != null)
            {
               BinariesLocationChanged(value);
            }
         }
      }

      public override string ConsoleDirectory
      {
         get
         {
            if (this.consoleDirectory == null)
            {
               return this.binariesDirectory;
            }

            return this.consoleDirectory;
         }
         set
         {
            this.consoleDirectory = value;

            if (ConsoleLocationChanged != null)
            {
               ConsoleLocationChanged(value);
            }
         }
      }

      public override string CommandsDirectory
      {
         get
         {
            if (this.commandsDirectory == null)
            {
               return this.binariesDirectory;
            }

            return this.commandsDirectory;
         }
         set
         {
            this.commandsDirectory = value;

            if (CommandsLocationChanged != null)
            {
               CommandsLocationChanged(value);
            }
         }
      }

      public override string CommandName
      {
         get { return "OpenCover.Console.exe"; }
      }

      //If there is one
      public override string ConsoleName
      {
         get {

            if (this.consoleName == null)
            {
               return this.CommandName;
            }

            return this.consoleName; 
         }
      }

      //If there is one
      public override string ConfigurationXml
      {
         get { return null; }
      }

      internal OpenCoverFramework(string binariesDirectoryPath, string consoleDirectoriesPath, string commandsDirectoryPath, AbstractTestFramework testFramework)
      {
         this.binariesDirectory = binariesDirectoryPath;
         this.consoleDirectory = consoleDirectoriesPath;
         this.commandsDirectory = commandsDirectoryPath;
         this.testFramework = testFramework;
      }

      public override bool Examine(TestFixturOption testFixturOption, DTE2 applicationObject)
      {
         if (testFixturOption.Project == null) {

            throw new ArgumentNullException("testFixturOption.Project can not be null");
         }

         if (applicationObject == null) {

            throw new ArgumentNullException("applicationObject can not be null");
         }

         try
         {
            string outPutXml = "OpenCoverOutput.xml";

            if (this.binariesDirectory == null || this.binariesDirectory == string.Empty) {

               BrowseForCoverLibraries tvLocateNUnitBinaries = new BrowseForCoverLibraries(UTGHelper.CommonErrors.ERR_UNABLE_TO_LOCATE_COVER_INSTALL_FOLDER, this);
               
               tvLocateNUnitBinaries.Show();

               return false;
            }

            Project project = testFixturOption.Project;

            string projectPath = ProjectExaminar.GetProjectPath(project);

            if (CoverageExaminationStarted != null)  {

               CoverageExaminationStarted();
            }

            string tvArgument = FormatFrameworkArgument(testFixturOption);

            System.Diagnostics.ProcessStartInfo psi =
                 new System.Diagnostics.ProcessStartInfo(System.IO.Path.Combine(this.ConsoleDirectory, this.ConsoleName), tvArgument);

            psi.UseShellExecute = false;

            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            psi.RedirectStandardOutput = true;

            psi.CreateNoWindow = true; //this is the one that works

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

            System.IO.StreamReader myOutput = process.StandardOutput;

            string output = string.Empty;

            if (!process.WaitForExit(exitWaitPeriod)) {
               return false;
            }

            output += myOutput.ReadToEnd();

            bool success = ShowReport(projectPath, outPutXml);

            if (success)
            {
               if (CoverageExaminationEnded != null)
               {
                  CoverageExaminationEnded(project, "Changed text to something else here...");
               }
            }

            //consider moving to http://reportgenerator.codeplex.com/ instead....

            return true;
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
         }

         return false;
      }

#region Helper Methods

      protected bool ShowReport(string targetDirectory, string reportFile)
      {
         if (targetDirectory == null)
         {
            throw new ArgumentNullException("targetDirectory can not be null");
         }

         if (reportFile == null)
         {
            throw new ArgumentNullException("reportFile can not be null");
         }

         if (targetDirectory.Equals(string.Empty))
         {
            throw new ArgumentNullException("targetDirectory can not be empty");
         }

         if (reportFile.Equals(string.Empty))
         {
            throw new ArgumentNullException("reportFile can not be empty");
         }

         string ReportGeneratorPath = ToShortPathName(@"C:\Users\Sam\Documents\Visual Studio 2010\Projects\NMate\ReportGenerator\ReportGenerator.exe");

         string arg1 = ToShortPathName(System.IO.Path.Combine(targetDirectory, reportFile));
         string arg2 = targetDirectory + "\\OpenCoverReports";
         //arg2 = ToShortPathName(arg2);
         string args = "\"" + arg1 + "\" \"" + arg2 + "\"";

         System.Diagnostics.ProcessStartInfo psi =
                 new System.Diagnostics.ProcessStartInfo(ReportGeneratorPath, args);

         psi.UseShellExecute = false;

         psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

         psi.RedirectStandardOutput = true;

         psi.CreateNoWindow = true; //this is the one that works

         System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

         System.IO.StreamReader myOutput = process.StandardOutput;

         string output = string.Empty;

         if (!process.WaitForExit(exitWaitPeriod)){
            return false;
         }

         output += myOutput.ReadToEnd();

         return true;
      }

      protected override string FormatFrameworkArgument(TestFixturOption runOption)
      {
         if (runOption.Project == null)
         {
            throw new ArgumentNullException("testFixturOption.Project can not be null");
         }

         string outPutXml = "OpenCoverOutput.xml";

         #region example in plain test
         //OpenCover.Console.exe -register:user -target:nunit-console-x86.exe -targetargs:"/noshadow Test.dll" -filter:+[*]* -output:coverage.xml
         #endregion

         //We are better off with a StringBuilder here but since this is not an operation often performed we can get away with it
         string tvArguments = string.Empty;

         tvArguments += " -register:user ";

         tvArguments += "-target:\"" + System.IO.Path.Combine(this.testFramework.ConsoleDirectory, this.testFramework.ConsoleName);

         Project project = runOption.Project;

         string projectPath = ProjectExaminar.GetProjectPath(project);

         string tvOutputReportFullName = projectPath + "\\" + outPutXml;

         if (System.IO.File.Exists(tvOutputReportFullName))
         {
            System.IO.File.Delete(tvOutputReportFullName);
         }

         if (!UTGHelper.IO.CreateFile(tvOutputReportFullName))
         {
            return string.Empty;
         }
        
         tvArguments += "\" -output:\"";

         tvArguments += ToShortPathName(tvOutputReportFullName);

         tvArguments += "\" -targetargs:\"/noshadow ";
 
         tvArguments += GetProjectOutputPath(runOption);
       
         //Just to get this working we are not filtering however recode this so that we filter based on the runOption
         tvArguments += "\" -filter:+[*]*";
         
         return tvArguments;
      }

#endregion

   }
}
