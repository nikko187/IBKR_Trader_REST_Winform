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
using WebSocketSharp;
using static System.Collections.Specialized.BitVector32;

namespace IBKR_Trader_REST
{
    public partial class Form1 : Form
    {
        // ##### GLOBAL VARIABLES ##### //
        public static Uri BaseUri = new Uri(baseURL);
        public static Uri SocketUri = new Uri(socketURL);
        public const string baseURL = "https://localhost:5000/v1/api";
        public const string socketURL = "wss://localhost:5000/v1/api/ws";
        public const string routeTickle = "/tickle";    //POST
        public const string routeSymbolSearch = "/iserver/secdef/search"; //POST
        public const string routeAuthStatus = "/iserver/auth/status"; //POST
        public const string routeValidate = "/sso/validate"; //GET
        public const string routeReauthenticate = "/iserver/reauthenticate"; //POST
        public static HttpClientHandler handler = new HttpClientHandler();
        public static HttpClient client = new HttpClient(handler);
        WebSocket ws = new WebSocket(socketURL);
        int contractID= 0;
        string companyName = "";
        string sessionID = "";

        public Form1()
        {
            InitializeComponent();

            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            client.DefaultRequestHeaders.Add("User-Agent", "Console");
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            var validate = await SSOValidation();
            lbData.Items.Add("Validated: " + validate);
            //var authResult = await AuthStatus();
            //lbData.Items.Add(authResult);

            var tickle = await Tickle();
            lbData.Items.Add("Session: " + tickle);

            await GetContractInfo();
            lbData.Items.Add("ConID: " + contractID);
            lbData.Items.Add("Name: " + companyName);

            await SocketStream(sessionID);
        }

        public async Task<string> AuthStatus()
        {
            try
            {
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("", ""));
                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                var request = new HttpRequestMessage(HttpMethod.Post, baseURL + routeAuthStatus)
                {
                    Method = HttpMethod.Post
                };
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var json = JsonConvert.DeserializeObject<AuthStatus>(result);
                    return json.authenticated.ToString();
                }
                else
                {
                    return response.StatusCode.ToString();
                    throw new Exception("Not Authenticated!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        public async Task<bool> SSOValidation()
        {
            try
            {
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("", ""));

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                var request = new HttpRequestMessage(HttpMethod.Get, baseURL + routeValidate)
                {
                    Method = HttpMethod.Get,
                    Content = content
                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var json = JsonConvert.DeserializeObject<Validate>(result);
                    return json.RESULT;
                }
                else
                {
                    throw new Exception("Session Validation failed: " + response.StatusCode);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        public async Task<string> Tickle()
        {
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("", ""));
            FormUrlEncodedContent contentTickle = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage(HttpMethod.Post, baseURL + routeTickle)
            {
                Method = HttpMethod.Post,
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var json = JsonConvert.DeserializeObject<Tickle>(result);
                sessionID = json.session;
                return json.session;

            }
            else
            {
                throw new Exception("Tickle Failure: " + response.StatusCode);
            }
        }
        public async Task<string> GetContractInfo()
        {
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("symbol", "SPY"));
            postData.Add(new KeyValuePair<string, string>("name", "true"));
            postData.Add(new KeyValuePair<string, string>("secType", "STK"));
            FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

            var request = new HttpRequestMessage(HttpMethod.Post, baseURL + routeSymbolSearch)
            {
                Method = HttpMethod.Post,
                Content = content
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var json = JsonConvert.DeserializeObject<List<SecDef>>(result);
                contractID = json[0].conid;
                companyName = json[0].companyName;
                return contractID.ToString() + companyName;
            }
            else
            {
                throw new Exception("Fail to get Contract Info :" + response.StatusCode);
            }
        }
        public async Task SocketStream(string session)
        {
            List<KeyValuePair<string, string>> websocket = new List<KeyValuePair<string, string>>();
            websocket.Add(new KeyValuePair<string, string>("session", session));
            FormUrlEncodedContent contentWS = new FormUrlEncodedContent(websocket);
           
            ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.None;
            ws.Connect();
            ws.Send("smd+" + contractID + "+{\"fields\":[\"31\", \"84\", \"86\"]}");
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;          
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                lbData.Items.Add(e.Data);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            try
            {
                lbData.Items.Add("## CONNECTION OPEN ##");
                lbData.Items.Add(e.ToString());
                ws.Send("smd+" + contractID + "+{\"fields\":[\"31\", \"84\", \"86\"]}");
            }
            catch (Exception ex) { Debug.WriteLine($"{ex.Message}"); }
        }
    
    }
}
