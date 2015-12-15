
using System;
using System.Collections.Generic;
using System.Text;

using Extensibility;
using EnvDTE;
using EnvDTE80;

using UTGHelper;

namespace UTGManagerAndExaminor
{
   public static class CodeExaminor
   {
      static private bool CodeElementContainsPoint(CodeElement ce, TextPoint p)
      {
         return (p.AbsoluteCharOffset < ce.EndPoint.AbsoluteCharOffset) &&
         (p.AbsoluteCharOffset >= ce.StartPoint.AbsoluteCharOffset);
      }

      static public CodeElement FindInnerMostCodeElement(CodeElements elements, TextPoint p)
      {
         foreach (CodeElement ce in elements)
         {
            if (CodeElementContainsPoint(ce, p))
            {
               CodeElement ce2 = FindInnerMostCodeElement(ce.Children, p);

               if (ce2 == null) return ce;
               else return ce2;
            }
         }

         return null;
      }


      public static List<CodeClass> GetCodeClasses(CodeModel codeModel)
      {
         List<CodeClass> lstCodeClasses = new List<CodeClass>();

         foreach (CodeElement tvCodeElementOutter in codeModel.CodeElements)
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
         }
       
         return lstCodeClasses;
      }
   }

}
