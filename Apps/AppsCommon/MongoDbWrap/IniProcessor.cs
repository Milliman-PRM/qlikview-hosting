using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbWrap
{
    /// <summary>
    /// Read only support for configuration files in .ini format, with case insensitive matching
    /// </summary>
    class IniProcessor
    {
        // this is the model, where all the contents are represented
        Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Constructor, which immediately processes the .ini file with the provided name
        /// </summary>
        /// <param name="file">Name of the .ini file to be processed</param>
        public IniProcessor(string file)
        {
            var txt = File.ReadAllText(file);

            Dictionary<string, string> currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // create a global section
            ini[""] = currentSection;

            // Consume each meaningful line in the file
            foreach (var line in txt.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                   .Where(t => !string.IsNullOrWhiteSpace(t))
                                   .Select(t => t.Trim()))
            {
                // skip comments
                if (line.TrimStart().StartsWith(";"))
                    continue;

                // new section
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    ini[line.Substring(1, line.LastIndexOf("]") - 1).Trim()] = currentSection;
                    continue;
                }

                // new key/value in most recently encountered section
                var idx = line.IndexOf("=");
                if (idx == -1)
                    currentSection[line.Trim()] = "";
                else
                    currentSection[line.Substring(0, idx).Trim()] = line.Substring(idx + 1).Trim();
            }
        }

        // With 1 argument, global section is assumed and default value is empty string
        /// <summary>
        /// Gets the value of a named key from global (unnamed) section, and returns a blank string if the key is not found. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return GetValue(key, "", "");
        }

        /// <summary>
        /// Gets the value of a named key from a named section, and returns a blank string if the key is not found. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public string GetValue(string key, string section)
        {
            return GetValue(key, section, "");
        }

        /// <summary>
        /// Gets the value of a named key from a named section, and returns a caller provided default value if the key is not found. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public string GetValue(string key, string section, string @default)
        {
            if (!ini.ContainsKey(section))
                return @default;

            if (!ini[section].ContainsKey(key))
                return @default;

            return ini[section][key];
        }

        /// <summary>
        /// Gets an array of all key names from the file
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public string[] GetKeys(string section)
        {
            if (!ini.ContainsKey(section))
                return new string[0];

            return ini[section].Keys.ToArray();
        }

        /// <summary>
        // Gets an array of all section names from the file
        /// </summary>
        /// <returns></returns>
        public string[] GetSections()
        {
            return ini.Keys.Where(t => t != "").ToArray();
        }
    }
}
