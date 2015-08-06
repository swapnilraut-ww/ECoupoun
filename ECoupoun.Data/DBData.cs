using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECoupoun.Data
{
    public class DBData
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
