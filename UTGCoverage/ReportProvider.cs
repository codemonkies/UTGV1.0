using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace UTGCoverage
{
   public class ReportProvider
   {
      private static string ExtractProperty(MdSpecial msSpecial, string name)
      {
         switch (msSpecial)
         {
            case MdSpecial.Add:
            case MdSpecial.Get:
            case MdSpecial.Set:
               return name.Substring(4);
            case MdSpecial.Remove:
               return name.Substring(7);
            default:
               return name;
         }
      }

      private static MdSpecial DefineMdSpecial(string name)
      {
         if (name.StartsWith("set_")) return MdSpecial.Set;
         if (name.StartsWith("get_")) return MdSpecial.Get;
         if (name.StartsWith("add_")) return MdSpecial.Add;
         if (name.StartsWith("remove_")) return MdSpecial.Remove;
         return MdSpecial.Unknown;
      }

      enum MdSpecial
      {
         Unknown,
         Get,
         Set,
         Add,
         Remove
      }

      public static TreeView ShowReport(CoverageReport report, TreeView tvItems)
      {
         tvItems.BeginUpdate();

         if (report == null)
            return tvItems;

         foreach (string assemblyName in ReportHelper.GetAssemblies(report))
         {
            AssemblyNode asmNode = new AssemblyNode(assemblyName);

            tvItems.Nodes.Add(asmNode);

            foreach (CoverageReport.TypeDescriptor dType in ReportHelper.GetTypes(report, assemblyName))
            {
               TreeNode namespaceNode = ReportTreeHelper.GetNamespaceNode(asmNode, dType.typeName);
               ClassNode classNode = new ClassNode(dType);

               namespaceNode.Nodes.Add(classNode);

               Dictionary<string, PropertyTreeNode> props = new Dictionary<string, PropertyTreeNode>();

               foreach (CoverageReport.MethodDescriptor md in dType.methods)
               {
                  if ((md.flags & 0x0800) == 0)
                  {
                     ReportTreeHelper.AddMethodNode(classNode, md);
                     continue;
                  }

                  //has special meaning
                  MdSpecial mdSpecial = DefineMdSpecial(md.methodName);
                  string propName = ExtractProperty(mdSpecial, md.methodName);

                  if (mdSpecial == MdSpecial.Unknown)
                  {
                     ReportTreeHelper.AddMethodNode(classNode, md);
                     continue;
                  }

                  PropertyTreeNode propertyNode;
                  if (!props.TryGetValue(propName, out propertyNode))
                  {
                     propertyNode = new PropertyTreeNode(propName);
                     props[propName] = propertyNode;
                     classNode.Nodes.Add(propertyNode);
                  }

                  MethodNode mdNode = new MethodNode(md);
                  switch (mdSpecial)
                  {
                     case MdSpecial.Get:
                     case MdSpecial.Remove:
                        propertyNode.Getter = mdNode;
                        break;
                     case MdSpecial.Set:
                     case MdSpecial.Add:
                        propertyNode.Setter = mdNode;
                        break;
                  }
               }

               foreach (KeyValuePair<string, PropertyTreeNode> kv in props)
               {
                  if (kv.Value.Getter != null) kv.Value.Nodes.Add(kv.Value.Getter);
                  if (kv.Value.Setter != null) kv.Value.Nodes.Add(kv.Value.Setter);
               }
            }

            asmNode.UpdateCoverageInfo();
         }

         tvItems.EndUpdate();


         return tvItems;

      }

      public static void ReadReport(CoverageReport report, TextReader reader) 
      { 
         XmlDocument xmlDocument = new XmlDocument();
         xmlDocument.Load(reader);

         //Get Main Element PartCoverReport...
         XmlNode root = xmlDocument.SelectSingleNode("/PartCoverReport");
         if (root == null)
            throw new Exception("Bad xml format");

         //Look for Main Element's ver attribute...
         XmlAttribute verAttribute = root.Attributes["ver"];
         if (verAttribute == null)
            throw new Exception("Bad xml format");
         
         //Main Element will have a bunch of file Nodes...
         foreach (XmlNode fileNode in xmlDocument.SelectNodes("/PartCoverReport/file"))
         {
            ReportHelper.AddFile
               (
               report,
               ReportHelper.GetAttributeAsUInt32(fileNode, "id"),
               ReportHelper.GetAttributeAsString(fileNode, "url")
               );
         }

         foreach(XmlNode typeNode in xmlDocument.SelectNodes("/PartCoverReport/type")) 
         {
             CoverageReport.TypeDescriptor dType = new CoverageReport.TypeDescriptor();
             dType.assemblyName = ReportHelper.GetAttributeAsString(typeNode, "asm");
             dType.typeName = ReportHelper.GetAttributeAsString(typeNode, "name");
             dType.flags = ReportHelper.GetAttributeAsUInt32(typeNode, "flags");

             foreach(XmlNode methodNode in typeNode.SelectNodes("method")) 
             {
                 
                CoverageReport.MethodDescriptor dMethod = new CoverageReport.MethodDescriptor(0);
                 
                dMethod.methodName = ReportHelper.GetAttributeAsString(methodNode, "name");
                 
                dMethod.methodSig = ReportHelper.GetAttributeAsString(methodNode, "sig");
                 
                dMethod.flags = ReportHelper.GetAttributeAsUInt32(methodNode, "flags");
                 
                dMethod.implFlags = ReportHelper.GetAttributeAsUInt32(methodNode, "iflags");

                 
                foreach(XmlNode blockNode in methodNode.SelectNodes("code")) 
                {
                   CoverageReport.InnerBlockData dBlock = new CoverageReport.InnerBlockData();
                    
                   foreach(XmlNode pointNode in blockNode.SelectNodes("pt")) 
                   {
                        
                      CoverageReport.InnerBlock dPoint = new CoverageReport.InnerBlock();
                         
                      dPoint.visitCount = ReportHelper.GetAttributeAsUInt32(pointNode, "visit");
                         
                      dPoint.position = ReportHelper.GetAttributeAsUInt32(pointNode, "pos");
                         
                      dPoint.blockLen = ReportHelper.GetAttributeAsUInt32(pointNode, "len");
                         
                      if (pointNode.Attributes["fid"] != null) 
                      {
                         dPoint.fileId = ReportHelper.GetAttributeAsUInt32(pointNode, "fid");
                         dPoint.startLine = ReportHelper.GetAttributeAsUInt32(pointNode, "sl");
                         dPoint.startColumn = ReportHelper.GetAttributeAsUInt32(pointNode, "sc");
                         dPoint.endLine = ReportHelper.GetAttributeAsUInt32(pointNode, "el");
                         dPoint.endColumn = ReportHelper.GetAttributeAsUInt32(pointNode, "ec"); 
                      }
                         
                      ReportHelper.AddBlock(dBlock, dPoint);
                   }
                   
                   ReportHelper.AddBlockData(dMethod, dBlock);
                 
                }
                 
                ReportHelper.AddMethod(dType, dMethod);
             
             }
             
            ReportHelper.AddType(report, dType);
         }
     }
   }
}
