using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace UTGCoverage
{
   public class ReportHelper
   {
      public static void AddFile(CoverageReport report, UInt32 id, String url)
      {
         CoverageReport.FileDescriptor[] newFiles = new CoverageReport.FileDescriptor[report.files.Length + 1];
         report.files.CopyTo(newFiles, 1);

         newFiles[0] = new CoverageReport.FileDescriptor();
         newFiles[0].fileId = id;
         newFiles[0].fileUrl = url;

         report.files = newFiles;
      }

      private static CoverageReport.TypeDescriptor FindExistingType(CoverageReport report, CoverageReport.TypeDescriptor dType)
      {
         foreach (CoverageReport.TypeDescriptor td in report.types)
            if (td.assemblyName == dType.assemblyName && td.typeName == dType.typeName && td.flags == dType.flags)
               return td;
         return null;
      }

      private static CoverageReport.MethodDescriptor FindExistingMethod(CoverageReport.TypeDescriptor dType, CoverageReport.MethodDescriptor dMethod)
      {
         foreach (CoverageReport.MethodDescriptor md in dType.methods)
            if (md.methodName == dMethod.methodName && md.methodSig == dMethod.methodSig && md.flags == dMethod.flags && md.implFlags == dMethod.implFlags)
               return md;
         return null;
      }

      private static CoverageReport.InnerBlockData FindExistingBlockData(CoverageReport.InnerBlockData[] datas, CoverageReport.InnerBlockData bData)
      {
         foreach (CoverageReport.InnerBlockData dataBlock in datas)
         {
            if (dataBlock.blocks.Length != bData.blocks.Length)
               continue;
            bool validBlock = true;
            for (int i = 0; validBlock && i < dataBlock.blocks.Length; ++i)
            {
               CoverageReport.InnerBlock existingBlock = dataBlock.blocks[i];
               CoverageReport.InnerBlock newBlock = FindBlock(bData.blocks, existingBlock);
               validBlock = newBlock != null;
            }
            if (validBlock)
               return dataBlock;
         }
         return null;
      }

      private static CoverageReport.InnerBlock FindBlock(CoverageReport.InnerBlock[] blocks, CoverageReport.InnerBlock block)
      {
         foreach (CoverageReport.InnerBlock dataBlock in blocks)
         {
            if (dataBlock.position != block.position || dataBlock.blockLen != block.blockLen)
               continue;
            if (dataBlock.fileId == block.fileId &&
                dataBlock.startLine == block.startLine && dataBlock.startColumn == block.startColumn &&
                dataBlock.endLine == block.endLine && dataBlock.endColumn == block.endColumn)
               return dataBlock;
         }
         return null;
      }

      public static void AddType(CoverageReport report, CoverageReport.TypeDescriptor dType)
      {
         CoverageReport.TypeDescriptor existingType = FindExistingType(report, dType);
         if (existingType == null)
         {
            CoverageReport.TypeDescriptor[] newTypes = new CoverageReport.TypeDescriptor[report.types.Length + 1];
            report.types.CopyTo(newTypes, 1);
            newTypes[0] = dType;
            report.types = newTypes;
            return;
         }

         foreach (CoverageReport.MethodDescriptor md in dType.methods)
         {
            CoverageReport.MethodDescriptor existingMethod = FindExistingMethod(existingType, md);
            if (existingMethod == null)
            {
               CoverageReport.MethodDescriptor[] newMethods = new CoverageReport.MethodDescriptor[existingType.methods.Length + 1];
               existingType.methods.CopyTo(newMethods, 1);
               newMethods[0] = md;
               existingType.methods = newMethods;
               continue;
            }

            Debug.Assert(md.insBlocks.Length == 1);

            CoverageReport.InnerBlockData existingBlockData = FindExistingBlockData(existingMethod.insBlocks, md.insBlocks[0]);

            if (existingBlockData == null)
            {
               CoverageReport.InnerBlockData[] newBlocks = new CoverageReport.InnerBlockData[existingMethod.insBlocks.Length + 1];
               existingMethod.insBlocks.CopyTo(newBlocks, 1);
               newBlocks[0] = md.insBlocks[0];
               existingMethod.insBlocks = newBlocks;
            }
            else
            {
               for (int i = 0; i < existingBlockData.blocks.Length; ++i)
               {
                  CoverageReport.InnerBlock existingBlock = existingBlockData.blocks[i];
                  CoverageReport.InnerBlock newBlock = FindBlock(md.insBlocks[0].blocks, existingBlock);
                  Debug.Assert(newBlock != null);
                  existingBlock.visitCount += newBlock.visitCount;
               }
            }
         }
      }

      public static void AddMethod(CoverageReport.TypeDescriptor dType, CoverageReport.MethodDescriptor dMethod)
      {
         CoverageReport.MethodDescriptor[] newMethods = new CoverageReport.MethodDescriptor[dType.methods.Length + 1];
         dType.methods.CopyTo(newMethods, 1);

         newMethods[0] = dMethod;

         dType.methods = newMethods;
      }

      public static void AddMethodBlock(CoverageReport.MethodDescriptor dMethod, CoverageReport.InnerBlock inner)
      {
         CoverageReport.InnerBlockData bData = dMethod.insBlocks[0];

         CoverageReport.InnerBlock[] newBlocks = new CoverageReport.InnerBlock[bData.blocks.Length + 1];
         bData.blocks.CopyTo(newBlocks, 1);

         newBlocks[0] = inner;

         bData.blocks = newBlocks;
      }

      public static void AddBlock(CoverageReport.InnerBlockData bData, CoverageReport.InnerBlock inner)
      {
         CoverageReport.InnerBlock[] newBlocks = new CoverageReport.InnerBlock[bData.blocks.Length + 1];
         bData.blocks.CopyTo(newBlocks, 1);
         newBlocks[0] = inner;
         bData.blocks = newBlocks;
      }

      public static void AddBlockData(CoverageReport.MethodDescriptor dMethod, CoverageReport.InnerBlockData bData)
      {
         CoverageReport.InnerBlockData[] newBlocks = new CoverageReport.InnerBlockData[dMethod.insBlocks.Length + 1];
         dMethod.insBlocks.CopyTo(newBlocks, 1);
         newBlocks[0] = bData;
         dMethod.insBlocks = newBlocks;
      }

      public static string[] GetAssemblies(CoverageReport report)
      {
         SortedList list = new SortedList();
         foreach (CoverageReport.TypeDescriptor dType in report.types)
            list[dType.assemblyName] = true;
         string[] res = new string[list.Count];
         list.Keys.CopyTo(res, 0);
         return res;
      }

      public static ICollection GetTypes(CoverageReport report, string assembly)
      {
         ArrayList res = new ArrayList();
         foreach (CoverageReport.TypeDescriptor dType in report.types)
            if (dType.assemblyName == assembly)
               res.Add(dType);
         return res;
      }

      public static string[] SplitNamespaces(string typedefName)
      {
         return typedefName.Split('.');
      }

      public static string GetTypeDefName(string typedefName)
      {
         string[] names = SplitNamespaces(typedefName);
         return names[names.Length - 1];
      }

      public static string GetFileUrl(CoverageReport report, UInt32 fileId)
      {
         foreach (CoverageReport.FileDescriptor fd in report.files)
            if (fd.fileId == fileId) return fd.fileUrl;
         return null;
      }

      public static UInt32 GetBlockCodeSize(CoverageReport.InnerBlockData bData)
      {
         UInt32 codeSize = 0;
         foreach (CoverageReport.InnerBlock ib in bData.blocks)
            codeSize += ib.blockLen;
         return codeSize;
      }

      public static UInt32 GetBlockCoveredCodeSize(CoverageReport.InnerBlockData bData)
      {
         UInt32 codeSize = 0;
         foreach (CoverageReport.InnerBlock ib in bData.blocks)
            if (ib.visitCount > 0) codeSize += ib.blockLen;
         return codeSize;
      }

      internal static float GetBlockCoverage(CoverageReport.InnerBlock ib)
      {
         return ib.visitCount > 0 ? 100 : 0;
      }

      internal static UInt32 GetAttributeAsUInt32(XmlNode node, string attr)
      {
         string strAttr = GetAttributeAsString(node, attr);
         try
         {
            return UInt32.Parse(strAttr);
         }
         catch { throw new Exception("Wrong report format"); }
      }

      internal static string GetAttributeAsString(XmlNode node, string attr)
      {
         XmlAttribute attrNode = node.Attributes[attr];
         if (attrNode == null || attrNode.Value == null) throw new Exception("Wrong report format");
         return attrNode.Value;
      }
   }
}
