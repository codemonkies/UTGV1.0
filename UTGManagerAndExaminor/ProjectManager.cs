using System;
using System.Text;
using Extensibility;
using EnvDTE;
using EnvDTE80;

using UTGHelper;

namespace UTGManagerAndExaminor
{
   public static class ProjectManager
   {
      public static Project CreateAndAddUnitTestProject(Solution2 sol, string csProjectName, string csProjectPath, UnitTestCodeType codeType)
      {
         //System.Windows.Forms.MessageBox.Show("You must have at least 1 project in your current solution with a project name that ends with UnitTest");
         Project proj = null;

         try
         {
            String csItemTemplatePath;

            switch (codeType)
            {
               case UnitTestCodeType.CSharp:
                  csItemTemplatePath = sol.GetProjectTemplate(UTGHelper.CommonStrings.DEFAULT_PROJECT_TEMPLATE, UTGHelper.CommonStrings.DEFAULT_LANGUAGE);
                  break;
               case UnitTestCodeType.VB:
                  csItemTemplatePath = sol.GetProjectTemplate(UTGHelper.CommonStrings.DEFAULT_PROJECT_TEMPLATE, UTGHelper.CommonStrings.DEFAULT_VB_LANGUAGE);
                  break;
               default:
                  throw new Exception(string.Format(UTGHelper.CommonErrors.ERR_LANUAGE_TYPE_NOT_SUPPORTED, codeType.ToString()));
            }

            proj = sol.AddFromTemplate(csItemTemplatePath, csProjectPath, csProjectName, false);
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            throw;
         }

         return proj;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="applicationObject"></param>
      /// <param name="project"></param>
      /// <param name="className">Whatever name you wish to give your class should you pass a null for classContent parameter</param>
      /// <param name="classContent">passing a string.Empty will add an empty class</param>
      /// <returns></returns>
      public static ProjectItem AddClassToProject(DTE2 applicationObject, Project project, string className, string classFullPath)
      {
         ProjectItem projectItem = null;

         //Are we addding a pre-exiting class, or creating an empty one?
         if (classFullPath != string.Empty)
         {
            projectItem =
               ProjectManager.AddClassToProject((Solution2)applicationObject.Solution, project, classFullPath);
         }
         else
         {
            projectItem =
               ProjectManager.AddClassToProject((Solution2)applicationObject.Solution, project, className, UTGHelper.CommonStrings.DEFAULT_CODE_FILE_EXTENSION);
         }

         return projectItem;
      }

      /// <summary>
      /// New
      /// </summary>
      /// <param name="applicationObject"></param>
      /// <returns></returns>
      public static string GetUnitTestProjectPath(string projectBeingTestedName, DTE2 applicationObject, string testProjectPostFix)
      {
         Project unitTestProject = ProjectExaminar.GetUnitTestProject(projectBeingTestedName, applicationObject, testProjectPostFix);

         if (unitTestProject != null)
            return UTGHelper.IO.GetFilePath(unitTestProject.FullName);

         Project parentProject =
            applicationObject.ActiveWindow.Document.ProjectItem.ContainingProject;

         return UTGHelper.IO.GetFilePath(parentProject.FullName);
      }

      static public ProjectItem AddClassToProject(Solution2 soln, Project proj, string classFullPath)
      {
         if (soln == null)
            throw new Exception("Invalid  DTE2 parameter");

         if (proj == null)
            throw new Exception("Invalid  Project parameter");

         ProjectItem projectItem = null;

         try
         {
            projectItem = proj.ProjectItems.AddFromFile(classFullPath);

         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            ErrorHandler.HandleException(ex);
         }

         return projectItem;
      }

      /// <summary>
      /// Adds a Class to the specified project in the solution.
      /// </summary>
      /// <param name="soln">The solution</param>
      /// <param name="proj">The project</param>
      /// <param name="className">The name of class you wish to create</param>
      /// <param name="extension">Extension like .cs or .vb</param>
      /// <returns>ProjectItem on success. Null otherwise</returns>
      static public ProjectItem AddClassToProject(Solution2 soln, Project proj, string className, string extension)
      {
         if (soln == null)
            throw new Exception("Invalid  DTE2 parameter");

         if (proj == null)
            throw new Exception("Invalid  Project parameter");

         if (className.Equals(string.Empty) || className.Equals(""))
            throw new Exception("Invalid  className parameter");

         if (extension.Equals(string.Empty) || extension.Equals(""))
            throw new Exception("Invalid  extension parameter");

         return AddClassToProject(soln, proj, className, "", "", extension);
      }


      /// <summary>
      /// Adds a Class to the specified project in the solution.
      /// </summary>
      /// <param name="soln">The solution</param>
      /// <param name="proj">The project</param>
      /// <param name="className">The name of class you wish to create</param>
      /// <param name="postFix">String postfix</param>
      /// <param name="preFix">String prefix</param>
      /// <param name="extension">Extension like .cs or .vb</param>
      /// <returns>ProjectItem on success. Null otherwise</returns>
      static public ProjectItem AddClassToProject(Solution2 soln, Project proj, string className, string postFix, string preFix, string extension)
      {
         if (soln == null)
            throw new Exception("Invalid  DTE2 parameter");

         if (proj == null)
            throw new Exception("Invalid  Project parameter");

         if (className.Equals(string.Empty) || className.Equals(""))
            throw new Exception("Invalid  className parameter");

         if (extension.Equals(string.Empty) || extension.Equals(""))
            throw new Exception("Invalid  extension parameter");

         ProjectItem projectItem = null;

         try
         {
            String csItemTemplatePath = soln.GetProjectItemTemplate("CodeFile",
             "CSharp");

            proj.ProjectItems.AddFromTemplate(csItemTemplatePath, className + preFix + "." + extension);

            projectItem = proj.ProjectItems.Item(className + preFix + "." + extension);
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);

            ErrorHandler.HandleException(ex);
         }

         return projectItem;
      }

      /// <summary>
      /// Requires NUnit 2.4.6 installed
      /// </summary>
      /// <param name="proj"></param>
      /// <returns></returns>
      [Obsolete("Use AddDllReferenceToProject() instead")]
      static public VSLangProj.Reference AddNUnitDllReferenceToProject(Project proj, string unitTestDllFullPath)
      {
         //FIX ME
         return AddDllReferenceToProject(proj, unitTestDllFullPath);
      }

      static public VSLangProj.References GetProjectReferences(Project project)
      {
         if (project == null)
            throw new Exception("Invalid  Project parameter");

         VSLangProj.VSProject vsProject = (VSLangProj.VSProject)project.Object;

         return vsProject.References;
      }

      static public void RemoveReferenceFromProject(int index, Project project)
      {
         try
         {
            VSLangProj.VSProject vsProject = (VSLangProj.VSProject)project.Object;

            vsProject.References.Item(index).Remove();
         }
         catch (Exception ex)
         {
            //ignore...
            Logger.LogException(ex);
         }
      }

      static public int GetReferenceIndexFromProject(string referenceName, Project project)
      {
         VSLangProj.VSProject vsProject = (VSLangProj.VSProject)project.Object;
         int index = 0;
         foreach(VSLangProj.Reference projRef in vsProject.References)
         {
            index++;
            if(projRef.Name.Equals(referenceName))
            {
               return index;
            }
         }
         return index;
      }

      static public void AddReferencesToProject(VSLangProj.References references, Project project)
      {
         if (project == null)
            throw new Exception("Invalid  Project parameter");

         VSLangProj.VSProject vsProject = (VSLangProj.VSProject)project.Object;

         foreach (VSLangProj.Reference refernce in references)
         {
            try
            {

               vsProject.References.Add(refernce.Path);
            }
            catch (Exception ex)
            {
               //ignore...
               Logger.LogException(ex);
            }
         }
      }

      static public VSLangProj.Reference AddDllReferenceToProject(Project proj, string dllReference)
      {
         if (proj == null)
            throw new Exception("Invalid  Project parameter");

         if (dllReference.Equals(string.Empty) || dllReference.Equals(""))
            throw new Exception("Invalid  dllReference parameter");

         VSLangProj.Reference reference  = null;

         VSLangProj.VSProject vsProject = (VSLangProj.VSProject)proj.Object;

         reference =
               vsProject.References.Find(dllReference);

         if (reference == null)
         {
            try
            {
               reference =
                  vsProject.References.Add(dllReference);

            }
            catch (Exception ex)
            {
               //ErrorHandler.HandleException(ex);
               Logger.LogException(ex);
            }
         }

         return reference;
      }
   }
}
