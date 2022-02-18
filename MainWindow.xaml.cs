using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace CreateMenuEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowLicense windowLicense = new WindowLicense();
            windowLicense.ShowDialog();
            if (windowLicense.accepted)
            {
                InitializeComponent();
                Refresh();
            }
            else Close();
        }

        public void Refresh()
        {
            try
            {
                list.Items.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            string[] subkeys = Registry.ClassesRoot.GetSubKeyNames();
            foreach(string sub in subkeys)
            {
                if (sub[0] != '.') continue;
                RegistryKey shellNew = Registry.ClassesRoot.OpenSubKey(sub + '\\' + "ShellNew");
                if (shellNew != null)
                {
                    Item item = new Item(sub);
                    if (item.valid) list.Items.Add(new listItem(item));
                    shellNew.Close();
                }
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem == null) MessageBox.Show("Select an item to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                Item item = ((listItem)list.SelectedItem).item;
                if (item.IsDangerousToDelete())
                {
                    MessageBox.Show("Deleting this entry is dangerous and is not allowed by this program. If you really want to delete it, do it via registry yourself", "Protected extension", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (MessageBox.Show($"Are you sure you want to delete \"{item.displayName}\" ({item.extension}) from create menu?",
                    "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    item.Delete();
                    Refresh();
                }
            }
        }

        public bool windowAdd_addedNew = false;
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowAdd wa = new WindowAdd(this);
            wa.ShowDialog();
            if (windowAdd_addedNew) Refresh();
            windowAdd_addedNew = false;
        }

        private void BtnRestartExplorer_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c taskkill /f /im explorer.exe && explorer";
            process.Start();
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/IgorRyaboff/CreateMenuEditor");
        }
    }

    public class Item
    {
        public string extension;
        public string className;
        public string displayName;
        public string fileName = null;
        public bool valid = false;

        public Item(string ext)
        {
            extension = ext;
            RegistryKey rootKey = Registry.ClassesRoot.OpenSubKey(extension);
            if (rootKey == null) return;
            RegistryKey shellNewKey = rootKey.OpenSubKey("ShellNew");
            if (shellNewKey == null) return;
            if (shellNewKey.GetValue("FileName") != null) fileName = (string)shellNewKey.GetValue("FileName");

            className = (string)rootKey.GetValue("");
            if (className == null) return;
            RegistryKey displayNameClass = Registry.ClassesRoot.OpenSubKey(className);
            if (displayNameClass == null) return;
            if (displayNameClass.GetValue("") == null) return;
            displayName = (string)displayNameClass.GetValue("");
            rootKey.Close();
            displayNameClass.Close();
            Console.WriteLine(extension + ' ' + displayName);
            valid = true;
        }

        public static void Create(string extension, string displayName, string fileName, string existClassName)
        {
            RegistryKey rootKey = Registry.ClassesRoot.OpenSubKey(extension, true);
            if (rootKey == null) rootKey = Registry.ClassesRoot.CreateSubKey(extension);

            if (existClassName == null)
            {
                rootKey.SetValue("", $"CreateMenuEditor_{extension}");
                RegistryKey classKey = Registry.ClassesRoot.OpenSubKey($"CreateMenuEditor_{extension}", true);
                if (classKey == null) classKey = Registry.ClassesRoot.CreateSubKey($"CreateMenuEditor_{extension}");
                classKey.SetValue("", displayName);
                classKey.Close();
            }

            RegistryKey shellNew = rootKey.OpenSubKey("ShellNew");
            if (shellNew == null) shellNew = rootKey.CreateSubKey("ShellNew");
            if (fileName != null) shellNew.SetValue("FileName", fileName);
            else shellNew.SetValue("NullFile", "1");
            shellNew.Close();
            rootKey.Close();
        }

        public void Delete()
        {
            RegistryKey rootKey = Registry.ClassesRoot.OpenSubKey(extension, true);
            Console.WriteLine(string.Join(", ", rootKey.GetSubKeyNames()));
            rootKey.DeleteSubKeyTree("ShellNew");
            if (rootKey.GetSubKeyNames().Length == 0) Registry.ClassesRoot.DeleteSubKeyTree(extension);
            rootKey.Close();

            if (className == ("CreateMenuEditor_" + extension)) Registry.ClassesRoot.DeleteSubKey(className);
        }

        public bool IsDangerousToDelete()
        {
            string[] extensions = { ".lnk", ".library-ms" };
            foreach (string etx in extensions)
            {
                if (this.extension == etx) return true;
            }
            return false;
        }
    }
}
