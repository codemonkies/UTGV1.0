

using System;
using System.Text;
using System.Collections.Generic;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using UTGHelper;

namespace UTGManagerAndExaminor
{
   public static class ProjectItemExaminor
   {
      public static List<CodeClass> GetCodeClasses(FileCodeModel fileCodeModel)
      {
         List<CodeClass> lstCodeClasses = new List<CodeClass>();

         if (fileCodeModel == null || fileCodeModel.CodeElements == null || fileCodeModel.CodeElements.Count == 0)
            return lstCodeClasses;

         try
         {
            foreach (CodeElement tvCodeElementOutter in fileCodeModel.CodeElements)
            {
               if (tvCodeElementOutter is CodeNamespace)
               {
                  try
                  {
                     foreach (CodeElement tvCodeElement in ((CodeNamespace)tvCodeElementOutter).Members)
                     {
                        if (tvCodeElement != null && tvCodeElement is CodeClass)
                        {
                           lstCodeClasses.Add((CodeClass)tvCodeElement);
                        }
                     }
                  }
                  catch (Exception ex)
                  {
                     //ignore
                     Logger.LogException(ex);
                  }
               }
               else if (tvCodeElementOutter is CodeClass)
               {
                  lstCodeClasses.Add((CodeClass)tvCodeElementOutter);
               }
            }
         }
         catch (Exception ex)
         {
            //ignore
            Logger.LogException(ex);
         }

         return lstCodeClasses;
      }

      
      public static string GetUnitTestClassOriginalClassName(string unitTestClassName, string unitTestClassPostFix)
      {
         return unitTestClassName.Replace(unitTestClassPostFix, "");
      }
      
      public static string GetUnitTestClassOriginalClassNamespace(string unitTestClassNamespace)
      {
         return unitTestClassNamespace.Replace(/*UTGHelper.CommonStrings.DEFAULT_UNIT_TEST_PROJECT_NAME +*/ ".UnitTests", "");
      }
   }

}
