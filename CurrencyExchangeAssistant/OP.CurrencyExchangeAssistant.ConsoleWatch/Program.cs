using System;
using RestSharp;
using RestSharp.Serializers;

namespace OP.CurrencyExchangeAssistant.ConsoleWatch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var exchangeFluctuationFromService = GetExchangeFluctuationFromService();
            if (exchangeFluctuationFromService.Result)
            {
                SendSms(exchangeFluctuationFromService);
            }

            Console.ReadKey();
        }

        private static ServiceResponse GetExchangeFluctuationFromService()
        {
            var client = new RestClient("http://localhost/exchange");
            var request = new RestRequest("api/BNRExchange/CheckIfANotableFluctuationHappenedToday?fluctuationThreshold=0.03", Method.GET);
            RestResponse<ServiceResponse> response = (RestResponse<ServiceResponse>) client.Execute<ServiceResponse>(request);

            return response.Data;
        }

        private static void SendSms(ServiceResponse exchangeFluctuationFromService)
        {
            var client = new RestClient("http://smsgateway.me");
            var request = new RestRequest("/api/v3/messages/send", Method.POST);
            //configuration data for the sms service
            request.AddParameter("message", new JsonSerializer().Serialize(exchangeFluctuationFromService));


            client.ExecuteAsync(request, response => { Console.WriteLine(response.Content); });
        }

        public class ServiceResponse
        {
            public bool Result { get; set; }
            public int Id { get; set; }
            public object Exception { get; set; }
            public int Status { get; set; }
            public bool IsCanceled { get; set; }
            public bool IsCompleted { get; set; }
            public int CreationOptions { get; set; }
            public object AsyncState { get; set; }
            public bool IsFaulted { get; set; }
        }
    }
}
