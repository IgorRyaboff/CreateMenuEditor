using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CreateMenuEditor
{
    /// <summary>
    /// Логика взаимодействия для WindowLicense.xaml
    /// </summary>
    public partial class WindowLicense : Window
    {
        public bool accepted = false;
        public WindowLicense()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            accepted = true;
            Close();
        }
    }
}
