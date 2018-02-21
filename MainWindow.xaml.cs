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
        object sender2;
        object sender_antigate;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Begin(object sender, RoutedEventArgs e)
        {
            Begin(sender1, sender2);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            sender1 = sender;
        }
        private void TextBox_TextChanged_Antigate(object sender, TextChangedEventArgs e)
        {
            sender_antigate = sender;
        }
        private void PasswordBox_TextChanged(object sender, RoutedEventArgs args)
        {
            sender2 = sender;
        }
        private void Begin(object sender1, object sender2)
        {
            TextBox textBox = (TextBox)sender1;
            PasswordBox PasswordBox = (PasswordBox)sender2;
            VkAuth(textBox, PasswordBox);

        }
        public void VkAuth(TextBox textBox, PasswordBox PasswordBox) {
            long groupid = 162108760;  //https://vk.com/club162108760
            long userid = 451472350; //https://vk.com/id451472350
            Func<string> code = () =>
            {
                string value = Microsoft.VisualBasic.Interaction.InputBox("Please enter code:", "Code Request", "Enter code there");
                MessageBox.Show(value);
                return value;
            };

            ulong appID = 6374736; // ID приложения, созданного в https://vk.com/apps
            var vk = new VkApi();
            Settings scope = Settings.Friends;
            vk.Authorize(new ApiAuthParams {
                ApplicationId = appID,
                Login = textBox.Text,
                Password = PasswordBox.Password,
                Settings = Settings.All,
                TwoFactorAuthorization = code
            }
            );
            MessageBox.Show(Convert.ToString(vk.UserId.Value));

           // vk.Groups.Invite(groupid, userid);
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

            //var records = vk.Audio.Get(vk.UserId.Value); // получаем список треков текущего пользователя
            //var records = vk.Audio.Get

            //MessageBox.Show("Records count: " + records.Count);
        }
    }
}
