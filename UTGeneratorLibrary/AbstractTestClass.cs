using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace UTGeneratorLibrary
{
   public abstract class AbstractTestClass
   {
      protected string classNamespace;
      protected string className;
      protected const string template = @"";
      protected const string extension = @"";
      private string fullName;
      protected bool isAbsract;

      //properties
      public string FullName
      {
         get { return fullName; }
         set { fullName = value; }
      }

      //abstracts
      protected abstract string format();
      public abstract CodeFunction GenerateTest(CodeClass unitTestCodeClass, CodeFunction originalClassCodeFuntion);
      public abstract CodeFunction GenerateTest(CodeClass unitTestCodeClass, CodeProperty originalClassCodeProperty);

      public static string GetNextAvailableCopyName(CodeClass codeClass, ref string codeFunctionName)
      {
         return GetNextAvailableCopyName(codeClass, ref codeFunctionName);
      }
      public static string GetNextAvailableCopyName(CodeClass codeClass, ref string codeFunctionName, Project project)
      {
         codeFunctionName = "CopyOf" + codeFunctionName;

         if (!CodeSelectionHandler.CanGenerateHandleCodeFunction(
           codeClass,
           codeFunctionName))
         {
            return GetNextAvailableCopyName(codeClass, ref codeFunctionName, project);
         }

         return codeFunctionName;
      }

      //virtual
      /// <summary>
      /// writes this file to disk
      /// </summary>
      /// <param name="directory">full path of directory where file should be written</param>
      /// <param name="postFix">postfix name for file, example NUnit, default is null</param>
      /// <returns>bool if successful at which point full name is set, false otherwise</returns>
      public virtual bool write(string directory, string postFix = null)
      {
         string content = format();

         string fullPath = string.Empty;

         if (postFix == null)
         {
            fullPath = System.IO.Path.Combine(directory, this.className + extension);
         }
         else
         {
            fullPath = System.IO.Path.Combine(directory, this.className + postFix + extension);
         }

         if (UTGHelper.IO.writeFileIfNotExists(fullPath, @content))
         {
            this.FullName = fullPath;

            return true;
         }

         return false;
      }

      /// <summary>
      /// Generates a Unit Test for a Class
      /// </summary>
      /// <param name="unitTestCodeClass">Unit Test Class's Code Function. A shell will suffice</param>
      /// <param name="codeFuntion">Class's Code Function</param>
      /// <returns>A string representation for the Class's Unit Test</returns>
      public virtual CodeClass GenerateTest(CodeClass unitTestCodeClass, CodeClass originalCodeClass)
      {
         string unitTestString = string.Empty;

         unitTestCodeClass.Comment = originalCodeClass.Comment;

         unitTestCodeClass.DocComment = originalCodeClass.DocComment;

         foreach (CodeElement ce in originalCodeClass.Members)
         {
            if (ce is CodeFunction && ((CodeFunction)ce).Access == vsCMAccess.vsCMAccessPublic)
            {
               CodeFunction newUnitTestCodeFunction = GenerateTest(unitTestCodeClass, (CodeFunction)ce);
            }
            else if (ce is CodeProperty && ((CodeProperty)ce).Access == vsCMAccess.vsCMAccessPublic)
            {
               CodeFunction newUnitTestCodeFunction = GenerateTest(unitTestCodeClass, (CodeProperty)ce);
            }
         }

         return unitTestCodeClass;
      }

      /// <summary>
      /// Generates a Unit Test for a Class
      /// </summary>
      /// <param name="unitTestCodeClass">Unit Test Class's Code Function. A shell will suffice</param>
      /// <param name="codeElement">Class's Code Function or Code Property</param>
      /// <param name="originalCodeClass">The class from which we have chosen our codeElement</param>
      /// <returns>A CodeClass representation the newly generated class</returns>
      public virtual CodeClass GenerateTest(CodeClass unitTestCodeClass, CodeClass originalCodeClass, CodeElement codeElement)
      {
         if (codeElement is CodeFunction || codeElement is CodeProperty)
         {
            string unitTestString = string.Empty;

            unitTestCodeClass.Comment = originalCodeClass.Comment;

            unitTestCodeClass.DocComment = originalCodeClass.DocComment;

            foreach (CodeElement ce in originalCodeClass.Members)
            {
               if (ce == codeElement)
               {
                  if (ce is CodeFunction && ((CodeFunction)ce).Access == vsCMAccess.vsCMAccessPublic)
                  {
                     GenerateTest(unitTestCodeClass, (CodeFunction)ce);
                  }
                  else if (ce is CodeProperty && ((CodeProperty)ce).Access == vsCMAccess.vsCMAccessPublic)
                  {
                     GenerateTest(unitTestCodeClass, (CodeProperty)ce);
                  }
               }
            }

            return unitTestCodeClass;
         }
         else
            throw new Exception(string.Format(UTGHelper.CommonErrors.ERR_TYPE_NOT_SUPPORTED, codeElement.GetType().ToString()));
      }
   }
}
