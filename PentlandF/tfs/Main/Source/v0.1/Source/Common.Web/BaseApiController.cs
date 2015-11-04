using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.Storage.EntityFramework;
using NextLAP.IP1.Storage.EntityFramework.Repositories;

namespace NextLAP.IP1.Common.Web
{
    public abstract class BaseApiController : ApiController
    {
        private readonly Ip1Context _context;
        private Ip1Repositories _repositories;
        protected Logger Log;

        protected BaseApiController()
        {
            Log = LogManager.GetLogger(GetType());
            _context = new Ip1Context();
        }

        protected Ip1Repositories Repositories
        {
            get
            {
                return _repositories ?? (_repositories = new Ip1Repositories(_context));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(_repositories != null) _repositories.Dispose();
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
