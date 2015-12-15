using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UTGCoverage
{
   public partial class BrowseForCoverLibraries : Form
   {
      private string location = string.Empty;
      private AbstractCodeCoverageFramework coverageFramework;

      public string BinariesLocation
      {
         get { return location; }
         set { location = value; }
      }

      public BrowseForCoverLibraries(string message, AbstractCodeCoverageFramework coverageFramework)
      {
         InitializeComponent();
         txtContent.Text = message;
         this.coverageFramework = coverageFramework;
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         DialogResult result = BrowserForNUnitBinariesDialog.ShowDialog();

         if (result == DialogResult.OK)
         {
            location = BrowserForNUnitBinariesDialog.SelectedPath;

            this.coverageFramework.BinariesDirectory = location;
         }
         else
         {
            location = string.Empty;
         }

         this.Close();
      }
   }
}
