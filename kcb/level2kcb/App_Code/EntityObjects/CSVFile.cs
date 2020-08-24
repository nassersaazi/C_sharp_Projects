using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Summary description for CSVFile
/// </summary>
public class CSVFile
{
    public static string LogFilePath = @"E:\PePay\GenericApi\Api\GenericApi\Logs\ResponseTimesLog.csv";
    public CSVFile()
    {

    }

    public static void WriteToLogFile(DateTime timeIn,DateTime timeOut,Transaction tran) 
    {
        try
        {
            List<string> allLines = new List<string>();
            string lineToAppend = timeIn + "," + timeOut + "," + tran.VendorCode + "," + tran.VendorTransactionRef;
            if (File.Exists(LogFilePath))
            {
                string[] oldLines = File.ReadAllLines(LogFilePath);
                allLines.AddRange(oldLines);
            }
            allLines.Add(lineToAppend);
            File.WriteAllLines(LogFilePath, allLines.ToArray());
        }
        catch (Exception) 
        {
        
        }

    }
}
