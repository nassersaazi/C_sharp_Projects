using System;
using System.IO;
using System.Net;
using System.Web.Services.Protocols;
using ConsoleApplication1.ControlObjects;


// Define a SOAP Extension that traces the SOAP request and SOAP
// response for the XML Web service method the SOAP extension is
// applied to.

    public class TraceExtension : SoapExtension
    {
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

        public override object GetInitializer(LogicalMethodInfo methodInfo, 
                               SoapExtensionAttribute attribute)
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
                    LogMessageFromStream(_workingStream,"SoapResponse");
                    break;

                case SoapMessageStage.AfterDeserialize:
                    break;

                case SoapMessageStage.BeforeSerialize:
                    break;

                case SoapMessageStage.AfterSerialize:
                    // Outgoing message
                    LogMessageFromStream(this._workingStream,"SoapRequest");
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
                    string Title = "-----" + RequestType + " at " + DateTime.Now + Environment.NewLine;
                    string whatToLog = Title + soapMessage;
                    //Procssor.log.Info(whatToLog);

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

        private void Copy(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.Write(reader.ReadToEnd());
            writer.Flush();
        }
    
    }


