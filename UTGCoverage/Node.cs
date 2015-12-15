using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace UTGCoverage
{
   interface ICoverageInformation
   {
      UInt32 GetCodeSize();
      UInt32 GetCoveredCodeSize();
      void UpdateCoverageInfo();
   }

   public class NodeBase : TreeNode
   {
      public NodeBase(string name) : base(name) { }
   }

   public class AssemblyNode : NodeBase, ICoverageInformation
   {
      private string assemblyName;

      public AssemblyNode(string assemblyName)
         : base(assemblyName)
      {
         this.assemblyName = assemblyName;
      }

      #region ICoverageInformation Members
      private UInt32 codeSize = 0;
      private UInt32 coveredCodeSize = 0;

      public UInt32 GetCodeSize() { return codeSize; }
      public UInt32 GetCoveredCodeSize() { return coveredCodeSize; }

      public void UpdateCoverageInfo()
      {
         codeSize = 0;
         coveredCodeSize = 0;

         foreach (ICoverageInformation iInfo in Nodes)
         {
            iInfo.UpdateCoverageInfo();
            codeSize += iInfo.GetCodeSize();
            coveredCodeSize += iInfo.GetCoveredCodeSize();
         }
         float percent = codeSize == 0 ? 0 : coveredCodeSize / (float)codeSize * 100;
         this.Text = string.Format("{0} ({1:#0}%)", assemblyName, percent);
         this.ForeColor = ColorScheme.GetPercentColor(percent);
      }

      #endregion
   }

   public class NamespaceNode : NodeBase, ICoverageInformation
   {
      string namespaceName;
      public string Namespace
      {
         get { return namespaceName; }
      }

      public NamespaceNode(string namespaceName)
         : base(namespaceName)
      {
         this.namespaceName = namespaceName;
      }

      #region ICoverageInformation Members
      private UInt32 codeSize = 0;
      private UInt32 coveredCodeSize = 0;

      public UInt32 GetCodeSize() { return codeSize; }
      public UInt32 GetCoveredCodeSize() { return coveredCodeSize; }

      public void UpdateCoverageInfo()
      {
         codeSize = 0;
         coveredCodeSize = 0;

         foreach (ICoverageInformation iInfo in Nodes)
         {
            iInfo.UpdateCoverageInfo();
            codeSize += iInfo.GetCodeSize();
            coveredCodeSize += iInfo.GetCoveredCodeSize();
         }
         float percent = codeSize == 0 ? 0 : coveredCodeSize / (float)codeSize * 100;
         this.Text = string.Format("{0} ({1:#0}%)", Namespace, percent);
         this.ForeColor = ColorScheme.GetPercentColor(percent);
      }

      #endregion
   }

   public class ClassNode : NodeBase, ICoverageInformation
   {
      CoverageReport.TypeDescriptor dType;
      public CoverageReport.TypeDescriptor Descriptor
      {
         get { return dType; }
      }

      public ClassNode(CoverageReport.TypeDescriptor dType)
         : base(ReportHelper.GetTypeDefName(dType.typeName))
      {
         this.dType = dType;
      }

      #region ICoverageInformation Members
      private UInt32 codeSize = 0;
      private UInt32 coveredCodeSize = 0;

      public UInt32 GetCodeSize() { return codeSize; }
      public UInt32 GetCoveredCodeSize() { return coveredCodeSize; }

      public void UpdateCoverageInfo()
      {
         codeSize = 0;
         coveredCodeSize = 0;

         foreach (ICoverageInformation iInfo in Nodes)
         {
            iInfo.UpdateCoverageInfo();
            codeSize += iInfo.GetCodeSize();
            coveredCodeSize += iInfo.GetCoveredCodeSize();
         }
         float percent = codeSize == 0 ? 0 : coveredCodeSize / (float)codeSize * 100;
         this.Text = string.Format("{0} ({1:#0}%)", ReportHelper.GetTypeDefName(dType.typeName), percent);
         this.ForeColor = ColorScheme.GetPercentColor(percent);
      }

      #endregion
   }

   public class PropertyTreeNode : NodeBase, ICoverageInformation
   {
      string _property;
      MethodNode _setter;
      MethodNode _getter;

      public PropertyTreeNode(string text)
         : base(text)
      {
         _property = text;
      }

      public MethodNode Setter
      {
         get { return _setter; }
         set { _setter = value; }
      }

      public MethodNode Getter
      {
         get { return _getter; }
         set { _getter = value; }
      }

      public uint GetCodeSize()
      {
         uint size = 0;
         if (Setter != null) size += Setter.GetCodeSize();
         if (Getter != null) size += Getter.GetCodeSize();
         return size;
      }

      public uint GetCoveredCodeSize()
      {
         uint size = 0;
         if (Setter != null) size += Setter.GetCoveredCodeSize();
         if (Getter != null) size += Getter.GetCoveredCodeSize();
         return size;
      }

      public void UpdateCoverageInfo()
      {
         if (Setter != null) Setter.UpdateCoverageInfo();
         if (Getter != null) Getter.UpdateCoverageInfo();

         float percent = GetCodeSize() == 0 ? 0 : GetCoveredCodeSize() / (float)GetCodeSize() * 100;

         Text = string.Format("{0} ({1:#0}%)", _property, percent);
         ForeColor = ColorScheme.GetPercentColor(percent);
      }
   }

   public class MethodNode : NodeBase, ICoverageInformation
   {
      CoverageReport.MethodDescriptor md;
      public CoverageReport.MethodDescriptor Descriptor
      {
         get { return md; }
      }

      public MethodNode(CoverageReport.MethodDescriptor md)
         : base(md.methodName)
      {
         this.md = md;
      }

      #region ICoverageInformation Members
      private UInt32 codeSize = 0;
      private UInt32 coveredCodeSize = 0;

      public UInt32 GetCodeSize() { return codeSize; }
      public UInt32 GetCoveredCodeSize() { return coveredCodeSize; }

      public void UpdateCoverageInfo()
      {
         foreach (ICoverageInformation iInfo in Nodes)
            iInfo.UpdateCoverageInfo();

         codeSize = 0;
         coveredCodeSize = 0;
         foreach (CoverageReport.InnerBlockData bData in md.insBlocks)
         {
            codeSize = ReportHelper.GetBlockCodeSize(bData);
            coveredCodeSize = ReportHelper.GetBlockCoveredCodeSize(bData);
         }
         float percent = codeSize == 0 ? 0 : coveredCodeSize / (float)codeSize * 100;
         this.Text = string.Format("{0} ({1:#0}%)", md.methodName, percent);
         this.ForeColor = ColorScheme.GetPercentColor(percent);
      }

      #endregion
   }

   public class BlockVariantTreeNode : NodeBase, ICoverageInformation
   {
      CoverageReport.InnerBlockData bData;
      public CoverageReport.InnerBlockData BlockData
      {
         get { return bData; }
      }

      public BlockVariantTreeNode(CoverageReport.InnerBlockData bData)
         : base("Block Data")
      {
         this.bData = bData;
      }

      #region ICoverageInformation Members

      private UInt32 codeSize = 0;
      private UInt32 coveredCodeSize = 0;

      public UInt32 GetCodeSize() { return codeSize; }
      public UInt32 GetCoveredCodeSize() { return coveredCodeSize; }

      public void UpdateCoverageInfo()
      {
         codeSize = ReportHelper.GetBlockCodeSize(BlockData);
         coveredCodeSize = ReportHelper.GetBlockCoveredCodeSize(BlockData);

         float percent = codeSize == 0 ? 0 : coveredCodeSize / (float)codeSize * 100;
         this.Text = string.Format("Block Data ({0:#0}%)", percent);
         this.ForeColor = ColorScheme.GetPercentColor(percent);
      }

      #endregion
   }

   public sealed class ColorScheme
   {
      private ColorScheme() { }

      public static Color GetPercentColor(float percent)
      {
         if (percent < 10) return Color.Red;
         if (percent < 20) return Color.Red;
         if (percent < 30) return Color.Red;
         if (percent < 40) return Color.Red;
         if (percent < 50) return Color.Red;
         if (percent < 60) return Color.Orange;
         if (percent < 70) return Color.Orange;
         if (percent < 80) return Color.Blue;
         if (percent < 90) return Color.Blue;
         return Color.Green;
      }

      public static Color GetForeColorForBlock(float percent)
      {
         if (percent < 50) return Color.Red;
         return Color.Blue;
      }
   }
}
