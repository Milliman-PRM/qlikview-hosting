using System;
using System.Security.Cryptography;
using System.Text;

namespace Milliman.Reduction.ReductionEngine {
    [Serializable]
    public class QMSSettings {
        private string _encryptedPassword;

        public string Name { get; set; }
        public string QMSURL { get; set; }
        public bool IsIntegratedSecurity { get; set; }
        public string UserName { get; set; }
        public string Password { get { return _encryptedPassword; } set { this.EncryptPassword(value); } }
        private void EncryptPassword(string password) {
            var salt = Encoding.UTF8.GetBytes("Th1s1s4T3rr1bl3!DE@".ToCharArray());
            var key = new Rfc2898DeriveBytes("H3ck n0, this w1ll n3v3r work !@#$", salt);
            RijndaelManaged r = new RijndaelManaged();
            r.IV = key.GetBytes(r.IV.Length);
            r.Key = key.GetBytes(r.Key.Length);
            var encryptor = r.CreateEncryptor();
            var mem = new System.IO.MemoryStream();
            using( var cs = new CryptoStream(mem, encryptor, CryptoStreamMode.Write) ) {
                using( var w = new System.IO.StreamWriter(cs) ) {
                    w.Write(password);
                }
            }
            this._encryptedPassword = Convert.ToBase64String(mem.ToArray());
        }
        public string GetPassword() {
            if( string.IsNullOrEmpty(this._encryptedPassword) )
                return string.Empty;

            var salt = Encoding.UTF8.GetBytes("Th1s1s4T3rr1bl3!DE@".ToCharArray());
            var key = new Rfc2898DeriveBytes("H3ck n0, this w1ll n3v3r work !@#$", salt);
            RijndaelManaged r = new RijndaelManaged();
            r.IV = key.GetBytes(r.IV.Length);
            r.Key = key.GetBytes(r.Key.Length);

            var decryptor = r.CreateDecryptor();
            var cipher = Convert.FromBase64String(this._encryptedPassword);

            using( var mem = new System.IO.MemoryStream(cipher) ) {
                using( var cs = new CryptoStream(mem, decryptor, CryptoStreamMode.Read) ) {
                    using( var reader = new System.IO.StreamReader(cs) ) {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
