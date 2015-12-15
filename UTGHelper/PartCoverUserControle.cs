using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UTGHelper
{
   public partial class PartCoverUserControle : UserControl
   {
      public PartCoverUserControle()
      {
         InitializeComponent();
      }

      public void AddView(TreeView treeView)
      {
        treeView.Dock = System.Windows.Forms.DockStyle.Fill;
        treeView.Location = new System.Drawing.Point(0, 0);
        treeView.Name = "treeView1";
        treeView.Size = new System.Drawing.Size(293, 283);
        treeView.TabIndex = 0;
        // 
        // PartCoverUserControle
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(treeView);
        this.Name = "PartCoverUserControle";
        this.Size = new System.Drawing.Size(293, 283);
        this.ResumeLayout(false);
      }
   }
}
