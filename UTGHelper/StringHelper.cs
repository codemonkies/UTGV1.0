using System;
using System.Collections.Generic;
using System.Text;

namespace UTGHelper
{
   public static class StringHelper
   {
      public static string GetTabString()
      {
         string tvVal = string.Empty;
         for (int i = 0; i < UserInviromentSettings.TAB; i++)
         {
            tvVal += "\t";
         }
         return tvVal;
      }
   }
}
