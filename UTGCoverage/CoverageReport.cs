using System;
using System.Collections.Generic;
using System.Text;

namespace UTGCoverage
{
   public sealed class CoverageReport
   {
      public struct FileDescriptor
      {
         public UInt32 fileId;
         public string fileUrl;
      }

      public class TypeDescriptor
      {
         public string assemblyName;
         public string typeName;
         public UInt32 flags;

         public MethodDescriptor[] methods = new MethodDescriptor[0];
      }

      public class MethodDescriptor
      {
         public string methodName;
         public string methodSig;
         public UInt32 flags;
         public UInt32 implFlags;

         public InnerBlockData[] insBlocks;

         public UInt32 GetCodeSize(int blockIndex)
         {
            UInt32 res = 0;
            foreach (InnerBlock inner in insBlocks[blockIndex].blocks)
               res += inner.blockLen;
            return res;
         }

         public UInt32 GetCoveredCodeSize(int blockIndex)
         {
            UInt32 res = 0;
            foreach (InnerBlock inner in insBlocks[blockIndex].blocks)
               if (inner.visitCount > 0) res += inner.blockLen;
            return res;
         }

         public MethodDescriptor(int initialBlockSize) { SetBlockDataSize(initialBlockSize); }
         public MethodDescriptor() { SetBlockDataSize(0); }

         private void SetBlockDataSize(int initialBlockSize)
         {
            insBlocks = new InnerBlockData[initialBlockSize];
            while (initialBlockSize-- > 0)
               insBlocks[initialBlockSize] = new InnerBlockData();
         }
      }

      public class InnerBlock
      {
         public UInt32 position;
         public UInt32 blockLen;
         public UInt32 visitCount;
         public UInt32 fileId;
         public UInt32 startLine;
         public UInt32 startColumn;
         public UInt32 endLine;
         public UInt32 endColumn;
      }

      public class InnerBlockData
      {
         public InnerBlock[] blocks = new InnerBlock[0];
      }

      public TypeDescriptor[] types = new TypeDescriptor[0];

      public FileDescriptor[] files = new FileDescriptor[0];
   }
}
