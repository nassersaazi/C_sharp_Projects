using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public interface IPayment
    {
        PostResponse pay(ITransaction tran);

    }
}
