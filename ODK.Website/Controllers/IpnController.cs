using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ODK.Umbraco.Payments;
using ODK.Umbraco.Web.Mvc;

namespace ODK.Website.Controllers
{
    // https://github.com/paypal/ipn-code-samples/blob/master/C%23/paypal_ipn_mvc.cs
    public class IpnController : OdkSurfaceControllerBase
    {
        private readonly PaymentService _paymentService;

        public IpnController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public HttpStatusCodeResult Receive(string appSecret)
        {
            //Store the IPN received from PayPal
            LogRequest(Request);

            //Fire and forget verification task
            Task.Run(() => VerifyTask(Request, appSecret));

            //Reply back a 200 code
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void VerifyTask(HttpRequestBase ipnRequest, string appSecret)
        {
            var verificationResponse = string.Empty;

            try
            {
                var verificationRequest = (HttpWebRequest)WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";
                var param = Request.BinaryRead(ipnRequest.ContentLength);
                var strRequest = Encoding.ASCII.GetString(param);

                //Add cmd=_notify-validate to the payload
                strRequest = "cmd=_notify-validate&" + strRequest;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                var streamOut = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(strRequest);
                streamOut.Close();

                //Send the request to PayPal and get the response
                var streamIn = new StreamReader(verificationRequest.GetResponse().GetResponseStream());
                verificationResponse = streamIn.ReadToEnd();
                streamIn.Close();

            }
            catch (Exception exception)
            {
                //Capture exception for manual investigation
            }

            ProcessVerificationResponse(verificationResponse, appSecret);
        }


        private void LogRequest(HttpRequestBase request)
        {
            // Persist the request values into a database or temporary data store
        }

        private void ProcessVerificationResponse(string verificationResponse, string appSecret)
        {
            if (verificationResponse.Equals("VERIFIED"))
            {
                if (Request["Payment_status"] == "Completed" && Guid.TryParse(Request["custom"], out Guid token))
                {
                    _paymentService.CompletePayment(token, appSecret);
                }

                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment
            }
            else if (verificationResponse.Equals("INVALID"))
            {
                //Log for manual investigation
            }
            else
            {
                //Log error
            }
        }
    }
}