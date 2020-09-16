using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class ProcessRequest
    {
        public void DetermineRequestType(string reuestOption)
        {
            string reqString = null;
            string GetTransStat = @"
                            <GetTransactionStatus><Username>EXTUSSDVENDOR</Username>         
                                 <Password>4hs56ikx</Password>      
                                   <TransactionId>21372211</TransactionId>    
                                       <MerchantCode>108633</MerchantCode> 
                                       <RequestId>21372211</RequestId> 
                                        <DigitalSignature>kS7W48loQWabNg2L3Espee8IfCUB8EfNao2KJ64g+L8y7rnbuZlxZint58HXL9+5uXM4D2uvpBAHYx3rOCHGdmWHcKJ1e3HVw+OKUBquDoVWT6MZfCgnyhv2fT7xTbjV0jBP9ULid6LpWYjBHlVewIP6KCgWw+vCEpCyuvAMGZLEgOHlk/P2orqp3/sJ51aPReJOfMATpJk3VBFSxYfHcS0QezXOCZ6QzlOPF/9h7Pl7K7LZJp2NQbrVDuX16kfo9qluQj3Sf0l69EWiPHpwoXG8GnghSIPVnJFQkQBMNkcFBFt6x4G8wAZolu77kGdJ5s3zdRFokG5SX7wYEubfFA== </DigitalSignature>
                                    <Channel>USSD</Channel>     
                             </GetTransactionStatus>
                            ";
            string MakeMPay = @"<MakeMerchantPayment>    
                                <Username>EXTUSSDVENDOR</Username>   
                                <Password>4hs56ikx</Password>        
                                <DigitalSignature>
                                px7obVBRlMXEPFjmz6jnapCh04xYFdgRXykTZO0GhKwZBari1qiyRKTt+QDmuYyb2r4VoiATo1CtllbMJUGWFgabut/WWYvgd889Hya0zwBZI5q2Y/8sLNciQXpTsugnDsC3SOXjWosSrF+W4CeMf9xWE2IPkPAz7Kfe/reMLkKxZDKiN2smFvD1ry3PoPE0URxMX49HMXxbbVPOpxrzcxwq0PgZCV0hKMFQByzpi1fqpO1xTbAiPFCF5USB0M3r4Z2nK36FisGnnWts82WlGe+wYptROeVgnJrjoInfINcPBBhYzk3W8Gm1S7UpqUxgTcjlIsipnZ3+tqo8cKPtDA==
                                </DigitalSignature>    
                                <Amount>500</Amount>     
                                <CustomerTel>256773826678</CustomerTel>      
                                <Channel>USSD</Channel>      
                                <MerchantCode>108633</MerchantCode>   
                                <CustomerRef>10083773544</CustomerRef>    
                                <Narration>Payment for goods</Narration>    
                                <RequestId>2137221199</RequestId>    
                                <TransactionDate>09/09/2020 11:06:31</TransactionDate>    
                                <RequestField1></RequestField1>   
                                <RequestField2></RequestField2>
                                <RequestField3></RequestField3>   
                                </MakeMerchantPayment>";
            string ValidCustREf = @" 
                                <ValidateCustomerRef>    
                                  <Username>EXTUSSDVENDOR</Username>
                                 <Password>4hs56ikx</Password>
                                    <MerchantCode>108633</MerchantCode>
                                    <Channel>USSD</Channel>
                                   <CustomerRef>40002</CustomerRef>
                                   <DigitalSignature>kS7W48loQWabNg2L3Espee8IfCUB8EfNao2KJ64g+L8y7rnbuZlxZint58HXL9+5uXM4D2uvpBAHYx3rOCHGdmWHcKJ1e3HVw+OKUBquDoVWT6MZfCgnyhv2fT7xTbjV0jBP9ULid6LpWYjBHlVewIP6KCgWw+vCEpCyuvAMGZLEgOHlk/P2orqp3/sJ51aPReJOfMATpJk3VBFSxYfHcS0QezXOCZ6QzlOPF/9h7Pl7K7LZJp2NQbrVDuX16kfo9qluQj3Sf0l69EWiPHpwoXG8GnghSIPVnJFQkQBMNkcFBFt6x4G8wAZolu77kGdJ5s3zdRFokG5SX7wYEubfFA==</DigitalSignature>
                                     <RequestId>21372211</RequestId>
                               </ValidateCustomerRef>";

            string ValidateMerch = @"
                                    <ValidateMerchant>
                                    <Username>STANBIC</Username>
                                     <Password>EO8DSAK</Password> 
                                     <MerchantCode>108633</MerchantCode>   
                                     <Channel>USSD</Channel> 
                                     <DigitalSignature>kS7W48loQWabNg2L3Espee8IfCUB8EfNao2KJ64g+L8y7rnbuZlxZint58HXL9+5uXM4D2uvpBAHYx3rOCHGdmWHcKJ1e3HVw+OKUBquDoVWT6MZfCgnyhv2fT7xTbjV0jBP9ULid6LpWYjBHlVewIP6KCgWw+vCEpCyuvAMGZLEgOHlk/P2orqp3/sJ51aPReJOfMATpJk3VBFSxYfHcS0QezXOCZ6QzlOPF/9h7Pl7K7LZJp2NQbrVDuX16kfo9qluQj3Sf0l69EWiPHpwoXG8GnghSIPVnJFQkQBMNkcFBFt6x4G8wAZolu77kGdJ5s3zdRFokG5SX7wYEubfFA==</DigitalSignature><RequestID>21372211</RequestID>   
                                    <RequestId>21372211</RequestId>    
                                    </ValidateMerchant>";

            string GetCharge = @"<GetMerchantCharge>     
                                 <Username>EXTUSSDVENDOR</Username>  
                                 <Password>4hs56ikx</Password>    
                                 <MerchantCode>108633</MerchantCode>   
                                 <Amount>87800</Amount>    
                                 <Channel>USSD</Channel>      
                                 <RequestId>21372211</RequestId>     
                                 <DigitalSignature>kS7W48loQWabNg2L3Espee8IfCUB8EfNao2KJ64g+L8y7rnbuZlxZint58HXL9+5uXM4D2uvpBAHYx3rOCHGdmWHcKJ1e3HVw+OKUBquDoVWT6MZfCgnyhv2fT7xTbjV0jBP9ULid6LpWYjBHlVewIP6KCgWw+vCEpCyuvAMGZLEgOHlk/P2orqp3/sJ51aPReJOfMATpJk3VBFSxYfHcS0QezXOCZ6QzlOPF/9h7Pl7K7LZJp2NQbrVDuX16kfo9qluQj3Sf0l69EWiPHpwoXG8GnghSIPVnJFQkQBMNkcFBFt6x4G8wAZolu77kGdJ5s3zdRFokG5SX7wYEubfFA==</DigitalSignature>    
                                 </GetMerchantCharge>
";


            if (reuestOption.Contains("1"))
            {
                reqString = ValidateMerch;
            }
            else if (reuestOption.Contains("2"))
            {
                reqString = ValidCustREf;
            }
            else if (reuestOption.Contains("3"))
            {
                reqString = GetCharge;
            }
            else if (reuestOption.Contains("4"))
            {
                reqString = MakeMPay;
            }
            else if (reuestOption.Contains("5"))
            {
                reqString = GetTransStat;
            }
            Program pr =new Program();
            pr.ProcessTransaction(reqString);
            Console.ReadLine();
        }
    }
}
