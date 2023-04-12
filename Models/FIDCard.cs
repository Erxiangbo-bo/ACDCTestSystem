using GJ.DEV.CARD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDCTestSystemPart1.Models
{
    public class FIDCard : CMFID
    {
        public FIDCard(int id, string _fidprot)
        {
            this.ID = id;
            this.FIDProt = _fidprot;
        }
        public int ID
        {
            get; set;
        }
        private string FIDProt
        {
            get; set;
        }
        private string outIDcard;
        public string outErrMessage;
        private int count = 0;

        public bool OpenFID()
        {
            if (!string.IsNullOrEmpty(FIDProt))
            {
                this.Open(FIDProt, out outErrMessage, "19200,E,8,1");
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<string> GetCardNum()
        {
            count = 5;
            outIDcard = "";
            do
            {
                this.GetRecord(this.ID, out outIDcard, out outErrMessage);
                count--;
                await Task.Delay(100);
            } while (outIDcard.Length != 10 && count > 0);
            return outIDcard;
        }
    }
}
