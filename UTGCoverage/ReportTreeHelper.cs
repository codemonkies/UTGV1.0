using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace UTGCoverage
{
   public class ReportTreeHelper
   {
      public static void AddMethodNode(ClassNode classNode, CoverageReport.MethodDescriptor md)
      {
         MethodNode mNode = new MethodNode(md);
         classNode.Nodes.Add(mNode);

         if (md.insBlocks.Length > 1)
         {
            foreach (CoverageReport.InnerBlockData bData in md.insBlocks)
               mNode.Nodes.Add(new BlockVariantTreeNode(bData));
         }
      }

      public static TreeNode GetNamespaceNode(TreeNode asmNode, string typedefName)
      {
         string[] names = ReportHelper.SplitNamespaces(typedefName);
         TreeNode parentNode = asmNode;
         for (int i = 0; i < names.Length - 1; ++i)
         {
            NamespaceNode nextNode = FindNamespaceNode(parentNode.Nodes, names[i]);
            if (nextNode == null)
            {
               nextNode = new NamespaceNode(names[i]);
               parentNode.Nodes.Add(nextNode);
            }
            parentNode = nextNode;
         }
         return parentNode;
      }

      public static NamespaceNode FindNamespaceNode(TreeNodeCollection nodes, string namespaceName)
      {
         foreach (TreeNode tn in nodes)
         {
            NamespaceNode ntn = tn as NamespaceNode;
            if (ntn == null)
               continue;
            if (ntn.Namespace == namespaceName)
               return ntn;
         }
         return null;
      }

   }

}
