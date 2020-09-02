using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using InterConnect.MailApi;

/// <summary>
/// Summary description for SendMail
/// </summary>
public class SendMail
{

	public SendMail()
	{
	}


    public static void sendUserCredentialsToTheirEmail(string Email, string Password, string FirstName, string LastName, string email, string vendorCode)
    {

        if (!string.IsNullOrEmpty(email))
        {

            string Subject = "SMS PORTAL USER CREDENTIALS";
            string Message = "Dear <b>" + FirstName + " " + LastName + "</b>, <br/><br/>" +
                             "Your SMS Portal Account has been created. <br/><br/>" +
                             "Below are the details to access the SMS Web Portal.<br/><br/>" +
                              "URL: <a href='" + Credentials.APP_URL + "' target='blank'>" + Credentials.APP_URL + "</a><br/><br/>" +
                             "Username: <b>" + Email + "</b><br/><br/>" +
                             "Password: <b>" + Password + "</b><br/><br/>" +
                             "<b>Thank you..........</b><br/><br/>";

            sendEmailNoAttachment(vendorCode, FirstName, email, Subject, Message);

        }


    }


    public static void ResetUserCredentials(string UserName, string Password, string FirstName, string LastName, string email, string vendorCode)
    {

        if (!string.IsNullOrEmpty(email))
        {

            string Subject = "SMS PORTAL USER CREDENTIALS";
            string Message = "Dear <b>" + FirstName + " " + LastName + "</b>, <br/><br/>" +
                             "Your SMS Portal Account details have been reset. <br/><br/>" +

                             "URL: <a href='" + Credentials.APP_URL + "' target='blank'>" + Credentials.APP_URL + "</a><br/><br/>" +
                             "Username: <b>" + UserName + "</b><br/><br/>" +
                             "Password: <b>" + Password + "</b><br/><br/>" +
                             "<b>Thank you..........</b><br/><br/>";

            sendEmailNoAttachment(vendorCode, FirstName, email, Subject, Message);

        }


    }

    public static void sendEmailNoAttachment(String bankCode, String name, string email, string Subject, string Message)
    {

        Email mail = new InterConnect.MailApi.Email();
        EmailAddress addr = new EmailAddress();
        addr.Address = email;
        addr.Name = name;
        addr.AddressType = EmailAddressType.To;

        EmailAddress[] addresses = { addr };
        mail.MailAddresses = addresses;
        mail.From = bankCode;
        mail.Message = Message;
        mail.Subject = Subject;


        Messenger mapi = new Messenger();

        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        Result result = mapi.PostEmail(mail);

    }

    public static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }




}