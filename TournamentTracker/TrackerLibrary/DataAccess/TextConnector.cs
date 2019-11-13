using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess

{
    public class TextConnector : IDataConnection

        private const string PrizesFile = "PrizesModel.csv";

    {   // TODO - Wire up the CreatePrize for text files 
        public PrizeModel CreatePrize(PrizeModel model)
        {
        // Load the text file and convert the text to List<prizeModel>
        List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

        // Find the max ID
        int currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
        model.Id = currentId;

        //Add the new record with the new ID (max +1)
        prizes.Add(model);

        //Convert the prizes to list<string>
        //Save the list<string> to the text file
        }
    }
}
 