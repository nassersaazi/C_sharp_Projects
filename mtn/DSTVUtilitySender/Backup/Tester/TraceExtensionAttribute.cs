using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using UtilReqSender.ControlObjects;
using Tester;


// Create a SoapExtensionAttribute for the SOAP Extension that can be
// applied to an XML Web service method.
[AttributeUsage(AttributeTargets.Method)]
public class TraceExtensionAttribute : SoapExtensionAttribute
{

    private string filename = DatabaseHandler.LogDirectoryPath;
    private int priority;

    public override Type ExtensionType
    {
        get { return typeof(TraceExtension); }
    }

    public override int Priority
    {
        get { return priority; }
        set { priority = value; }
    }

    public string Filename
    {
        get
        {
            return filename;
        }
        set
        {
            filename = value;
        }
    }
}


