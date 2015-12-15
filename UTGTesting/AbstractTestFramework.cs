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
using UTGTesting.ProcessTypes;

namespace UTGTesting
{
   public abstract class AbstractTestFramework
   {
      [DllImport("kernel32.dll")]
      public static extern bool CreateProcess(string lpApplicationName,
             string lpCommandLine, IntPtr lpProcessAttributes,
             IntPtr lpThreadAttributes,
             bool bInheritHandles, ProcessCreationFlags dwCreationFlags,
             IntPtr lpEnvironment, string lpCurrentDirectory,
             ref StartupInfo lpStartupInfo,
             out ProcessInfo lpProcessInformation);

      //Externals
      [DllImport("kernel32.dll")]
      protected static extern uint SuspendThread(IntPtr hThread);

      [DllImport("kernel32.dll")]
      protected static extern uint ResumeThread(IntPtr hThread);

      //Delegates and Events
      public delegate void TestStartedDelegate();
      public delegate void TestEndedDelegate(
      Project project,
      List<UnitTestError> lstUnitTestPassed,
      List<UnitTestError> lstUnitTestErrors,
      List<UnitTestError> lstUnitTestsFialedToExecute
      );
      public delegate void BinariesLocationChangeDelegate(string newLocation);
      public delegate void ConsoleLocationChangeDelegate(string newLocation);
      public delegate void CommandsLocationChangeDelegate(string newLocation);

      public abstract event TestStartedDelegate TestStarted;
      public abstract event TestEndedDelegate TestEnded;
      public abstract event BinariesLocationChangeDelegate BinariesLocationChanged;
      public abstract event ConsoleLocationChangeDelegate ConsoleLocationChanged;
      public abstract event CommandsLocationChangeDelegate CommandsLocationChanged;

      //Members
      protected string binariesDirectory, consoleDirectory, commandsDirectory, commandName, consoleName, configurationXml; 

      //Properties
      public abstract string TestClassFixturePostFix { get; }
      public abstract string TestProjectConfiurationPostFix { get; }
      public abstract string TestProjectPostFix { get; }
      public abstract List<string> FrameworkDlls { get; }
      public abstract List<string> FrameworkTestAttributes { get; }

      public abstract string BinariesDirectory { get; set; }
      public abstract string ConsoleDirectory { get; set; }
      public abstract string CommandsDirectory { get; set; }
      public abstract string CommandName { get; }
      public abstract string ConsoleName { get; }
      public abstract string ConfigurationXml { get; }

      //Methods
      protected void AttachToProcessThread(object applicationObject)
      {
         Process process = GetProcess(applicationObject);

         if (process != null)
            process.Attach();
      }

      protected Process GetProcess(object applicationObject)
      {
         DTE2 applicationObjectCast = applicationObject as DTE2;

         if (applicationObjectCast == null)
         {
            UTGHelper.ErrorHandler.ShowMessage("Failed to attach to process. Cast Failed");

            return null;
         }

         Debugger2 lDebugger = (Debugger2)applicationObjectCast.Debugger;

         foreach (Process lLocalProcess in lDebugger.LocalProcesses)
         {
            if (lLocalProcess.Name.IndexOf(ConsoleName) >= 0)
            {
               return lLocalProcess;
            }
         }

         return null;
      }

      public abstract bool run(TestFixturOption TestFixturOption, DTE2 applicationObject);
   }
}
