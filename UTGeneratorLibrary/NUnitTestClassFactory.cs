using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UTGeneratorLibrary
{
   public class NUnitTestClassFactory : AbstractTestClassFactory
   {
      public override AbstractTestClass CreateCSharpTestClass(string classNamespace, string className, bool classIsAbstract)
      {
         NUnitCSharpTestClass testClass = new NUnitCSharpTestClass(classNamespace, className, classIsAbstract);
         return testClass;
      }

      public override AbstractTestClass CreateVBTestClass(string classNamespace, string className, bool classIsAbstract)
      {
         throw new NotImplementedException();
      }
   }
}
