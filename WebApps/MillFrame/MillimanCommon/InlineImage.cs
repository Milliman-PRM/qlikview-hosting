using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MillimanCommon
{
    static public class InlineImage
    {
        static public string Inline(string PathFile, bool IsRelative = true)
        {
            try
            {
                string imagePath = PathFile;
                if (IsRelative)
                    imagePath = System.Web.HttpContext.Current.Server.MapPath(imagePath);
                String extension = System.IO.Path.GetExtension(imagePath).Substring(1);
                Byte[] imageData = System.IO.File.ReadAllBytes(imagePath);

                return String.Format("data:image/{0};base64,{1}", extension, Convert.ToBase64String(imageData));
            }
            catch (Exception)
            {
            }
            return "";
        }
    }
}
