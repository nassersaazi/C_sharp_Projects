using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

/// <summary>
/// Summary description for TotalLogic
/// </summary>
public class TotalLogic
{
    string certPath = @"E:\Certificates\TOTAL\mtn-uganda.2033.pfx";
    string certPassword = "totalcard";
    string Url = "https://mswebservices.infratotal.net:11443/tomcard";
    string xmlString = "";

    public TotalCardValidationResponse ValidateCard(string cardNo)
    {
        TotalCardValidationResponse response = new TotalCardValidationResponse();

        try
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(Url);
            Req.Headers.Add(@"SOAPAction:http://tempuri.org/IServiceEfuel/GetCardInformation");
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            Req.Method = "POST";
            X509Certificate2 cert = new X509Certificate2(certPath, certPassword, X509KeyStorageFlags.MachineKeySet);
            Req.ClientCertificates.Add(cert);

            XmlDocument SOAPReqBody = new XmlDocument();
            SOAPReqBody.LoadXml("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:tem=\"http://tempuri.org/\" xmlns:efu=\"http://schemas.datacontract.org/2004/07/EfuelWebServices.Contracts\">" +
                                   "<soapenv:Header/>" +
                                    "<soapenv:Body>" +
                                        "<tem:GetCardInformation>" +
                                            "<tem:CardInformation>" +
                                                "<efu:CardIDList>" + cardNo + "</efu:CardIDList>" +
                                            "</tem:CardInformation>" +
                                        "</tem:GetCardInformation>" +
                                    "</soapenv:Body>" +
                                "</soapenv:Envelope>");

            using (Stream stream = Req.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            //Geting response from request  
            using (WebResponse Serviceres = Req.GetResponse())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    string ServiceResult = rd.ReadToEnd();
                    xmlString = ServiceResult;
                }
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            try
            {
                XmlNodeList holder = doc.GetElementsByTagName("a:HolderName");

                if (!(holder.Count > 0))
                {
                    response.StatusCode = "100";
                    response.StatusDescription = "INVALID CARD DETAILS";
                }
                else
                {
                    response.StatusCode = "00";
                    response.HolderName = holder[0].InnerText.ToString();
                    response.StatusDescription = "SUCCESS";
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = "100";
                response.StatusDescription = ex.Message;
            }
        }
        catch (WebException webex)
        {
            string feedback = "";
            try
            {
                using (Stream stream = webex.Response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    feedback = reader.ReadToEnd();
                }
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(feedback);

                XmlNodeList _status_code = doc.GetElementsByTagName("ReturnCode");

                response.StatusCode = _status_code[0].InnerText.ToString();
                response.StatusDescription = GetErrorDescription(response.StatusCode);

            }
            catch (Exception ex)
            {
                response.StatusCode = "100";
                response.StatusDescription = webex.Message;
            }
        }
        catch (Exception exc)
        {
            response.StatusCode = "100";
            response.StatusDescription = exc.Message;
        }
        return response;
    }

    internal string GetErrorDescription(string errorCode)
    {
        string error = "";
        if (errorCode == "EX_DEFAULT_10001")
        {
            error = "An error occurred.";
        }
        else if (errorCode == "EX_UNK_10001")
        {
            error = "Unknown Error. Unable to execute the action.";
        }
        else if (errorCode == "EX_AUTH_10001")
        {
            error = "An exception occurred during authentication.";
        }
        else if (errorCode == "EX_AUTH_10002")
        {
            error = "Security information not available.";
        }
        else if (errorCode == "EX_AUTH_10003")
        {
            error = "Missing information.";
        }
        else if (errorCode == "EX_AUTH_10004")
        {
            error = "Missing Token.";
        }
        else if (errorCode == "EX_AUTH_10005")
        {
            error = "Invalid Token.";
        }
        else if (errorCode == "EX_AUTH_10006")
        {
            error = "No symmetric key found for the session.";
        }
        else if (errorCode == "EX_AUTH_10007")
        {
            error = "The maximum number of login failures reached.";
        }
        else if (errorCode == "EX_AUTH_10008")
        {
            error = "Validating authenticating user failed.";
        }
        else if (errorCode == "EX_AUTH_10009")
        {
            error = "Security information are not matching.";
        }
        else if (errorCode == "EX_AUTH_10010")
        {
            error = "OneTimePassword must be set before calling this method.";
        }
        else if (errorCode == "EX_AUTH_10011")
        {
            error = "OneTimePassword is not valid. You must open a new session.";
        }
        else if (errorCode == "EX_AUTH_10012")
        {
            error = "The given token has expired.";
        }
        else if (errorCode == "EX_AUTH_10013")
        {
            error = "The user has no right for calling this method.";
        }
        else if (errorCode == "EX_CARD_10001")
        {
            error = "The card does not exist.";
        }
        else if (errorCode == "EX_CARD_10002")
        {
            error = "The card has no client.";
        }
        else if (errorCode == "EX_CARD_10003")
        {
            error = "The card is not a prepaid card.";
        }
        else if (errorCode == "EX_CARD_10004")
        {
            error = "The card is not valid.";
        }
        else if (errorCode == "EX_CARD_10005")
        {
            error = "The card does not allow indirect credit.";
        }
        else if (errorCode == "EX_CARD_10006")
        {
            error = "Credit card limit.";
        }
        else if (errorCode == "EX_CARD_10007")
        {
            error = "Transaction not allowed.";
        }
        else if (errorCode == "EX_CARD_10008")
        {
            error = "The card is disabled";
        }
        else if (errorCode == "EX_CARD_10009")
        {
            error = "The card is blocked.";
        }
        else if (errorCode == "EX_CARD_10010")
        {
            error = "The card has expired.";
        }
        else if (errorCode == "EX_CARD_10011")
        {
            error = "Invalid currency.";
        }
        else if (errorCode == "EX_CARD_10012")
        {
            error = "The card doesn’t match with the customer.";
        }
        else if (errorCode == "EX_CARD_10013")
        {
            error = "Missing card balance information.";
        }
        else if (errorCode == "EX_CARD_10014")
        {
            error = "Card type doesn’t exist.";
        }
        else if (errorCode == "EX_CARD_10015")
        {
            error = "Card name must not have null or exceed 100 characters.";
        }
        else if (errorCode == "EX_CARD_10016")
        {
            error = "Invalid secret code.";
        }
        else if (errorCode == "EX_CARD_10017")
        {
            error = "The card type does not contain a magnetic track.";
        }
        else if (errorCode == "EX_CARD_10018")
        {
            error = "Creating card failed.";
        }
        else if (errorCode == "EX_CARD_10019")
        {
            error = "Activating card failed.";
        }
        return error.ToUpper();
    }
}