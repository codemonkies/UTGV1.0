using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.CommandBars;
using UTGHelper;
using Extensibility;
using EnvDTE;
using EnvDTE80;


namespace UTG
{
   public enum CommandControlType
   {
      CommandControlTypeCodeWindow = 0,
      CommandControlTypeProject,
      CommandControlTypeSolution
   }

   public static class CommandBarControlFactory
   {
      public static CommandBarControl CreateCommandBarControl(DTE2 application, AddIn addIn, int cmd, CommandControlType type, string name, string caption, string toolTip, bool beginGroup)
      {
         Command command = null;
         CommandBarControl control = null;
         CommandBar commandBar = null;
         CommandBars commandBars = null;

         if (application == null) {
            throw new ArgumentNullException("application argument can not be null");
         }

         if (application.Commands == null){
            throw new ArgumentNullException("application argument commands can not be null");
         }

         if (addIn == null) {
            throw new ArgumentException("addIn argument can not be null");
         }

         if (cmd <= 0){
            throw new ArgumentException("cmd can not be less or equal to zero");
         }

         if (name.Length == 0){
            throw new ArgumentException("command name can not have zero length");
         }

         if (caption.Length == 0) {
            throw new ArgumentException("caption can not have zero length");
         }

         if (toolTip.Length == 0) {
            throw new ArgumentException("toolTip can not have zero length");
         }

         try {

            command = application.Commands.Item(addIn.ProgID + "." + name, 0);
         }
         catch (Exception ex){
            Logger.LogException(ex);
         }

         if (command != null){

            return null;
         }

         try {
            object[] obj = new object[] { null };

            command = application.Commands.AddNamedCommand(
                     addIn,
                     name,
                     name,
                     toolTip,
                     true,
                     cmd,
                     ref obj,
                     (int)(vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported));
          
            commandBars = application.CommandBars as CommandBars;

            if (commandBars == null) {
               Logger.LogMessage("Failed to retrieve commandBars. CommandBars not a CommandBars. CreateCommandBarControl returning null.");
               return null;
            }

            string commandBarName = "Code Window";

            switch(type) {
               case CommandControlType.CommandControlTypeCodeWindow:
                  commandBarName = "Code Window";
                  break;
               case CommandControlType.CommandControlTypeProject:
                  commandBarName = "Project";
                  break;
               case CommandControlType.CommandControlTypeSolution:
                  commandBarName = "Solution";
                  break;
               default:
                  Logger.LogMessage("Not supported command type. Defaulting to Code Window type.");
                  break;
            }

            commandBar = commandBars[commandBarName];

            if (commandBar == null)  {
               Logger.LogMessage("Failed to retrieve commandBar. CreateCommandBarControl returning null.");

               return null;
            }

            control = (CommandBarControl)command.AddControl(commandBar, commandBar.Controls.Count + 1);

            if (control == null) {

               Logger.LogMessage("Failed to add control. control is null. CreateCommandBarControl returning null.");

               return null;
            }

            control.Caption = caption;
            control.TooltipText = toolTip;
            control.BeginGroup = beginGroup;

            return control;
         }
         catch (Exception ex)
         {
            Logger.LogException(ex);
         }

         return null;
      }
   }
}