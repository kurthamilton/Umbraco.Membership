using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ODK.Umbraco.Payments;
using ODK.Umbraco.Web.Mvc;
using ODK.Website.Models.Payments;

namespace ODK.Website.Controllers
{
    public class IpnController : OdkSurfaceControllerBase
    {
        private readonly PaymentService _paymentService;

        public IpnController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public HttpStatusCodeResult Receive()
        {
            PaypalNotification notification = new PaypalNotification(Request);
            LogRequest(notification);

            string verificationResponse = GetVerificationResponse(Request);

            ProcessVerificationResponse(notification, verificationResponse);

            //Reply back a 200 code
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // https://github.com/paypal/ipn-code-samples/blob/master/C%23/paypal_ipn_mvc.cs
        private string GetVerificationResponse(HttpRequestBase ipnRequest)
        {
            try
            {
                WebRequest verificationRequest = WebRequest.Create("https://ipnpb.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = HttpMethod.Post.Method;
                verificationRequest.ContentType = "application/x-www-form-urlencoded";
                byte[] param = Request.BinaryRead(ipnRequest.ContentLength);
                string request = Encoding.ASCII.GetString(param);

                //Add cmd=_notify-validate to the payload
                request = "cmd=_notify-validate&" + request;
                verificationRequest.ContentLength = request.Length;

                //Attach payload to the verification request
                using (StreamWriter streamOut = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII))
                {
                    streamOut.Write(request);
                }

                //Send the request to PayPal and get the response
                using (StreamReader streamIn = new StreamReader(verificationRequest.GetResponse().GetResponseStream()))
                {
                    return streamIn.ReadToEnd();
                }
            }
            catch (Exception exception)
            {
                //Capture exception for manual investigation
                return string.Empty;
            }
        }

        private void LogRequest(PaypalNotification notification)
        {
            // Persist the request values into a database or temporary data store
        }

        private void ProcessVerificationResponse(PaypalNotification notification, string verificationResponse)
        {
            if (verificationResponse.Equals("VERIFIED"))
            {
                if (notification.PaymentStatus == PaymentStatus.Completed)
                {
                    _paymentService.CompletePayment(notification.Token, notification.CurrencyCode, notification.Amount);
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