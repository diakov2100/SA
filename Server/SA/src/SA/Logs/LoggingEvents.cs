using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Logs
{
    public class LoggingEvents
    {
        public const int PUT = 1000;
        public const int POST = 1001;
        public const int DELETE_ITEM = 1002;

        public const int PUT_ITEM_NOTFOUND = 4000;
        public const int POST_ITEM_NOTFOUND = 4001;
        public const int DELETE_ITEM_NOTFOUND = 4002;

        public const int PUT_EXCEPTION = 4010;
        public const int POST_EXCEPTION = 4011;
        public const int DELETE_EXCEPTION = 4012;

        public const int PUT_REQUEST_ERROR = 4020;
        public const int POST_REQUEST_ERROR = 4021;
        public const int DELETE_REQUEST_ERROR = 4022;
    }
}
