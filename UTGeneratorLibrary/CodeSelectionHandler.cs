using System;
using System.Collections.Generic;
using System.Text;

using Extensibility;
using EnvDTE;
using EnvDTE80;

using UTGHelper;
using UTGTesting;
using UTGManagerAndExaminor;

namespace UTGeneratorLibrary
{
   public static class CodeSelectionHandler
   {
      internal static void OnGenerationStart(DTE2 applicationObject)
      {
         applicationObject.StatusBar.Text = "Staring generation of Unit Test/s...";
      }

      internal static void OnGenerationFaliure(DTE2 applicationObject)
      {
         applicationObject.StatusBar.Text = "Unable to complete generation of Unit Test/s...";
      }

      internal static bool CanGenerateHandleCodeFunction(string testClassFixturePostFix, CodeClass parentCodeClass, CodeFunction codeFunction, Project unitTestProject)
      {
         foreach (ProjectItem projectItem in unitTestProject.ProjectItems)
         {
            List<CodeClass> lstProjectCodeClasses = UTGManagerAndExaminor.ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

            foreach (CodeClass codeClass in lstProjectCodeClasses)
            {
               if ((parentCodeClass.Name + testClassFixturePostFix).Equals(codeClass.Name))
               {
                  foreach (CodeElement codeElement in codeClass.Members)
                  {
                     if (codeElement is CodeFunction)
                     {
                        if (codeFunction.Name.Equals(((CodeFunction)codeElement).Name))
                           return false;

                     }
                  }
               }
            }
         }

         return true;
      }

      internal static bool CanGenerateHandleCodeFunction(CodeClass parentCodeClass, string codeFunctionName)
      {

         foreach (CodeElement codeElement in parentCodeClass.Members)
         {
            if (codeElement is CodeFunction)
            {
               if (codeFunctionName.Equals(((CodeFunction)codeElement).Name))
                  return false;

            }
         }

         return true;
      }

      internal static bool CanGenerateHandleCodeProperty(string testClassFixturePostFix, CodeClass parentCodeClass, CodeProperty codeProperty, Project unitTestProject)
      {
         foreach (ProjectItem projectItem in unitTestProject.ProjectItems)
         {
            List<CodeClass> lstProjectCodeClasses = UTGManagerAndExaminor.ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

            foreach (CodeClass codeClass in lstProjectCodeClasses)
            {
               if ((parentCodeClass.Name + testClassFixturePostFix).Equals(codeClass.Name))
               {
                  foreach (CodeElement codeElement in codeClass.Members)
                  {
                     if (codeElement is CodeProperty)
                     {
                        if (codeProperty.Name.Equals(((CodeProperty)codeElement).Name))
                           return false;

                     }
                  }
               }
            }
         }

         return true;
      }

      internal static bool CanGenerateHandleCodeClass(string testClassFixturePostFix, CodeClass ce, Project unitTestProject)
      {
         foreach (ProjectItem projectItem in unitTestProject.ProjectItems)
         {
            List<CodeClass> lstProjectCodeClasses = UTGManagerAndExaminor.ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

            foreach (CodeClass codeClass in lstProjectCodeClasses)
            {
               if ((ce.Name + testClassFixturePostFix).Equals(codeClass.Name))
                  return false;
            }
         }

         return true;

      }

      public static void OnCodeElementSelection(AbstractTestFramework testFramework, Project project, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         try
         {
            foreach (ProjectItem pItem in project.ProjectItems)
            {
               try
               {
                 
                  foreach (CodeElement tvCodeElementOutter in pItem.FileCodeModel.CodeElements)
                  {
                     if (tvCodeElementOutter is CodeNamespace)
                     {
                        try
                        {
                           foreach (CodeElement tvCodeElement in ((CodeNamespace)tvCodeElementOutter).Members)
                           {
                              if (tvCodeElement != null &&
                                 tvCodeElement is CodeClass
                                 )
                              {
                                 try
                                 {
                                    if( ((CodeClass)tvCodeElement).Access == vsCMAccess.vsCMAccessPublic)
                                       OnCodeClassSelection(testFramework, (CodeClass)tvCodeElement, applicationObject, codeType/*, license*/);
                                 }
                                 catch (Exception ex)
                                 {
                                    Logger.LogException(ex);

                                    //ignore
                                    UTGHelper.ErrorHandler.HandleException(ex);
                                    return;
                                 }
                              }
                           }
                        }
                        catch (Exception ex)
                        {
                           //ignore this 
                           Logger.LogException(ex);

                        }
                     }
                     else if (tvCodeElementOutter is CodeClass)
                     {
                        try
                        {
                           if (((CodeClass)tvCodeElementOutter).Access == vsCMAccess.vsCMAccessPublic)
                              OnCodeClassSelection(testFramework, (CodeClass)tvCodeElementOutter, applicationObject, codeType/*, license*/);
                        }
                        catch (Exception ex)
                        {
                           Logger.LogException(ex);

                           //ignore
                           UTGHelper.ErrorHandler.HandleException(ex);
                           return;
                        }
                     }

                  }//if (tvCodeElementOutter is CodeNamespace)
               }
               catch (Exception ex)
               {
                  //ignore this on;
                  Logger.LogException(ex);
               }

            }//foreach (ProjectItem pItem in project.ProjectItems)

         }//first try
         catch (Exception ex)
         {
            //ignore this on;throw;
            Logger.LogException(ex);
         }
      }

      public static void OnCodeElementSelection(AbstractTestFramework testFramework, CodeElement ce, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         OnGenerationStart(applicationObject);

         if (ce is CodeNamespace)
         {
            ErrorHandler.ShowMessage(string.Format("You have selected the namesapce {0}! Currently there are no features which support namespaces.", ((CodeNamespace)ce).FullName));
         }
         //else if (codeClass != null)
         else if (ce is CodeClass)
         {
            if ( ((CodeClass)ce).Access != vsCMAccess.vsCMAccessPublic)
               return;

            OnCodeClassSelection(testFramework, (CodeClass)ce, applicationObject, codeType);
         }
         //else if (codeFunction != null)
         else if (ce is CodeFunction)
         {
            if ( ((CodeFunction)ce).Access != vsCMAccess.vsCMAccessPublic)
               return;

            OnCodeClassSelection(testFramework, (CodeFunction)ce, applicationObject, codeType);
         }
         else if (ce is CodeEnum)
         {
            if ( ((CodeEnum)ce).Access != vsCMAccess.vsCMAccessPublic)
               return;

            ErrorHandler.ShowMessage(string.Format("You have selected the Enum {0}!", ((CodeEnum)ce).FullName));
         }
         else if (ce is CodeStruct)
         {
            ErrorHandler.ShowMessage(string.Format("You have selected the Structure {0}!", ((CodeStruct)ce).FullName));
         }
         else if (ce is CodeProperty)
         {
            if ( ((CodeProperty)ce).Access != vsCMAccess.vsCMAccessPublic)
               return;

            OnCodeClassSelection(testFramework, (CodeProperty)ce, applicationObject, codeType);
         }
         else
         {
            ErrorHandler.ShowMessage(UTGHelper.CommonErrors.ERR_ILLEGAL_TEXT_SELECTION);
         }
      }

      internal static void OnCodeClassSelection(AbstractTestFramework testFramework, CodeFunction ce, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         Project parentProject = ProjectExaminar.GetCodeClassParentProject((CodeClass)ce.Parent);

         Project unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         if (unitTestProject == null) {

            OnCodeClassSelection(testFramework, (CodeClass)ce.Parent, (CodeElement)ce, applicationObject, codeType);
            return;
         }

         //In cases where we can not simply generate automatically (definition already exists) we will
         //ask the user for input, wait on input, then async back to the function actual useful handler
         //HandelCodeClassSelection
         if (!CanGenerateHandleCodeFunction(testFramework.TestClassFixturePostFix, (CodeClass)ce.Parent, ce, unitTestProject))
         {
            //UTGHelper.ErrorHandler.HandleMessage(CommonUserMessages.MSG_TEST_ALREADY_EXISTS);

            UTGHelper.UserAlertActionRequired tvUserAlertActionRequired = new UserAlertActionRequired(
               "Function definition already exits! What would you like to do?",
               typeof(CodeFunction),
               (CodeElement)ce,
               ref applicationObject,
               codeType,
               testFramework);

            if (!tvUserAlertActionRequired.IsDisposed)
            {
               tvUserAlertActionRequired.FormClosed += new System.Windows.Forms.FormClosedEventHandler(tvUserAlertActionRequired_FormClosed);

               tvUserAlertActionRequired.Show();
            }
            else
            {
               HandelCodeClassSelection(testFramework, ce, applicationObject, codeType);
            }

            return;
         }

         //otherwise simply call HandelCodeClassSelection
         HandelCodeClassSelection(testFramework, ce, applicationObject, codeType);
      }

      private static void OnCodeClassSelection(AbstractTestFramework testFramework, CodeProperty ce, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         Project parentProject = ProjectExaminar.GetCodeClassParentProject((CodeClass)ce.Parent);

         Project unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         if (unitTestProject == null) {

            OnCodeClassSelection(testFramework, (CodeClass)ce.Parent, (CodeElement)ce, applicationObject, codeType);
            return;
         }

         //In cases where we can not simply generate automatically (definition already exists) we will
         //ask the user for input, wait on input, then async back to the function actual useful handler
         //HandelCodeClassSelection
         if (!CanGenerateHandleCodeProperty(testFramework.TestClassFixturePostFix, (CodeClass)ce.Parent, ce, unitTestProject))
         {
            UTGHelper.UserAlertActionRequired tvUserAlertActionRequired = new UserAlertActionRequired(
               "Property definition already exits! What would you like to do?",
               typeof(CodeProperty),
               (CodeElement)ce,
               ref applicationObject,
               codeType,
               testFramework);

            if (!tvUserAlertActionRequired.IsDisposed)
            {
               tvUserAlertActionRequired.FormClosed += new System.Windows.Forms.FormClosedEventHandler(tvUserAlertActionRequired_FormClosed);

               tvUserAlertActionRequired.Show();
            }
            else
            {
               HandelCodeClassSelection(testFramework, ce, applicationObject, codeType);
            }

            return;
         }

         //otherwise simply call HandelCodeClassSelection
         HandelCodeClassSelection(testFramework, ce, applicationObject, codeType);

      }

      /// Consider Re-factoring against OnCodeClassSelection(CodeClass codeClass, DTE2 applicationObject, UnitTestCodeType codeType)
      /// <summary>
      /// Nearly identical to overloaded but without the CodeElement parameter. Difference is that this
      /// will create a new class with a single test only for CodeElement.
      /// </summary>
      /// <param name="ce"></param>
      /// <param name="cElement"></param>
      /// <param name="applicationObject"></param>
      /// <param name="codeType"></param>
      private static void OnCodeClassSelection(AbstractTestFramework testFramework, CodeClass codeClass, CodeElement cElement, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         if (codeClass.Access != vsCMAccess.vsCMAccessPublic)
            return;

         Project parentProject = ProjectExaminar.GetCodeClassParentProject(codeClass);

         Project  unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         if (unitTestProject != null && !CanGenerateHandleCodeClass(testFramework.TestClassFixturePostFix, codeClass, unitTestProject)) {

            UTGHelper.ErrorHandler.ShowMessage(CommonUserMessages.MSG_TEST_ALREADY_EXISTS);
            OnGenerationFaliure(applicationObject);
            return;
         }

         string path = ProjectManager.GetUnitTestProjectPath(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         path = UTGHelper.IO.GetFilePath(path);

         System.IO.DirectoryInfo newDirectoryInfo;

         string fullPath = System.IO.Path.Combine(path, parentProject.Name + testFramework.TestProjectPostFix);

         try {
            //Another way of doing this would be to try and find this project in current open solution saving us the exception
            newDirectoryInfo =
               System.IO.Directory.CreateDirectory(fullPath);
         }
         catch (Exception ex) {
            Logger.LogException(ex);
         }
         finally {
            //Either case we have one by now so lets get its directory information
            newDirectoryInfo = new System.IO.DirectoryInfo(fullPath);
         }

         //If for whatever reason we are still failing then simply quit this 
         if (newDirectoryInfo == null)
         {
            OnGenerationFaliure(applicationObject);

            throw new Exception(string.Format("Unable to create new Directory {0}", fullPath));
         }

         if (unitTestProject == null)
            unitTestProject = ProjectManager.CreateAndAddUnitTestProject((Solution2)applicationObject.Solution, parentProject.Name + testFramework.TestProjectPostFix, newDirectoryInfo.FullName, codeType);

         //Since above operation fails in either case then try getting the new Unit Test Project if created and added ok..
         if (unitTestProject == null)
            unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         if (unitTestProject == null) {

            OnGenerationFaliure(applicationObject);
            throw new Exception("Can not create new UnitTest Project");
         }

         AbstractTestClassFactory testClassFactory = new NUnitTestClassFactory();

         AbstractTestClass nunitClass = null;

         if (codeType == UnitTestCodeType.CSharp)
         {
            nunitClass = testClassFactory.CreateCSharpTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
         }
         else if (codeType == UnitTestCodeType.VB)
         {
            nunitClass = testClassFactory.CreateVBTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
         }

         if (!nunitClass.write(newDirectoryInfo.FullName, testFramework.TestClassFixturePostFix))
         {
            OnGenerationFaliure(applicationObject);
            return;
         }

         //add new class to project
         ProjectItem projectItem = ProjectManager.AddClassToProject(applicationObject, unitTestProject, null, nunitClass.FullName);

         List<CodeClass> lstCodeClasses = ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

         if (lstCodeClasses == null || lstCodeClasses.Count == 0)
         {
            OnGenerationFaliure(applicationObject);
            return;
         }

         nunitClass.GenerateTest(lstCodeClasses[0], codeClass, cElement);

         //To consider: this should be handled by an object that inherits from project type
         copyReferences(testFramework, parentProject, unitTestProject);

         applicationObject.ItemOperations.OpenFile(nunitClass.FullName, EnvDTE.Constants.vsViewKindAny);
      }


      /// Consider Re-factoring against OnCodeClassSelection(CodeClass codeClass, DTE2 applicationObject, UnitTestCodeType codeType)
      /// <summary>
      /// Very close to overloaded except that this is used in cases where a code function or a property is selected
      /// and the parent class has not been yet generated, however the unit test project does exists.
      /// </summary>
      /// <param name="ce">The code class from which we will generate</param>
      /// <param name="cElement">The code element chosen. Either CodeFunction or CodeProperty</param>
      /// <param name="applicationObject">The application object</param>
      /// <param name="unitTestProject">The Unit Test Project which already exits for this class' unit test</param>
      /// <param name="codeType">The Unit Test Project which already exits for this class' unit test</param>
      internal static void OnCodeClassSelection(AbstractTestFramework testFramework, CodeClass codeClass, CodeElement cElement, DTE2 applicationObject, Project unitTestProject, UnitTestCodeType codeType)
      {
         if (codeClass.Access != vsCMAccess.vsCMAccessPublic)
            return;

         Project parentProject = ProjectExaminar.GetCodeClassParentProject(codeClass);

         if (unitTestProject != null && !CanGenerateHandleCodeClass(testFramework.TestClassFixturePostFix, codeClass, unitTestProject)) {

            UTGHelper.ErrorHandler.ShowMessage(CommonUserMessages.MSG_TEST_ALREADY_EXISTS);
            OnGenerationFaliure(applicationObject);
            return;
         }

         string path = ProjectManager.GetUnitTestProjectPath(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         path = UTGHelper.IO.GetFilePath(path);

         System.IO.DirectoryInfo newDirectoryInfo = null;

         string fullPath = System.IO.Path.Combine(path, parentProject.Name + testFramework.TestProjectPostFix);

         try
         {
            //Another way of doing this would be to try and find this project in current open solution saving us the exception
            newDirectoryInfo = new System.IO.DirectoryInfo(fullPath);
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
         }

         //If for whatever reason we are still failing then simply quit this 
         if (newDirectoryInfo == null)
         {
            OnGenerationFaliure(applicationObject);

            throw new Exception(string.Format("Unable to create new Directory {0}", fullPath));
         }

         AbstractTestClassFactory testClassFactory = new NUnitTestClassFactory();

         AbstractTestClass nunitClass = null;

         if (codeType == UnitTestCodeType.CSharp)
         {
            nunitClass = testClassFactory.CreateCSharpTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
         }
         else if (codeType == UnitTestCodeType.VB)
         {
            nunitClass = testClassFactory.CreateVBTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
         }

         if (!nunitClass.write(newDirectoryInfo.FullName, testFramework.TestClassFixturePostFix))
         {
            OnGenerationFaliure(applicationObject);
            return;
         }

         //add new class to project
         ProjectItem projectItem = ProjectManager.AddClassToProject(applicationObject, unitTestProject, null, nunitClass.FullName);

         List<CodeClass> lstCodeClasses = ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

         if (lstCodeClasses == null || lstCodeClasses.Count == 0)
         {
            OnGenerationFaliure(applicationObject);
            return;
         }

         CodeClass unitTestCodeClass = lstCodeClasses[0];

         //passed this point the actual object will take care of it self.
         nunitClass.GenerateTest(unitTestCodeClass, codeClass, cElement);

         //To consider: this should be handled by an object that inherits from project type
         copyReferences(testFramework, parentProject, unitTestProject);

         applicationObject.ItemOperations.OpenFile(nunitClass.FullName, EnvDTE.Constants.vsViewKindAny);
      }

      //OK
      internal static void OnCodeClassSelection(AbstractTestFramework testFramework, CodeClass codeClass, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         //We can only test public methods
         if (codeClass.Access != vsCMAccess.vsCMAccessPublic)
            return;

         Project parentProject = ProjectExaminar.GetCodeClassParentProject(codeClass);

         Project unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         if (unitTestProject != null && !CanGenerateHandleCodeClass(testFramework.TestClassFixturePostFix, codeClass, unitTestProject)) {
            UTGHelper.ErrorHandler.ShowMessage(CommonUserMessages.MSG_TEST_ALREADY_EXISTS);
            OnGenerationFaliure(applicationObject);
            return;
         }

         string path = ProjectManager.GetUnitTestProjectPath(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         path = UTGHelper.IO.GetFilePath(path);

         System.IO.DirectoryInfo newDirectoryInfo;

         string fullPath = System.IO.Path.Combine(path, parentProject.Name + testFramework.TestProjectPostFix);

         try{
            //Another way of doing this would be to try and find this project in current open solution saving us the exception
            newDirectoryInfo =
               System.IO.Directory.CreateDirectory(fullPath);
         }
         catch (Exception ex){
            Logger.LogException(ex);
         }
         finally{
            //Either case we have one by now so lets get its directory information
            newDirectoryInfo = new System.IO.DirectoryInfo(fullPath);
         }

         //If for whatever reason we are still failing then simply quit this 
         if (newDirectoryInfo == null)
         {
            OnGenerationFaliure(applicationObject);

            throw new Exception(string.Format("Unable to create new Directory {0}", System.IO.Path.Combine(path, parentProject.Name + testFramework.TestProjectPostFix)));
         }

         if (unitTestProject == null)
         {
            try{
               unitTestProject = ProjectManager.CreateAndAddUnitTestProject((Solution2)applicationObject.Solution, parentProject.Name + testFramework.TestProjectPostFix, newDirectoryInfo.FullName, codeType/*, license*/);
            }
            catch (Exception ex){
               Logger.LogException(ex);

               OnGenerationFaliure(applicationObject);

               throw;
            }
         }

         //Rethink this bit
         //Since above operation fails in either case then try getting the new Unit Test Project if created and added ok..
         if (unitTestProject == null) {
            unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);
         }

         if (unitTestProject == null) {
            OnGenerationFaliure(applicationObject);
            throw new Exception("Can not create new UnitTest Project");
         }
 
         AbstractTestClassFactory testClassFactory = new NUnitTestClassFactory();

         AbstractTestClass nunitClass = null;

         if (codeType == UnitTestCodeType.CSharp) {
            nunitClass = testClassFactory.CreateCSharpTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
         }
         else if (codeType == UnitTestCodeType.VB) {
            nunitClass = testClassFactory.CreateVBTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
         }

         if (!nunitClass.write(newDirectoryInfo.FullName, testFramework.TestClassFixturePostFix)) {
            OnGenerationFaliure(applicationObject);
            return;
         }
        
         //add new class to project
         ProjectItem projectItem = ProjectManager.AddClassToProject(applicationObject, unitTestProject, null, nunitClass.FullName);

         List<CodeClass> lstCodeClasses = ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

         if (lstCodeClasses == null || lstCodeClasses.Count == 0) {
            OnGenerationFaliure(applicationObject);
            return;
         }

         CodeClass unitTestCodeClass = lstCodeClasses[0];

         //passed this point the actual object will take care of it self.
         nunitClass.GenerateTest(unitTestCodeClass, codeClass);

         //To consider: this should be handled by an object that inherits from project type
         copyReferences(testFramework, parentProject, unitTestProject);

         applicationObject.ItemOperations.OpenFile(nunitClass.FullName, EnvDTE.Constants.vsViewKindAny);
      }

      internal static void HandelCodeClassSelection(AbstractTestFramework testFramework, CodeFunction function, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         Project parentProject = ProjectExaminar.GetCodeClassParentProject((CodeClass)function.Parent);

         Project unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         //We might or might not use this factory, we don't know at this point
         AbstractTestClassFactory testClassFactory = new NUnitTestClassFactory();

         foreach (ProjectItem projectItem in unitTestProject.ProjectItems)
         {
            System.Collections.Generic.List<CodeClass> lstCodeClasses =
                 ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

            foreach (CodeClass codeClass in lstCodeClasses)
            {
               CodeClass parentClass = (CodeClass)function.Parent;

               string className = codeClass.Name;
               string testClassName = ((CodeClass)function.Parent).Name + testFramework.TestClassFixturePostFix;
               
               //find the parent for function passed in
               //if (codeClass.Name.Equals(((CodeClass)function.Parent).Name + CommonStrings.DEFAULT_STRING_SEPERATOR + UTGHelper.CommonStrings.DEFAULT_UNIT_TEST_CLASS_POSTFIX))
               if (className.Equals(testClassName) == true)
               {
                  //if found 
                  AbstractTestClass nunitClass = null;

                  if (codeType == UnitTestCodeType.CSharp)
                  {
                     nunitClass = testClassFactory.CreateCSharpTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
                  }
                  else if (codeType == UnitTestCodeType.VB)
                  {
                     nunitClass = testClassFactory.CreateVBTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
                  }

                  nunitClass.GenerateTest(codeClass, function);

                  //we sure expect just one class, the first class we find matching
                  return;
               }
            }
         }

         //if we have reached this point then it means that we have not been able to locate a parent class in our
         //unit test project, then we simply create a class unit test

         OnCodeClassSelection(testFramework, (CodeClass)function.Parent, (CodeElement)function, applicationObject, unitTestProject, codeType);
      }

      internal static void HandelCodeClassSelection(AbstractTestFramework testFramework, CodeProperty property, DTE2 applicationObject, UnitTestCodeType codeType)
      {
         Project parentProject = ProjectExaminar.GetCodeClassParentProject((CodeClass)property.Parent);

         Project unitTestProject = ProjectExaminar.GetUnitTestProject(parentProject.Name, applicationObject, testFramework.TestProjectPostFix);

         //We might or might not use this factory, we don't know at this point
         AbstractTestClassFactory testClassFactory = new NUnitTestClassFactory();

         foreach (ProjectItem projectItem in unitTestProject.ProjectItems)
         {
            System.Collections.Generic.List<CodeClass> lstCodeClasses =
                 ProjectItemExaminor.GetCodeClasses(projectItem.FileCodeModel);

            foreach (CodeClass codeClass in lstCodeClasses)
            {
               if (codeClass.Name.Equals(((CodeClass)property.Parent).Name + testFramework.TestClassFixturePostFix))
               {
                  //if found 
                  AbstractTestClass nunitClass = null;

                  if (codeType == UnitTestCodeType.CSharp)
                  {
                     nunitClass = testClassFactory.CreateCSharpTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
                  }
                  else if (codeType == UnitTestCodeType.VB)
                  {
                     nunitClass = testClassFactory.CreateVBTestClass(codeClass.Namespace.FullName, codeClass.Name, codeClass.IsAbstract);
                  }

                  nunitClass.GenerateTest(codeClass, property);

                  //we sure expect just one class, the first class we find matching
                  return;
               }
            }
         }

         //if we have reached this point then it means that we have not been able to locate a parent class in our
         //unit test project, then we simply create a class unit test

         OnCodeClassSelection(testFramework, (CodeClass)property.Parent, (CodeElement)property, applicationObject, unitTestProject, codeType);
      }

      static void tvUserAlertActionRequired_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
      {
         UTGHelper.UserAlertActionRequired tvUserAlertActionRequired = (UTGHelper.UserAlertActionRequired)sender;

         if (tvUserAlertActionRequired.ApplicableAcctionState == UserAlertActionRequired.ApplicableAcction.ignore)
            return;

         if(tvUserAlertActionRequired.ApplicableAcctionState == UserAlertActionRequired.ApplicableAcction.mutate)
            HandelCodeClassSelection((AbstractTestFramework)tvUserAlertActionRequired.TestFramework, (CodeFunction)tvUserAlertActionRequired.FormCodeElement, tvUserAlertActionRequired.FormApplicationObject, (UnitTestCodeType)tvUserAlertActionRequired.FromCodeType);
      }

      internal static void copyReferences(AbstractTestFramework testFramework, Project parentProject, Project unitTestProject)
      {
         //Add a unit test dll like nunit.framework.dll ref
         string tvTempUnitTestFolderHolder = testFramework.BinariesDirectory;

         if (!tvTempUnitTestFolderHolder.Equals(string.Empty))
         {
            foreach(string dll in testFramework.FrameworkDlls)
            {
               VSLangProj.Reference nunitReferance =
               ProjectManager.AddDllReferenceToProject(unitTestProject, System.IO.Path.Combine(tvTempUnitTestFolderHolder, dll));
            }
         }

         //add original projects outputted dll
         string tvTempOriginalProjectOutputDll =
            UTGManagerAndExaminor.ProjectExaminar.GetProjectOutputFullAssemblyName(parentProject);

         ProjectManager.AddDllReferenceToProject(unitTestProject, tvTempOriginalProjectOutputDll);

         //add original projects refs
         VSLangProj.References references = ProjectManager.GetProjectReferences(parentProject);

         ProjectManager.AddReferencesToProject(references, unitTestProject);
      }
   }
}
