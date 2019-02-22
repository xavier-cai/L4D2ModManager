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
    /// WindowSetValue.xaml
    /// </summary>
    public partial class WindowSetValue : Window
    {
        private object InitializeValue = null;
        public object InputValue = null;
        public Predicate<object> Verify = o => true;
        public string VerifyErrorMsg = null;

        public WindowSetValue()
        {
            InitializeComponent();
        }

        public void Initialize<T>(T value, string title, string valueName)
        {
            InitializeValue = value;
            Title = title;
            ctlLabel.Content = valueName;
        }

        public void Initialize<T>(T value)
        {
            Initialize(value, "", StringAdapter.GetResource("Value"));
        }

        private void InitialWindowSize(object sender, SizeChangedEventArgs e)
        {
            this.SizeChanged -= InitialWindowSize;
            ctlTextBox.Width = ctlAnchor.ActualWidth * 8;
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            bool valid = false;
            bool convert = true;
            object o = null;
            try
            {
                o = Newtonsoft.Json.JsonConvert.DeserializeObject(ctlTextBox.Text, InitializeValue.GetType());
            }
            catch
            {
                convert = false;
            }
            if (convert && Verify(o))
            {
                InputValue = o;
                valid = true;
                this.Close();
            }
            if (!valid)
            {
                var msg = StringAdapter.GetResource("Invalid_Value");
                if (VerifyErrorMsg != null)
                    msg += "\r\n" + VerifyErrorMsg;
                MessageBox.Show(msg);
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ctlTextBox.Text = Newtonsoft.Json.JsonConvert.SerializeObject(InitializeValue);
        }
    }
}
