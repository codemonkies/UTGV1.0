using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Microsoft.Win32;
using System.Configuration;
using System.Windows.Forms;

namespace UTGHelper
{
   public static class InstalledSoftware
   {
      /// <summary>
      /// 
      /// </summary>
      /// <returns>The folder where NUnit has been installed. string.Empty otherwise</returns>
      public static string GetInstalledNUnitBinariesFolder()
      {
         string uninstallKey = @"Software\Microsoft\.NETFramework";

         using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(uninstallKey))
         {
            RegistryKey rkInner = rk.OpenSubKey("AssemblyFolders");

            if (rkInner == null)
                return string.Empty;

            foreach (string skName in rkInner.GetSubKeyNames())
            {
               using (RegistryKey sk = rkInner.OpenSubKey(skName))
               {
                   if (sk == null)
                       return string.Empty;

                  //sk.Name will have the version of nunit
                  if (skName.ToLower().Contains("nunit"))
                  {
                     foreach (string innerValues in sk.GetValueNames())
                     {
                        Console.WriteLine(innerValues);
                        string temp = (string)sk.GetValue(innerValues);
                        if(temp.ToLower().Contains("nunit") && temp.ToLower().Contains("bin"))
                           return temp;
                     } 
                  }
               }
            }
         }

         return string.Empty;
      }

      static public string GetFilePath(string fileFullName)
      {
         int lastIndexOfSlash = fileFullName.LastIndexOf("\\");

         if (lastIndexOfSlash >= 0)
         {
            return fileFullName.Substring(0, lastIndexOfSlash);
         }

         return string.Empty;
      }

      public static string GetPartCoverBinariesFolder()
      {
         //string uninstallKey = @"SOFTWARE\Classes";

         //using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
         //{
         //   RegistryKey rkInner = rk.OpenSubKey("CLSID");

         //   foreach (string skName in rkInner.GetSubKeyNames())
         //   {
         //      using (RegistryKey sk = rkInner.OpenSubKey(skName))
         //      {
         //         //sk.Name will have the version of nunit
         //         if (skName.Contains("{6F6225EA-0897-41fa-B1EF-8B4D3E15325E}"))
         //         {
         //            RegistryKey mostInnerKey = sk.OpenSubKey("InprocServer32");

         //            foreach (string innerValues in mostInnerKey.GetValueNames())
         //            {
         //               Console.WriteLine(innerValues);
         //               string temp = (string)mostInnerKey.GetValue(innerValues);
         //               if (temp.ToLower().Contains("partcover"))
         //                  return GetFilePath(temp);
         //            }
         //         }
         //      }
         //   }
         //}

         return string.Empty;
      }

      public static string GetInstalledVersion(string displayName)
      {
         string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

         using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
         {
            foreach (string skName in rk.GetSubKeyNames())
            {
               using (RegistryKey sk = rk.OpenSubKey(skName))
               {
                  if (sk.GetValue("DisplayName") != null && sk.GetValue("DisplayName").Equals(displayName))
                  {
                     string[] valueNames = sk.GetValueNames();

                     foreach (string valueName in valueNames)
                     {
                        if(valueName.Equals("DisplayVersion"))
                           return (string)sk.GetValue("DisplayVersion");
                     }
                  }   
               }
            }
         }

         return string.Empty;
      }

      /// <summary>
      /// Gets the real name and not the display name
      /// </summary>
      /// <param name="displayName">the display name</param>
      /// <returns></returns>
      public static string GetInstalledRealName(string displayName)
      {
         string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
         
         using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
         {
            foreach (string skName in rk.GetSubKeyNames())
            {
               using (RegistryKey sk = rk.OpenSubKey(skName))
               {
                  if (sk.GetValue("DisplayName") != null && sk.GetValue("DisplayName").Equals(displayName))
                  {
                     return sk.Name;
                  }
                  
               }
            }
         }

         return string.Empty;
      }
   }
}
