using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventDetection
{
     public class CodeForm : Form
        {
            private string code;

            public string Code
            {
                get { return code; }
                set { code = value; }
            }
     }
    public static class CodeFormFactory
{
        public static CodeForm FormCreate(Uri address, String checkingAddress)
        {
            WebBrowser loginBrowser = new WebBrowser();
            loginBrowser.Dock = DockStyle.Fill;
            loginBrowser.Name = "webBrowser";

            loginBrowser.ScrollBarsEnabled = false;
            loginBrowser.TabIndex = 0;
            loginBrowser.Url = address;
            loginBrowser.ScriptErrorsSuppressed = false;



            CodeForm form = new CodeForm();
            form.WindowState = FormWindowState.Maximized;
            form.Controls.Add(loginBrowser);
            form.Name = "Browser";

            loginBrowser.Navigated += (object sender, WebBrowserNavigatedEventArgs e) =>
            {
                if (loginBrowser.Url.ToString().IndexOf(checkingAddress) == 0)
                {
                    var urlParams = System.Web.HttpUtility.ParseQueryString(e.Url.Query);
                    form.Code = urlParams.Get("code");
                    form.Close();
                }

            };

            return form;
}
}

        }

