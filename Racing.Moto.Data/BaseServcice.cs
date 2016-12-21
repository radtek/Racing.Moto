using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data
{
    public abstract class BaseServcice : IDisposable
    {
        protected TransportationDbContext _context = null;
        protected TransportationDbContext db
        {
            get { return _context; }
        }

        public BaseServcice()
        {
            _context = new TransportationDbContext();
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
