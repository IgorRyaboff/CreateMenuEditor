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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CreateMenuEditor
{
    /// <summary>
    /// Логика взаимодействия для listItem.xaml
    /// </summary>
    public partial class listItem : UserControl
    {
        public Item item;
        public listItem(Item item)
        {
            InitializeComponent();
            tbExt.Text = item.extension;
            tbDisplayName.Text = item.displayName;
            tblFileName.Text = item.fileName == null ? "Empty file" : item.fileName;
            this.item = item;
        }
    }
}
