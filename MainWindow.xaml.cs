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

namespace PracticaWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        object sender1;
        object sender2;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Begin(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Have not implemented yet!");
            Begin(sender1, sender2);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox textBox = (TextBox)sender;
            sender1 = sender;
            //MessageBox.Show(textBox.Text);
        }
        private void PasswordBox_TextChanged(object sender, RoutedEventArgs args)
        {
            //PasswordBox PasswordBox = (PasswordBox)sender;
            sender2 = sender;
            //MessageBox.Show(PasswordBox.Password);
        }
        private void Begin(object sender1, object sender2)
        {
            // MessageBox.Show("Have not implemented yet!");
            TextBox textBox = (TextBox)sender1;
            PasswordBox PasswordBox = (PasswordBox)sender2;
            MessageBox.Show(textBox.Text);
            MessageBox.Show(PasswordBox.Password);

        }
    }
}
