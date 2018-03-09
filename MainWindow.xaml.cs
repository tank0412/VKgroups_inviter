using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet;
using VkNet.Enums.Filters;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using Anticaptcha_example.Api;
using Anticaptcha_example.Helper;

namespace PracticaWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        object sender1;
        object sender_antigate;
        object sender_Group_ID;
        long GroupID;
        int SuccessfulInvitesCount = 0;
        int UnsuccessfulInvitesCount = 0;
        int SuccessfulLogin = 0;
        int UnsuccessfulLogin = 0;
        int SuccessfulCaptcha = 0;
        int UnsuccessfulCaptcha = 0;
        Boolean TwoFactorAuthorization = true;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Begin(object sender, RoutedEventArgs e)
        {
            Begin(sender1);
        }
        private void GetAntigateBalance_Button(object sender, RoutedEventArgs e)
        {
            if (GetAntigateBalance() == null)
                System.Windows.MessageBox.Show("GetAntigateBalance() failed. ");
            else
                System.Windows.MessageBox.Show("Balance: " + GetAntigateBalance());
        }
        private void GetProgress_Button(object sender, RoutedEventArgs e)
        {
                System.Windows.MessageBox.Show
                     ("Число успешных приглашений: " + Convert.ToString(SuccessfulInvitesCount) + "\n"
                    + "Число неуспешных приглашений: " + Convert.ToString(UnsuccessfulInvitesCount) + "\n"
                    + "Число успешных авторизаций: " + Convert.ToString(SuccessfulLogin) + "\n"
                    + "Число неуспешных авторизаций: " + Convert.ToString(UnsuccessfulLogin) + "\n"
                    + "Число успешных разгадок капчи: " + Convert.ToString(SuccessfulCaptcha) + "\n"
                    + "Число неуспешных разгадок капчи: " + Convert.ToString(UnsuccessfulCaptcha) + "\n"
                    );
        }
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.RichTextBox textBox = (System.Windows.Controls.RichTextBox)sender1;
            sender1 = sender;

        }
        private void TextBox_TextChanged_Antigate(object sender, TextChangedEventArgs e)
        {
            sender_antigate = sender;
        }
        private void TextBox_TextChanged_Group_ID(object sender, TextChangedEventArgs e)
        {
            sender_Group_ID = sender;
        }
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            TwoFactorAuthorization = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFactorAuthorization = false;
        }
        private double? GetAntigateBalance() {
            if (sender_antigate is null)
            {
                System.Windows.MessageBox.Show("Значение ключа Antigate не было введено! ");
                return null;
            }
            System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)sender_antigate;
            var api = new ImageToText
            {
                ClientKey = textbox.Text
            };

            var balance = api.GetBalance();
            return balance;
        }
        private void Begin(object sender1)
        {
            int i = 0;
            String[] Auth = new string[3];
            System.Windows.Controls.RichTextBox richtextBox = (System.Windows.Controls.RichTextBox)sender1;
            TextRange textRange = new TextRange(richtextBox.Document.ContentStart, richtextBox.Document.ContentEnd);
            string data = textRange.Text;
            String[] substrings = data.Split('\n');
            String[] substrings2 = data.Split(':');
            foreach (var substring in substrings)
            {
                //MessageBox.Show(substring);
                String[] line = substring.Split(':');
                foreach (var substring2 in line)
                {
                   // MessageBox.Show(substring2);
                    Auth[i] = substring2;
                    i++;
                }
                i = 0;
                if (Auth[0] != "")
                {
                    try
                    {
                        VkAuth(Auth[0], Auth[1], Convert.ToUInt64(Auth[2]));
                    }
                    catch (System.FormatException)
                    {
                        System.Windows.MessageBox.Show("Входная строка имела неверный формат.");
                    }
                }
                Array.Clear(Auth, 0, Auth.Length);
        }



        }
        public void VkAuth(string login, string password, ulong ID) {
            if (sender_Group_ID is null) {
                System.Windows.MessageBox.Show("Значение Group ID не было введено! ");
                return;
            }
            System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)sender_Group_ID;
            GroupID = Convert.ToInt64(textbox.Text);

            Func<string> code = () =>
            {
                string value = Microsoft.VisualBasic.Interaction.InputBox("Please enter code:", "Code Request", "Enter code there");
                //MessageBox.Show(value);
                return value;
            };

            ulong appID = ID; // ID приложения, созданного в https://vk.com/apps
            var vk = new VkApi();
            long captchaSid = 0;
            string captchaKey = "";
            try
            {
                try
                {
                    if (TwoFactorAuthorization == true)
                    {
                        vk.Authorize(new ApiAuthParams
                        {
                            ApplicationId = appID,
                            Login = login,
                            Password = password,
                            Settings = Settings.All,
                            TwoFactorAuthorization = code,
                            CaptchaKey = captchaKey
                        }
                    );
                    }
                    else {
                        vk.Authorize(new ApiAuthParams
                        {
                            ApplicationId = appID,
                            Login = login,
                            Password = password,
                            Settings = Settings.All,
                            CaptchaKey = captchaKey
                        }
);
                    }
                }
                catch (VkNet.Exception.VkApiException)
                {
                    UnsuccessfulLogin++;
                    System.Windows.MessageBox.Show("Неуспешная авторизация!");
                    return;
                }
                /*
                catch (System.InvalidOperationException)
                 System.Windows.MessageBox.Show("Эта операция не поддерживается для относительных URI-адресов.");
                 */

                try
                {
                    SuccessfulLogin++;
                    var users = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
                    {
                    });
                    List<VkNet.Model.User> IDs = users.ToList();
                    foreach (VkNet.Model.User id in IDs)
                    {
                        SuccessfulInvitesCount++;
                        vk.Groups.Invite(GroupID, id.Id, captchaSid, captchaKey);
                        System.Threading.Thread.Sleep(5000);
                        //System.Windows.MessageBox.Show(Convert.ToString(id.Id));
                    }
                }
                catch (VkNet.Exception.AccessDeniedException e)
                {
                    System.Windows.MessageBox.Show("Access denied: ");
                    UnsuccessfulInvitesCount++;
                    SuccessfulInvitesCount--;
                }
                catch (VkNet.Exception.CannotBlacklistYourselfException e)
                {
                    System.Windows.MessageBox.Show("Access denied: user should be friend");
                    UnsuccessfulInvitesCount++;
                    SuccessfulInvitesCount--;

                }
            }

            catch (VkNet.Exception.CaptchaNeededException cap)
            {
                /*

                PictureBox PictureBox1 = new PictureBox();
                PictureBox1.Show();

                string captchaUrl = cap.Img.ToString();
                captchaSid = cap.Sid;

                PictureBox1.ImageLocation = captchaUrl;

                //System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender_antigate;

                string captchaKey = Microsoft.VisualBasic.Interaction.InputBox("Please enter captcha code:", "Captcha Code Request", "Enter captcha code there");
                */
                captchaSid = cap.Sid;
                if (sender_antigate is null)
                {
                    System.Windows.MessageBox.Show("Значение ключа Antigate не было введено! Не возможно отправить капчу на Antigate! ");
                    return;
                }
                if (GetAntigateBalance() == 0)
                {
                    System.Windows.MessageBox.Show("Нет средств на балансе Antigate");
                    return;
                }

                System.Windows.Controls.TextBox textbox2 = (System.Windows.Controls.TextBox)sender_antigate;
                var api = new ImageToText
                {
                    ClientKey = textbox2.Text,
                    FilePath = cap.Img.ToString()
                };
                if (!api.CreateTask())
                {
                    System.Windows.MessageBox.Show("API v2 send failed. " + api.ErrorMessage);
                    UnsuccessfulCaptcha++;

                }
                else if (!api.WaitForResult())
                {
                    System.Windows.MessageBox.Show("Could not solve the captcha.");
                    UnsuccessfulCaptcha++;
                }
                else
                {
                    //System.Windows.MessageBox.Show("Result: " + api.GetTaskSolution().Text);
                    string captchaUrl = api.GetTaskSolution().Text;
                    SuccessfulCaptcha++;
                }
            }
        }

    }
}
