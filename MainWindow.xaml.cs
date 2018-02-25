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

namespace PracticaWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        object sender1;
        object sender_antigate;
        int N;
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
            RichTextBox textBox = (RichTextBox)sender1;
            sender1 = sender;

        }
        private void TextBox_TextChanged_Antigate(object sender, TextChangedEventArgs e)
        {
            sender_antigate = sender;
        }
        private void Begin(object sender1)
        {
            int i = 0;
            String[] Auth = new string[3];
            RichTextBox richtextBox = (RichTextBox)sender1;
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
        public void VkAuth(string login, string password, ulong ID ) {
            long groupid = 162108760;  //https://vk.com/club162108760
            long userid = 451472350; //https://vk.com/id451472350
            Func<string> code = () =>
            {
                string value = Microsoft.VisualBasic.Interaction.InputBox("Please enter code:", "Code Request", "Enter code there");
                MessageBox.Show(value);
                return value;
            };

            ulong appID = ID; // ID приложения, созданного в https://vk.com/apps
            var vk = new VkApi();
            Settings scope = Settings.Friends;
            vk.Authorize(new ApiAuthParams {
                ApplicationId = appID,
                Login = login,
                Password = password,
                Settings = Settings.All,
                TwoFactorAuthorization = code
            }
            );
           // MessageBox.Show(Convert.ToString(vk.UserId.Value));
            try
            {
                vk.Groups.Invite(groupid, userid);
            }
            catch (VkNet.Exception.AccessDeniedException e)
            {
                MessageBox.Show("Access denied: ");
            }
            catch (VkNet.Exception.CannotBlacklistYourselfException e)
            {
                MessageBox.Show("Access denied: user should be friend");
            }
        }

    }
}
