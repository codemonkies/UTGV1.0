using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace UTGTesting
{
   public partial class BrowseForUnitLibraries : Form
   {
      public enum BrowseForNunitDialogType
       {
          libraries,
          console
       }

      protected string location = string.Empty;
      protected BrowseForNunitDialogType browsingType;
      protected AbstractTestFramework testFramework;

      public BrowseForUnitLibraries(string message, BrowseForNunitDialogType browsingForType, AbstractTestFramework testFramework)
      {
         InitializeComponent();
         txtContent.Text = message;
         this.browsingType = browsingForType;
         this.testFramework = testFramework;
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         DialogResult result = BrowserForNUnitBinariesDialog.ShowDialog();

         if (result == DialogResult.OK)
         {
            location = BrowserForNUnitBinariesDialog.SelectedPath;
         }
         else
         {
            location = string.Empty;
         }

         if (browsingType == BrowseForNunitDialogType.libraries)
         {
            this.testFramework.BinariesDirectory = location;
         }
         else if (browsingType == BrowseForNunitDialogType.console)
         {
            this.testFramework.ConsoleDirectory = location;
         }

         this.Close();
      }

      private void BrowseForNUnitLibraries_Load(object sender, EventArgs e)
      {

      }
   }
}
