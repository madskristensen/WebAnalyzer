using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLinter
{
    public class LintingEventArgs : EventArgs
    {
        public LintingEventArgs(int total, int amountOfTotal, string providerName, int files)
        {
            Total = total;
            AmountOfTotal = amountOfTotal;
            ProviderName = providerName;
            Files = files;
        }

        public int Files { get; set; }
        public int Total { get; set; }
        public int AmountOfTotal { get; set; }
        public string ProviderName { get; set; }
    }
}
