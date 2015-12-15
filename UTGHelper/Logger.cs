using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UTGHelper
{
   public static class Logger
   {
      static public string ivErrorFileShortName = "\\UTGSupport.txt";

      public static string UPDATENOTIFYFILE = ".\\UpdateNotification.txt";

      public static void WriteToFile(string fileFullName, string content)
      {
         try
         {
            // create reader & open file
            TextWriter tr = new StreamWriter(fileFullName);

            tr.WriteLine(content);

            // close the stream
            tr.Close();

         }
         catch (Exception)
         {
         }
      }

      public static string ReadFromFile(string fileFullName)
      {
         try
         {
            // create reader & open file
            TextReader tr = new StreamReader(fileFullName);

            string content = tr.ReadToEnd();

            // close the stream
            tr.Close();

            return content;
         }
         catch (Exception)
         {
            throw;
         }
      }

      static public string GetFilePath(string fileFullName)
      {
         fileFullName = fileFullName.Replace("/", "\\");

         int lastIndexOfSlash = fileFullName.LastIndexOf("\\");

         if (lastIndexOfSlash >= 0)
         {
            return fileFullName.Substring(0, lastIndexOfSlash);
         }

         return string.Empty;
      }

      public static void LogMessage(string message)
      {
         try
         {
#if _ENCRYPT_
            message = UTGHelper.Helper.EncryptString(message, "support");
#endif

            string currentDirectiory = GetFilePath(
               System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", "")
               );

            IO.writeFileIfNotExistsAppending(currentDirectiory + ivErrorFileShortName, message);
         }
         catch (Exception) { }

      }

      /// <summary>
      /// Simply logs an exception silently. Use HandleException to alert user that an error has occurred
      /// </summary>
      /// <param name="ex"></param>
      public static void LogException(Exception ex)
      {
         try
         {
            string currentDirectiory = GetFilePath(
               System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", "")
               );

            string tvMessage = ex.Message;

#if _ENCRYPT_
            tvMessage = UTGHelper.Helper.EncryptString(tvMessage, "support");
#endif
            IO.writeFileIfNotExistsAppending(currentDirectiory + ivErrorFileShortName, tvMessage);

            tvMessage = ex.StackTrace;

#if _ENCRYPT_
            tvMessage = UTGHelper.Helper.EncryptString(tvMessage, "support");
#endif
            IO.writeFileIfNotExistsAppending(currentDirectiory + ivErrorFileShortName, tvMessage);

            //NEW
            //ErrorHandler.HandleException(ex);
         }
         catch (Exception) { }

      }
   }
}
