namespace UTGTesting
{
   partial class BrowseForUnitLibraries
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowseForUnitLibraries));
         this.txtContent = new System.Windows.Forms.TextBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.BrowserForNUnitBinariesDialog = new System.Windows.Forms.FolderBrowserDialog();
         this.SuspendLayout();
         // 
         // txtContent
         // 
         this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtContent.BackColor = System.Drawing.SystemColors.Control;
         this.txtContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.txtContent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.txtContent.Location = new System.Drawing.Point(14, 13);
         this.txtContent.Margin = new System.Windows.Forms.Padding(20, 22, 20, 22);
         this.txtContent.Multiline = true;
         this.txtContent.Name = "txtContent";
         this.txtContent.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtContent.Size = new System.Drawing.Size(263, 105);
         this.txtContent.TabIndex = 6;
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(105, 126);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 5;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // BrowseForNUnitLibraries
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.Control;
         this.ClientSize = new System.Drawing.Size(292, 156);
         this.Controls.Add(this.txtContent);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "BrowseForNUnitLibraries";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Browse For NUnit Libraries";
         this.Load += new System.EventHandler(this.BrowseForNUnitLibraries_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox txtContent;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.FolderBrowserDialog BrowserForNUnitBinariesDialog;
   }
}