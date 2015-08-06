using ECoupoun.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECoupoun.Web.Controllers
{
    public class BaseController : Controller
    {
        private ECoupounEntities _db;
        public ECoupounEntities db
        {
            get
            {
                if (_db == null)
                {
                    _db = new ECoupounEntities();
                }
                return _db;
            }
        }
    }
}