
using System;
using System.Text;
using Extensibility;
using EnvDTE;
using EnvDTE80;

using UTGHelper;

namespace UTGManagerAndExaminor
{
   public class CodeClassExaminor
   {
      private CodeClass ivCodeClass;

      protected CodeClass CodeClassElement
      {
         get { return ivCodeClass; }
         set { ivCodeClass = value; }
      }

      public CodeClassExaminor(CodeClass codeClass) {
         ivCodeClass = codeClass;
      }

      public bool HasFunctions()
      {
         if (ivCodeClass != null)
         {
            foreach (CodeElement ce in ivCodeClass.Members)
            {
               if (ce is CodeFunction || ce is CodeFunction2)
               {
                  return true;
               }
            }
         }

         return false;
      }

      public bool HasProperties()
      {
         if (ivCodeClass != null)
         {
            foreach (CodeElement ce in ivCodeClass.Members)
            {
               if (ce is CodeProperty || ce is CodeProperty2)
               {
                  return true;
               }
            }
         }

         return false;
      }

      public bool PropertiesHaveLogic()
      {
         if (HasProperties())
         {
            //Need to find a way to determine if a single property has logic other than 
            //usual set and get.
            return true;
         }

         return false;
      }
   }
}
