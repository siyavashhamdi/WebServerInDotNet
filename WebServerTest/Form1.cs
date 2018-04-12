using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebServerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var ws = new LocalWebServer("http://localhost:8080/", SendResponse);
            ws.Run();

            //Console.WriteLine("A simple webserver. Press a key to quit.");
            //Console.ReadKey();
            //ws.Stop();
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            return $"<HTML><BODY>My web page.<br>{DateTime.Now}</BODY></HTML>";
        }
    }
}
