using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UTGHelper
{
   public partial class UserAlertBox : Form
   {
  
      public enum UserAlertType
      {
         exception,
         unitTestErrorMessage,
         regularMessage
      }

      static bool ivNeverShowThisAgainForThisSession = false;
      private string ivDetails;

      public UserAlertBox()
      {
         InitializeComponent();
      }

      public UserAlertBox(string message, UserAlertType userAlertType)
      {
         InitializeComponent();

         if (userAlertType == UserAlertType.unitTestErrorMessage)
         {
            chkbNeverShowAgain.Visible = true;
            chkbNeverShowAgain.Checked = ivNeverShowThisAgainForThisSession;
            this.Refresh();
         }
         else if (userAlertType == UserAlertType.exception)
         {
            this.Text = UTGHelper.CommonErrors.DEFAULT_EXCEPTION_TEXT_TITLE;
            this.txtContent.Text = UTGHelper.CommonErrors.DEFAULT_EXCEPTION_TEXT;
         }
         else if (userAlertType == UserAlertType.regularMessage)
         {
             this.Text = UTGHelper.CommonErrors.DEFAULT_MESSAGE_TEXT_TITLE;
             this.txtContent.Text = UTGHelper.CommonErrors.DEFAULT_MESSAGE_TEXT;
         }

         if (userAlertType == UserAlertType.unitTestErrorMessage && ivNeverShowThisAgainForThisSession)
         {
            this.Close();
            return;
         }

         this.txtContent.Text += message;
      }

      public UserAlertBox(string message, string details, UserAlertType userAlertType)
      {
         InitializeComponent();

         ivDetails = details;

         if (userAlertType == UserAlertType.unitTestErrorMessage)
         {
            chkbNeverShowAgain.Visible = true;
            chkbNeverShowAgain.Checked = ivNeverShowThisAgainForThisSession;
            this.lblReportThis.Visible = false;
            this.btnReportThis.Visible = false;
            this.Refresh();
         }
         else if (userAlertType == UserAlertType.exception)
         {
            this.Text = UTGHelper.CommonErrors.DEFAULT_EXCEPTION_TEXT_TITLE;
            this.txtContent.Text = UTGHelper.CommonErrors.DEFAULT_EXCEPTION_TEXT;
            this.lblReportThis.Visible = true;
            this.btnReportThis.Visible = true;
            this.Refresh();
         }
         else if (userAlertType == UserAlertType.regularMessage)
         {
            this.Text = UTGHelper.CommonErrors.DEFAULT_MESSAGE_TEXT_TITLE;
            this.txtContent.Text = UTGHelper.CommonErrors.DEFAULT_MESSAGE_TEXT;
            this.lblReportThis.Visible = false;
            this.btnReportThis.Visible = false;
            this.Refresh();
         }

         if (userAlertType == UserAlertType.unitTestErrorMessage && ivNeverShowThisAgainForThisSession)
         {
            this.Close();
            return;
         }

         this.txtContent.Text += message;
      }
      
      private void btnOK_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      private void chkbNeverShowAgain_CheckedChanged(object sender, EventArgs e)
      {
         ivNeverShowThisAgainForThisSession = chkbNeverShowAgain.Checked;
      }

      public void SendEmail(string message)
      {
         string currentDirectiory = Logger.GetFilePath (
            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", "")
            );

         try
         {
            //System.Windows.Forms.DialogResult tvDialogResults =
            //   System.Windows.Forms.MessageBox.Show(
            //   "Would you like to send error data to nMate support?",
            //   "Report Bug",
            //   System.Windows.Forms.MessageBoxButtons.YesNo,
            //   System.Windows.Forms.MessageBoxIcon.Question,
            //   System.Windows.Forms.MessageBoxDefaultButton.Button2,
            //   System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);

            //if (tvDialogResults == System.Windows.Forms.DialogResult.Yes)
            //{
            System.Net.Mail.MailMessage tvMailMessage = new System.Net.Mail.MailMessage(
               "sydevelopments@yahoo.com",
               "sydevelopments@yahoo.com");
            tvMailMessage.Subject = "User encountered an error";
            tvMailMessage.Body = message;
            tvMailMessage.Attachments.Add(new System.Net.Mail.Attachment(currentDirectiory + Logger.ivErrorFileShortName));
            //System.Net.Mail.SmtpClient tvClient = new System.Net.Mail.SmtpClient("samxp", 25);
            System.Net.Mail.SmtpClient tvClient = new System.Net.Mail.SmtpClient("nmate.dyndns.biz", 25);
            tvClient.Send(tvMailMessage);
            //}
         }
         catch (Exception ex)
         {
            IO.writeFileIfNotExistsAppending(currentDirectiory + Logger.ivErrorFileShortName, ex.Message + " " + ex.StackTrace);

            //ErrorHandler.ShowMessage(CommonErrors.ERR_UNABLE_TO_REPORT_ISSUE);
         }
         finally
         {
            this.btnReportThis.Enabled = false;
         }
      }

      private void btnReportThis_Click(object sender, EventArgs e)
      {
         SendEmail(ivDetails);
      }
   }
}
