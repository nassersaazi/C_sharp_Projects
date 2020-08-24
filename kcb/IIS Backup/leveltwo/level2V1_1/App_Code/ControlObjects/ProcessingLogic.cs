using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using UtilityReferences.SageWood;

/// <summary>
/// Summary description for ProcessingLogic
/// </summary>
public class ProcessingLogic
{

    public SageWoodResponse ValidateSageWoodMeter(string meterNo)
    {
        string rawRequest;
        SageWoodResponse woodResponse = new SageWoodResponse();
        DatabaseHandler dh = new DatabaseHandler();
        try
        {
            string clientId = "2019112800001";
            string opName = "UDEMO";
            string password = "HXagent";
            //string meterno = "14012018064";//replace with user input
            string date = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            string randomnumber = GenerateRandomNumber().ToString();
            MeterInfo custInfo = new MeterInfo();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (delegate { return true; });
            EANDeviceID deviceId = new EANDeviceID();
            deviceId.ean = clientId;
            GenericDeviceID genericDeviceID = new GenericDeviceID();
            genericDeviceID.id = clientId;
            PurchaseValueCurrency purchaseValue = new PurchaseValueCurrency();
            purchaseValue.amt = new Currency();
            purchaseValue.amt.symbol = "UGX";
            purchaseValue.amt.value = Decimal.Parse("10");
            Electricity electricity = new Electricity();
            MsgID msg = new MsgID();
            msg.dateTime = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            msg.uniqueNumber = randomnumber;
            AuthCred authCred = new AuthCred();
            authCred.opName = opName;
            authCred.password = password;
            authCred.newPassword = "";
            MsgID messageId = msg;
            AuthCred creds = authCred;
            MeterDetail meter = new MeterDetail();
            meter.msno = meterNo;
            VendIDMethod vendIDMethod = new VendIDMethod();
            vendIDMethod.meterIdentifier = meter;
            ConfirmCustomerReq request = new ConfirmCustomerReq();
            XMLVendService21 sageWood = new XMLVendService21();
            request.clientID = deviceId;
            request.msgID = msg;
            request.authCred = authCred;
            request.terminalID = genericDeviceID;
            request.idMethod = vendIDMethod;
            rawRequest = XmlConfirmCustomerReq(request);
            ConfirmCustomerResp response = sageWood.ConfirmCustomerRequest(request);
            string rawResponse = XmlConfirmCustomerResp(response);
            //log raw request and response
            dh.LogRequestAndResponse("SAGEWOOD", meterNo, rawRequest, rawResponse);
            foreach (ConfirmCustResult meterDetails in response.confirmCustResult)
            {
                woodResponse.Name = meterDetails.custVendDetail.name;
                woodResponse.Msno = meterDetails.custVendDetail.accNo;
                woodResponse.Address = meterDetails.custVendDetail.address;
                woodResponse.ContactNo = meterDetails.custVendDetail.contactNo;
                woodResponse.AccNo = meterDetails.custVendDetail.accNo;
                woodResponse.LocRef = meterDetails.custVendDetail.locRef;
                woodResponse.DaysOflastPurchase = meterDetails.custVendDetail.daysLastPurchase;
                woodResponse.StatusCode = "0";
                woodResponse.StatusDescription = "SUCCESS";
            }
        }
        catch (SoapException soapException)
        {
            //XMLVendFaultResp xmlVendFaultResp = XMLDeserialize.Deserialize(soapException.Detail);

            // if (xmlVendFaultResp.fault.GetType() ==
            //typeof(UnknownMeterEx))
            // {
            //     Console.WriteLine("Unknown Meter Serial Number!");
            // }
            // else
            // {
            //     string fert = xmlVendFaultResp.fault.GetType().ToString();
            //     Console.WriteLine("Unexpected XMLVend Fault Response" +
            //                 "received: (" + xmlVendFaultResp.fault.GetType().ToString()
            //     + ")" + xmlVendFaultResp.fault.desc);
            // }
            dh.LogError(soapException.Detail.InnerXml, meterNo, DateTime.Now, "SAGEWOOD");
            woodResponse.StatusCode = "100";
            woodResponse.Name = "INVALID METER NUMBER";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
            dh.LogError(ex.ToString(), meterNo, DateTime.Now, "SAGEWOOD");
            woodResponse.StatusCode = "100";
            woodResponse.Name = "INVALID METER NUMBER";
        }
        return woodResponse;
    }
    public SageWoodResponse PaySageWoodElectricity(string MNo, string amount, string transactionId)
    {
        SageWoodResponse woodResponse = new SageWoodResponse();
        string rawRequest;
        DatabaseHandler dh = new DatabaseHandler();
        try
        {
            string clientId = "2019112800001";
            string opName = "UDEMO";
            string password = "HXagent";
            string meterno = "14012018064";
            string date = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            string randomnumber = GenerateRandomNumber().ToString();
            MeterInfo custInfo = new MeterInfo();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (delegate { return true; });
            EANDeviceID deviceId = new EANDeviceID();
            deviceId.ean = clientId;
            GenericDeviceID genericDeviceID = new GenericDeviceID();
            genericDeviceID.id = clientId;
            PurchaseValueCurrency purchaseValue = new PurchaseValueCurrency();
            purchaseValue.amt = new Currency();
            purchaseValue.amt.symbol = "UGX";
            purchaseValue.amt.value = Decimal.Parse("10");
            Electricity electricity = new Electricity();
            MsgID msg = new MsgID();
            msg.dateTime = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            msg.uniqueNumber = randomnumber;
            AuthCred authCred = new AuthCred();
            authCred.opName = opName;
            authCred.password = password;
            authCred.newPassword = "";
            MsgID messageId = msg;
            AuthCred creds = authCred;
            MeterDetail meter = new MeterDetail();
            meter.msno = meterno;
            Cash pay = new Cash();
            pay.tenderAmt = new Currency();
            pay.tenderAmt.symbol = "UGX";
            pay.tenderAmt.value = Decimal.Parse("100");
            VendIDMethod vendIDMethod = new VendIDMethod();
            vendIDMethod.meterIdentifier = meter;
            CreditVendReq request = new CreditVendReq();
            XMLVendService21 sageWood = new XMLVendService21();
            request.clientID = deviceId;
            request.msgID = msg;
            request.authCred = authCred;
            request.terminalID = genericDeviceID;
            request.idMethod = vendIDMethod;
            request.resource = electricity;
            request.purchaseValue = purchaseValue;
            request.payType = pay;
            rawRequest = XmlCreditVend(request);
            CreditVendResp response = sageWood.CreditVendRequest(request);
            Tx tx = response.creditVendReceipt.transactions.tx[0];
            CreditVendTx creditVendTx = (CreditVendTx)tx;
            CreditVendReceipt resp = response.creditVendReceipt;
            STS1Token sts1Token = (STS1Token)creditVendTx.creditTokenIssue.token;
            woodResponse.Token = sts1Token.stsCipher;
            woodResponse.ReceiptNo = response.creditVendReceipt.receiptNo;
            woodResponse.Units = creditVendTx.creditTokenIssue.units.value.ToString();
            string rawResponse = XmlCreditVendResp(response);

            //log raw request and response
            dh.LogRequestAndResponse("SAGEWOOD", transactionId, rawRequest, rawResponse);
        }
        catch (SoapException soapException)
        {
            // XMLVendFaultResp xmlVendFaultResp = XMLDeserialize.Deserialize(soapException.Detail);

            // if (xmlVendFaultResp.fault.GetType() ==
            //typeof(UnknownMeterEx))
            // {
            //     Console.WriteLine("Unknown Meter Serial Number!");
            // }
            // else
            // {
            //     string fert = xmlVendFaultResp.fault.GetType().ToString();
            //     Console.WriteLine("Unexpected XMLVend Fault Response" +
            //                 "received: (" + xmlVendFaultResp.fault.GetType().ToString()
            //     + ")" + xmlVendFaultResp.fault.desc);
            // }
            dh.LogError(soapException.Detail.InnerXml, transactionId, DateTime.Now, "SAGEWOOD");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
            dh.LogError(ex.ToString(), transactionId, DateTime.Now, "SAGEWOOD");
        }
        return woodResponse;
    }

    public string ToXML(CreditVendResp feedback)
    {
        using (var stringwriter = new System.IO.StringWriter())
        {
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }
    }

    public int GenerateRandomNumber()
    {
        Random random = new Random();
        int randomNumber = random.Next(000000, 999999);
        return randomNumber;
    }

    public string XmlCreditVend(Object der)
    {
        var xml = "";
        try

        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(CreditVendReq));
            var subReq = der;


            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }
        catch (Exception ert)
        {
            throw ert;
        }
    }

    public string XmlConfirmCustomerReq(Object der)
    {
        var xml = "";
        try

        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ConfirmCustomerReq));
            var subReq = der;


            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }
        catch (Exception ert)
        {
            throw ert;
        }
    }

    public string XmlCreditVendResp(Object der)
    {
        var xml = "";
        try

        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(CreditVendResp));
            var subReq = der;


            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }
        catch (Exception ert)
        {
            throw ert;

        }
    }

    public string XmlConfirmCustomerResp(Object der)
    {
        var xml = "";
        try

        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ConfirmCustomerResp));
            var subReq = der;


            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }
        catch (Exception ert)
        {
            throw ert;

        }
    }
    
}