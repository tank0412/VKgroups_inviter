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
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Begin(object sender, RoutedEventArgs e)
        {
            Begin(sender1);
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
                VkAuth(Auth[0], Auth[1], Convert.ToUInt64(Auth[2]));
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
            Settings scope = Settings.Friends;

            try
            {
                string captchaKey = "";
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

                try
                {
                    var users = vk.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
                    {
                    });
                    List<VkNet.Model.User> IDs = users.ToList();
                    foreach (VkNet.Model.User id in IDs)
                    {
                        System.Threading.Thread.Sleep(5000);
                        vk.Groups.Invite(GroupID, id.Id);
                        //System.Windows.MessageBox.Show(Convert.ToString(id.Id));
                    }
                }
                catch (VkNet.Exception.AccessDeniedException e)
                {
                    System.Windows.MessageBox.Show("Access denied: ");
                }
                catch (VkNet.Exception.CannotBlacklistYourselfException e)
                {
                    System.Windows.MessageBox.Show("Access denied: user should be friend");

                }
            }

            catch (VkNet.Exception.CaptchaNeededException cap)
            {
                PictureBox PictureBox1 = new PictureBox();
                PictureBox1.Show();

                string captchaUrl = cap.Img.ToString();

                PictureBox1.ImageLocation = captchaUrl;

                System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender_antigate;
                string captchaKey = textBox.Text;
            }
        }

    }
}
