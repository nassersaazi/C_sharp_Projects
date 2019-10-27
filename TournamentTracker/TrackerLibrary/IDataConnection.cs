using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary
{
    public interface IDataConnection
    {   /// <summary>
    /// Interface for data sources
    /// </summary>
    /// <param name="model"></param>
    /// <returns>PrizeModel</returns>
        PrizeModel CreatePrize(PrizeModel model);
    }
}
