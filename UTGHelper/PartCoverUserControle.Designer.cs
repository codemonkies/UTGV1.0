namespace UTGHelper
{
   partial class PartCoverUserControle
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         treeView1 = new System.Windows.Forms.TreeView();
         SuspendLayout();
         // 
         // treeView1
         // 
         treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
         treeView1.Location = new System.Drawing.Point(0, 0);
         treeView1.Name = "treeView1";
         treeView1.Size = new System.Drawing.Size(293, 283);
         treeView1.TabIndex = 0;
         // 
         // PartCoverUserControle
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(treeView1);
         this.Name = "PartCoverUserControle";
         this.Size = new System.Drawing.Size(293, 283);
         this.ResumeLayout(false);

      }

      #endregion

      public static System.Windows.Forms.TreeView treeView1;


   }
}
