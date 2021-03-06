using System;
using System.Collections.Generic;
using System.Text;

using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using UTGHelper;
using UTGeneratorLibrary;
using UTGManagerAndExaminor;
using System.Security.Cryptography;
using System.Windows.Forms;
using UTGTesting;
using UTGCoverage;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace UTG
{
   /// <summary>The object for implementing an Add-in.
   /// </summary>
   /// <seealso class='IDTExtensibility2' />

   [GuidAttribute("B8133BC0-BC88-4417-94BD-B2D2D942875B")]
   public class Connect : Package, IDTExtensibility2, IDTCommandTarget
   {
      private DTE2 _applicationObject;
      private AddIn _addInInstance;

      private static ErrorListProvider tvErrorListProvider;
      static int REBUILD_COMMAND_BAR_VS_ID = 3604482; //rebuild = 746;
        
      private static int ivRan = 0;
      private static int ivSuccess = 0;
      private static int ivFailed = 0;
      private static int ivNorun = 0;
      private static int command = 1000;

      AbstractTestFramework testFramework;
      AbstractCodeCoverageFramework coverageFramework;

      /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
      public Connect()
      {
         //Creating the unit test framework object. Default NUnit.
         testFramework = TestFrameworkFactory.CreateNUnitTestFramework(Settings.Default.UnitBinariesDirectory, Settings.Default.UnitConsoleDirectory, Settings.Default.UnitCommandsDirectory);
         testFramework.TestStarted += new AbstractTestFramework.TestStartedDelegate(testFramework_testStartedEvent);
         testFramework.TestEnded += new AbstractTestFramework.TestEndedDelegate(testFramework_testEndedEvent);
         testFramework.BinariesLocationChanged += new AbstractTestFramework.BinariesLocationChangeDelegate(testFramework_BinariesLocationChanged);
         testFramework.ConsoleLocationChanged += new AbstractTestFramework.ConsoleLocationChangeDelegate(testFramework_ConsoleLocationChanged);
         testFramework.CommandsLocationChanged += new AbstractTestFramework.CommandsLocationChangeDelegate(testFramework_CommandsLocationChanged);

           
         //Creating the coverage framework object. Default OpenCover.
         coverageFramework = CodeCoverageFactory.CreateOpenCoverFramework(Settings.Default.CoverageBinariesDirectory, Settings.Default.CoverageBinariesDirectory, Settings.Default.CoverageBinariesDirectory, this.testFramework);
         coverageFramework.CoverageExaminationStarted += new AbstractCodeCoverageFramework.CoverageExaminationStartedDelegate(coverageFramework_CoverageExaminationStarted);
         coverageFramework.CoverageExaminationEnded += new AbstractCodeCoverageFramework.CoverageExaminationEndedDelegate(coverageFramework_CoverageExaminationEnded);
         coverageFramework.BinariesLocationChanged += new AbstractCodeCoverageFramework.BinariesLocationChangeDelegate(coverageFramework_BinariesLocationChanged);
         coverageFramework.ConsoleLocationChanged += new AbstractCodeCoverageFramework.ConsoleLocationChangeDelegate(coverageFramework_ConsoleLocationChanged);
         coverageFramework.CommandsLocationChanged += new AbstractCodeCoverageFramework.CommandsLocationChangeDelegate(coverageFramework_CommandsLocationChanged);
      }

      
      void testFramework_CommandsLocationChanged(string newLocation)
      {
         Settings.Default.UnitCommandsDirectory = newLocation;
         Settings.Default.Save();
      }

      
      void testFramework_ConsoleLocationChanged(string newLocation)
      {
         Settings.Default.UnitConsoleDirectory = newLocation;
         Settings.Default.Save();
      }

      
      void testFramework_BinariesLocationChanged(string newLocation)
      {
         Settings.Default.UnitBinariesDirectory = newLocation;
         Settings.Default.Save();
      }


      void testFramework_testEndedEvent(Project project, List<UnitTestError> lstUnitTestPassed, List<UnitTestError> lstUnitTestErrors, List<UnitTestError> lstUnitTestsFialedToExecute)
      {
         try
         {
            OutputWindowPane unitTestOutputPane = null;

            foreach (OutputWindowPane outputWindowPane in _applicationObject.ToolWindows.OutputWindow.OutputWindowPanes)
            {

               if (outputWindowPane.Name.Equals(UTGHelper.CommonStrings.UNIT_TEST_OUTPUT_PANE_NAME))
                  unitTestOutputPane = outputWindowPane;
            }

            if (unitTestOutputPane == null)
               unitTestOutputPane = _applicationObject.ToolWindows.OutputWindow.OutputWindowPanes.Add(UTGHelper.CommonStrings.UNIT_TEST_OUTPUT_PANE_NAME);

            unitTestOutputPane.Clear();

            foreach (UnitTestError unitTestError in lstUnitTestsFialedToExecute)
            {
               unitTestOutputPane.OutputString(string.Format("Failed to execute: File {0} Token {1}\n", unitTestError.File, unitTestError.Name));
            }

            foreach (UnitTestError unitTestPassed in lstUnitTestPassed)
            {
               string tvMessage = string.Format("Success: File {0} Token {1}\n", unitTestPassed.File, unitTestPassed.Name);

               unitTestOutputPane.OutputString(tvMessage);
            }

            Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_applicationObject;

            ServiceProvider serviceProvider = new ServiceProvider(sp);

            IVsHierarchy hierarchy = ProjectExaminar.GetHierarchy(serviceProvider, project);

            for (int i = 0; i < lstUnitTestErrors.Count; i++)
            {
               UnitTestError unitTestError = lstUnitTestErrors[i];

               ErrorTask errTask = new ErrorTask();
               errTask.Line = unitTestError.Line;
               errTask.Document = unitTestError.File;
               errTask.Text = UTGHelper.CommonStrings.UNIT_TEST_OUTPUT_PANE_TEXT_PREFIX + unitTestError.Text;
               errTask.HierarchyItem = hierarchy;

               errTask.Navigate += new EventHandler(NavigateToError);

               ErrorTaskApplicationObject errorApplication = new ErrorTaskApplicationObject();
               errorApplication.TheErrorTask = errTask;
               errorApplication.ApplicationObject = _applicationObject;

               unitTestError.TheErrorTask = errorApplication;
               unitTestError.ProjectName = project.Name;
            }

            OutputUnitTestErrorsToVisualStudioErrorList(lstUnitTestErrors, project);

            ivSuccess = lstUnitTestPassed.Count;
            ivFailed = lstUnitTestErrors.Count;
            ivNorun = lstUnitTestsFialedToExecute.Count;
            ivRan = ivSuccess + ivFailed + ivRan;

            OnUnitTestingFinished();
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            UTGHelper.ErrorHandler.HandleException(ex);
         }
      }


      void testFramework_testStartedEvent()
      {
         _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_TESTING_STARTED;
      }
      

      void coverageFramework_CommandsLocationChanged(string newLocation)
      {
         Settings.Default.CoverageCommandsDirectory = newLocation;
         Settings.Default.Save();
      }


      void coverageFramework_ConsoleLocationChanged(string newLocation)
      {
         Settings.Default.CoverageConsoleDirectory = newLocation;
         Settings.Default.Save();
      }
      

      void coverageFramework_BinariesLocationChanged(string newLocation)
      {
         Settings.Default.CoverageBinariesDirectory = newLocation;
         Settings.Default.Save();
      }


      void coverageFramework_CoverageExaminationEnded(Project project, string message)
      {
         OnCoverFinished(message);

         CreatePartCoverView();
      }


      void coverageFramework_CoverageExaminationStarted()
      {
         OnCoverStarted();
      }

 
      private Window CreatePartCoverView()
      {
         try
         {
            EnvDTE80.Windows2 wins2obj;
            Window newWinobj = null;
            AddIn addinobj;

            foreach (AddIn addIn in _applicationObject.AddIns)
            {
               if (!addIn.ProgID.Equals("UTG.Connect"))
                  continue;

               addinobj = addIn;//_applicationObject.AddIns.Item("NMateForNUnit");
               wins2obj = (Windows2)_applicationObject.Windows;
               object ctlobj = new object();

               newWinobj = wins2obj.CreateToolWindow2(
                  addinobj,
                  UTGManagerAndExaminor.ProjectExaminar.GetFilePath(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\UTGHelper.dll",
                  "UTGHelper.PartCoverUserControle",
                  UTGHelper.CommonStrings.APPLICATION_NAME + " CodeCoverageResults",
                  "{33991ECB-20D1-48b3-B9C0-86E6480FBD4E}",
                  ref ctlobj);

               newWinobj.Visible = true;


               return newWinobj;
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            //System.Windows.Forms.MessageBox.Show("Failed to create NMate's Error List! Errors are being directed to Visual Studio's Error List. " + ex.Message);
            throw new Exception("Failed to create Code Coverage Window!");
         }

         return null;
      }


      protected override void Initialize()
      {
         //Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
         base.Initialize();
      }


      private void CheckForUpdates()
      {
         try{
            
            string content = UTGHelper.Logger.ReadFromFile(System.Environment.SystemDirectory + "\\" + UTGHelper.Logger.UPDATENOTIFYFILE);

            if (content != null && content.Length > 0 && content.Trim().Equals("1"))
            {
               UTGHelper.Logger.WriteToFile(System.Environment.SystemDirectory + "\\" + UTGHelper.Logger.UPDATENOTIFYFILE, "0");

               System.Windows.Forms.DialogResult tvDialogResult = System.Windows.Forms.MessageBox.Show(
                  UTGHelper.CommonUserMessages.MSG_UPDATE_AVAILABLE,
                  "nMate Notification",
                    System.Windows.Forms.MessageBoxButtons.YesNo, 
                    System.Windows.Forms.MessageBoxIcon.Question,
                    System.Windows.Forms.MessageBoxDefaultButton.Button1,
                    System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);

               //If user chooses to update then do whatever needed for update
               if (tvDialogResult == System.Windows.Forms.DialogResult.Yes){
                  //start the update process example download update file
                  //there are other ways such as automatic update
                  //System.Diagnostics.Process.Start("http://www.nmateupdate.com/updatefile.zip");
               }
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
         }
      }

      /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
      /// <param term='application'>Root object of the host application.</param>
      /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
      /// <param term='addInInst'>Object representing this Add-in.</param>
      /// <seealso class='IDTExtensibility2' />
      public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
      {
         _applicationObject = (DTE2)application;
         _addInInstance = (AddIn)addInInst;

         try
         {
            UTGHelper.UserInviromentSettings.TAB = Settings.Default.Tab; 
         }
         catch (Exception ex) {
            Logger.LogException(ex);
         }
      }    


      /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
      /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
      /// <param term='custom'>Array of parameters that are host application specific.</param>
      /// <seealso class='IDTExtensibility2' />
      public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom){}


      /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
      /// <param term='custom'>Array of parameters that are host application specific.</param>
      /// <seealso class='IDTExtensibility2' />		
      public void OnAddInsUpdate(ref Array custom){}


      /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
      /// <param term='custom'>Array of parameters that are host application specific.</param>
      /// <seealso class='IDTExtensibility2' />
      public void OnStartupComplete(ref Array custom)
      {
         //Create CommandBarControls 
         CommandBarControl tokenCommandBarControle = CreateTokenCodeCommand();

         if (tokenCommandBarControle == null){
            Logger.LogMessage("Failed to create CommandBarControl. tokenCommandBarControle resulted in null");
         }

         CommandBarControl projectCommandBarControle = CreateProjectCodeCommand();

         if (projectCommandBarControle == null){
            Logger.LogMessage("Failed to create CommandBarControl. projectCommandBarControle resulted in null");
         }

         CommandBarControl tokenAttachToProcessCommandBarControle = CreateAttachToProcessCodeCommand();

         if (tokenAttachToProcessCommandBarControle == null) {
            Logger.LogMessage("Failed to create CommandBarControl. tokenAttachToProcessCommandBarControle resulted in null");
         }

         CommandBarControl testMethodOnlyCommandBar = CreateUnitTestMethodCodeCommand();

         if (testMethodOnlyCommandBar == null) {
            Logger.LogMessage("Failed to create CommandBarControl. testMethodOnlyCommandBar resulted in null");
         }

         CommandBarControl testClassOnlyCommandBar = CreateUnitTestClassCodeCommand();

         if (testClassOnlyCommandBar == null) {
            Logger.LogMessage("Failed to create CommandBarControl. testClassOnlyCommandBar resulted in null");
         }

         CommandBarControl partCoverClassCommandBar = CreatePartCoverCodeWindowCommand();

         if (partCoverClassCommandBar == null) {
            Logger.LogMessage("Failed to create CommandBarControl. partCoverClassCommandBar resulted in null");
         }

         CommandBarControl projectBarButton = CreateProjectCommand();

         if (projectBarButton == null){
            Logger.LogMessage("Failed to create CommandBarControl. projectBarButton resulted in null");
         }

         CommandBarControl projectSolutionBarButton = CreateSolutionCommand();

         if (projectSolutionBarButton == null){
            Logger.LogMessage("Failed to create CommandBarControl. projectSolutionBarButton resulted in null");
         }

         CommandBarControl projectErrorListClearBarButton = CreateErrorListClearCommand();

         if (projectErrorListClearBarButton == null) {
            Logger.LogMessage("Failed to create CommandBarControl. projectErrorListClearBarButton resulted in null");
         }

         CommandBarControl projectResyncRefsClearBarButton = CreateSyncProjectReferencesCommand();

         if (projectResyncRefsClearBarButton == null) {
            Logger.LogMessage("Failed to create CommandBarControl. projectResyncRefsClearBarButton resulted in null");
         }

         CommandBarControl projectPartCoverBarButton = CreatePartCoverProjectCommand();

         if (projectPartCoverBarButton == null) {
            Logger.LogMessage("Failed to create CommandBarControl. projectPartCoverBarButton resulted in null");
         }
      }


      /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
      /// <param term='custom'>Array of parameters that are host application specific.</param>
      /// <seealso class='IDTExtensibility2' />
      public void OnBeginShutdown(ref Array custom) {}

      #region IDTCommandTarget Members

      public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
      {
         try
         {
            Handled = false;

            if (ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
               //re-factor
               if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_TOKEN_COMMAND_NAME)
               {
                  OnTokenCodeWindowCommandHandler();
                  Handled = true;
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_TEST_METHOD_COMMAND_NAME)
               {
                  OnTestMethodCommandHandler();
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_TEST_CLASS_COMMAND_NAME)
               {
                  OnTestClassCommandHandler();
               }
               else
               if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_SOLUTION_COMMAND_NAME)
               {
                  OnSolutionCommandHandler();
               }
               else
               if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_ATTACH_COMMAND_NAME)
               {
                  OnAttachToProcessFile();
                  Handled = true;
               }
               else
               if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_NAME)
               {
                  OnProjectCodeWindowCommandHandler();
                  Handled = true;
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_PARTCOVER_COMMAND_NAME)
               {
                  List<TestFixturOption> lstRunOptions = new List<TestFixturOption>();
                  
                  TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

                  //current scope
                  FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

                  //Noticed that null happens on metadata files...
                  if (fcm == null)
                  {
                     throw new Exception(UTGHelper.CommonErrors.ERR_ILLEGAL_TEXT_SELECTION);
                  }

                  CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

                  if (ce != null && (ce is CodeClass || ce is CodeClass2))
                  {
                     TestFixturOption runOption = new TestFixturOption();
                     runOption.Project = ce.ProjectItem.ContainingProject;
                     CodeClass cc = ce as CodeClass;
                     runOption.CClass = cc;
                     lstRunOptions.Add(runOption);
                  }

                  OnPartCoverCodeWindowCommandHandler(lstRunOptions);
                  
                  Handled = true;
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_COMMAND_NAME)
               {
                  List<TestFixturOption> lstRunOptions = new List<TestFixturOption>();
                  
                  List<Project> lstProjects = GetSelectedProjectInProjectHierarchy();

                  foreach (Project pro in lstProjects)
                  {
                     TestFixturOption runOptions = new TestFixturOption();
                     runOptions.Project = pro;
                     runOptions.IsDebugging = false;
                     lstRunOptions.Add(runOptions);
                  }

                  OnRunTestCommandHandler(lstRunOptions);

                  Handled = true;
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_PARTCOVER_COMMAND_NAME)
               {
                  List<TestFixturOption> lstRunOptions = new List<TestFixturOption>();
                  List<Project> lstProjects = GetSelectedProjectInProjectHierarchy();

                  foreach (Project pro in lstProjects)
                  {
                     TestFixturOption runOptions = new TestFixturOption();
                     runOptions.Project = pro;
                     runOptions.IsDebugging = false;
                     lstRunOptions.Add(runOptions);
                  }

                  OnPartCoverProjectCommandHandler(lstRunOptions);

                  Handled = true;
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_COMMAND_ERROR_CLEAR_NAME)
               {
                  OnProjectErrorListForceClear();
                  Handled = true;
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_SYNC_REF_COMMAND_NAME)
               {
                  OnProjectResyncReferences();
                  Handled = true;
               }
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            ErrorHandler.HandleException(ex);
         }
      }

      public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
      {
         try
         {
            Logger.LogMessage(CmdName);

            if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
               if (CmdName == "UTG.Connect.TestAddIn")
               {
                  StatusOption = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                  return;
               }
            }

            #region Notes:
            //VS uses this to determine what our addin should do as environment changes..open new files, solutions, docs and so on...

            //Set when button must be available...

            //Check for our command being quired...

            //Make is available when the code window is showing certain files
            #endregion

            //This section requires refactoring
            if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
               if(CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_SOLUTION_COMMAND_NAME)
               {
                  StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_TOKEN_COMMAND_NAME)
               {
                  if (_applicationObject != null && _applicationObject.ActiveWindow != null && _applicationObject.ActiveWindow.Document != null)
                  {
                     string currentFile = _applicationObject.ActiveWindow.Document.FullName.ToLower();

                     //Change this so that each supported language returns its extension so that we can add all extensions to a list where we simply would search if extension exists
                     if (currentFile.EndsWith(".cs") || currentFile.EndsWith(".vb"))
                     {
                        TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

                        //current scope
                        FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

                        //Noticed that null happens on metadata files...
                        if (fcm == null)
                        {
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                           return;
                        }

                        CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

                        if (ce != null && !sel.Text.Equals(string.Empty) && IsSupported(ce))
                           StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                        else
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                     else
                     {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                  }
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_TEST_METHOD_COMMAND_NAME)
               {
                  if (_applicationObject != null && _applicationObject.ActiveWindow != null && _applicationObject.ActiveWindow.Document != null)
                  {
                     string currentFile = _applicationObject.ActiveWindow.Document.FullName.ToLower();

                     //Change this so that each supported language returns its extension so that we can add all extensions to a list where we simply would search if extension exists
                     if (currentFile.EndsWith(".cs") || currentFile.EndsWith(".vb"))
                     {
                        TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

                        //current scope
                        FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

                        //Noticed that null happens on metadata files...
                        if (fcm == null)
                        {
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                           return;
                        }

                        CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

                        if (ce != null && !UTGManagerAndExaminor.ProjectExaminar.IsUnitTestProject(ce.ProjectItem.ContainingProject, this.testFramework.FrameworkTestAttributes))
                        {
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                           return;
                        }

                        if (ce != null && !sel.Text.Equals(string.Empty) && ce is CodeFunction)
                           StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                        else
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                     else
                     {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                  }
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_TEST_CLASS_COMMAND_NAME ||
                        CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_PARTCOVER_COMMAND_NAME)
               {
                  if (_applicationObject != null && _applicationObject.ActiveWindow != null && _applicationObject.ActiveWindow.Document != null)
                  {
                     string currentFile = _applicationObject.ActiveWindow.Document.FullName.ToLower();

                     //Change this so that each supported language returns its extension so that we can add all extensions to a list where we simply would search if extension exists
                     if (currentFile.EndsWith(".cs") || currentFile.EndsWith(".vb"))
                     {
                        TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

                        //current scope
                        FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

                        //Noticed that null happens on metadata files...
                        if (fcm == null)
                        {
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                           return;
                        }

                        CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

                        if (ce != null && !UTGManagerAndExaminor.ProjectExaminar.IsUnitTestProject(ce.ProjectItem.ContainingProject, this.testFramework.FrameworkTestAttributes))
                        {
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                           return;
                        }

                        if (ce != null && !sel.Text.Equals(string.Empty) && ce is CodeClass)
                           StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                        else
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                     else
                     {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                  }
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_ATTACH_COMMAND_NAME)
               {
                  if (_applicationObject != null && _applicationObject.ActiveWindow != null && _applicationObject.ActiveWindow.Document != null)
                  {
                     string currentFile = _applicationObject.ActiveWindow.Document.FullName.ToLower();

                     //Change this so that each supported language returns its extension so that we can add all extensions to a list where we simply would search if extension exists
                     if (currentFile.EndsWith(".cs") || currentFile.EndsWith(".vb"))
                     {
                        //current scope
                        FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

                        //Noticed that null happens on metadata files...
                        if (fcm == null)
                        {
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                           return;
                        }

                        TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

                        CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

                        if (ce != null && !UTGManagerAndExaminor.ProjectExaminar.IsUnitTestProject(ce.ProjectItem.ContainingProject, this.testFramework.FrameworkTestAttributes))
                        {
                           StatusOption = vsCommandStatus.vsCommandStatusSupported;
                           return;
                        }

                       
                        StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                        
                     }
                     else
                     {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                  }
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_NAME)
               {
                  if (_applicationObject != null && _applicationObject.ActiveWindow != null && _applicationObject.ActiveWindow.Document != null)
                  {
                     //Here we are doing this to try and disable this command if the file open is not a codefilemodel file
                     //like metedata files...
                     
                     FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

                     if (fcm == null)
                     {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                        return;
                     }

                     string currentFile = _applicationObject.ActiveWindow.Document.FullName.ToLower();

                     //Change this so that each supported language returns its extension so that we can add all extensions to a list where we simply would search if extension exists
                     if (currentFile.EndsWith(".cs") || currentFile.EndsWith(".vb"))
                     {
                        StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                     }
                     else
                     {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                     }
                  }
               }
               else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_COMMAND_NAME ||
                     CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_PARTCOVER_COMMAND_NAME)
                  {
                     UIHierarchy hierarchy = _applicationObject.ToolWindows.SolutionExplorer;

                     if (hierarchy != null && hierarchy.SelectedItems != null)
                     {
                        Object[] hierarchySelectedItems = (Object[])hierarchy.SelectedItems;

                        foreach (UIHierarchyItem hirarchySelectedItem in hierarchySelectedItems)
                        {
                           if (hirarchySelectedItem.Object is Project)
                           {
                              if (CanRunUnitTestProjectCommand((Project)hirarchySelectedItem.Object))
                              {
                                 StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                              }
                              else
                              {
                                 StatusOption = vsCommandStatus.vsCommandStatusSupported;
                              }
                           }
                        }
                     }
                  }
                  else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_COMMAND_ERROR_CLEAR_NAME)
                  {

                     UIHierarchy hierarchy = _applicationObject.ToolWindows.SolutionExplorer;

                     if (hierarchy != null && hierarchy.SelectedItems != null)
                     {
                        Object[] hierarchySelectedItems = (Object[])hierarchy.SelectedItems;

                        foreach (UIHierarchyItem hirarchySelectedItem in hierarchySelectedItems)
                        {
                           if (hirarchySelectedItem.Object is Project)
                           {
                              ErrorListProvider tvErrorListProvider = GetErrorListProvider();

                              if (tvErrorListProvider.Tasks.Count > 0)
                              {
                                 StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                              }
                              else
                              {
                                 StatusOption = vsCommandStatus.vsCommandStatusSupported;
                              }
                           }
                        }
                     }
                  }
                  else if (CmdName == _addInInstance.ProgID + "." + UTGHelper.CommonStrings.PROJECT_SYNC_REF_COMMAND_NAME)
                  {
                     UIHierarchy hierarchy = _applicationObject.ToolWindows.SolutionExplorer;

                     if (hierarchy != null && hierarchy.SelectedItems != null)
                     {
                        Object[] hierarchySelectedItems = (Object[])hierarchy.SelectedItems;

                        foreach (UIHierarchyItem hirarchySelectedItem in hierarchySelectedItems)
                        {
                           if (hirarchySelectedItem.Object is Project)
                           {
                              if (IsProjectReferencesSynced())
                              {
                                 StatusOption = vsCommandStatus.vsCommandStatusSupported;
                              }
                              else
                              {
                                 if (CanRunUnitTestProjectCommand((Project)hirarchySelectedItem.Object))
                                 {
                                    StatusOption = (vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                                 }
                                 else
                                 {
                                    StatusOption = vsCommandStatus.vsCommandStatusSupported;
                                 }
                              }
                           }
                        }
                     }
                  }
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            ErrorHandler.HandleException(ex);
         }
      }

      #endregion


      private void HandleException(Exception ex)
      {
         //for now we route it to HandleError which shows msg box
         HandleError(ex.Message);
      }


      private void HandleError(string errorMessage)
      {
         System.Windows.Forms.MessageBox.Show(errorMessage);
      }


      private bool IsSupported(CodeElement ce)
      {
         if ( 
            (ce is CodeClass && ((CodeClass)ce).Access == vsCMAccess.vsCMAccessPublic) 
            || 
            ( ce is CodeFunction  && ((CodeFunction)ce).Access == vsCMAccess.vsCMAccessPublic)
            || 
            (ce is CodeProperty && ((CodeProperty)ce).Access == vsCMAccess.vsCMAccessPublic)
            )
            return true;

         return false;
      }


      /// <summary>
      /// Returns a list of all selected projects
      /// </summary>
      /// <returns></returns>
      private System.Collections.Generic.List<Project> GetSelectedProjectInProjectHierarchy()
      {
         System.Collections.Generic.List<Project> lstSelectedProjects = new System.Collections.Generic.List<Project>();

         #region older code that gets UIHierarchy and iterates through its items to find project items
         
         UIHierarchy hierarchy = _applicationObject.ToolWindows.SolutionExplorer;

         if (hierarchy != null && hierarchy.SelectedItems != null)
         {
            Object[] hierarchySelectedItems = (Object[])hierarchy.SelectedItems;

            foreach (UIHierarchyItem hirarchySelectedItem in hierarchySelectedItems)
            {
               if (hirarchySelectedItem.Object is Project)
               {
                  lstSelectedProjects.Add((Project)hirarchySelectedItem.Object);
               }
            }
         }
         #endregion

         return lstSelectedProjects;

      }


      /// <summary>
      /// Returns a list of all selected projects
      /// </summary>
      /// <returns></returns>
      private System.Collections.Generic.List<Project> GetProjectInProjectHierarchy()
      {
         System.Collections.Generic.List<Project> lstSelectedProjects = new System.Collections.Generic.List<Project>();

         #region older code that gets UIHierarchy and iterates through its items to find project items
         UIHierarchy hierarchy = _applicationObject.ToolWindows.SolutionExplorer;

         if (hierarchy != null && hierarchy.SelectedItems != null)
         {
            UIHierarchyItems hierarchySelectedItems = hierarchy.UIHierarchyItems;

            foreach (UIHierarchyItem hirarchySelectedItem in hierarchySelectedItems)
            {
               if (hirarchySelectedItem.Object is Solution)
               {
                  EnvDTE.Projects projects = ((Solution)(hirarchySelectedItem.Object)).Projects;
                  foreach (EnvDTE.Project project in projects)
                  {
                     lstSelectedProjects.Add(project);
                  }
               }
               if (hirarchySelectedItem.Object is Project)
               {
                  lstSelectedProjects.Add((Project)hirarchySelectedItem.Object);
               }
            }
         }
         #endregion

         return lstSelectedProjects;

      }


      private CommandBarButton GetProjectBuildCommandBar()
      {
         CommandBars commandBars = (CommandBars)_applicationObject.CommandBars;

         if (commandBars != null)
         {
            CommandBar projectCommandBar = commandBars["Project"];

            if (projectCommandBar != null)
            {
               foreach (CommandBarControl commmandBarControle in projectCommandBar.Controls)
               {
                  if (commmandBarControle is CommandBarButton)
                  {
                     //FIX ME. What if caption is not English!
                     //746
                     if (((CommandBarButton)commmandBarControle).Id == REBUILD_COMMAND_BAR_VS_ID /*||
                        ((CommandBarButton)commmandBarControle).Caption.Contains("R&ebuild")*/
                                                                                              )
                     {
                        return (CommandBarButton)commmandBarControle;
                     }
                  }
               }
            }
         }

         return null;
      }


      private UnitTestCodeType GetSelectionUnitTestCodeType()
      {
         string currentFile = _applicationObject.ActiveWindow.Document.FullName.ToLower();
         if (currentFile.EndsWith(".cs"))
            return UnitTestCodeType.CSharp;
         else if (currentFile.EndsWith(".vb"))
            return UnitTestCodeType.VB;
         else
            throw new Exception(UTGHelper.CommonErrors.ERR_LANUAGE_TYPE_NOT_SUPPORTED);
      }


      #region Command Handlers

      private void OnProjectCodeWindowCommandHandler()
      {
         try
         { 
            OnGenerationStart();

            CodeSelectionHandler.OnCodeElementSelection(this.testFramework, _applicationObject.ActiveDocument.ProjectItem.ContainingProject, _applicationObject, GetSelectionUnitTestCodeType());
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            OnGenerationFaliure();

            UTGHelper.ErrorHandler.HandleException(ex);
         }
      }


      private void OnAttachToProcessFile()
      {
         TestFixturOption runOption = new TestFixturOption();

         List<TestFixturOption> lstRunOptions = new List<TestFixturOption>();

         try
         {
            string currentFile = _applicationObject.ActiveWindow.Document.FullName.ToLower();
           
            if (currentFile.EndsWith(".cs") || currentFile.EndsWith(".vb"))
            {
               runOption.IsDebugging = true;
               runOption.Project = _applicationObject.ActiveDocument.ProjectItem.ContainingProject;
               lstRunOptions.Add(runOption);  
            }

           
            System.Threading.Thread t = new System.Threading.Thread(
               new System.Threading.ParameterizedThreadStart(OnProjectCommandHandler));

            t.Start((object)lstRunOptions);

         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            UTGHelper.ErrorHandler.HandleException(ex);
         }

      }


      private void OnTokenCodeWindowCommandHandler()
      {
         OnGenerationStart();

         try
         {
            TextSelection textSelection = null;

            textSelection = (TextSelection)_applicationObject.ActiveWindow.Document.Selection;

            if (!textSelection.Text.Equals(string.Empty))
            {
               //Get the selection first
               TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

               //current scope
               FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

               CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

               if (ce == null)
               {
                  UTGHelper.ErrorHandler.ShowMessage(UTGHelper.CommonErrors.ERR_ILLEGAL_TEXT_SELECTION);
               }

               if (IsSupported(ce))
               {
                  CodeSelectionHandler.OnCodeElementSelection(this.testFramework, ce, _applicationObject, GetSelectionUnitTestCodeType());
               }
            }
            else
            {
               System.Windows.Forms.MessageBox.Show(UTGHelper.CommonStrings.CODEWINDOW_TOKEN_SELECTION_NONESELECTED);
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            OnGenerationFaliure();

            UTGHelper.ErrorHandler.HandleException(ex);
         }
      }


      private void OnTestMethodCommandHandler()
      {
         List<TestFixturOption> lstRunOptions = new List<TestFixturOption>();

         FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

         //Noticed that null happens on metadata files...
         if (fcm == null)
         {
            UTGHelper.UserAlertBox userAlert = new UserAlertBox("No selection is made!", UserAlertBox.UserAlertType.regularMessage);
            userAlert.Show();
            return;
         }

         TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

         
         CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

         if (ce is CodeFunction)
         {
            TestFixturOption runOption = new TestFixturOption();
            runOption.IsDebugging = false;
            runOption.Project = ce.ProjectItem.ContainingProject;
            CodeFunction cf = ce as CodeFunction;
            runOption.CFunction = cf;
            lstRunOptions.Add(runOption);
         }
         else
         {
            UTGHelper.UserAlertBox userAlert = new UserAlertBox("Selection is not a Method or Function!", UserAlertBox.UserAlertType.regularMessage);
            userAlert.Show();
            return;
         }

         OnRunTestCommandHandler(lstRunOptions);
      }

      
      private void OnTestClassCommandHandler()
      {
         List<TestFixturOption> lstRunOptions = new List<TestFixturOption>();
         
         FileCodeModel2 fcm = (FileCodeModel2)_applicationObject.ActiveDocument.ProjectItem.FileCodeModel;

         //Noticed that null happens on metadata files...
         if (fcm == null)
         {
            UTGHelper.UserAlertBox userAlert = new UserAlertBox("No selection is made!", UserAlertBox.UserAlertType.regularMessage);
            userAlert.Show();
            return;
         }

         TextSelection sel = (TextSelection)_applicationObject.ActiveDocument.Selection;

         
         CodeElement ce = CodeExaminor.FindInnerMostCodeElement(fcm.CodeElements, sel.TopPoint);

         if (ce is CodeClass)
         {
            TestFixturOption runOption = new TestFixturOption();
            runOption.IsDebugging = false;
            runOption.Project = ce.ProjectItem.ContainingProject;
            runOption.CClass = ce as CodeClass;
            lstRunOptions.Add(runOption);
         }
         else
         {
            UTGHelper.UserAlertBox userAlert = new UserAlertBox("Selection is not a Method or Function!", UserAlertBox.UserAlertType.regularMessage);
            userAlert.Show();
            return;
         }

         OnRunTestCommandHandler(lstRunOptions);
      }


      private void OnSolutionCommandHandler()
      {
         List<TestFixturOption> lstRunOptions = new List<TestFixturOption>();
         List<Project> lstProjects = GetProjectInProjectHierarchy();

         foreach (Project proj in lstProjects)
         {
            if (ProjectExaminar.IsUnitTestProject(proj, this.testFramework.FrameworkTestAttributes))
            {
               TestFixturOption runOption = new TestFixturOption();
               runOption.IsDebugging = false;
               runOption.Project = proj;
               lstRunOptions.Add(runOption);
            }
         }


         OnRunTestCommandHandler(lstRunOptions);
      }


      private void OnProjectCommandHandler(object obj)
      {
         try
         {
            List<TestFixturOption> runOption = (List<TestFixturOption>)obj;

            OnRunTestCommandHandler(runOption);
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
         }
      }


      private void OnRunTestCommandHandler(List<TestFixturOption> lstRunOptions)
      {
         try
         {
            #region keeping for reference older code that gets UIHierarchy and iterates through its items to find project items
            //UIHierarchy hierarchy = _applicationObject.ToolWindows.SolutionExplorer;

            //if (hierarchy != null && hierarchy.SelectedItems != null)
            //{
            //   Object[] hierarchySelectedItems = (Object[])hierarchy.SelectedItems;

            //   foreach (UIHierarchyItem hirarchySelectedItem in hierarchySelectedItems)
            //   {
            //      if (hirarchySelectedItem.Object is Project)
            //      {
            //      }
            //   }
            //}
            #endregion

            string projectPath = string.Empty;
            string unitTestProjectConfigurationFileName = string.Empty;
            string unitTestProjectOutputDirectory = string.Empty;
            string unitTestAssemblyName = string.Empty;
            string unitTestProjectConfigurationFileContent = string.Empty;

            ivRan = 0;
            ivSuccess = 0;
            ivFailed = 0;
            ivNorun = 0;

            ErrorListProvider tvErrorListProvider = GetErrorListProvider();

            tvErrorListProvider.Tasks.Clear();

            foreach (TestFixturOption runOptions in lstRunOptions)
            {
               Project selectedProject = runOptions.Project;

               if (!runOptions.IsDebugging)
               {
                  _applicationObject.Solution.SolutionBuild.BuildProject(
                     _applicationObject.Solution.SolutionBuild.ActiveConfiguration.Name,
                     selectedProject.UniqueName,
                     true);
               }

               unitTestProjectConfigurationFileName = selectedProject.Name + testFramework.TestProjectConfiurationPostFix;
               //UTGHelper.CommonStrings.UNIT_TEST_PROJECT_CONFIGURATION_POSTFIX;

               unitTestProjectOutputDirectory =
                  ProjectExaminar.GetProjectOutputDirectory(selectedProject);

               if (unitTestProjectOutputDirectory.Equals(string.Empty))
                  throw new Exception(UTGHelper.CommonErrors.ERR_UNABLE_TO_LOCATE_UNITTEST_PROJECT_OUTPUT_DIRECTORY);

               unitTestAssemblyName =
                   ProjectExaminar.GetProjectOutputAssemblyName(selectedProject);

               unitTestProjectConfigurationFileContent =
                  UTGHelper.IO.GetUnitTestProjectConfigurationFileContent();

               //Get the configuration file
               unitTestProjectConfigurationFileContent =
                  UTGeneratorLibrary.UnitTestProjectConfiguration.XMLTemplateFormatter.FormateTemplate(
                  unitTestProjectConfigurationFileContent, unitTestAssemblyName);

               try
               {
                  //Here we specify actual content since if one exits the our InputOutput.writeFileIfNotExists will return
                  //its content. In any case, the content returned we may not be so interested in since we will call
                  //nunit.ext on the configuration file it self

                  if (unitTestProjectOutputDirectory.Equals("bin\\Debug\\"))
                  {
                     if (UTGManagerAndExaminor.ProjectExaminar.GetProjectPath(selectedProject) != string.Empty)
                        unitTestProjectOutputDirectory = UTGManagerAndExaminor.ProjectExaminar.GetProjectPath(selectedProject) + "\\bin\\Debug\\";
                  }

                  UTGHelper.IO.writeFileIfNotExistsAppending(
                     unitTestProjectOutputDirectory + "\\" + unitTestProjectConfigurationFileName,
                     unitTestProjectConfigurationFileContent);

               }
               catch (Exception ex)
               {
                  Logger.LogException(ex);
               }

               try
               {
                  RunUnitTests(runOptions);
               }
               catch (Exception ex)
               {
                  Logger.LogException(ex);

                  UTGHelper.ErrorHandler.HandleException(ex);
               }
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            UTGHelper.ErrorHandler.HandleException(ex);
         }
      }


      private void OnPartCoverProjectCommandHandler(List<TestFixturOption> lstRunOptions)
      {
         try {

            foreach (TestFixturOption runOptoin in lstRunOptions)
            {
               if (!runOptoin.IsDebugging)
               {
                  _applicationObject.Solution.SolutionBuild.BuildProject(
                     _applicationObject.Solution.SolutionBuild.ActiveConfiguration.Name,
                     runOptoin.Project.UniqueName,
                     true);
               }

               ExamineCodeCoverage(runOptoin);
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            UTGHelper.ErrorHandler.HandleException(ex);
         }
      }


      private void OnPartCoverCodeWindowCommandHandler(List<TestFixturOption> lstRunOptions)
      {
         OnPartCoverProjectCommandHandler(lstRunOptions);
      }

      #endregion

      private void AddNMateErrorListToTasksFrame(Window window)
      {
         Window2 Frame;
         Windows2 wins;

         wins = (EnvDTE80.Windows2)_applicationObject.Windows;
         Window2 w4 = (EnvDTE80.Window2)_applicationObject.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);

         Frame = (EnvDTE80.Window2)wins.CreateLinkedWindowFrame(window, w4, vsLinkedWindowType.vsLinkedWindowTypeDocked);

         Frame.Height = 200;
         Frame.IsFloating = false;
         Frame.Linkable = true;
      }


      private void OutputUnitTestErrorsToVisualStudioErrorList(List<UnitTestError> lstUnitTestErrors, Project project)
      {
         Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_applicationObject;

         ServiceProvider serviceProvider = new ServiceProvider(sp);

         IVsHierarchy hierarchy = ProjectExaminar.GetHierarchy(serviceProvider, project);

         foreach (UnitTestError unitTestError in lstUnitTestErrors)
         {
            ErrorTask errTask = new ErrorTask();
            errTask.Line = unitTestError.Line;
            errTask.Document = unitTestError.File;
            errTask.Text = UTGHelper.CommonStrings.UNIT_TEST_OUTPUT_PANE_TEXT_PREFIX + unitTestError.Text;
            errTask.HierarchyItem = hierarchy;

            errTask.Navigate += new EventHandler(NavigateToError);
            errTask.ImageIndex = 44;
            errTask.Category = TaskCategory.User;

            //GetErrorListProvider().Tasks.Add(errTask);
            tvErrorListProvider.Tasks.Add(errTask);
         }
      }


      private Window CreateNMateErrorListIfNotExits()
      {
         try
         {
            EnvDTE80.Windows2 wins2obj;
            Window newWinobj = null;
            AddIn addinobj;

            foreach (AddIn addIn in _applicationObject.AddIns)
            {
               if (!addIn.ProgID.Equals("UTG.Connect"))
                  continue;

               addinobj = addIn;//_applicationObject.AddIns.Item("NMateForNUnit");
               wins2obj = (Windows2)_applicationObject.Windows;
               object ctlobj = new object();

               newWinobj = wins2obj.CreateToolWindow2(
                  addinobj,
                  UTGManagerAndExaminor.ProjectExaminar.GetFilePath(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\UTGHelper.dll",
                  "UTGHelper.ErrorsListUserControle",
                  UTGHelper.CommonStrings.APPLICATION_NAME + " Test Results",
                  "{E8D2FE25-11F0-4126-8385-ADB38C3E09BF}",
                  ref ctlobj);
               newWinobj.Visible = true;


               return newWinobj;
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            throw new Exception("Failed to create NMate's Error List! Errors are being directed to Visual Studio's Error List.");
         }

         return null;
      }


      private void ExamineCodeCoverage(TestFixturOption runOption)
      {
         try {

            if (!this.coverageFramework.Examine(runOption, _applicationObject))
            {
               UTGHelper.ErrorHandler.ShowMessage("Failed to examine code!");
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            UTGHelper.ErrorHandler.HandleException(ex);
         }
      }


      private void RunUnitTests(TestFixturOption testRunOption)
      {
         try {

            if (!this.testFramework.run(testRunOption, _applicationObject))
            {
               UTGHelper.ErrorHandler.ShowMessage("Failed to test code!");
            }
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            UTGHelper.ErrorHandler.HandleException(ex);
         }
      }


      /* Older code
      private UIHierarchyItem GetUnitTestProjectHierarchyItem(Project project)
      {
         UIHierarchy hierarchy = _applicationObject.ToolWindows.SolutionExplorer;

         if (hierarchy != null && hierarchy.SelectedItems != null)
         {
            Object[] hierarchySelectedItems = (Object[])hierarchy.SelectedItems;

            foreach (UIHierarchyItem hirarchySelectedItem in hierarchySelectedItems)
            {
               if (hirarchySelectedItem.Object is Project)
               {
                  if (hirarchySelectedItem.Object == project)
                  {
                     return hirarchySelectedItem;
                  }
               }
            }
         }

         return null;
      }
      */
   
      void NavigateToError(object sender, EventArgs e)
      {
         ErrorTask error = sender as ErrorTask;

         if (error != null && _applicationObject != null)
         {
            _applicationObject.ItemOperations.OpenFile(error.Document, EnvDTE.Constants.vsViewKindAny);

            TextSelection selection = _applicationObject.ActiveDocument.Selection as TextSelection;

            selection.GotoLine(error.Line, true);
         }
      }
      

      private void OnProjectErrorListForceClear()
      {
         ErrorListProvider erroListProvider =
            GetErrorListProvider();

         ClearErroList(erroListProvider);
      }


      private bool IsProjectReferencesSynced()
      {
         System.Collections.Generic.List<Project> lstSelectedProjects =
              GetSelectedProjectInProjectHierarchy();

         foreach (Project unitTestProject in lstSelectedProjects)
         {
            Project projectBeingTested = ProjectExaminar.GetUnitTestProjectsOriginalProject(unitTestProject.Name, _applicationObject, testFramework.TestProjectPostFix);

            if (projectBeingTested == null)
               continue;

            string tvProjectBeingTestedOuputDll = ProjectExaminar.GetProjectOutputAssemblyName(projectBeingTested);

            tvProjectBeingTestedOuputDll = tvProjectBeingTestedOuputDll.Replace(".dll", "");

            if (projectBeingTested == null)
               continue;

            VSLangProj.References pojectBeingTestingReferences
               = ProjectManager.GetProjectReferences(projectBeingTested);

            VSLangProj.References pojectUNitTestsReferences
              = ProjectManager.GetProjectReferences(unitTestProject);

            foreach (VSLangProj.Reference projRefUnitTest in pojectUNitTestsReferences)
            {
               bool found = false;

               foreach (VSLangProj.Reference projTested in pojectBeingTestingReferences)
               {
                  if (projRefUnitTest.Name.Equals(tvProjectBeingTestedOuputDll))
                  {
                     found = true;
                     break;
                  }

                  if (projRefUnitTest.Name.Equals("nunit.framework"))
                  {
                     found = true;
                     break;
                  }

                  if (projRefUnitTest.Name.Equals(projTested.Name))
                  {
                     found = true;
                     break;
                  }
               }

               if (!found)
               {
                  return false;
               }
            }


            foreach (VSLangProj.Reference projBeingTestedRef in pojectBeingTestingReferences)
            {
               bool matchFound = false;

               foreach (VSLangProj.Reference projUNitTestRef in pojectUNitTestsReferences)
               {
                  if (projBeingTestedRef.Name.Equals(projUNitTestRef.Name))
                  {
                     matchFound = true;
                     continue;
                  }
               }

               if (!matchFound)
               {
                  return false;
               }
            }
         }

         return true;
      }


      private void OnProjectResyncReferences()
      {
         OnSynchingStarted();

         System.Collections.Generic.List<Project> lstSelectedProjects =
              GetSelectedProjectInProjectHierarchy();

         foreach (Project unitTestProject in lstSelectedProjects)
         {
            Project projectBeingTested = ProjectExaminar.GetUnitTestProjectsOriginalProject(unitTestProject.Name, _applicationObject, testFramework.TestProjectPostFix);

            if (projectBeingTested == null)
               continue;

            string tvProjectBeingTestedOuputDll = ProjectExaminar.GetProjectOutputAssemblyName(projectBeingTested);

            tvProjectBeingTestedOuputDll = tvProjectBeingTestedOuputDll.Replace(".dll", "");

            if (projectBeingTested == null)
               continue;

            VSLangProj.References pojectBeingTestingReferences
               = ProjectManager.GetProjectReferences(projectBeingTested);

            VSLangProj.References pojectUNitTestsReferences
              = ProjectManager.GetProjectReferences(unitTestProject);

            List<string> toRemoveFromUnitTestProject = new List<string>();

            foreach (VSLangProj.Reference projRefUnitTest in pojectUNitTestsReferences)
            {
               bool found = false;

               foreach (VSLangProj.Reference projTested in pojectBeingTestingReferences)
               {
                  if (projRefUnitTest.Name.Equals(tvProjectBeingTestedOuputDll))
                  {
                     found = true;
                     break;
                  }

                  if (projRefUnitTest.Name.Equals("nunit.framework"))
                  {
                     found = true;
                     break;
                  }

                  if (projRefUnitTest.Name.Equals(projTested.Name))
                  {
                     found = true;
                     break;
                  }
               }

               if (!found)
               {
                  //indexs.Add(index);
                  toRemoveFromUnitTestProject.Add(projRefUnitTest.Name);
               }
            }

            foreach (string toRemove in toRemoveFromUnitTestProject)
            {
               int index =
                  ProjectManager.GetReferenceIndexFromProject(toRemove, unitTestProject);

               if (index >= 0)
                  ProjectManager.RemoveReferenceFromProject(index, unitTestProject);
            }


            //Now backwards

            foreach (VSLangProj.Reference projBeingTestedRef in pojectBeingTestingReferences)
            {
               bool matchFound = false;

               foreach (VSLangProj.Reference projUNitTestRef in pojectUNitTestsReferences)
               {
                  if (projBeingTestedRef.Name.Equals(projUNitTestRef.Name))
                  {
                     matchFound = true;
                     continue;
                  }
               }

               if (!matchFound)
               {
                  ProjectManager.AddDllReferenceToProject(unitTestProject, projBeingTestedRef.Path);
               }
            }
         }

         OnSynchingFinished();
      }


      private bool HasBuildErrors(Project project)
      {
         IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
         ErrorList myErrors;
         int count;
         IVsHierarchy hierarchy;

         if (solution == null)
         {
             //commenting this out because when the user first opens up a solution and right clicks on a project the solution is always
             //returned as null. This however works the second time around!! So until a solution is found I will just return false
             //throw new Exception("HasBuildErrors(): While attempting to to retrieve solution, GetGlobalService returned null solution");
             return false;
         }

         solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

         myErrors = _applicationObject.ToolWindows.ErrorList;

         count = myErrors.ErrorItems.Count;

         if (count != 0)
         {
            for (int i = 1; i <= count; i++)
            {
               try
               {
                  if (myErrors.ErrorItems.Item(i).Project.IndexOf(project.Name + ".csproj") >= 0)
                  {
                     if (myErrors.ErrorItems.Item(i).Description.IndexOf(UTGHelper.CommonStrings.UNIT_TEST_OUTPUT_PANE_TEXT_PREFIX) < 0)
                        return true;
                  }
               }
               //at one point there was an exception being thrown here. If you generate test for entire project 
               //then right click on new project an exception was thrown. I am unable to find cause for this exception as of yet
               //so I added a goble catch to stop annoying the user. The exception did not consistently
               catch (Exception ex)
               {//gobble...
                  Logger.LogException(ex);
               }
            }
         }

         return false;
      }


      private bool CanRunUnitTestProjectCommand(Project project)
      {
         if (ProjectExaminar.IsUnitTestProject(project, this.testFramework.FrameworkTestAttributes))
         {
            Project parentProject = ProjectExaminar.GetUnitTestProjectsOriginalProject(project.Name, (DTE2)project.DTE, testFramework.TestProjectPostFix);

            if (parentProject != null)
            {
               if (HasBuildErrors(parentProject) || parentProject.IsDirty) {
                  return false;
               }
            }

            if (ProjectExaminar.IsUnitTestProject(project, this.testFramework.FrameworkTestAttributes) && !HasBuildErrors(project) && !project.IsDirty)
            {
               return true;
            }
         }

         return false;
      }


      private void ClearErroList(ErrorListProvider errorProvider)
      {
         if (errorProvider == null)
            return;

         try
         {
            List<ErrorTask> erroTasks = new List<ErrorTask>();

            foreach (ErrorTask task in errorProvider.Tasks)
            {
               if (task.Text.IndexOf(UTGHelper.CommonStrings.APPLICATION_NAME) >= 0)
               {
                  erroTasks.Add(task);
               }
            }

            foreach (ErrorTask erroTask in erroTasks)
            {
               errorProvider.Tasks.Remove(erroTask);
            }
         }
         catch (Exception ex) {

            Logger.LogException(ex);
         }
      }


      private ErrorListProvider GetErrorListProvider()
      {
         if (_applicationObject == null)
            throw new Exception("To early to call this method..");

         if (tvErrorListProvider != null)
            return tvErrorListProvider;

         Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
            _applicationObject;

         ServiceProvider serviceProvider = new ServiceProvider(sp);

         tvErrorListProvider = new ErrorListProvider(serviceProvider);

         return tvErrorListProvider;
      }


      #region Create Commands

      private CommandBarControl CreateTokenCodeCommand()
      {
          return CommandBarControlFactory.CreateCommandBarControl(
            _applicationObject, 
            _addInInstance,
            291,
            CommandControlType.CommandControlTypeCodeWindow,
            UTGHelper.CommonStrings.CODEWINDOW_TOKEN_COMMAND_NAME,
            UTGHelper.CommonStrings.CODEWINDOW_TOKEN_BUTTON_CAPTION,
            UTGHelper.CommonStrings.CODEWINDOW_TOKEN_COMMAND_TOOLTIP, 
            true);
      }

      private CommandBarControl CreateAttachToProcessCodeCommand()
      {
         return CommandBarControlFactory.CreateCommandBarControl(
            _applicationObject,
            _addInInstance,
            192,
            CommandControlType.CommandControlTypeCodeWindow,
            UTGHelper.CommonStrings.CODEWINDOW_ATTACH_COMMAND_NAME,
            UTGHelper.CommonStrings.CODEWINDOW_ATTACH_BUTTON_CAPTION,
            UTGHelper.CommonStrings.CODEWINDOW_ATTACH_COMMAND_TOOLTIP,
            false);
      }

      private CommandBarControl CreateUnitTestMethodCodeCommand()
      {
         return CommandBarControlFactory.CreateCommandBarControl(
            _applicationObject,
            _addInInstance,
            99,
            CommandControlType.CommandControlTypeCodeWindow,
            UTGHelper.CommonStrings.CODEWINDOW_TEST_METHOD_COMMAND_NAME,
            UTGHelper.CommonStrings.CODEWINDOW_TEST_METHOD_BUTTON_CAPTION,
            UTGHelper.CommonStrings.CODEWINDOW_TEST_METHOD_COMMAND_TOOLTIP,
            false);
      }

      private CommandBarControl CreateUnitTestClassCodeCommand()
      {
         return CommandBarControlFactory.CreateCommandBarControl(
            _applicationObject,
            _addInInstance,
            99,
            CommandControlType.CommandControlTypeCodeWindow,
            UTGHelper.CommonStrings.CODEWINDOW_TEST_CLASS_COMMAND_NAME,
            UTGHelper.CommonStrings.CODEWINDOW_TEST_CLASS_BUTTON_CAPTION,
            UTGHelper.CommonStrings.CODEWINDOW_TEST_CLASS_COMMAND_TOOLTIP,
            false);
      }

      private CommandBarControl CreateProjectCodeCommand()
      {
         return CommandBarControlFactory.CreateCommandBarControl(
           _applicationObject,
           _addInInstance,
           173,
           CommandControlType.CommandControlTypeCodeWindow,
           UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_NAME,
           UTGHelper.CommonStrings.CODEWINDOW_PROJECT_BUTTON_CAPTION,
           UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_TOOLTIP,
           false);

         #region older code
         /*
         Command codeWindowCommande = null;

         //to tie it to this button controle...
         CommandBarControl codeWindowButton = null;

         //Right click pop up in the code window
         CommandBar codeCommandBar = null;

         //collection of command bars in visual studio will be stored in this collection
         CommandBars commandBars = null;

         try
         {
            try
            {
               codeWindowCommande = _applicationObject.Commands.Item(_addInInstance.ProgID + "." + UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_NAME, 0);
            }
            catch (Exception ex) {

               ErrorHandler.LogException(ex);
            }

            if (codeWindowCommande == null)
            {
               object[] obj = new object[] { null };

               if (_applicationObject != null && _applicationObject.Commands != null)
               {
                  if (_addInInstance != null)
                  {
                     codeWindowCommande = _applicationObject.Commands.AddNamedCommand(
                        _addInInstance,
                        UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_NAME,
                        UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_NAME,
                        UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_TOOLTIP,
                        true,
                        173,
                        ref obj,
                        (int)(vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported));


                     commandBars = (CommandBars)_applicationObject.CommandBars;

                     if (commandBars != null)
                     {
                        codeCommandBar = commandBars["Code Window"];

                        if (codeCommandBar != null)
                        {
                           codeWindowButton = (CommandBarControl)codeWindowCommande.AddControl(codeCommandBar, codeCommandBar.Controls.Count + 1);

                           if (codeWindowButton != null)
                           {
                              codeWindowButton.Caption = UTGHelper.CommonStrings.CODEWINDOW_PROJECT_BUTTON_CAPTION;

                              codeWindowButton.TooltipText = UTGHelper.CommonStrings.CODEWINDOW_PROJECT_COMMAND_TOOLTIP;

                              return codeWindowButton;
                           }
                        }
                     }
                  }
               }
            }
         }
         catch (Exception ex)
         {
            ErrorHandler.LogException(ex);

            HandleException(ex);
         }

         return null;
          * */
         #endregion
      }

      private CommandBarControl CreatePartCoverCodeWindowCommand()
      { 
         return CommandBarControlFactory.CreateCommandBarControl(
             _applicationObject,
             _addInInstance,
             82,
             CommandControlType.CommandControlTypeCodeWindow,
             UTGHelper.CommonStrings.CODEWINDOW_PARTCOVER_COMMAND_NAME,
             UTGHelper.CommonStrings.CODEWINDOW_PARTCOVER_BUTTON_CAPTION,
             UTGHelper.CommonStrings.CODEWINDOW_PARTCOVER_BUTTON_TOOLTIPTEXT,
             false);
      }

      private CommandBarControl CreateProjectCommand()
      {
         return CommandBarControlFactory.CreateCommandBarControl(
           _applicationObject,
           _addInInstance,
           99,
           CommandControlType.CommandControlTypeProject,
           UTGHelper.CommonStrings.PROJECT_COMMAND_NAME,
           UTGHelper.CommonStrings.PROJECT_BUTTON_CAPTION,
           UTGHelper.CommonStrings.PROJECT_BUTTON_TOOLTIPTEXT,
           true);
      }

      private CommandBarControl CreateErrorListClearCommand()
      {
         command++;

         return CommandBarControlFactory.CreateCommandBarControl(
            _applicationObject,
            _addInInstance,
            111,
            CommandControlType.CommandControlTypeProject,
            UTGHelper.CommonStrings.PROJECT_COMMAND_ERROR_CLEAR_NAME,
            UTGHelper.CommonStrings.PROJECT_BUTTON_ERROR_CLEAR_CAPTION,
            UTGHelper.CommonStrings.PROJECT_BUTTON_ERROR_CLEAR_TOOLTIPTEXT,
            false);
      }

      private CommandBarControl CreateSyncProjectReferencesCommand()
      {
         command++;

         return CommandBarControlFactory.CreateCommandBarControl(
            _applicationObject,
            _addInInstance,
            11,
            CommandControlType.CommandControlTypeProject,
            UTGHelper.CommonStrings.PROJECT_SYNC_REF_COMMAND_NAME,
            UTGHelper.CommonStrings.PROJECT_SYNC_REF_BUTTON_CAPTION,
            UTGHelper.CommonStrings.PROJECT_SYNC_REF_BUTTON_TOOLTIPTEXT,
            false);
      }

      private CommandBarControl CreatePartCoverProjectCommand()
      {
         command++;

         return CommandBarControlFactory.CreateCommandBarControl(
            _applicationObject,
            _addInInstance,
            82,
            CommandControlType.CommandControlTypeProject,
            UTGHelper.CommonStrings.PROJECT_PARTCOVER_COMMAND_NAME,
            UTGHelper.CommonStrings.PROJECT_PARTCOVER_BUTTON_CAPTION,
            UTGHelper.CommonStrings.PROJECT_PARTCOVER_BUTTON_TOOLTIPTEXT,
            false);
      }

      private CommandBarControl CreateSolutionCommand()
      {
         command++;

         return CommandBarControlFactory.CreateCommandBarControl(
          _applicationObject,
          _addInInstance,
          99,
          CommandControlType.CommandControlTypeSolution,
          UTGHelper.CommonStrings.PROJECT_SOLUTION_COMMAND_NAME,
          UTGHelper.CommonStrings.PROJECT_SOLUTION_BUTTON_CAPTION,
          UTGHelper.CommonStrings.PROJECT_SOLUTION_BUTTON_TOOLTIPTEXT,
          true);
      }

      #endregion


      #region Test run output messages. We can do better by passing the error message constant and handle it in a single method


      private void OnGenerationStart()
      {
         _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_GENERATION_STARTED;
      }


      private void OnGenerationFaliure()
      {
         _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_GENERATION_ERROR;
      }


      private void OnSynchingStarted()
      {
         _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_DELL_MATCHING_STARTED;
      }


      private void OnSynchingFinished()
      {
         _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_DELL_MATCHING_FINISHED;
      }


      private void OnSolutionBuildForUnitTestStarted()
      {
          _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_SOLUTIONBUILD_STARTED;
      }


      private void OnSolutionBuildForUnitTestFinished()
      {
          _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_SOLUTIONBUILD_FINISHED;
      }


      //See Output pane for more detail
      private void OnUnitTestingFinished(int ran, int success, int failed, int norun)
      {
         string tvMessage = string.Format(
            UTGHelper.CommonUserMessages.STATUS_TESTING_FINISHED +
            " Total ran {0}, success {1}, failed {2}, failed to execute {3}. See Output pane for more detail.",
            ran,
            success,
            failed,
            norun);

         _applicationObject.StatusBar.Text = tvMessage;
      }


      private void OnCoverStarted()
      {
         _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_COVER_STARTED;
      }


      private void OnCoverFinished(string message)
      {
         _applicationObject.StatusBar.Text = UTGHelper.CommonUserMessages.STATUS_COVER_FINISHED + ". " + message;
      }


      private void OnBuildYourSolutionFirst()
      {
          _applicationObject.StatusBar.Text =
               UTGHelper.CommonUserMessages.MSG_TEST_FAILED_BUILDSOULTION;
      }


      private void OnUnitTestingFinished()
      {
         string tvMessage = string.Format(
           UTGHelper.CommonUserMessages.STATUS_TESTING_FINISHED +
           " Total ran {0}, success {1}, failed {2}, failed to execute {3}. See Output pane for more detail.",
           ivRan,
           ivSuccess,
           ivFailed,
           ivNorun);

         _applicationObject.StatusBar.Text = tvMessage;

      }

      #endregion
   }
}