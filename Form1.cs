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
        public Form1()
        {
            InitializeComponent();

        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            await Helper.AuthStatus();
            
        }
    }
}
