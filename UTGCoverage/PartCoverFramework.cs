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
   public class PartCoverFramework : AbstractCodeCoverageFramework
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
         get { return "PartCover.exe"; }
      }

      //If there is one
      public override string ConsoleName
      {
         get {

            if (this.consoleName == null)
            {
               return this.commandName;
            }

            return this.consoleName;
         }
      }

      //If there is one
      public override string ConfigurationXml
      {
         get { return null; }
      }

      internal PartCoverFramework(string binariesDirectoryPath, string consoleDirectoriesPath, string commandsDirectoryPath, AbstractTestFramework testFramework)
      {
         this.binariesDirectory = binariesDirectoryPath;
         this.consoleDirectory = consoleDirectoriesPath;
         this.commandsDirectory = commandsDirectoryPath;
         this.testFramework = testFramework;
      }

      public override bool Examine(TestFixturOption testFixturOption, DTE2 applicationObject)
      {
         if (testFixturOption.Project == null)
         {
            throw new ArgumentNullException("testFixturOption.Project can not be null");
         }

         if (applicationObject == null)
         {
            throw new ArgumentNullException("applicationObject can not be null");
         }

         try
         {
            if (this.binariesDirectory == null || this.binariesDirectory == string.Empty) {

               BrowseForCoverLibraries tvLocateNUnitBinaries = new BrowseForCoverLibraries(UTGHelper.CommonErrors.ERR_UNABLE_TO_LOCATE_COVER_INSTALL_FOLDER, this);
               
               tvLocateNUnitBinaries.Show();

               return false;
            }

            Project project = testFixturOption.Project;

            if (CoverageExaminationStarted != null)  {
               CoverageExaminationStarted();
            }

            string tvArgument = FormatFrameworkArgument(testFixturOption);

            System.Diagnostics.ProcessStartInfo psi =
                 new System.Diagnostics.ProcessStartInfo(System.IO.Path.Combine(this.BinariesDirectory, this.CommandName), tvArgument);

            psi.UseShellExecute = false;

            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            psi.RedirectStandardOutput = true;

            psi.CreateNoWindow = true; //this is the one that works

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

            System.IO.StreamReader myOutput = process.StandardOutput;

            string output = string.Empty;

            if (!process.WaitForExit(exitWaitPeriod))
            {
               return false;
            }

            output += myOutput.ReadToEnd();

            UTGCoverage.CoverageReport report = new UTGCoverage.CoverageReport();

            System.IO.StreamReader reader = new System.IO.StreamReader(ProjectExaminar.GetProjectPath(testFixturOption.Project) + "\\PartCoverOutput.xml");

            UTGCoverage.ReportProvider.ReadReport(report, reader);

            reader.Close();

            System.Windows.Forms.TreeView trview = UTGCoverage.ReportProvider.ShowReport(report, UTGHelper.PartCoverUserControle.treeView1);

            UTGHelper.PartCoverUserControle.treeView1.Refresh();

            if (CoverageExaminationEnded != null) {

               CoverageExaminationEnded(testFixturOption.Project, string.Format("{0} at {1}.", "PartCoverOutput.xml", ProjectExaminar.GetProjectPath(testFixturOption.Project)));   
            }

            return true;
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
         }

         return false;
      }

#region Helper Methods

      protected override string FormatFrameworkArgument(TestFixturOption runOption)
      {
         #region example in plain test
         //PartCover.exe 
         //--target="C:\Program Files\NUnit 2.4.6\bin\nunit-console.exe" 
         //--target-args="C:\Temp\MyClassLibrary\MyClassLibrary.nunit\MyClassLibrary.nunit.csproj" 
         //--include=[MyClassLibrary]*
         #endregion

         //fix was found http://www.pinvoke.net/default.aspx/kernel32.GetShortPathName
         string tvArguments = string.Empty;

         tvArguments += "--target=\"" + System.IO.Path.Combine(this.CommandsDirectory, this.ConsoleName);

         string tvOutputReportFullName = ProjectExaminar.GetProjectPath(runOption.Project) + "\\PartCoverOutput.xml";

         if (System.IO.File.Exists(tvOutputReportFullName))
         {
            System.IO.File.Delete(tvOutputReportFullName);
         }

         if (!UTGHelper.IO.CreateFile(tvOutputReportFullName))
         {
            return string.Empty;
         }

         tvArguments += "\" --output=\"";

         tvArguments += ToShortPathName(tvOutputReportFullName);

         tvArguments += "\" --target-args=\"";

         //Next line can be iffy when doing this runOption.Project.Name + ".csproj"
         tvArguments += ToShortPathName(ProjectExaminar.GetProjectPath(runOption.Project)) + "\\" + runOption.Project.Name + ".csproj";

         if (runOption.CClass == null)
         {
            tvArguments += "\" --include=[";
            tvArguments += ProjectExaminar.GetProjectNamespaceName(ProjectExaminar.GetUnitTestProjectsOriginalProject(
                  runOption.Project.Name, (DTE2)runOption.Project.DTE, testFramework.TestProjectPostFix));
            tvArguments += "]";
            tvArguments += "*";
         }
         else
         {
            tvArguments += "\"";

            string tvClassName = ProjectItemExaminor.GetUnitTestClassOriginalClassName(runOption.CClass.Name, testFramework.TestClassFixturePostFix);
            string tvClassNamespace = ProjectItemExaminor.GetUnitTestClassOriginalClassNamespace(runOption.CClass.Namespace.FullName);

            Project project = ProjectExaminar.GetUnitTestProjectsOriginalProject(runOption.Project.Name, (DTE2)runOption.Project.DTE, testFramework.TestProjectPostFix);

            if (project == null)
            {
               throw new Exception(string.Format("Unable to locate Unit Test Project for the Project {0}.", runOption.Project.Name));
            }

            tvArguments += " --include=[";
            tvArguments += ProjectExaminar.GetProjectNamespaceName(project);
            tvArguments += "]";
            tvArguments += tvClassNamespace;
            tvArguments += ".";
            tvArguments += tvClassName;
            tvArguments += "* ";
         }

         return tvArguments;
      }

#endregion

   }
}
