

using System;
using System.Collections.Generic;
using System.Text;

using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace UTGHelper
{
   public static class Tracer
   {
      public static void Trace(string message)
      {
         System.Diagnostics.Trace.Write(message);
      }

      public static void ToStatusBar(EnvDTE.DTE applicationObject, string message)
      {
         applicationObject.StatusBar.Text = message;
      }
   }
}
