using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UTGHelper;

using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace UTGeneratorLibrary
{
   public class NUnitVBTestClass : AbstractTestClass
   {
      //Will change this to be part of the template file
      protected new const string extension = @".vb";

      //Will change this to be template file based
      protected static new string template = @"
Imports nunit.Framework
Imports NUnit.Framework.SyntaxHelpers

<TestFixture()> _ 
Public Class {0}_NUnit
   Dim iv{0}Type As {0}

   <NUnit.Framework.SetUp()> _
   Sub Init()
      iv{0}Type = New {0}()
   End Sub

   <NUnit.Framework.TearDown()> _
   Sub Clean()
      iv{0}Type = Nothing
   End Sub
End Class
";

      public NUnitVBTestClass()
      {
         this.className = string.Empty;
         this.classNamespace = string.Empty;
         this.isAbsract = false;
      }

      public NUnitVBTestClass(string classNamespace, string className, bool classIsAbstract)
      {
         this.className = className;
         this.classNamespace = classNamespace;
         this.isAbsract = classIsAbstract;
      }

      protected override string format()
      {
         if (this.className == string.Empty)
            throw new InvalidOperationException("class name must be initialized before formatting template");

         if (this.classNamespace == string.Empty)
            throw new InvalidOperationException("class namespace must be initialized before formatting template");

         StringBuilder templateBuilder = new StringBuilder(template);

         templateBuilder.Replace("{0}", this.className);

         return templateBuilder.ToString();
      }

      /// <summary>
      /// writes this file to disk
      /// </summary>
      /// <param name="directory">full path of directory where file should be written</param>
      /// <param name="postFix">postfix name for file, example NUnit, default is null</param>
      /// <returns>bool if successful at which point full name is set, false otherwise</returns>
      public override bool write(string directory, string postFix = null)
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

      public override CodeFunction GenerateTest(CodeClass unitTestCodeClass, CodeFunction originalClassCodeFuntion)
      {
         vsCMFunction functionKind = vsCMFunction.vsCMFunctionFunction;
         object functionType = null;

         functionKind = vsCMFunction.vsCMFunctionSub;
         functionType = vsCMTypeRef.vsCMTypeRefVoid;

         string nextAvailableName = originalClassCodeFuntion.Name;

         if (!CodeSelectionHandler.CanGenerateHandleCodeFunction(unitTestCodeClass,
            nextAvailableName))
         {
            nextAvailableName = GetNextAvailableCopyName(unitTestCodeClass, ref nextAvailableName, unitTestCodeClass.ProjectItem.ContainingProject);
         }

         CodeFunction unitTestCodeFunction = unitTestCodeClass.AddFunction(
            nextAvailableName,
            functionKind,
            functionType,
            -1,
            originalClassCodeFuntion.Access,
            -1);

         bool tvIsStatic = originalClassCodeFuntion.IsShared;

         //add the NUnit attribute to the function
         unitTestCodeFunction.AddAttribute("NUnit.Framework.Test", "", -1);

         try
         {
            unitTestCodeFunction.Comment = originalClassCodeFuntion.Comment;
            unitTestCodeFunction.DocComment = originalClassCodeFuntion.DocComment;
         }
         catch (Exception ex)
         {
            //ignore, for some reason the doc throws in vb
            Logger.LogException(ex);
         }

         string tvFunctionCallTemplate = string.Empty; //"iv{0}Type.{1}(";
         string tvFunctionReturnTemplate = string.Empty; //"{0} iv{1}Return = ";

         tvFunctionCallTemplate = "iv{0}Type.{1}(";

         if (tvIsStatic)
         {
            tvFunctionCallTemplate = "{0}.{1}(";
         }

         tvFunctionReturnTemplate = "Dim iv{1}Return As {0} = ";

         string tvTempParameterList = string.Empty;
         string tvFunctionCall = tvFunctionCallTemplate;
         string tvFunctionReturn = tvFunctionReturnTemplate;


         if (!originalClassCodeFuntion.FunctionKind.ToString().Equals("vsCMFunctionConstructor"))
         {
            CodeElements tvParameters = originalClassCodeFuntion.Parameters;

            foreach (CodeElement tvCodeElement in tvParameters)
            {
               if (!tvFunctionCall.Equals(tvFunctionCallTemplate))
               {
                  tvFunctionCall += ", ";
               }

               CodeParameter2 tvCodeParameter = (CodeParameter2)tvCodeElement;

               string parameterName = tvCodeParameter.Name;

               CodeTypeRef tvParameterType = tvCodeParameter.Type;

               vsCMParameterKind tvParameterKind = tvCodeParameter.ParameterKind;

               string parameterTypeAsString = tvParameterType.AsString;

               tvTempParameterList += "Dim " + parameterName + " As " + parameterTypeAsString + " = New " + parameterTypeAsString + "()" + Environment.NewLine + StringHelper.GetTabString();

               if (tvParameterKind == vsCMParameterKind.vsCMParameterKindRef)
               {
                  tvFunctionCall += parameterName;
               }
               else if (tvParameterKind == vsCMParameterKind.vsCMParameterKindOut)
               {
                  tvFunctionCall += parameterName;
               }
               else if (tvParameterKind == vsCMParameterKind.vsCMParameterKindIn)
               {
                  tvFunctionCall += parameterName;
               }
               else
               {
                  tvFunctionCall += parameterName;
               }
            }

            tvFunctionCall = string.Format(tvFunctionCall + ")" + Environment.NewLine, ((CodeClass)originalClassCodeFuntion.Parent).Name, originalClassCodeFuntion.Name);
         }

         if (originalClassCodeFuntion.Type.TypeKind != vsCMTypeRef.vsCMTypeRefVoid)
         {
            tvFunctionReturn = string.Format(tvFunctionReturn, originalClassCodeFuntion.Type.AsString, originalClassCodeFuntion.Name);
            tvFunctionCall = tvFunctionReturn + tvFunctionCall;
         }

         TextPoint bodyStartingPoint =
            unitTestCodeFunction.GetStartPoint(vsCMPart.vsCMPartBody);

         EditPoint boydEditPoint = bodyStartingPoint.CreateEditPoint();

         if (!originalClassCodeFuntion.FunctionKind.ToString().Equals("vsCMFunctionConstructor"))
         {
            boydEditPoint.Insert("\t\t\t' TODO: Update variable/s' defaults to meet test needs" + Environment.NewLine);

            boydEditPoint.Insert(StringHelper.GetTabString() + tvTempParameterList + Environment.NewLine);
            boydEditPoint.Insert(StringHelper.GetTabString() + tvFunctionCall + Environment.NewLine);
         }

         if (originalClassCodeFuntion.Type.TypeKind != vsCMTypeRef.vsCMTypeRefVoid)
         {
            string stringHolder = "iv{0}Return";
            stringHolder = string.Format(stringHolder, originalClassCodeFuntion.Name);
            //FIX ME (tabing)
            //boydEditPoint.Insert(string.Format("\t\t\tAssert.AreEqual({0}, default({1}));\r\n", stringHolder, originalClassCodeFuntion.Type.AsString));
         }

          
         boydEditPoint.Insert(Environment.NewLine);
         boydEditPoint.Insert("\t\t\t'TODO: Update Assert to meet test needs" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t'Assert.AreEqual( , )" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\tThrow New Exception 'Not Implemented'" + Environment.NewLine);

         return unitTestCodeFunction;
      }

      public override CodeFunction GenerateTest(CodeClass unitTestCodeClass, CodeProperty originalClassCodeProperty)
      {
         vsCMFunction functionKind = vsCMFunction.vsCMFunctionFunction;
         object functionType = null;

         functionKind = vsCMFunction.vsCMFunctionSub;
         functionType = vsCMTypeRef.vsCMTypeRefVoid;

         string nextAvailableName = originalClassCodeProperty.Name;

         if (!CodeSelectionHandler.CanGenerateHandleCodeFunction(unitTestCodeClass,
            nextAvailableName))
         {
            nextAvailableName = GetNextAvailableCopyName(unitTestCodeClass, ref nextAvailableName, unitTestCodeClass.ProjectItem.ContainingProject);
         }

         CodeFunction unitTestCodeFunction = unitTestCodeClass.AddFunction(
            nextAvailableName,
            functionKind,
            functionType,
            -1,
            originalClassCodeProperty.Access,
            -1);

         unitTestCodeFunction.AddAttribute("NUnit.Framework.Test", "", -1);

         try
         {
            unitTestCodeFunction.Comment = originalClassCodeProperty.Comment;
            unitTestCodeFunction.DocComment = originalClassCodeProperty.DocComment;
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
            //ignore
         }

         TextPoint bodyStartingPoint =
               unitTestCodeFunction.GetStartPoint(vsCMPart.vsCMPartBody);

         EditPoint boydEditPoint = bodyStartingPoint.CreateEditPoint();

         //Stop here if not read-write type property now...

         if (originalClassCodeProperty.Setter == null)
         {
            boydEditPoint = bodyStartingPoint.CreateEditPoint();

            boydEditPoint.Insert(StringHelper.GetTabString() + "' Property is not read-write please add your own code here." + Environment.NewLine);

            return unitTestCodeFunction;
         }

         string tvFunctionCallTemplate = string.Empty; // "iv{0}Type.{1} = default({2});";

         tvFunctionCallTemplate = "iv{0}Type.{1} = New {2}()" + Environment.NewLine;

         string tvFunctionCall = tvFunctionCallTemplate;

         CodeTypeRef tvPropertyType = originalClassCodeProperty.Type;

         string tvPropertyTypeAsString = tvPropertyType.AsString;

         tvFunctionCall = string.Format(tvFunctionCall, ((CodeClass)originalClassCodeProperty.Parent).Name, originalClassCodeProperty.Name, tvPropertyTypeAsString);

         bodyStartingPoint = unitTestCodeFunction.GetStartPoint(vsCMPart.vsCMPartBody);

         boydEditPoint = bodyStartingPoint.CreateEditPoint();

         //FIX ME (tabing)
         boydEditPoint.Insert(StringHelper.GetTabString() + tvFunctionCall + Environment.NewLine);

         //FIX ME (tabbing)
         string tvTempString = string.Empty; // "iv{0}Type.{1}, default({2})";
         tvTempString = "iv{0}Type.{1}, New {2}()";

         tvTempString = string.Format(tvTempString, ((CodeClass)originalClassCodeProperty.Parent).Name, originalClassCodeProperty.Name, tvPropertyTypeAsString);

         boydEditPoint.Insert(Environment.NewLine);
         boydEditPoint.Insert("\t\t\t'TODO: Update Assert to meet test needs" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t'Assert.AreEqual(" + tvTempString + ")" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\tThrow New Exception 'Not Implemented'" + Environment.NewLine);
        
         return unitTestCodeFunction;
      }
   }
}
