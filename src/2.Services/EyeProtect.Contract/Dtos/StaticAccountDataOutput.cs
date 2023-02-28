using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Contract.Dtos
{
    public class StaticAccountDataOutput
    {
        public int Used { get; set; }

        public int UnUse { get; set; }

        public int UnSale { get; set; }

        public int Expire { get; set; }

        public string ServerDate => DateTime.Now.ToShortDateString();
    }
}
