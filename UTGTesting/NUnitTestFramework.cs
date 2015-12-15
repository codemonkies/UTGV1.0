//#define _PROCESS_START_WIN32_

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using UTGHelper;
using UTGManagerAndExaminor;

namespace UTGTesting
{
   public class NUnitTestFramework : AbstractTestFramework
   {
      public override event TestStartedDelegate TestStarted;
      public override event TestEndedDelegate TestEnded;
      public override event BinariesLocationChangeDelegate BinariesLocationChanged;
      public override event ConsoleLocationChangeDelegate ConsoleLocationChanged;
      public override event CommandsLocationChangeDelegate CommandsLocationChanged;

      public override string TestClassFixturePostFix
      {
         get { return "NUnit"; } //Update the class as well
      }

      public override string TestProjectConfiurationPostFix
      {
         get { return @".nunit"; }
      }

      public override string TestProjectPostFix
      {
         get { return ".nunit"; }
      }

      public override List<string> FrameworkDlls
      {
         get
         {
            List<string> dlls = new List<string>();
            dlls.Add("nunit.framework.dll");
            return dlls;
         }
      }

      public override List<string> FrameworkTestAttributes
      {
         get
         {
            List<string> attributes = new List<string>();
            attributes.Add("test");
            attributes.Add("nunit.framework.test");
            attributes.Add("teardown");
            attributes.Add("nunit.framework.teardown");
            return attributes;
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

      //If there is a default one
      public override string CommandName
      {
         get { return null; }
      }

      //If there is one
      public override string ConsoleName
      {
         get { return "nunit-console.exe"; }
      }

      //If there is one
      public override string ConfigurationXml
      {
         get { return null; }
      }

      internal NUnitTestFramework(string binariesDirectoryPath, string consoleDirectoriesPath, string commandsDirectoryPath)
      {
         this.binariesDirectory = binariesDirectoryPath;
         this.consoleDirectory = consoleDirectoriesPath;
         this.commandsDirectory = commandsDirectoryPath;
      }

      /// <summary>
      /// A Patch function to get rid of everything before the xml tag
      /// </summary>
      /// <param name="testOutput">the xmlOutput of an nunit framework test</param>
      /// <returns></returns>
      private string PatchFunctionMessageTestOutput(string testOutput)
      {
         int indexOfDocBegening =
            testOutput.IndexOf("<?xml version=\"1.0\"");

         if (indexOfDocBegening < 0)
            return string.Empty;

         testOutput = testOutput.Substring(indexOfDocBegening, (testOutput.Length - 1) - indexOfDocBegening);

         return testOutput;
      }

      public override bool run(TestFixturOption testFixturOption, DTE2 applicationObject)
      {
         try
         {
            string output = string.Empty;
            string tvTestFixtureOptions = string.Empty;
            string tvOutputFileFullname = string.Empty;

            if (testFixturOption.CClass != null)
            {
               tvTestFixtureOptions = " /fixture:";

               tvTestFixtureOptions += testFixturOption.CClass.FullName;

               tvOutputFileFullname = UTGManagerAndExaminor.ProjectExaminar.GetProjectOutputFullAssemblyName(testFixturOption.CClass.ProjectItem.ContainingProject);

               tvTestFixtureOptions += " \"" + tvOutputFileFullname + "\"";

               if (testFixturOption.ConfigurationFile != string.Empty)
                  tvTestFixtureOptions += " /config:\"" + tvOutputFileFullname + ".config\"";
            }
            else if (testFixturOption.CFunction != null)
            {
               tvTestFixtureOptions = " /run:";

               CodeClass parentClass = testFixturOption.CFunction.Parent as CodeClass;

               tvTestFixtureOptions += testFixturOption.CFunction.FullName;

               tvOutputFileFullname = UTGManagerAndExaminor.ProjectExaminar.GetProjectOutputFullAssemblyName(parentClass.ProjectItem.ContainingProject);

               tvTestFixtureOptions += " \"" + tvOutputFileFullname + "\"";
             
               if (tvTestFixtureOptions.Length > 0)
               {
                  if (testFixturOption.ConfigurationFile != string.Empty)
                     tvTestFixtureOptions += " /config:\"" + tvOutputFileFullname + ".config\"";
               }
            }
            else
            {
               if (tvOutputFileFullname.Equals(string.Empty))
               {
                  tvOutputFileFullname = UTGManagerAndExaminor.ProjectExaminar.GetProjectOutputFullAssemblyName(testFixturOption.Project);
                  tvTestFixtureOptions = "\"" + tvOutputFileFullname + "\"";
               }

               if (testFixturOption.ConfigurationFile != string.Empty)
                  tvTestFixtureOptions += " /config:\"" + tvOutputFileFullname + ".config\"";
            }

            tvTestFixtureOptions += " /xmlConsole";

            ProjectExaminar.GetProjectOutputType(testFixturOption.Project);

            ProjectItem projectItem = ProjectExaminar.GetProjectConfigurationFile(testFixturOption.Project);

            Project project = testFixturOption.Project;

            if (this.binariesDirectory == null || this.binariesDirectory == string.Empty) {
               BrowseForUnitLibraries tvLocateNUnitBinaries = new BrowseForUnitLibraries(
                         UTGHelper.CommonErrors.ERR_UNABLE_TO_LOCATE_NUNIT_INSTALL_FOLDER, BrowseForUnitLibraries.BrowseForNunitDialogType.libraries, this);

               tvLocateNUnitBinaries.Show();

               return false;
            }

            if (this.consoleDirectory == null || this.consoleDirectory == string.Empty) {

               BrowseForUnitLibraries tvLocateNUnitBinaries = new BrowseForUnitLibraries(
                       UTGHelper.CommonErrors.ERR_UNABLE_TO_LOCATE_NUNIT_CONSOLE, BrowseForUnitLibraries.BrowseForNunitDialogType.console, this);

               tvLocateNUnitBinaries.Show();

               return false;
            }

            string processFullPath = System.IO.Path.Combine(this.consoleDirectory, this.ConsoleName);

            if (TestStarted != null)
            {
               TestStarted();
            }

#if _PROCESS_START_WIN32_
            
            ProcessTypes.StartupInfo startInfo = new ProcessTypes.StartupInfo();
            ProcessTypes.ProcessInfo processInfo = new ProcessTypes.ProcessInfo();

            IntPtr ThreadHandle = IntPtr.Zero;

            string commandLine = (arguments != null ? libraryPath + arguments : libraryPath);

            //string commandLine = @"C:\Users\Sam\documents\visual studio 2010\Projects\MyClassLibrary\MyClassLibrary.nunit\bin\Debug\MyClassLibrary.nunit.dll"; 

            bool success = CreateProcess(processFullPath, commandLine, IntPtr.Zero, IntPtr.Zero, false,
               ProcessTypes.ProcessCreationFlags.CREATE_SUSPENDED, 
               IntPtr.Zero, null, ref startInfo, out processInfo);

            if (!success) {
               UTGHelper.Logger.LogMessage("Failed to start unit test process");
               return false;
            }

            ThreadHandle = processInfo.hThread;
            uint PID = processInfo.dwProcessId;

            ResumeThread(ThreadHandle);

            return true;
           
#else

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(processFullPath, tvTestFixtureOptions);

            //we choose to redirect output, but we could also just use the TestResult.xml file 
            psi.RedirectStandardOutput = true;

            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            psi.UseShellExecute = false;

            psi.CreateNoWindow = true; //this is the one that works

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

            if (testFixturOption.IsDebugging)
            {
               AttachToProcessThread(applicationObject);
            }

            System.IO.StreamReader myOutput = process.StandardOutput;

            process.WaitForExit(60000);

            if (!process.HasExited)
            {
               UTGHelper.ErrorHandler.ShowMessage(UTGHelper.CommonErrors.ERR_UNABLE_TO_RUN_UNITTEST + " " + "Failed to exit process");

               return false;
            }

            output += myOutput.ReadToEnd();
#endif
            //This should be fixed by simple passing the argument that suppresses the copyright text /nologo 
            output = PatchFunctionMessageTestOutput(output);

            if (output.Equals(string.Empty))
            {

               UTGHelper.ErrorHandler.ShowMessage(UTGHelper.CommonErrors.ERR_UNABLE_TO_RUN_UNITTEST);

               return false;
            }

            System.Xml.XmlDocument xmlTestResultDoc =
               NUnitXmlResultProccessor.ToXmlDoc(output);

            System.Xml.XmlNodeList xmlTestResultsNodeList =
               NUnitXmlResultProccessor.GetTestCases(xmlTestResultDoc);

            List<System.Xml.XmlNode> successfulTestSuites =
              NUnitXmlResultProccessor.GetSuccessfulTestSuites(xmlTestResultsNodeList);

            List<System.Xml.XmlNode> failedTestSuites =
             NUnitXmlResultProccessor.GetFailedTestSuites(xmlTestResultsNodeList);

            List<System.Xml.XmlNode> failedExecutionSuites =
               NUnitXmlResultProccessor.GetFailedExecutionSuites(xmlTestResultsNodeList);

            List<UnitTestError> lstUnitTestPassed =
               NUnitXmlResultProccessor.GetSuccessfulTestSuitesAsUnitTestErrors(successfulTestSuites);

            List<UnitTestError> lstUnitTestErrors =
               NUnitXmlResultProccessor.GetFailedTestSuitesAsUnitTestErrors(failedTestSuites);

            List<UnitTestError> lstUnitTestsFialedToExecute =
               NUnitXmlResultProccessor.GetFailedExecutionsAsUnitTestErrors(failedExecutionSuites);

            if (TestEnded != null)
            {
               TestEnded(testFixturOption.Project, lstUnitTestPassed, lstUnitTestErrors, lstUnitTestsFialedToExecute);
            }

            return true;
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
         }

         return false;
      }

      /*
      public override bool runOlder1(string testOptions, TestFixturOption TestFixturOption, DTE2 applicationObject)
      {
         try {

            ProjectExaminar.GetProjectOutputType(TestFixturOption.Project);

            ProjectItem projectItem = ProjectExaminar.GetProjectConfigurationFile(TestFixturOption.Project);

            Project project = TestFixturOption.Project;

            if (this.binariesDirectory == null || this.binariesDirectory == string.Empty) {
               BrowseForUnitLibraries tvLocateNUnitBinaries = new BrowseForUnitLibraries(
                         UTGHelper.CommonErrors.ERR_UNABLE_TO_LOCATE_NUNIT_INSTALL_FOLDER, BrowseForUnitLibraries.BrowseForNunitDialogType.libraries, this);

               tvLocateNUnitBinaries.Show();

               return false;
            }

            if (this.consoleDirectory == null || this.consoleDirectory == string.Empty) {

               BrowseForUnitLibraries tvLocateNUnitBinaries = new BrowseForUnitLibraries(
                       UTGHelper.CommonErrors.ERR_UNABLE_TO_LOCATE_NUNIT_CONSOLE, BrowseForUnitLibraries.BrowseForNunitDialogType.console, this);

               tvLocateNUnitBinaries.Show();

               return false;
            }

            string processFullPath = System.IO.Path.Combine(this.consoleDirectory, this.ConsoleName);

            if (TestStarted != null) {
               TestStarted();
            }

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(processFullPath, testOptions);

            psi.RedirectStandardOutput = true;

            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            psi.UseShellExecute = false;

            psi.CreateNoWindow = true; //this is the one that works

            //if (TestFixturOption.IsDebugging)  {
            //   psi.Arguments = "CREATE_SUSPENDED";
            //}

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

            if (TestFixturOption.IsDebugging)
            {
               foreach (System.Diagnostics.ProcessThread t in process.Threads)
               {
                  SuspendThread((IntPtr)t.StartAddress);
               }

               System.Threading.ParameterizedThreadStart attachThread = new System.Threading.ParameterizedThreadStart(AttachToProcessThread);

               attachThread.Invoke(applicationObject);

               semaphore.WaitOne();

               foreach (System.Diagnostics.ProcessThread thread in process.Threads)
               {
                  ResumeThread((IntPtr)thread.StartAddress);
               }
            }

            System.IO.StreamReader myOutput = process.StandardOutput;

            string output = string.Empty;

            while (!process.HasExited)
            {
               //Add text to status bar saying running test
               //....

               System.Threading.Thread.Sleep(1000);
            }

            output += myOutput.ReadToEnd();

            //This should be fixed by simple passing the argument that suppresses the copyright text /nologo 
            output = PatchFunctionMessageTestOutput(output);

            if (output.Equals(string.Empty))
            {
               UTGHelper.ErrorHandler.ShowMessage(UTGHelper.CommonErrors.ERR_UNABLE_TO_RUN_UNITTEST);

               return false;
            }

            System.Xml.XmlDocument xmlTestResultDoc =
               NUnitXmlResultProccessor.ToXmlDoc(output);

            System.Xml.XmlNodeList xmlTestResultsNodeList =
               NUnitXmlResultProccessor.GetTestCases(xmlTestResultDoc);

            List<System.Xml.XmlNode> successfulTestSuites =
              NUnitXmlResultProccessor.GetSuccessfulTestSuites(xmlTestResultsNodeList);

            List<System.Xml.XmlNode> failedTestSuites =
             NUnitXmlResultProccessor.GetFailedTestSuites(xmlTestResultsNodeList);

            List<System.Xml.XmlNode> failedExecutionSuites =
               NUnitXmlResultProccessor.GetFailedExecutionSuites(xmlTestResultsNodeList);

            List<UnitTestError> lstUnitTestPassed =
               NUnitXmlResultProccessor.GetSuccessfulTestSuitesAsUnitTestErrors(successfulTestSuites);

            List<UnitTestError> lstUnitTestErrors =
               NUnitXmlResultProccessor.GetFailedTestSuitesAsUnitTestErrors(failedTestSuites);

            List<UnitTestError> lstUnitTestsFialedToExecute =
               NUnitXmlResultProccessor.GetFailedExecutionsAsUnitTestErrors(failedExecutionSuites);

            if (TestEnded != null) {

               TestEnded(TestFixturOption.Project, lstUnitTestPassed, lstUnitTestErrors, lstUnitTestsFialedToExecute);
            }

            return true;
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            UTGHelper.ErrorHandler.HandleException(ex);
         }

         return false;
      }
       */
   }
}
