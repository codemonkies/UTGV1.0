
//#define _ENCRYPT_

using System;
using System.Collections.Generic;
using System.Text;

namespace UTGHelper
{
   public static class ErrorHandler
   {
      public static bool CanRun()
      {
         System.DateTime nowTime = System.DateTime.Now;
         System.DateTime then = new DateTime(2008, 05, 1);

         if (nowTime > then)
            return false;

         return true;
      }

      /// <summary>
      /// Alerts user of error condition. Gives user chance to report error. Logs exceptions.
      /// </summary>
      /// <param name="ex"></param>
      public static void HandleException(Exception ex)
      {
         ShowError(ex.Message, ex.Message);
      }

      private static void ShowError(string generalMessage, string details)
      {
         UserAlertBox txtUserAlert = new UserAlertBox(
                 generalMessage,
                 details,
                 UserAlertBox.UserAlertType.exception);

         txtUserAlert.Show();
      }

      /// <summary>
      /// Use to alert the user with a message. Will not log. Use HandleException on unexpected errors
      /// </summary>
      /// <param name="message"></param>
      /// <param name="emp"></param>
      public static void ShowMessage(string message)
      {
         UserAlertBox txtUserAlert = new UserAlertBox(
                 message,
                 UserAlertBox.UserAlertType.regularMessage);

         txtUserAlert.Show();
      }
   }
}
