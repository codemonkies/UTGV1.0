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
   public class NUnitCSharpTestClass : AbstractTestClass
   {
      //Will change this to be part of the template file
      protected new const string extension = @".cs";

      //Will change this to be template file based
      protected new const string template = @"
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace {0}.UnitTests
{
   [NUnit.Framework.TestFixture()]
   public partial class {1}NUnit
   {
     private {1} iv{1}Type;

      [NUnit.Framework.SetUp()]
      public void Init()
      {
         //
         // TODO: Code to run at the start of every test case
         //

         iv{1}Type = new {1}();
      }


      [NUnit.Framework.TearDown()]
      public void Clean()
      {
         //
         // TODO: Code that will be called after each Test case
         //

         iv{1}Type = null;
      }
   }
}
";

      public NUnitCSharpTestClass()
      {
         this.className = string.Empty;
         this.classNamespace = string.Empty;
         this.FullName = string.Empty;
      }

      public NUnitCSharpTestClass(string classNamespace, string className, bool classIsAbstract)
      {
         this.className = className;
         this.classNamespace = classNamespace;
         this.isAbsract = classIsAbstract;
      }

      protected override string format()
      {
         if (this.className == null)
            throw new ArgumentNullException("class name must be initialized before formatting template");

         if (this.classNamespace == null)
            throw new ArgumentNullException("class namespace must be initialized before formatting template");

         if (this.className == string.Empty)
            throw new InvalidOperationException("class name can not be empty");

         if (this.classNamespace == string.Empty)
            throw new InvalidOperationException("class namespace can not be empty");

         StringBuilder templateBuilder = new StringBuilder(template);

         templateBuilder.Replace("{0}", this.classNamespace);
         templateBuilder.Replace("{1}", this.className);

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

         functionKind = vsCMFunction.vsCMFunctionFunction;
         functionType = vsCMTypeRef.vsCMTypeRefVoid;

         /*
          functionKind = vsCMFunction.vsCMFunctionSub;
               functionType = vsCMTypeRef.vsCMTypeRefVoid;
          */

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

         try{
            unitTestCodeFunction.Comment = originalClassCodeFuntion.Comment;
            unitTestCodeFunction.DocComment = originalClassCodeFuntion.DocComment;
         }
         catch (Exception ex){
            //ignore, for some reason the doc throws in vb
            Logger.LogException(ex);
         }

         string tvFunctionCallTemplate = string.Empty; //"iv{0}Type.{1}(";
         string tvFunctionReturnTemplate = string.Empty; //"{0} iv{1}Return = ";

         tvFunctionCallTemplate = "iv{0}Type.{1}(";

         if (tvIsStatic){
            tvFunctionCallTemplate = "{0}.{1}(";
         }
         
         tvFunctionReturnTemplate = "{0} iv{1}Return = ";
         //tvFunctionReturnTemplate = "Dim iv{1}Return As {0} = ";

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

               tvTempParameterList += parameterTypeAsString + " " + parameterName + " = default(" + parameterTypeAsString + ");" + Environment.NewLine + StringHelper.GetTabString();
               //tvTempParameterList += "Dim " + parameterName + " As " + parameterTypeAsString + " = New " + parameterTypeAsString + "()" + Environment.NewLine + StringHelper.GetTabString();

               if (tvParameterKind == vsCMParameterKind.vsCMParameterKindRef)
               {
                  tvFunctionCall += "ref " + parameterName;
                  // tvFunctionCall += parameterName;
               }
               else if (tvParameterKind == vsCMParameterKind.vsCMParameterKindOut)
               {
                  tvFunctionCall += "out " + parameterName;
                  // tvFunctionCall += parameterName
               }
               else if (tvParameterKind == vsCMParameterKind.vsCMParameterKindIn)
               {
                  tvFunctionCall += "in " + parameterName;
                  //tvFunctionCall += parameterName;
               }
               else
               {
                  tvFunctionCall += parameterName;
               }
            }

            tvFunctionCall = string.Format(tvFunctionCall + ");" + Environment.NewLine, ((CodeClass)originalClassCodeFuntion.Parent).Name, originalClassCodeFuntion.Name);
            //tvFunctionCall = string.Format(tvFunctionCall + ")" + Environment.NewLine, ((CodeClass)originalClassCodeFuntion.Parent).Name, originalClassCodeFuntion.Name);
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
            boydEditPoint.Insert("\t\t\t// TODO: Update variable/s' defaults to meet test needs" + Environment.NewLine);
            //boydEditPoint.Insert("\t\t\t' TODO: Update variable/s' defaults to meet test needs" + Environment.NewLine);

          
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
         boydEditPoint.Insert("\t\t\t//TODO: Update Assert to meet test needs" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t//Assert.AreEqual( , );" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\tthrow new Exception(\"Not Implemented\");" + Environment.NewLine);

         return unitTestCodeFunction;
      }

      public override CodeFunction GenerateTest(CodeClass unitTestCodeClass, CodeProperty originalClassCodeProperty)
      {
         vsCMFunction functionKind = vsCMFunction.vsCMFunctionFunction;
         object functionType = null;

         functionKind = vsCMFunction.vsCMFunctionFunction;
         functionType = vsCMTypeRef.vsCMTypeRefVoid;

         //functionKind = vsCMFunction.vsCMFunctionSub;
         //functionType = vsCMTypeRef.vsCMTypeRefVoid;
         
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

         try{
            unitTestCodeFunction.Comment = originalClassCodeProperty.Comment;
            unitTestCodeFunction.DocComment = originalClassCodeProperty.DocComment;
         }
         catch (Exception ex){
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

            boydEditPoint.Insert(StringHelper.GetTabString() + "// Property is not read-write please add your own code here." + Environment.NewLine);
            //boydEditPoint.Insert(StringHelper.GetTabString() + "' Property is not read-write please add your own code here." + Environment.NewLine);

            return unitTestCodeFunction;
         }

         string tvFunctionCallTemplate = string.Empty; // "iv{0}Type.{1} = default({2});";

         tvFunctionCallTemplate = "iv{0}Type.{1} = default({2});" + Environment.NewLine;
         // tvFunctionCallTemplate = "iv{0}Type.{1} = New {2}()" + Environment.NewLine;

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
         tvTempString = "iv{0}Type.{1}, default({2})";
         // tvTempString = "iv{0}Type.{1}, New {2}()";

         tvTempString = string.Format(tvTempString, ((CodeClass)originalClassCodeProperty.Parent).Name, originalClassCodeProperty.Name, tvPropertyTypeAsString);

         boydEditPoint.Insert(Environment.NewLine);
         boydEditPoint.Insert("\t\t\t// TODO: Update Assert to meet test needs" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t//Assert.AreEqual(" + tvTempString + ");" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\t" + Environment.NewLine);
         boydEditPoint.Insert("\t\t\tthrow new Exception(\"Not Implemented\");" + Environment.NewLine);

         return unitTestCodeFunction;
      }
   }
}
