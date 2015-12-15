using System;
using System.Collections.Generic;
using System.Text;

using Extensibility;
using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Shell.Interop;

namespace UTGTesting
{
   public class ErrorTaskApplicationObject
   {
      Microsoft.VisualStudio.Shell.ErrorTask theErrorTask;

      public Microsoft.VisualStudio.Shell.ErrorTask TheErrorTask
      {
         get { return theErrorTask; }
         set { theErrorTask = value; }
      }

      DTE2 applicationObject;

      public DTE2 ApplicationObject
      {
         get { return applicationObject; }
         set { applicationObject = value; }
      }
   }

   public struct UnitTestError
   {
      ProjectItem projectItem;

      public ProjectItem UnitTestProjectItem
      {
         get { return projectItem; }
         set { 
            projectItem = value;
         }
      }

      Project project;

      public Project UnitTestProject
      {
         get { return project; }
         set { 
            project = value;
            if(value != null)
               projectName = ((Project)value).Name;
         }
      }

      ErrorTaskApplicationObject theErrorTask;

      public ErrorTaskApplicationObject TheErrorTask
      {
         get { return theErrorTask; }
         set { theErrorTask = value; }
      }

      string name;

      public string Name
      {
         get { return name; }
         set { name = value; }
      }
      string text;

      public string Text
      {
         get { return text; }
         set { text = value; }
      }
      int line;

      public int Line
      {
         get { return line; }
         set { line = value; }
      }
      string file;

      public string File
      {
         get { return file; }
         set { file = value; }
      }

      string projectName;

      public string ProjectName
      {
         get { return projectName; }
         set { projectName = value; }
      }

      string asserts;

      public string Asserts
      {
         get { return asserts; }
         set { asserts = value; }
      }
   }
}
