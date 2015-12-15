using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace UTGHelper
{
   public partial class UserAlertActionRequired : Form
   {
      public enum ApplicableAcction
      {
         ignore,
         mutate,
         overwrite
      }

      //static string DEFAULT_MESSAGE_TEXT_TITLE = UTGHelper.CommonStrings.APPLICATION_NAME +" Message";
      //static string DEFAULT_MESSAGE_TEXT = "";

      static Dictionary<Type, ApplicableAcction> lstApplicableTypes = new Dictionary<Type, ApplicableAcction>();

      private Type ivType;

      private ApplicableAcction ivApplicableAcction;

      private CodeElement ivCodeElement;

      public CodeElement FormCodeElement
      {
         get { return ivCodeElement; }
         set { ivCodeElement = value; }
      }

      private UnitTestCodeType ivFromCodeType;

      public object TestFramework
      {
         get;
         set;
      }

      public UnitTestCodeType FromCodeType
      {
         get { return ivFromCodeType; }
         set { ivFromCodeType = value; }
      }

      private DTE2 tvApplicationObject;

      public DTE2 FormApplicationObject
      {
         get { return tvApplicationObject; }
         set { tvApplicationObject = value; }
      }

      public ApplicableAcction ApplicableAcctionState
      {
         get { return ivApplicableAcction; }
         set { ivApplicableAcction = value; }
      }

      public UserAlertActionRequired(string message, Type codeElementType, CodeElement ce, ref DTE2 applicationObject, UnitTestCodeType codeType, object testFramework)
      {
         InitializeComponent();

         ivType = codeElementType;

         FormCodeElement = ce;

         FromCodeType = codeType;

         FormApplicationObject = applicationObject;

         TestFramework = testFramework;

         Dictionary<Type, ApplicableAcction>.KeyCollection.Enumerator it =
            lstApplicableTypes.Keys.GetEnumerator();

         while (it.MoveNext())
         {
            Type itemType = it.Current;

            if (itemType == codeElementType)
            {
               ApplicableAcction tvApplicableAcction;

               lstApplicableTypes.TryGetValue(itemType, out tvApplicableAcction);

               if (tvApplicableAcction == ApplicableAcction.ignore)
               {
                  ApplicableAcctionState = ApplicableAcction.ignore;
               }
               else if (tvApplicableAcction == ApplicableAcction.mutate)
               {
                  ApplicableAcctionState = ApplicableAcction.mutate;
               }
               else if (tvApplicableAcction == ApplicableAcction.overwrite)
               {
                  ApplicableAcctionState = ApplicableAcction.overwrite;
               }

               this.Close();
               return;
            }
         }

         this.txtContent.Text += message;
      }

      private bool applicableTypeExits(Type codeElementType)
      {
        Dictionary<Type, ApplicableAcction>.KeyCollection.Enumerator it =
            lstApplicableTypes.Keys.GetEnumerator();

        while (it.MoveNext())
        {
           Type itemType = it.Current;

           if (itemType == codeElementType)
           {
              return true;
           }
        }

         return false;
      }

      private void chkbNeverShowAgain_CheckedChanged(object sender, EventArgs e)
      {
         if (!applicableTypeExits(ivType))
         {
            lstApplicableTypes.Add(ivType, ApplicableAcctionState);
         }
      }

      private void btnDismiss_Click(object sender, EventArgs e)
      {
         ApplicableAcctionState = ApplicableAcction.ignore;
         this.Close();
      }

      private void btnMutate_Click(object sender, EventArgs e)
      {
         ApplicableAcctionState = ApplicableAcction.mutate;
         this.Close();
      }

      private void btnOverwrite_Click(object sender, EventArgs e)
      {
         ApplicableAcctionState = ApplicableAcction.overwrite;
         this.Close();
      }
   }
}
