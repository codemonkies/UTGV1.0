using System;
using System.Collections.Generic;
using System.Text;

namespace UTGeneratorLibrary.UnitTestProjectConfiguration
{
   public static class XMLTemplateFormatter
   {
      public static string FormateTemplate(string templateContent, string libraryPath)
      {
         if (templateContent.Equals(string.Empty))
            throw new Exception("Invalid parameter templateContent");

         if (libraryPath.Equals(string.Empty))
            throw new Exception("Invalid parameter nameSpace");

         //return string.Format(@templateContent, nameSpace, className, dateTime);
         templateContent = templateContent.Replace("{0}", libraryPath);
      
         return templateContent;
      }
   }
}
