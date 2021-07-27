using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UpdatesClient.Core.Network;
using UpdatesClient.Core.Network.Models.Request;
using UpdatesClient.Core.Network.Models.Response;
using UpdatesClient.Modules.Configs;
using UpdatesClient.Modules.Debugger;
using UpdatesClient.UI.Pages.Models.AuthModels;
using Res = UpdatesClient.Properties.Resources;

namespace UpdatesClient.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : UserControl
    {
        public delegate void AuthResult();
        public event AuthResult SignIn;

        public FormModel FormModel { get; set; }

        public Authorization()
        {
            InitializeComponent();

            FormModel = new FormModel
            {
                AuthModel = new AuthModel(),
                RegModel = new RegModel(),
                RecPswrdModel = new RecPswrdModel(),
                CurrentView = FormModel.View.SignIn
            };
            DataContext = FormModel;
        }

        private void Open_AuthPanel(object sender, RoutedEventArgs e)
        {
            FormModel.CurrentView = FormModel.View.SignIn;
            FormModel.AuthModel.Error = null;
        }
        private void Open_RegisterPanel(object sender, RoutedEventArgs e)
        {
            FormModel.CurrentView = FormModel.View.SignUp;
            FormModel.RegModel.Error = null;
        }
        private void Open_ForgotPassPanel(object sender, RoutedEventArgs e)
        {
            FormModel.CurrentView = FormModel.View.Recov;
            FormModel.RecPswrdModel.Error = null;
        }

        private async void Signin_Click(object sender, RoutedEventArgs e)
        {
            authPanel.IsEnabled = false;
            AuthModel auth = FormModel.AuthModel;
            auth.Error = "";

            Settings.RememberMe = auth.RememberMe;

            bool can = true;

            if (string.IsNullOrWhiteSpace(auth.Email))
            {
                auth.Error += Res.EmailEmpty + "\n";
                can = false;
            }
            else if (!Regex.IsMatch(auth.Email, @".+@.+\..+"))
            {
                auth.Error += Res.EmailInvalid + "\n";
                can = false;
            }
            if (string.IsNullOrEmpty(passwordBoxAuth.Password))
            {
                auth.Error += Res.PasswordEmpty + "\n";
                can = false;
            }
            auth.Error = auth.Error.Trim();

            if (can)
            {
                try
                {
                    ReqLoginModel model = new ReqLoginModel()
                    {
                        Email = auth.Email,
                        Password = passwordBoxAuth.Password
                    };
                    ResLoginModel ds = await Account.Login(model);

                    Settings.UserId = ds.Id;
                    Settings.UserToken = ds.Token;
                    Settings.Save();
                    SignIn?.Invoke();
                }
                catch (WebException we)
                {
                    auth.Error = GetError(we);
                }
                catch (Exception err)
                {
                    Logger.Error("Auth_Login", err);
                }
            }
            authPanel.IsEnabled = true;
        }
        private async void Signup_Click(object sender, RoutedEventArgs e)
        {
            registerPanel.IsEnabled = false;
            RegModel reg = FormModel.RegModel;
            reg.Error = "";

            bool can = true;

            if (string.IsNullOrWhiteSpace(reg.Email))
            {
                reg.Error += Res.EmailEmpty + "\n";
                can = false;
            }
            else if (!Regex.IsMatch(reg.Email, @".+@.+\..+"))
            {
                reg.Error += Res.EmailInvalid + "\n";
                can = false;
            }
            if (string.IsNullOrWhiteSpace(reg.Login))
            {
                reg.Error += Res.UsernameEmpty + "\n";
                can = false;
            }
            else if (reg.Login.Length < 2)
            {
                reg.Error += Res.UsernameLonger + "\n";
                can = false;
            }
            else if (reg.Login.Length > 32)
            {
                reg.Error += Res.UsernameShoter + "\n";
                can = false;
            }
            if (string.IsNullOrEmpty(passwordBoxReg.Password))
            {
                reg.Error += Res.PasswordEmpty + "\n";
                can = false;
            }
            else if (passwordBoxReg.Password.Length < 6)
            {
                reg.Error += Res.PasswordLonger + "\n";
                can = false;
            }
            reg.Error = reg.Error.Trim();

            if (can)
            {
                try
                {
                    ReqRegisterModel model = new ReqRegisterModel()
                    {
                        Email = reg.Email,
                        Name = reg.Login,
                        Password = passwordBoxReg.Password
                    };
                    ResRegisterModel ds = await Account.Register(model);
                    Open_AuthPanel(null, null);
                }
                catch (WebException we)
                {
                    reg.Error = GetError(we);
                }
                catch (Exception err)
                {
                    Logger.Error("Auth_Register", err);
                }
            }
            registerPanel.IsEnabled = true;
        }
        private async void Forgot_Click(object sender, RoutedEventArgs e)
        {
            forgotPassPanel.IsEnabled = false;
            RecPswrdModel rec = FormModel.RecPswrdModel;
            rec.Error = "";

            bool can = true;

            if (string.IsNullOrWhiteSpace(rec.Email))
            {
                rec.Error += Res.EmailEmpty + "\n";
                can = false;
            }
            else if (!Regex.IsMatch(rec.Email, @".+@.+\..+"))
            {
                rec.Error += Res.EmailInvalid + "\n";
                can = false;
            }
            rec.Error = rec.Error.Trim();

            if (can)
            {
                try
                {
                    ReqResetPassword model = new ReqResetPassword()
                    {
                        Email = rec.Email
                    };
                    await Account.ResetPassword(model);
                    await Task.Delay(200);
                    Open_AuthPanel(null, null);
                }
                catch (WebException we)
                {
                    rec.Error = GetError(we);
                }
                catch (Exception err)
                {
                    Logger.Error("Auth_ResetPassword", err);
                }
            }
            forgotPassPanel.IsEnabled = true;
        }

        private string GetError(WebException we)
        {
            if (we.Response != null)
            {
                string raw;
                using (StreamReader reader = new StreamReader(we.Response.GetResponseStream())) raw = reader.ReadToEnd();

                try
                {
                    JArray jObject = JArray.Parse(raw);
                    foreach (JToken par in jObject.Children())
                    {
                        string a = par.Value<string>("property");
                        raw = ((JProperty)par.Value<JToken>("constraints").First()).Value.ToString();
                    }
                }
                catch { }

                switch (raw)
                {
                    case "Login failed":
                        raw = Res.LoginFailed;
                        break;
                    case "The specified e-mail address already exists":
                        raw = Res.EmailExists;
                        break;
                    case "A user with the same name already exists":
                        raw = Res.UserExists;
                        break;
                }
                return raw;
            }
            return Res.Error;
        }
    }
}
