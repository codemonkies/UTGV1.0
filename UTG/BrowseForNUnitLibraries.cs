using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace UTG
{
   public partial class BrowseForNUnitLibraries : Form
   {
      public enum BrowseForNunitDialogType
       {
          libraries,
          console
       }

      protected string location = string.Empty;
      protected BrowseForNunitDialogType browsingType;

      public string BinariesLocation
      {
         get { return location; }
         set { location = value; }
      }

      public BrowseForNUnitLibraries(string message, BrowseForNunitDialogType browsingFor)
      {
         InitializeComponent();
         txtContent.Text = message;
         browsingType = browsingFor;
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         DialogResult result = BrowserForNUnitBinariesDialog.ShowDialog();

         if (result == DialogResult.OK)
            location = BrowserForNUnitBinariesDialog.SelectedPath;
         else
            location = string.Empty;

         if (browsingType == BrowseForNunitDialogType.libraries)
         {
             Settings.Default.UnitBinariesDirectory = location;

             Settings.Default.Save();
         }
         else if (browsingType == BrowseForNunitDialogType.console)
         {
             Settings.Default.UnitConsoleDirectory = location;

             Settings.Default.Save();
         }

         this.Close();
      }

      private void BrowseForNUnitLibraries_Load(object sender, EventArgs e)
      {

      }
   }
}
