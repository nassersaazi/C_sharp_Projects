using System;
//using System.Configuration;
using System.Collections.Generic;
using System.Text;

// Load the text file
// Convert the text to List<PrizeModel>
// Find max ID
//Add the new record with the new ID (max + 1)
// Convert the prizes to list<string>
// Save the list<string> to the text file

namespace TrackerLibrary.DataAccess.TextConnector
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(string fileName ) //PrizeModels.csv
        {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName}"
        }
    }
}
