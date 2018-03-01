using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ThinkSharp.Solutions.TemplateDefinition
{
    public class TemplateDefinitionHelper
    {
        private static XmlSerializer theSerializer = new XmlSerializer(typeof(TemplateDefinition));
        private static ILog theLogger = LogManager.GetLogger(typeof(TemplateDefinitionHelper));

        public static TemplateDefinition Parse(string file)
        {
            if (!File.Exists(file))
                return null;

            try
            {
                using (var stream = File.OpenRead(file))
                    return theSerializer.Deserialize(stream) as TemplateDefinition;
            }
            catch (Exception ex)
            {
                theLogger.Error($"Error while deserializing template definition file '{file}'", ex);
                return null;
            }
        }

        public static string[] SplitEscapedString(string stringToSplit, char splitChar = ',')
        {
            const string escapedText = ":|:EscapedCharacterString:|:";

            if (stringToSplit == null)
                return null;

            string textToEscape = "\\" + splitChar;
            return stringToSplit
                .Replace(textToEscape, escapedText)
                .Split(splitChar)
                .Select(s => s.Replace(escapedText, splitChar.ToString()).Trim())
                .ToArray();
        }
    }
}
