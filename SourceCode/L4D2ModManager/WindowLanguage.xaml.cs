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
using System.Windows.Shapes;

namespace L4D2ModManager
{
    /// <summary>
    /// WindowLanguage.xaml
    /// </summary>
    public partial class WindowLanguage : Window
    {
        public object Init { get; set; } = null;
        public object Result { get; private set; } = null;
        public WindowLanguage()
        {
            InitializeComponent();
        }

        private void OnButtonOkClick(object sender, RoutedEventArgs e)
        {
            Result = (ctlComboBox.SelectedItem as ComboBoxItem).Tag;
            Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ctlComboBox.SelectedItem = ctlComboBox.Items[0];
            foreach (var item in ctlComboBox.Items)
            {
                if ((item as ComboBoxItem).Tag.Equals(Init))
                {
                    ctlComboBox.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
