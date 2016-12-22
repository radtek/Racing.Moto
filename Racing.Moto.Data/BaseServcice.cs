using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data
{
    public abstract class BaseServcice : IDisposable
    {
        protected RacingDbContext _context = null;
        protected RacingDbContext db
        {
            get { return _context; }
        }

        public BaseServcice()
        {
            _context = new RacingDbContext();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }

}
