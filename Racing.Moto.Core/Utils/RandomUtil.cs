using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Core.Utils
{
    public class RandomUtil
    {
        //public static List<int> GetRandomList(int len, int max)
        //{
        //    var info = new List<int>();

        //    int rep = 1;
        //    int num = 0;
        //    long num2 = DateTime.Now.Ticks + rep;
        //    rep++;
        //    Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
        //    for (int i = 0; i < len; i++)
        //    {
        //        num = Convert.ToInt32((char)(0x30 + ((ushort)(random.Next() % 10))));

        //        info.Add(num);
        //    }

        //    return info;
        //}

        public static List<int> GetRandomList(int min, int max)
        {
            var list = new List<int>();
            for (var i = min; i <= max; i++)
            {
                list.Add(i);
            }
            return list.OrderBy(i => Guid.NewGuid()).ToList();
        }
    }
}
