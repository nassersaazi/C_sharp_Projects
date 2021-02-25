using System;
using System.Collections.Generic;
using System.Text;

namespace UtilMSQProcessor.EntityObjects
{
   public class SMS
    {
       private string message, phone, vendorTranId, mask, sent, sender, reference;

       public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

       public string VendorTranId
       {
           get
           {
               return vendorTranId;
           }
           set
           {
               vendorTranId = value;
           }
       }
       public string Phone
       {
           get
           {
               return phone;
           }
           set
           {
               phone = value;
           }
       }
       public string Mask
       {
           get
           {
               return mask;
           }
           set
           {
               mask = value;
           }
       }

       public string Sent
       {
           get
           {
               return sent;
           }
           set
           {
               sent = value;
           }
       }
       public string Sender
       {
           get
           {
               return sender;
           }
           set
           {
               sender = value;
           }
       }
       public string Reference
       {
           get
           {
               return reference;
           }
           set
           {
               reference = value;
           }
       }

    }
}
