

using System;
using System.Collections.Generic;
using System.Text;

namespace UTGHelper
{
   public static class CommonStrings
   {
      public const string APPLICATION_NAME = "nMate";

      public const string DEMO_VERSION_NOT_SUPPORTED = "Feature Not Supported In Demo";

      public const string DEMO_VERSION_LIMITS_EXCEEDED = "Demo Limits Exceeded";



      //Token Code Command Window...
      public const string CODEWINDOW_TOKEN_COMMAND_NAME = "UnitTestCodeGeneration";

      public const string CODEWINDOW_TOKEN_BUTTON_CAPTION = "Create Test";

      public const string CODEWINDOW_TOKEN_COMMAND_TOOLTIP = "Generate Test stub for this Token";

      public const string CODEWINDOW_TOKEN_SELECTION_NONESELECTED = "No selection is made!";


      //Code Command Window...
      public const string CODEWINDOW_PROJECT_COMMAND_NAME = "UnitTestProjectCodeGeneration";

      public const string CODEWINDOW_PROJECT_BUTTON_CAPTION = "Create Project Tests";

      public const string CODEWINDOW_PROJECT_COMMAND_TOOLTIP = "Generate tests for this entire Project at once";


      //Attach Command Window...
      public const string CODEWINDOW_ATTACH_COMMAND_NAME = "NMateAttachToProccess";

      public const string CODEWINDOW_ATTACH_BUTTON_CAPTION = "InProcess Debug";

      public const string CODEWINDOW_ATTACH_COMMAND_TOOLTIP = "Attach to this process to Debug";


      public const string CODEWINDOW_TEST_METHOD_COMMAND_NAME = "TestCodeFunction";

      public const string CODEWINDOW_TEST_METHOD_BUTTON_CAPTION = "Test Method";

      public const string CODEWINDOW_TEST_METHOD_COMMAND_TOOLTIP = "Run Single Test";



      public const string CODEWINDOW_TEST_CLASS_COMMAND_NAME = "TestCodeClass";

      public const string CODEWINDOW_TEST_CLASS_BUTTON_CAPTION = "Test Fixture";

      public const string CODEWINDOW_TEST_CLASS_COMMAND_TOOLTIP = "Run Class Test";




      //Project Run Unit Test Button...
      public const string PROJECT_COMMAND_NAME = "RunUnitTest";

      public const string PROJECT_BUTTON_CAPTION = "Run Tests";

      public const string PROJECT_BUTTON_TOOLTIPTEXT = "Run all tests for this Project now";


      public const string PROJECT_SOLUTION_COMMAND_NAME = "SolutionRunUnitTest";

      public const string PROJECT_SOLUTION_BUTTON_CAPTION = "Run Tests";

      public const string PROJECT_SOLUTION_BUTTON_TOOLTIPTEXT = "Run all tests for this Project now";

      //Project Clear Errors Buttons
      public const string PROJECT_COMMAND_ERROR_CLEAR_NAME = "ErrorListClear";

      public const string PROJECT_BUTTON_ERROR_CLEAR_CAPTION = "Clear Errors";

      public const string PROJECT_BUTTON_ERROR_CLEAR_TOOLTIPTEXT = "Force clear " + APPLICATION_NAME + "' type Errors in Error List now";


      //Project Sync Refs
      public const string PROJECT_SYNC_REF_COMMAND_NAME = "ReSyncReferences";

      public const string PROJECT_SYNC_REF_BUTTON_CAPTION = "Synchronize dlls";

      public const string PROJECT_SYNC_REF_BUTTON_TOOLTIPTEXT = "Synchronizes dll references with original parent Project";


      public const string PROJECT_PARTCOVER_COMMAND_NAME = "ProjectPartCover";

      public const string PROJECT_PARTCOVER_BUTTON_CAPTION = "Run Coverage";

      public const string PROJECT_PARTCOVER_BUTTON_TOOLTIPTEXT = "Run Code Coverage for this Project";


      public const string CODEWINDOW_PARTCOVER_COMMAND_NAME = "CodeWindowPartCover";

      public const string CODEWINDOW_PARTCOVER_BUTTON_CAPTION = "Run Coverage";

      public const string CODEWINDOW_PARTCOVER_BUTTON_TOOLTIPTEXT = "Run Code Coverage for this Fixture";

      public static string DEFAULT_LANGUAGE = "CSharp";

      public static string DEFAULT_VB_LANGUAGE = "VisualBasic";

      public static string DEFAULT_PROJECT_TEMPLATE = "ClassLibrary.zip";

      public static string DEFAULT_CODE_FILE_EXTENSION = ".cs";

      public static string DEFAULT_CODE_VB_FILE_EXTENSION = ".vb";

      //public static string DEFAULT_STRING_SEPERATOR = "_";

      //Error Pane realted..
      public static string UNIT_TEST_OUTPUT_PANE_NAME = CommonStrings.APPLICATION_NAME + " Pane";

      public static string UNIT_TEST_OUTPUT_PANE_TEXT_PREFIX = CommonStrings.APPLICATION_NAME + ": ";
   }

   public static class CommonErrors
   { 
      public static string DEFAULT_EXCEPTION_TEXT_TITLE = "Error occurred!";

      public static string DEFAULT_EXCEPTION_TEXT = "An unexpected error has occurred. We are sorry for your inconvenience. ";

      public static string DEFAULT_MESSAGE_TEXT_TITLE = CommonStrings.APPLICATION_NAME + " Message";
       
      public static string DEFAULT_MESSAGE_TEXT = "";

      public const string ERR_TYPE_NOT_SUPPORTED = "Type {0} not currently supported. Please contact " + CommonStrings.APPLICATION_NAME + " Administrator to add this support";

      public const string ERR_LANUAGE_TYPE_NOT_SUPPORTED = "Language {0} not currently supported. Please contact " + CommonStrings.APPLICATION_NAME + " Administrator to add this support";

      public const string ERR_UNABLE_TO_LOCATE_NUNIT_INSTALL_FOLDER =
                        "Unable to locate nunit binaries. Please locate them manually now (most likely in the framework folder for nunit). You can modify the binaries location by updating nMates config file.";

      public const string ERR_UNABLE_TO_LOCATE_NUNIT_CONSOLE =
                       "Unable to locate nunit nunit-console.exe. Please locate it manually now (most likely directly in your nunit folder). You can modify its location by updating nMates config file.";

      public const string ERR_UNABLE_TO_LOCATE_COVER_INSTALL_FOLDER =
                       "Unable to locate Coverage Framework installation folder. Please locate it manually now. You can modify its location by updating nMates config file.";


      public const string ERR_ILLEGAL_TEXT_SELECTION = "Emmm...Illegal Selection made!";

      public const string ERR_UNABLE_TO_RUN_UNITTEST =
         "Could not run Unit Test. Possible reasons are: You need to perform a Build then try running this test again.";

      public const string ERR_UNABLE_TO_LOCATE_UNITTEST_PROJECT_OUTPUT_DIRECTORY = "Can't find Unit Test Project Output Directory!";

      public const string ERR_DEMO_EXPIRED = @"
Demo has expired. To avoid seeing this message again disable nMate through 
Visual Studio's Addin Manager or purchase nMate. Thank you for trying nMate.";

      public const string ERR_UNABLE_TO_REPORT_ISSUE = "Sorry. Unable to report your issue at this time";

   }

   public static class CommonUserMessages
   {
      public static string MSG_TEST_FAILED_BUILDSOULTION = "Testing failed. Please try again or build your solution first then try testing.";
      
      public static string MSG_TEST_FAILED_TO_EXECUTE = "One or more Unit Tests have failed to execute. Please view " + CommonStrings.APPLICATION_NAME  +"'s Output Pane Window";
      
      public static string MSG_TEST_ALREADY_EXISTS = "A Test has already been generated for this item!";

      public static string STATUS_GENERATION_STARTED = "Staring generation of Unit Test/s...";

      public static string STATUS_GENERATION_ERROR = "Unable to complete generation of Unit Test/s...";

      public static string STATUS_DELL_MATCHING_STARTED = "Matching dlls...";

      public static string STATUS_DELL_MATCHING_FINISHED = "Matching dlls finished";

      public static string STATUS_SOLUTIONBUILD_STARTED = "Building solution started. Waiting for solution build to finish...";

      public static string STATUS_SOLUTIONBUILD_FINISHED = "Building solution finished";

      public static string STATUS_TESTING_STARTED = "Running Unit Test...";

      public static string STATUS_TESTING_FINISHED = "Unit Test Finished";

      public static string STATUS_COVER_STARTED = "Running Code Coverage...";

      public static string STATUS_COVER_FINISHED = "Code Coverage Finished";

      public static string STATUS_COVER_ERROR = "Code Coverage Un Expected Error Occurred!";

      public static string STATUS_COVER_FAILED = "Code Coverage Failed! You may try again.";

      public static string STATUS_COVER_SUCCESS = "Code Coverage completed successfully. Find your  report";

      public static string MSG_UPDATE_AVAILABLE = "A new update is now available for nMate. Using your default browser, would you like you to download update?";
   }

   public static class CommonGeneralUsageStrings
   {
      public static string DefaultEmptyReportContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<PartCoverReport ver=""1.0.2796.35184"" />";
   }

}
