using System;
using System.Collections.Generic;
using System.Text;

using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace UTGHelper
{
   public class TestFixturOption
   {
      private bool isDebugging = false;
      Project project = null;
      //List<object> codeClasses = new List<object>(); //error is generated when the template type of this list is an embeded interop type like CodeClass or CodeFunction and is crossing library domains. So i changed this to base object type for the timebeing until i find the root cause and a fix
      //List<object> codeFunctions = new List<object>();
      CodeClass cClass;
      CodeFunction cFunction;
      private string configurationFile = string.Empty;

      public string ConfigurationFile
      {
         get { return configurationFile; }
         set { configurationFile = value; }
      }

      public void Initialize()
      {
         isDebugging = false;

         project = null;
      }

      public bool IsDebugging
      {
         get { return isDebugging; }
         set { isDebugging = value; }
      }

      public Project Project
      {
         get { return project; }
         set { project = value; }
      }

      public CodeClass CClass
      {
         get { return cClass; }
         set { cClass = value; }
      }

      public CodeFunction CFunction
      {
         get { return cFunction; }
         set { cFunction = value; }
      }
   }
}
