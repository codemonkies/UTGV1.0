using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UTGHelper
{
   /// This source is under the New BSD License
   /// Do not modify, distribute, or keep copies of this program for any reason unless you have read and understand the New BSD License.
   public partial class BrowseForPartCoverLibraries : Form
   {
      private string location = string.Empty;
      
      public static string SessionPrivatePartCoverValueHolder;

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
            location = BrowserForNUnitBinariesDialog.SelectedPath;
         else
            location = string.Empty;

         SessionPrivatePartCoverValueHolder = location;

         this.Close();
      }
   }
}
