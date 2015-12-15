

using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Security.Cryptography;

namespace UTGHelper
{
   public static class IO
   {
      public enum traceLevel
      {
         none,
         debug,
         verbose
      }

      public enum PathType
      {
         memeory,
         diskFile
      }

      public static string readMemeory(string memory)
      {
         return (string)memory.Clone();
      }

      public static string readFile(string file)
      {
         string content = "";

         StringBuilder fileContentBuilder = new StringBuilder();

         System.IO.FileStream fs = null;

         try
         {
            fs = System.IO.File.OpenRead(file);

            byte[] bytesArray = new byte[1000];

            int bytesRead = bytesRead = fs.Read(bytesArray, 0, 1000);


            while (bytesRead > 0)
            {

               for (int i = 0; i < bytesRead; i++)
               {
                  fileContentBuilder.Append((char)bytesArray[i]);
               }

               bytesRead = fs.Read(bytesArray, 0, 1000);
            }

            content = fileContentBuilder.ToString();
         }
         catch (Exception ex)
         {
            ErrorHandler.HandleException(ex);
         }
         finally
         {
            if (fs != null)
               fs.Close();
         }

         return content;
      }

      public static bool writeFileIfNotExistsAppending(string file, string content)
      {
         try
         {
            System.IO.StreamWriter sw = new StreamWriter(file, true);

            sw.WriteLine(string.Format("Time {0}", System.DateTime.Now));
            sw.WriteLine(content);

            sw.Close();
         }
         catch (System.IO.IOException)
         {
            //do nothing
            //UTGHelper.ErrorHandler.HandleException(ex);

            return false;
         }
         catch (Exception ex)
         {
            UTGHelper.ErrorHandler.HandleException(ex);

            return false;
         }

         return true;
      }

      public static bool writeFileIfNotExists(string file, string content)
      {
         try
         {
            System.IO.StreamWriter sw = new StreamWriter(file, false);

            sw.WriteLine(content);

            sw.Close();
         }
         catch (System.IO.IOException)
         {
            //do nothing
            //UTGHelper.ErrorHandler.HandleException(ex);

            return false;
         }
         catch (Exception ex)
         {
            UTGHelper.ErrorHandler.HandleException(ex);

            return false;
         }

         return true;
      }

      public static bool CreateFile(string fileFullName)
      {
         System.IO.FileStream fs = null;

         try
         {
            fs = System.IO.File.Create(fileFullName, 1024);
         }
         catch (Exception ex)
         {
            UTGHelper.Logger.LogException(ex);

            return false;
         }
         finally
         {
            if (fs != null)
            {
               fs.Dispose();
            }
         }

         return true;
      }

      public static void consoleWriteLine(string message, traceLevel traceLevel)
      {

      }

      public static bool DirectoryExists(string directoryFullPath)
      {
         System.IO.DirectoryInfo drInfo = null;

         try
         {
            drInfo = new System.IO.DirectoryInfo(directoryFullPath);

            return drInfo.Exists;
         }

         catch (Exception ex)
         {
            UTGHelper.ErrorHandler.HandleException(ex);

            return false;
         }
      }

      public static bool DirectoryCreate(string directoryFullPath)
      {
         System.IO.DirectoryInfo drInfo = null;

         try
         {
            drInfo = System.IO.Directory.CreateDirectory(directoryFullPath);

            return drInfo.Exists;
         }
         catch (Exception ex)
         {
            UTGHelper.ErrorHandler.HandleException(ex);

            return false;
         }
      }

      /// <summary>
      /// New
      /// </summary>
      /// <param name="fileFullName"></param>
      /// <returns></returns>
      public static string GetFilePath(string fileFullName)
      {
         if (fileFullName.Equals(string.Empty))
            throw new Exception("Invalid Parameter fileFullName!");

         System.IO.DirectoryInfo directoryInfo = null;

         directoryInfo = new System.IO.DirectoryInfo(fileFullName);

         //This will include the solution name as well! There has to be a better call.. 
         //remove the solution name for time being
         int parentDirectory =
            directoryInfo.FullName.LastIndexOf('\\');

         string path = directoryInfo.FullName.Substring(0, parentDirectory);

         return path;
      }

      public static string GetUnitTestProjectConfigurationFileContent()
      {

         return @"
<NUnitProject>
  <Settings activeconfig=""Debug"" />
  <Config name=""Debug"" binpathtype=""Auto"">
    <assembly path=""{0}"" />
  </Config>
  <Config name=""Release"" binpathtype=""Auto"" />
</NUnitProject>
";

         //string fileContent = string.Empty;

         //try
         //{
         //   fileContent = InputOutput.readFile(UNIT_TEST_PROJECT_CONFIGURATION_XML_TEMPLATE_FILE);
         //   fileContent = fileContent.Trim();
         //   fileContent = fileContent.TrimStart();
         //   fileContent = fileContent.TrimEnd();
         //}
         //catch (Exception ex)
         //{
         //   ErrorHandler.HandleException(ex);

         //   throw;
         //}

         //return fileContent;
      }
   }
}
