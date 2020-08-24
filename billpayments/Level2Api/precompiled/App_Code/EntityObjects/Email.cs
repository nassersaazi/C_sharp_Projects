using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Collections.Generic;
using System.Net;
using System.Threading;

/// <summary>
/// Summary description for Email
/// </summary>
public class Email
{

    private const string smtpUsername = "emorutdeogratius@gmail.com";
    private const string smtpPassword = "derokevin@123";
    private const string smtpServer = "smtp.gmail.com";
    private const int smtpPort = 587;
    private static string[] orangeEmailAddresses ={ 
                                                    "nkasozi@gmail.com",
                                                    "brian.muhumuza@orange.co.ug",
                                                    "techsupport@pegasustechnologies.co.ug",
                                                    "agnes.nansubuga@orange.co.ug",
                                                    "simon.muwanga@orange.co.ug",
                                                    "Victoria.kisakye@orange.co.ug",
                                                    "Ann.nkangi@orange.co.ug", 
                                                    "Lydia.adoch@orange.co.ug", 
                                                    "Hussein.samanya@orange.co.ug"
                                                  };
    public string message = "";
    public string emailAddressTo = "";
    public static Thread emailSender;
    public UmemeTransaction recoveredTransaction;
    public string amountRecovered;

    public Email(UmemeTransaction trans, string amountRecovered)
    {
        this.recoveredTransaction = trans;
        this.amountRecovered = amountRecovered;
        emailSender = new Thread(DoWork);
    }

    //this generates the content of the orange email to be sent
    public Email[] GetEmailsToSend()
    {
        List<Email> emails = new List<Email>();
        foreach (string emailAddress in orangeEmailAddresses)
        {
            Email email = new Email(recoveredTransaction, amountRecovered);
            email.emailAddressTo = emailAddress;
            email.message = "Hi, Pegasus on behalf of Orange has recovered " + amountRecovered + " from "
                            + recoveredTransaction.CustName + " with meterNumber "
                            + recoveredTransaction.CustRef + " and phone Number "
                            + recoveredTransaction.CustomerTel + " on " + DateTime.Now;
            emails.Add(email);
        }
        return emails.ToArray();
    }

    //starts a thread to send emails ansychronously
    public void StartSendingEmails()
    {
        emailSender.Start();
    }

    public void DoWork()
    {
        SendEmailsToOrange(GetEmailsToSend());
    }

    public void SendEmailsToOrange(Email[] emails2Orange)
    {
        foreach (Email email in emails2Orange)
        {
            try
            {
                SendEmail(email);
            }
            catch (Exception)
            {
                //failure to send one email should not affect
                //sending of other emails
            }
        }
    }

    public static void SendEmail(Email email)
    {
        try
        {
            //BUILD EMAIL
            MailMessage message = new MailMessage();
            MailAddress toEmail = new MailAddress(email.emailAddressTo);
            message.To.Clear();
            message.To.Add(toEmail);
            message.Subject = "Orange Money Recovery Has Occurred.";
            message.Body = email.message;
            message.IsBodyHtml = true;
            message.From = new MailAddress(smtpUsername, "Orange Money Recovery");

            //I USE GMAIL AS THE SMTP SERVER..for more info google
            NetworkCredential cred = new NetworkCredential(smtpUsername, smtpPassword);
            SmtpClient mailClient = new SmtpClient(smtpServer, smtpPort);
            mailClient.EnableSsl = true;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.UseDefaultCredentials = false;
            mailClient.Timeout = 20000;
            mailClient.Credentials = cred;

            //SEND EMAIL
            mailClient.Send(message);
        }
        catch (Exception up)
        {
            throw up;
        }
    }
}
