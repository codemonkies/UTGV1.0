using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using UTGHelper;

namespace UTGTesting
{
   public static class NUnitXmlResultProccessor
   {
      internal static string messageRegexExpresion = @"<message><!\[CDATA\[\s*(?<message>[\w\W]*?)\]\]></message>";
      //internal static string stackTraceRegexExpresion = @"<stack\-trace><!\[CDATA\[at\s*[\w\W]*?\s*in\s*(?<filename>[\w\W]*?):line\s(?<line>[0-9]*)";
      internal static string stackTraceRegexExpresion = @"<stack\-trace>\s*[\w\W]*?<!\[CDATA*?\[\s*[\w\w]*?\bat\b\s*[\w\W]*?\s*[\w\w]*?\bin\b\s*[\w\W]*?\s*(?<filename>[\w\W]*?)\s*:\s*\bline\b\s*(?<line>[0-9]*)";
     
      public static List<XmlNode> GetSuccessfulTestSuites(XmlNodeList xmlNodeList)
      {
         List<XmlNode> resultList = new List<XmlNode>();

         for (int i = 0; i < xmlNodeList.Count; ++i)
         {
            XmlNode nameNode =
               xmlNodeList.Item(i).Attributes.GetNamedItem("name");

            XmlNode successNode =
               xmlNodeList.Item(i).Attributes.GetNamedItem("success");

            XmlNode executedNode =
              xmlNodeList.Item(i).Attributes.GetNamedItem("executed");

            if (successNode == null) continue;

            if (executedNode == null) continue;

            try
            {
               if (successNode.Value.Equals("True"))
                  resultList.Add(xmlNodeList.Item(i));
            }
            catch (Exception ex)
            {
               //ignore...
               Logger.LogException(ex);
            }
         }

         return resultList;
      }

      public static List<UnitTestError> GetSuccessfulTestSuitesAsUnitTestErrors(List<XmlNode> xmlNodeList)
      {
         List<UnitTestError> lstErros = new List<UnitTestError>();

         foreach (XmlNode xmlNode in xmlNodeList)
         {
            UnitTestError uterro = new UnitTestError();

            //uterro.Text = xmlNode.InnerXml;

            XmlNode nameNode =
               xmlNode.Attributes.GetNamedItem("name");

            if (nameNode != null)
               uterro.Name = nameNode.Value;

            XmlNode assertsNode =
               xmlNode.Attributes.GetNamedItem("asserts");

            if (assertsNode != null)
               uterro.Asserts = assertsNode.Value;

            //string regexMessagePattern = @"<message><!\[CDATA\[\s*(?<message>[\w.:,""'\\\s*-`%&_\^@!#><;=\)\(~\$\+/]*)\s*\]\]\s*>\s*</message>\s*";
            string regexMessagePattern = messageRegexExpresion;

            System.Text.RegularExpressions.Regex regExMessage = new System.Text.RegularExpressions.Regex(regexMessagePattern);

            System.Text.RegularExpressions.Match matchMessage =
               regExMessage.Match(xmlNode.InnerXml);

            if (matchMessage.Success)
            {
               try
               {
                  uterro.Text = (string)matchMessage.Groups["message"].Value;
               }
               catch (Exception ex)
               {
                  Logger.LogException(ex);
               }
            }

            //string regexPattern = @"<stack\-trace><!\[CDATA\[at\s*[0-9a-zA-Z_.(),\s*]*\s*in\s*(?<filename>[0-9a-zA-Z_.:,\\\s*]*):line\s(?<line>[0-9]*)";
            string regexPattern = stackTraceRegexExpresion;
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(regexPattern);

            System.Text.RegularExpressions.Match match =
               regEx.Match(xmlNode.InnerXml);

            if (match.Success)
            {
               try
               {
                  uterro.File = (string)match.Groups["filename"].Value;
               }
               catch (Exception ex)
               {
                  Logger.LogException(ex);
               }

               try
               {
                  uterro.Line = int.Parse((string)match.Groups["line"].Value);
               }
               catch (Exception ex)
               {
                  Logger.LogException(ex);
               }

            }
            else
            {
               //FIX ME .cs
               uterro.File = GetFielNameFromTestCaseElementAttributeName(nameNode.Value) + ".cs";
            }

            lstErros.Add(uterro);
         }

         return lstErros;
      }

      public static List<XmlNode> GetFailedExecutionSuites(XmlNodeList xmlNodeList)
      {
         List<XmlNode> resultList = new List<XmlNode>();

         for (int i = 0; i < xmlNodeList.Count; ++i)
         {
            XmlNode nameNode =
               xmlNodeList.Item(i).Attributes.GetNamedItem("name");

            XmlNode executedNode =
             xmlNodeList.Item(i).Attributes.GetNamedItem("executed");

            XmlNode assertsNode =
               xmlNodeList.Item(i).Attributes.GetNamedItem("asserts");

            if (executedNode == null) continue;

            try
            {
               if (executedNode.Value.Equals("False"))
                  resultList.Add(xmlNodeList.Item(i));
            }
            catch (Exception ex)
            {
               //ignore...
               Logger.LogException(ex);
            }
         }

         return resultList;
      }

      public static List<XmlNode> GetFailedTestSuites(XmlNodeList xmlNodeList)
      {
         List<XmlNode> resultList = new List<XmlNode>();

         for (int i = 0; i < xmlNodeList.Count; ++i)
         {
            XmlNode nameNode =
               xmlNodeList.Item(i).Attributes.GetNamedItem("name");

            XmlNode successNode =
               xmlNodeList.Item(i).Attributes.GetNamedItem("success");

            XmlNode executedNode =
             xmlNodeList.Item(i).Attributes.GetNamedItem("executed");

            XmlNode assertsNode =
               xmlNodeList.Item(i).Attributes.GetNamedItem("asserts");

            if (successNode == null) continue;

            if (executedNode == null) continue;

            try
            {
               if (successNode.Value.Equals("False"))
                  resultList.Add(xmlNodeList.Item(i));
            }
            catch (Exception ex)
            {
               //ignore...
               Logger.LogException(ex);
            }
         }

         return resultList;
      }

      static string GetFielNameFromTestCaseElementAttributeName(string testCaseElementAttributeName)
      {
         string[] nameParts = testCaseElementAttributeName.Split(new char[] { '.' });
         if (nameParts.Length <= 0)
            return string.Empty;
         else if (nameParts.Length == 1)
            return nameParts[0];
         else if (nameParts.Length == 2)
            return nameParts[1];
         else
         {
            return nameParts[nameParts.Length - 2];
         }
      }

      public static List<UnitTestError> GetFailedTestSuitesAsUnitTestErrors(List<XmlNode> xmlNodeList)
      {
         List<UnitTestError> lstErros = new List<UnitTestError>();

         
         foreach (XmlNode xmlNode in xmlNodeList)
         {
            UnitTestError uterro = new UnitTestError();

            //uterro.Text = xmlNode.InnerXml;

            XmlNode nameNode =
               xmlNode.Attributes.GetNamedItem("name");

            if(nameNode != null)
               uterro.Name = nameNode.Value;

            XmlNode assertsNode =
               xmlNode.Attributes.GetNamedItem("asserts");

            if (assertsNode != null)
               uterro.Asserts = assertsNode.Value;

            //string regexMessagePattern = @"<message><!\[CDATA\[\s*(?<message>[\w.:,""'\\\s*-`%&_\^@!#><;=\)\(~\$\+/]*)\s*\]\]\s*>\s*</message>\s*";
            string regexMessagePattern = messageRegexExpresion;

            System.Text.RegularExpressions.Regex regExMessage = new System.Text.RegularExpressions.Regex(regexMessagePattern);

            System.Text.RegularExpressions.Match matchMessage =
               regExMessage.Match(xmlNode.InnerXml);

            if (matchMessage.Success)
            {
               try
               {
                  uterro.Text = (string)matchMessage.Groups["message"].Value;
               }
               catch (Exception ex) {
                  Logger.LogException(ex);
               }
            }

            //string regexPattern = @"<stack\-trace><!\[CDATA\[at\s*[0-9a-zA-Z_.(),\s*]*\s*in\s*(?<filename>[0-9a-zA-Z_.:,\\\s*]*):line\s(?<line>[0-9]*)";
            string regexPattern = stackTraceRegexExpresion;
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(regexPattern);

            System.Text.RegularExpressions.Match match = 
               regEx.Match(xmlNode.InnerXml);

            if (match.Success)
            {
               try
               {
                  uterro.File = (string)match.Groups["filename"].Value;
               }
               catch (Exception ex) {
                  Logger.LogException(ex);
               }

               try
               {
                  uterro.Line = int.Parse((string)match.Groups["line"].Value);
               }
               catch (Exception ex) {
                  Logger.LogException(ex);
               }

            }
            else
            {
               //FIX ME .cs
               uterro.File = GetFielNameFromTestCaseElementAttributeName(nameNode.Value) + ".cs";
            }

            lstErros.Add(uterro);
         }

         return lstErros;
      }

      /// <summary>
      /// Gets a UNiteTestError Collection for all failed to execute tests. Those test were never run!
      /// In the case of a fauiler to run, nunit will not give the file name and line location so 
      /// the UnitTestErro object will not have its File property/memeber set
      /// </summary>
      /// <param name="xmlNodeList"></param>
      /// <returns></returns>
      public static List<UnitTestError> GetFailedExecutionsAsUnitTestErrors(List<XmlNode> xmlNodeList)
      {
         List<UnitTestError> lstErros = new List<UnitTestError>();


         foreach (XmlNode xmlNode in xmlNodeList)
         {
            UnitTestError uterro = new UnitTestError();

            //uterro.Text = xmlNode.InnerXml;

            XmlNode nameNode =
               xmlNode.Attributes.GetNamedItem("name");

            if (nameNode != null)
               uterro.Name = nameNode.Value;

            //NEW NEW NEW
            //string regexMessagePattern = @"<message><!\[CDATA\[\s*(?<message>[0-9a-zA-Z_.:,""'\\\s*-]*)\s*\]\]\s*>\s*</message>\s*";
            //string regexMessagePattern = @"<message><!\[CDATA\[(?<message>.*)\]\]\s*></message>";
            string regexMessagePattern = messageRegexExpresion;

            System.Text.RegularExpressions.Regex regExMessage = new System.Text.RegularExpressions.Regex(regexMessagePattern);

            System.Text.RegularExpressions.Match matchMessage =
               regExMessage.Match(xmlNode.InnerXml);

            if (matchMessage.Success)
            {
               try
               {
                  uterro.Text = (string)matchMessage.Groups["message"].Value;
               }
               catch (Exception ex) {
                  Logger.LogException(ex);
               }
            }

            lstErros.Add(uterro);
         }

         return lstErros;
      }

      public static List<string> GetFailedTestSuitesFormated(List<XmlNode> xmlNodeList)
      {
         List<string> errorList = new List<string>();

         string tvFormatedSingleError = "Failure: Test {0} Result {1} Asserts {2} Error {3}";

         foreach (XmlNode xmlNode in xmlNodeList)
         {
            XmlNode nameNode =
               xmlNode.Attributes.GetNamedItem("name");

            XmlNode successNode =
               xmlNode.Attributes.GetNamedItem("success");

            XmlNode executedNode =
             xmlNode.Attributes.GetNamedItem("executed");

            XmlNode assertsNode =
               xmlNode.Attributes.GetNamedItem("asserts");

   
            if (xmlNode.FirstChild != null || xmlNode.FirstChild.InnerText != null)
               errorList.Add(string.Format(tvFormatedSingleError, nameNode.Value, successNode.Value, assertsNode.Value, xmlNode.FirstChild.InnerText));
            else
               errorList.Add(string.Format(tvFormatedSingleError, nameNode.Value, successNode.Value, assertsNode.Value, ""));
         }

         return errorList;
      }

      /// <summary>
      /// 
      /// Note: Refractor with GetSuccessTestSuites 
      /// </summary>
      /// <param name="xmlNodeList"></param>
      /// <returns></returns>
      public static List<XmlNode> GetSuccessfulTestSuitesOlder(XmlNodeList xmlNodeList)
      {
         List<XmlNode> resultList = new List<XmlNode>();

         for (int i = 0; i < xmlNodeList.Count; ++i)
         {
            XmlNode successNode =
               xmlNodeList.Item(0).Attributes.GetNamedItem("success");

            if(successNode.Value.Equals("True"))
               resultList.Add(successNode);
         }

         return resultList;
      }

      /// <summary>
      /// 
      /// Note: Refractor with GetSuccessTestSuites 
      /// </summary>
      /// <param name="xmlNodeList"></param>
      /// <returns></returns>
      public static List<XmlNode> GetFailedTestSuitesOlder(XmlNodeList xmlNodeList)
      {
         List<XmlNode> resultList = new List<XmlNode>();

         for (int i = 0; i < xmlNodeList.Count; ++i)
         {
            XmlNode successNode =
               xmlNodeList.Item(0).Attributes.GetNamedItem("success");

            if (successNode.Value.Equals("False"))
               resultList.Add(successNode);
         }

         return resultList;
      }

      public static List<XmlNode> GetSuccessTestSuitesOlder(XmlNodeList xmlNodeList)
      {
         List<XmlNode> resultList = new List<XmlNode>();

         for (int i = 0; i < xmlNodeList.Count; ++i)
         {
            XmlNode successNode =
               xmlNodeList.Item(0).Attributes.GetNamedItem("success");

            resultList.Add(successNode);
         }

         return resultList;
      }

      public static XmlNodeList GetTestSuitesOlder(XmlDocument xmlDocument)
      {
         // Load( strFilename) to read a file.
         XmlNodeList test_suite = xmlDocument.GetElementsByTagName("test-suite");

         return test_suite;
      }

      public static XmlNodeList GetTestCases(XmlDocument xmlDocument)
      {
         // Load( strFilename) to read a file.
         XmlNodeList test_suite = xmlDocument.GetElementsByTagName("test-case");

         return test_suite;
      }

      public static System.Xml.XmlDocument ToXmlDoc(string xmlInput)
      {
         XmlDocument xmlDocument = new XmlDocument();
         xmlDocument.LoadXml(xmlInput);
         return xmlDocument;
      }
   }
}
