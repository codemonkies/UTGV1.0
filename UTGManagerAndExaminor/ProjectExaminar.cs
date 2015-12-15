using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using UTGHelper;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System.Collections.Generic;

namespace UTGManagerAndExaminor
{
   public static class ProjectExaminar
   {
      /// <summary>
      /// Returns the current unit test project based on the criteria supplied. The likeName is
      /// compared to the project name.
      /// </summary>
      /// <param name="dte"></param>
      /// <returns>The unit test project if on exists. Null otherwise</returns>
      
      //FIX ME
      static public Project GetUnitTestProject(string projectBeingTestedName, DTE2 dte, string testProjectPostFix)
      {
         if (dte == null)
            throw new Exception("Invalid  DTE2 parameter");

         Solution2 soln = (Solution2)dte.Solution;

         int solutionsCount = soln.Projects.Count;

         for (int i = 1; i <= solutionsCount; i++)
         {
            Project proj = soln.Projects.Item(i);

            System.Text.RegularExpressions.Regex regXProject = new System.Text.RegularExpressions.Regex("\\b" + projectBeingTestedName + testProjectPostFix + "\\b");

            System.Text.RegularExpressions.Match isMatchied = 
               regXProject.Match(proj.Name);

            if (isMatchied.Success)
               return proj;
         }

         return null;
      }



      //FIX ME
      static public Project GetUnitTestProjectsOriginalProject(string unitTestPorjectName, DTE2 dte, string testProjectPostFix)
      {
         if (dte == null)
            throw new Exception("Invalid  DTE2 parameter");

         Solution2 soln = (Solution2)dte.Solution;

         // Open a C# solution in the Visual Studio IDE
         // before running this add-in.
         int solutionsCount = soln.Projects.Count;

         string tvOriginalPojectExpectName = unitTestPorjectName.Replace(testProjectPostFix, "");

         if (tvOriginalPojectExpectName.Equals(unitTestPorjectName))
            return null;

         for (int i = 1; i <= solutionsCount; i++)
         {
            Project proj = soln.Projects.Item(i);

            if (proj.Name.Equals(tvOriginalPojectExpectName))
            {
               return proj;
            }
         }

         return null;
      }

      /// <summary>
      /// Gets the full name of the output directory
      /// </summary>
      /// <param name="project"></param>
      /// <returns>The output for the build files on success. Otherwise string.Empty is returned</returns>
      static public string GetProjectOutputDirectory(Project project)
      {
         //Build ... Output path
    
         EnvDTE.Configuration activeConfig = project.ConfigurationManager.ActiveConfiguration;

         string outputpath = activeConfig.Properties.Item("OutputPath").Value.ToString();

         return outputpath;
      }

      static public string GetProjectOutputType(Project project)
      {
         foreach (OutputGroup outputGroup in project.ConfigurationManager.ActiveConfiguration.OutputGroups)
         {
            try
            {
               if(((string[])outputGroup.FileNames).Length > 0)
                  return (((string[])outputGroup.FileNames)[0]);
            }
            catch (Exception) { }
         }

         return string.Empty;
      }

      static public ProjectItem GetProjectConfigurationFile(Project project)
      {
         foreach (ProjectItem projectItem in project.ProjectItems)
         {
            if (projectItem.FileCodeModel == null && projectItem.Name.Equals("App.config", StringComparison.InvariantCultureIgnoreCase))
               return projectItem;
         }

         return null;
      }

      static public string GetFilePath(string fileFullName)
      {
         int lastIndexOfSlash = fileFullName.LastIndexOf("\\");

         if (lastIndexOfSlash >= 0)
         {
            return fileFullName.Substring(0, lastIndexOfSlash);
         }

         return string.Empty;
      }

      static public string GetProjectPath(Project project)
      {
         int lastIndexOfSlash = project.FullName.LastIndexOf("\\");

         if (lastIndexOfSlash >= 0)
         {
            return project.FullName.Substring(0, lastIndexOfSlash);
         }

         return string.Empty;
      }

      static public string GetProjectNamespaceName(Project project)
      {
         EnvDTE.Configuration activeConfig = project.ConfigurationManager.ActiveConfiguration;

         string defaultNamespace = project.Properties.Item("DefaultNamespace").Value.ToString();

         return defaultNamespace;
      }

      static public string GetProjectOutputAssemblyName(Project project)
      {
         EnvDTE.Configuration activeConfig = project.ConfigurationManager.ActiveConfiguration;

         string outputFileName = project.Properties.Item("OutputFileName").Value.ToString();

         return outputFileName;
      }

      static public string GetProjectOutputAssemblyNameExtensionless(Project project)
      {
         EnvDTE.Configuration activeConfig = project.ConfigurationManager.ActiveConfiguration;

         string outputFileName = project.Properties.Item("OutputFileName").Value.ToString();

         int tvIndex = outputFileName.LastIndexOf('.');

         if (tvIndex > -1)
            return outputFileName.Substring(0, tvIndex);

         return outputFileName;
      }

      static public string GetProjectOutputFullAssemblyName(Project project)
      {
         string outputPath = GetProjectOutputDirectory(project);

         string outputFileName = GetProjectOutputAssemblyName(project);

         //return outputPath + "\\" + outputFileName;

         string tvProjectPath = GetProjectPath(project);

         if (outputPath[outputPath.Length - 1] != '\\' && outputFileName[0] != '\\')
            return tvProjectPath + "\\" + outputPath + "\\" + outputFileName;

         return tvProjectPath + "\\" + outputPath + outputFileName;
      }

      static public Project GetCodeClassParentProject(CodeClass codeClass)
      {
         return codeClass.ProjectItem.ContainingProject;
      }

      //FIX ME: This will be a pretty expensive operation when projects are large (which is in most cases)
      //Need to try and find a better way
      static public bool IsUnitTestProject(Project project, List<string>testAttributes)
      {
         foreach (ProjectItem projectItem in project.ProjectItems)
         {
            try
            {
               foreach (CodeElement tvCodeElementOutter in projectItem.FileCodeModel.CodeElements)
               {
                  if (tvCodeElementOutter is CodeClass)
                  {
                     try
                     {
                        foreach (CodeElement tvCodeFunction in ((CodeClass)tvCodeElementOutter).Members)
                        {
                           if (tvCodeFunction is CodeFunction)
                           {
                              foreach (CodeAttribute tvCodeAttribute in ((CodeFunction)tvCodeFunction).Attributes)
                              {
                                 foreach (string attribute in testAttributes)
                                 {
                                    if (tvCodeAttribute.Name.ToLower().Equals(attribute.ToLower()))
                                    {
                                       return true;
                                    }
                                 }
                              }
                           }
                        }
                     }
                     catch (Exception ex)
                     {
                        Logger.LogException(ex);
                        //ignore
                     }
                  }
                  else if (tvCodeElementOutter is CodeNamespace)
                  {
                     try
                     {
                        foreach (CodeElement tvCodeElement in ((CodeNamespace)tvCodeElementOutter).Members)
                        {
                           if (tvCodeElement != null && tvCodeElement is CodeClass)
                           {
                              try
                              {
                                 foreach (CodeElement tvCodeFunction in ((CodeClass)tvCodeElement).Members)
                                 {
                                    if (tvCodeFunction is CodeFunction)
                                    {
                                       foreach (CodeAttribute tvCodeAttribute in ((CodeFunction)tvCodeFunction).Attributes)
                                       {
                                          if (tvCodeAttribute.Name.ToLower().Equals("test") || 
                                             tvCodeAttribute.Name.ToLower().Equals("nunit.framework.test") ||
                                            tvCodeAttribute.Name.ToLower().Equals("teardown") ||
                                             tvCodeAttribute.Name.ToLower().Equals("nunit.framework.teardown")
                                             )
                                          {
                                             return true;
                                          }
                                       }
                                    }
                                 }
                              }
                              catch (Exception ex)
                              {
                                 //ignore
                                 Logger.LogException(ex);
                              }
                           }
                        }
                     }
                     catch (Exception ex)
                     {
                        //ignore
                        Logger.LogException(ex);
                     }
                  }
               }
            }
            catch (Exception ex)
            {
               //ignore
               Logger.LogException(ex);
            }
         }

         return false;
      }

      static public IVsHierarchy GetHierarchy(IServiceProvider serviceProvider, Project project)
      {
         var solution =

             serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;

         IVsHierarchy hierarchy;

         solution.GetProjectOfUniqueName(project.FullName, out hierarchy);

         return hierarchy;
      }
   }
}
