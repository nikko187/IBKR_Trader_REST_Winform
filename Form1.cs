using System.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace IBKR_Trader_REST
{
    public partial class Form1 : Form
    {
        // ##### GLOBAL VARIABLES ##### //
        public static Uri BaseUri = new Uri(baseURL);
        public const string baseURL = "https://localhost:5000/v1/api";
        public const string routeTickle = "/tickle";    //POST
        public const string routeSymbolSearch = "/iserver/secdef/search"; //POST
        public const string routeAuthStatus = "/iserver/auth/status"; //POST
        public const string routeValidate = "/sso/validate"; //GET
        public const string routeReauthenticate = "/iserver/reauthenticate"; //POST
        public static HttpClientHandler handler = new HttpClientHandler();
        public static HttpClient client = new HttpClient(handler);

        public Form1()
        {
            InitializeComponent();

        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            var result = await AuthStatus();
            MessageBox.Show(result.ToString());
        }

        public static async Task<AuthStatus?> AuthStatus()
        {
            try
            {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                client.DefaultRequestHeaders.Add("User-Agent", "Console");

                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("", ""));
                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                var request = new HttpRequestMessage(HttpMethod.Post, baseURL + routeAuthStatus)
                {
                    Method = HttpMethod.Post,
                    Content = content
                };

                var response = await client.SendAsync(request);

                string result = response.Content.ReadAsStringAsync().Result;
                var newResult = JsonConvert.DeserializeObject<AuthStatus>(result);
                return newResult;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        public static async Task<Validate> Validation()
        {
            try
            {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                client.DefaultRequestHeaders.Add("User-Agent", "Console");

                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("symbol", "SPY"));
                postData.Add(new KeyValuePair<string, string>("name", "true"));
                postData.Add(new KeyValuePair<string, string>("secType", "STK"));
                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                var request = new HttpRequestMessage(HttpMethod.Post, baseURL + routeValidate)
                {
                    Method = HttpMethod.Post,
                    Content = content
                };
                var response = await client.SendAsync(request);
                string result = response.Content.ReadAsStringAsync().Result;
                var newResult = JsonConvert.DeserializeObject(result);

                return (Validate)newResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
