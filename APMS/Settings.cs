using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.ComponentModel;

namespace APMS.Properties
{
    internal sealed partial class Settings : INotifyPropertyChanged
    {
        internal byte[] Key
        {
            get { return Convert.FromBase64String(Base1); }
            set { Base1 = Convert.ToBase64String(value); }
        }

        internal byte[] IV
        {
            get { return Convert.FromBase64String(Base2); }
            set { Base2 = Convert.ToBase64String(value); }
        }

        /// <summary>
        /// 数据库服务器
        /// </summary>
        public String ServerName
        {
            get { return this.数据库服务器; }
            set { this.数据库服务器 = value; }
        }

        /// <summary>
        /// 数据库用户名
        /// </summary>
        public String UserName
        {
            get { return this.数据库用户名; }
            set { this.数据库用户名 = value; }
        }

        /// <summary>
        /// 数据库密码
        /// </summary>
        public String UserPass
        {
            get { return this.Decode(this.数据库密码); }
            set { this.数据库密码 = this.Encode(value); }
        }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public String DataBaseName
        {
            get { return this.数据库名称; }
            set
            {
                if (this.数据库名称 != value)
                {
                    NotifyPropertyChanged("DataBaseName");
                    this.数据库名称 = value;
                    NotifyPropertyChanged("数据库名称");
                }
            }
        }

        /// <summary>
        /// 是否使用Windows集成验证
        /// </summary>
        public bool IsWindowsIntegrated
        {
            get { return this.使用Windows集成验证; }
            set
            {
                if (this.使用Windows集成验证 != value)
                {
                    NotifyPropertyChanged("IsWindowsIntegrated");
                    this.使用Windows集成验证 = value;
                    NotifyPropertyChanged("使用Windows集成验证");
                }
            }
        }

        public new void Reload()
        {
            base.Reload();
            NotifyPropertyChanged("ServerName");
            NotifyPropertyChanged("UserName");
            NotifyPropertyChanged("UserPass");
            NotifyPropertyChanged("DataBaseName");
            NotifyPropertyChanged("IsWindowsIntegrated");
        }

        public new void Reset()
        {
            base.Reset();
            NotifyPropertyChanged("ServerName");
            NotifyPropertyChanged("UserName");
            NotifyPropertyChanged("UserPass");
            NotifyPropertyChanged("DataBaseName");
            NotifyPropertyChanged("IsWindowsIntegrated");
        }

        #region INotifyPropertyChanged 成员

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.BeginInvoke(this, new PropertyChangedEventArgs(info), null, null);
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    internal static class SettingsHelper
    {
        internal static String Encode(this Settings s, String Org)
        {
            if (Org == null || Org.Length <= 0)
            {
                Org = "APMS";
            }
            AesManaged AES = null;
            try
            {
                AES = new AesManaged();
                if (s.IsFirst)
                {
                    s.GenerateKey(ref AES);
                }
                else
                {
                    if (s.IV == null || s.IV.Length <= 0 || s.Key == null || s.Key.Length <= 0)
                    {
                        s.GenerateKey(ref AES);
                    }
                    AES.IV = s.IV;
                    AES.Key = s.Key;
                    MemoryStream msEncrypt = new MemoryStream();
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(Org);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (AES != null)
                {
                    AES.Clear();
                }
            }
            return "";
        }

        internal static void GenerateKey(this Settings s, ref AesManaged AES)
        {
            AES.GenerateIV();
            AES.GenerateKey();
            s.Key = AES.Key;
            s.IV = AES.IV;
            s.IsFirst = false;
        }

        internal static String Decode(this Settings s, String Ent)
        {
            if (Ent == null || Ent.Length <= 0)
            {
                return "APMS";
            }
            AesManaged AES = null;
            string plaintext = "";
            try
            {
                AES = new AesManaged();
                if (s.IsFirst)
                {
                    return "APMS";
                }
                else
                {
                    if (s.IV == null || s.IV.Length <= 0 || s.Key == null || s.Key.Length <= 0)
                    {
                        return "";
                    }
                    AES.IV = s.IV;
                    AES.Key = s.Key;
                    MemoryStream msEncrypt = new MemoryStream();
                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(Ent)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, AES.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                    return plaintext;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return "APMS";
            }
            finally
            {
                if (AES != null)
                {
                    AES.Clear();
                }
            }
        }
    }
}

