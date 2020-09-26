using Microsoft.Win32;
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
    /// Логика взаимодействия для WindowAdd.xaml
    /// </summary>
    public partial class WindowAdd : Window
    {
        MainWindow mainWindow;
        public WindowAdd(MainWindow mw)
        {
            InitializeComponent();
            mainWindow = mw;
        }

        private void RbtnIsTemplate_Checked(object sender, RoutedEventArgs e)
        {
            fileTemplateGrid.Visibility = Visibility.Visible;
        }

        private void RbtnIsTemplate_Unchecked(object sender, RoutedEventArgs e)
        {
            fileTemplateGrid.Visibility = Visibility.Hidden;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string ext = tbxExt.Text;
            if (ext.IndexOf('.') > 0)
            {
                MessageBox.Show("Incorrect extension - dot should be before extension", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ext.IndexOf('.') == -1) ext = '.' + ext;

            string dn = tbxDisplayName.Text;
            if (dn.Length == 0)
            {
                MessageBox.Show("Display name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fn = null;
            if ((bool)rbtnIsTemplate.IsChecked)
            {
                fn = tbxFileName.Text;
                if (fn.Length == 0)
                {
                    MessageBox.Show("File name cannot be empty. If you want an empty file to be created, select \"Create empty file\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            RegistryKey rootKey = Registry.ClassesRoot.OpenSubKey(ext);
            if (rootKey != null)
            {
                RegistryKey classKey = Registry.ClassesRoot.OpenSubKey((string)rootKey.GetValue(""));
                if (classKey != null && classKey.GetValue("") != null && ((string)classKey.GetValue("")).Length != 0)
                {
                    switch (MessageBox.Show("This extension already has display name: " + classKey.GetValue("") + "\nYES - keep old display name; NO - overwrite it with new", "Extension class already exists",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                    {
                        case MessageBoxResult.Yes:
                            Item.Create(ext, null, fn, (string)classKey.GetValue(""));
                            mainWindow.windowAdd_addedNew = true;
                            Close();
                            break;
                        case MessageBoxResult.No:
                            Item.Create(ext, dn, fn, null);
                            mainWindow.windowAdd_addedNew = true;
                            Close();
                            break;
                    }
                }
                else
                {
                    Item.Create(ext, dn, fn, null);
                    mainWindow.windowAdd_addedNew = true;
                    Close();
                }
            }
            else
            {
                Item.Create(ext, dn, fn, null);
                mainWindow.windowAdd_addedNew = true;
                Close();
            }
        }
    }
}
