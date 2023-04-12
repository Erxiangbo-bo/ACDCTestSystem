using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDCTestSystemPart1.Models
{
    public class ProductData
    {
        public string WorkNum
        {
            get; set;
        }

        public string CurrentModel
        {
            get; set;
        }

        public int Voltage_Num
        {
            get; set;
        }

        public int VoltPassNum
        {
            get; set;
        }

        public int Function_Num
        {
            get; set;
        }

        public int FuncPassNum
        {
            get; set;
        }

        public List<string> BlackList
        {
            get; set;
        }

    }
}
