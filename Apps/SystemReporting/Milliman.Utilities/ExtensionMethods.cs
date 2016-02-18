using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities
{
    public static class ExtensionMethods
    {
        public static string ToPropertyName(this string original)
        {
            // Need to insert a '_' at the beginning, in case name starts with a number!
            var name = original.Trim().Replace(" ", "").Insert(0, "_");
            name = name.Replace("+", "");
            for (int i = 0; i < name.Length; i++)
            {
                var letter = name[i];
                if (!char.IsLetterOrDigit(letter))
                    name = name.Replace(letter, '_');
            }
            return name;

        }

        public static string ToMD5(this string original)
        {
            if (original != null)
            {
                var MD5ByteArray = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(original));
                var hashBuilder = new StringBuilder();
                foreach (var byght in MD5ByteArray)
                    hashBuilder.Append(byght.ToString("x2").ToLower());

                return hashBuilder.ToString();
            }
            else
                throw new ArgumentNullException("Cannot generate an MD5 hash of the given string because the given string was null.");
        }
    }
}
