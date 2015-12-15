using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UTG
{
   public partial class BrowseForPartCoverLibraries : Form
   {
      private string location = string.Empty;

      public string BinariesLocation
      {
         get { return location; }
         set { location = value; }
      }

      public BrowseForPartCoverLibraries(string message)
      {
         InitializeComponent();
         txtContent.Text = message;
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         DialogResult result = BrowserForNUnitBinariesDialog.ShowDialog();

         if (result == DialogResult.OK)
         {
             location = BrowserForNUnitBinariesDialog.SelectedPath;

             Settings.Default.UnitBinariesDirectory = location;

             Settings.Default.Save();
         }
         else
             location = string.Empty;

         this.Close();
      }
   }
}
