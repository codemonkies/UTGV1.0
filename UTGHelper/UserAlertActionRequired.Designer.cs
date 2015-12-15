namespace UTGHelper
{
   partial class UserAlertActionRequired
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserAlertActionRequired));
         this.chkbNeverShowAgain = new System.Windows.Forms.CheckBox();
         this.txtContent = new System.Windows.Forms.TextBox();
         this.btnDismiss = new System.Windows.Forms.Button();
         this.btnOverwrite = new System.Windows.Forms.Button();
         this.btnMutate = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // chkbNeverShowAgain
         // 
         this.chkbNeverShowAgain.AutoSize = true;
         this.chkbNeverShowAgain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.chkbNeverShowAgain.Location = new System.Drawing.Point(12, 171);
         this.chkbNeverShowAgain.Name = "chkbNeverShowAgain";
         this.chkbNeverShowAgain.Size = new System.Drawing.Size(226, 19);
         this.chkbNeverShowAgain.TabIndex = 4;
         this.chkbNeverShowAgain.Text = "Take this approch for rest of session";
         this.chkbNeverShowAgain.UseVisualStyleBackColor = true;
         this.chkbNeverShowAgain.CheckedChanged += new System.EventHandler(this.chkbNeverShowAgain_CheckedChanged);
         // 
         // txtContent
         // 
         this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtContent.BackColor = System.Drawing.SystemColors.Control;
         this.txtContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.txtContent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.txtContent.Location = new System.Drawing.Point(12, 13);
         this.txtContent.Margin = new System.Windows.Forms.Padding(20, 22, 20, 22);
         this.txtContent.Multiline = true;
         this.txtContent.Name = "txtContent";
         this.txtContent.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtContent.Size = new System.Drawing.Size(290, 148);
         this.txtContent.TabIndex = 5;
         // 
         // btnDismiss
         // 
         this.btnDismiss.Location = new System.Drawing.Point(205, 196);
         this.btnDismiss.Name = "btnDismiss";
         this.btnDismiss.Size = new System.Drawing.Size(75, 25);
         this.btnDismiss.TabIndex = 6;
         this.btnDismiss.TabStop = false;
         this.btnDismiss.Text = "Dismiss";
         this.btnDismiss.UseVisualStyleBackColor = true;
         this.btnDismiss.Click += new System.EventHandler(this.btnDismiss_Click);
         // 
         // btnOverwrite
         // 
         this.btnOverwrite.Enabled = false;
         this.btnOverwrite.Location = new System.Drawing.Point(12, 196);
         this.btnOverwrite.Name = "btnOverwrite";
         this.btnOverwrite.Size = new System.Drawing.Size(75, 25);
         this.btnOverwrite.TabIndex = 7;
         this.btnOverwrite.Text = "Overwrite";
         this.btnOverwrite.UseVisualStyleBackColor = true;
         this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
         // 
         // btnMutate
         // 
         this.btnMutate.Location = new System.Drawing.Point(110, 196);
         this.btnMutate.Name = "btnMutate";
         this.btnMutate.Size = new System.Drawing.Size(75, 25);
         this.btnMutate.TabIndex = 8;
         this.btnMutate.Text = "Mutate";
         this.btnMutate.UseVisualStyleBackColor = true;
         this.btnMutate.Click += new System.EventHandler(this.btnMutate_Click);
         // 
         // UserAlertActionRequired
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(314, 227);
         this.Controls.Add(this.btnMutate);
         this.Controls.Add(this.btnOverwrite);
         this.Controls.Add(this.btnDismiss);
         this.Controls.Add(this.chkbNeverShowAgain);
         this.Controls.Add(this.txtContent);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "UserAlertActionRequired";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Unit Test Generator Alert";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.CheckBox chkbNeverShowAgain;
      private System.Windows.Forms.TextBox txtContent;
      private System.Windows.Forms.Button btnDismiss;
      private System.Windows.Forms.Button btnOverwrite;
      private System.Windows.Forms.Button btnMutate;
   }
}