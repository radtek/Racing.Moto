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
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">当前页 从1开始</param>
        /// <param name="pageSize">默认15</param>
        /// <returns></returns>
        public static PagerResult<TSource> Pager<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize = 15) where TSource : class
        {
            PagerResult<TSource> result = new PagerResult<TSource>();

            result.RowCount = source.Count();

            var pageCount = (int)Math.Ceiling(result.RowCount * 1.0 / pageSize);
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageIndex > pageCount)
            {
                pageIndex = pageCount;
            }
            if (pageCount == 0)
            {
                pageIndex = 1;
                pageCount = 1;
            }
            result.PageCount = pageCount;

            result.Data = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return result;
        }
    }

    public class PagerResult<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }
        public int RowCount { get; set; }
        public int PageCount { get; set; }

        public PagerResult(bool success = true, string message = null)
        {
            Success = success;
            Message = message;
        }
    }
}
