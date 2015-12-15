namespace UTGHelper
{
   partial class UserAlertBox
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserAlertBox));
         this.txtContent = new System.Windows.Forms.TextBox();
         this.btnOK = new System.Windows.Forms.Button();
         this.chkbNeverShowAgain = new System.Windows.Forms.CheckBox();
         this.btnReportThis = new System.Windows.Forms.Button();
         this.lblReportThis = new System.Windows.Forms.Label();
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
         this.txtContent.Location = new System.Drawing.Point(12, 43);
         this.txtContent.Margin = new System.Windows.Forms.Padding(20, 22, 20, 22);
         this.txtContent.Multiline = true;
         this.txtContent.Name = "txtContent";
         this.txtContent.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtContent.Size = new System.Drawing.Size(290, 154);
         this.txtContent.TabIndex = 3;
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(244, 222);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(58, 25);
         this.btnOK.TabIndex = 1;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // chkbNeverShowAgain
         // 
         this.chkbNeverShowAgain.AutoSize = true;
         this.chkbNeverShowAgain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.chkbNeverShowAgain.Location = new System.Drawing.Point(12, 12);
         this.chkbNeverShowAgain.Name = "chkbNeverShowAgain";
         this.chkbNeverShowAgain.Size = new System.Drawing.Size(215, 19);
         this.chkbNeverShowAgain.TabIndex = 0;
         this.chkbNeverShowAgain.Text = "Don\'t bother me for rest of session";
         this.chkbNeverShowAgain.UseVisualStyleBackColor = true;
         this.chkbNeverShowAgain.Visible = false;
         this.chkbNeverShowAgain.CheckedChanged += new System.EventHandler(this.chkbNeverShowAgain_CheckedChanged);
         // 
         // btnReportThis
         // 
         this.btnReportThis.Location = new System.Drawing.Point(12, 222);
         this.btnReportThis.Name = "btnReportThis";
         this.btnReportThis.Size = new System.Drawing.Size(93, 25);
         this.btnReportThis.TabIndex = 4;
         this.btnReportThis.Text = "Report This";
         this.btnReportThis.UseVisualStyleBackColor = true;
         this.btnReportThis.Visible = false;
         this.btnReportThis.Click += new System.EventHandler(this.btnReportThis_Click);
         // 
         // lblReportThis
         // 
         this.lblReportThis.AutoSize = true;
         this.lblReportThis.Location = new System.Drawing.Point(11, 205);
         this.lblReportThis.Name = "lblReportThis";
         this.lblReportThis.Size = new System.Drawing.Size(291, 14);
         this.lblReportThis.TabIndex = 5;
         this.lblReportThis.Text = "No personnel information will be collected when you report";
         this.lblReportThis.Visible = false;
         // 
         // UserAlertBox
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(314, 255);
         this.Controls.Add(this.lblReportThis);
         this.Controls.Add(this.btnReportThis);
         this.Controls.Add(this.chkbNeverShowAgain);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.txtContent);
         this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "UserAlertBox";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Unit Test Generator Alert";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox txtContent;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.CheckBox chkbNeverShowAgain;
      private System.Windows.Forms.Button btnReportThis;
      private System.Windows.Forms.Label lblReportThis;
   }
}