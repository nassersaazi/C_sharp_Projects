using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UtilityReferences.WenrecoApi;

namespace Tester
{
    class XMLDeserialize
    {
        public static XMLVendFaultResp Deserialize(XmlNode faultDetail)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLVendFaultResp));

            MemoryStream memoryStream = new MemoryStream(UnicodeEncoding.UTF8.GetBytes(faultDetail.InnerXml));
            object xmlVendFaultResp = serializer.Deserialize(memoryStream);

            return (XMLVendFaultResp)xmlVendFaultResp;
        }
    }
}
