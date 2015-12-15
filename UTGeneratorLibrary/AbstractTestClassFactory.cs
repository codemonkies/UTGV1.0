using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UTGeneratorLibrary
{
   public abstract class AbstractTestClassFactory
   {
      public abstract AbstractTestClass CreateCSharpTestClass(string classNamespace, string className, bool classIsAbstract);
      public abstract AbstractTestClass CreateVBTestClass(string classNamespace, string className, bool classIsAbstract);
      //add any other language specific create method ...
   }
}
