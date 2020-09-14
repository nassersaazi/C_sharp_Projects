using MerchantListener.PegPayApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantListener.EntityObjects
{
    class TotalLogic
    {
        public TotalLogic()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public CardValidationResponse CallWebService(string cardNo)
        {
            CardValidationResponse resp = new CardValidationResponse();
            try
            {
                PegPay api = new PegPay();
                QueryRequest request = new QueryRequest();
                Response apiResponse = new Response();

                request.QueryField1 = cardNo;
                request.QueryField4 = "TOTAL";
                request.QueryField5 = "STANBIC_VAS";
                request.QueryField6 = "53P48KU262";
                apiResponse = api.QueryCustomerDetails(request);

                resp.StatusCode = apiResponse.ResponseField6;
                resp.HolderName = apiResponse.ResponseField2;
                resp.StatusDescription = apiResponse.ResponseField7;
            }
            catch (Exception ee)
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "An Error Occured";
            }



            return resp;
        }
    }

    public class CardValidationResponse
    {
        public string StatusCode;
        public string StatusDescription;
        public string HolderName;
    }
}

