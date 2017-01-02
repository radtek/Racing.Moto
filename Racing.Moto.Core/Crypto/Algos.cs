using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Core.Crypto
{
    #region HashAlgo
    public abstract class HashAlgo
    {
        protected HashAlgorithm _algo;

        public HashAlgo(HashAlgorithm algo)
        {
            _algo = algo;
        }

        public string ComputeHash(string source)
        {
            byte[] bytesSource = Encoding.UTF8.GetBytes(source);
            byte[] bytesHash = _algo.ComputeHash(bytesSource);

            return Convert.ToBase64String(bytesHash);
        }
    }

    public class MD5Algo : HashAlgo
    {
        public MD5Algo()
            : base(MD5.Create()) { }
    }

    public class SHA1Algo : HashAlgo
    {
        public SHA1Algo(byte[] key)
            : base(new HMACSHA1(key)) { }
    }

    #endregion


    public class AESAlgo : SymAlgo
    {
        public AESAlgo(byte[] key)
            : base(new RijndaelManaged(), key) { }
    }

    public class DESAlgo : SymAlgo
    {
        public DESAlgo(byte[] key)
            : base(new DESCryptoServiceProvider(), key) { }
    }

    public class TripleDESAlgo : SymAlgo
    {
        public TripleDESAlgo(byte[] key)
            : base(new TripleDESCryptoServiceProvider(), key) { }
    }
}
