using ACDCTestSystemPart1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDCTestSystemPart1.Share
{
    public static class ShareModel
    {
        public static bool IsEndApplication;
        public static bool AppRunning;
        public static bool LoginToken;
        public static bool IsAdmin;
        public static ConnectionConfig connectionConfig = new ConnectionConfig();
        public static ProductData productData = new ProductData();
    }
}
