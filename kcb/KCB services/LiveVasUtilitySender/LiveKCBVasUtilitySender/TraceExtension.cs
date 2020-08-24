using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System.Web.Services.Protocols;
using System.Xml;

namespace Tester
{
    public class TraceExtension : SoapExtension
    {

        public static readonly ILog log = LogManager.GetLogger(typeof(TraceExtension));
        Stream _originalStream;
        Stream _workingStream;

        public override Stream ChainStream(Stream stream)
        {
            // ChainStream method is called twice in the lifecycle of the SOAP
            // message processing. BEFORE the actual web service operation is
            // invoked and AFTER it has completed. 
            //
            // In case of outgoing response, .Net framework initializes a stream
            // as an instance of type SoapExtensionStream which does NOT support
            // reading from but write operations only. Therefore, we will chain
            // a local stream instance which will be passed to for processing by
            // actual web service method and will be read from when web service
            // method finishes processing. 
            //
            // Therefore, we need to copy contents from original stream to
            // working stream instance before 
            // Once we have read outgoing SOAP message, we will write contents 
            // from working stream to the original (SoapExtensionStream) instance, 
            // for HTTP pipeline to return it to caller.


            // Store reference to incoming stream locally
            _originalStream = stream;

            // Create a new working stream to work with
            _workingStream = new MemoryStream();
            return _workingStream;
        }

        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
            // do nothing...
        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeDeserialize:
                    // Incoming message
                    Copy(_originalStream, _workingStream);
                    LogMessageFromStream(_workingStream, "BeforeDeserialize");
                    break;

                case SoapMessageStage.AfterDeserialize:
                    // Outgoing message
                    //LogMessageFromStream(this._workingStream, "AfterDeserialize");
                    //Copy(this._originalStream, this._workingStream);
                    break;

                case SoapMessageStage.BeforeSerialize:
                    // Incoming message
                    //Copy(_originalStream, _workingStream);
                    //LogMessageFromStream(_workingStream, "BeforeSerialize");
                    break;

                case SoapMessageStage.AfterSerialize:
                    // Outgoing message
                    LogMessageFromStream(this._workingStream, "AfterSerialize");
                    Copy(this._workingStream, this._originalStream);
                    break;
            }
        }

        private void LogMessageFromStream(Stream stream, string RequestType)
        {
            try
            {
                string soapMessage = string.Empty;

                // Just making sure again that we have got a stream which we 
                // can read from AND after reading reset its position 
                //------------------------------------------------------------
                if (stream.CanRead && stream.CanSeek)
                {
                    stream.Position = 0;

                    StreamReader rdr = new StreamReader(stream);
                    soapMessage = rdr.ReadToEnd() + Environment.NewLine;
                    string Title = Environment.NewLine + "-----" + RequestType + " at " + DateTime.Now + Environment.NewLine;
                    string whatToLog = Title + GetUserFriendlyXML(soapMessage) + Environment.NewLine;

                    if (whatToLog.Contains("ExecuteDataSet"))
                    {
                        //dont log
                    }
                    else
                    {
                        log.Info(whatToLog);
                    }

                    // IMPORTANT!! - Set the position back to zero on the original 
                    // stream so that HTTP pipeline can now process it
                    //------------------------------------------------------------
                    stream.Position = 0;
                }

            }
            catch (Exception e)
            {

            }
        }

        public String GetUserFriendlyXML(String XML)
        {
            String Result = XML;

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(XML);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException)
            {
            }
            finally
            {
                mStream.Close();
                writer.Close();
            }

            return Result;
        }

        private void Copy(Stream from, Stream to)
        {
            try
            {
                TextReader reader = new StreamReader(from);
                TextWriter writer = new StreamWriter(to);
                writer.Write(reader.ReadToEnd());
                writer.Flush();
            }
            catch (Exception ex)
            {
                log.Info(Environment.NewLine + "Copying of Stream Failed because of Exception :=" + ex.Message + Environment.NewLine);
            }
        }
    }

}
