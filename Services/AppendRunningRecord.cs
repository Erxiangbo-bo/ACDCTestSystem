using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDCTestSystemPart1.Services
{
    public class AppendRunningRecord
    {
        private string Record
        {
            get; set;
        }

        public string AppendString(string msg)
        {
            lock (this)
            {
                Record = $"{DateTime.Now}: {msg} \r {Record}";
                return Record;
            }
        }
        public void ClearString()
        {
            lock (this)
            {
                Record = "";
            }
        }
    }
}
