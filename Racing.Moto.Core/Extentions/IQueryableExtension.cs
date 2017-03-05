using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Core.Extentions
{
    public static class IQueryableExtension
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex">当前页 从1开始</param>
        /// <param name="pageSize">默认15</param>
        /// <returns></returns>
        public static PagerResult<T> Pager<T>(this IQueryable<T> source, int pageIndex, int pageSize = 15) where T : class
        {
            var result = new PagerResult<T>();

            result.RowCount = source.Count();
            result.PageCount = (int)Math.Ceiling(result.RowCount * 1.0 / pageSize);

            if (pageIndex >= result.PageCount && result.PageCount > 0)
            {
                pageIndex = result.PageCount - 1;
            }
            else
            {
                pageIndex = (pageIndex > 0) ? pageIndex - 1 : 0;
            }

            result.Items = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return result;
        }
    }

    public class PagerResult<T> where T : class
    {
        public List<T> Items { get; set; }
        public int RowCount { get; set; }
        public int PageCount { get; set; }
    }
}
