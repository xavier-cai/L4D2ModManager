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
    /// WindowColors.xaml
    /// </summary>
    public partial class WindowColors : Window
    {
        class ColorRow
        {
            public Label Label { get; private set; }
            private Rectangle Button { get; set; }
            public Border Border { get; private set; }
            public TextBox TextBox { get; private set; }
            public Color Color { get; private set; }
            private Action<Color> Callback;

            public ColorRow(string text, Color color, Action<Color> callback)
            {
                Callback = callback;

                Label = new Label();
                Label.Content = text + " : ";
                Label.Margin = new Thickness(20, 5, 10, 5);

                Button = new Rectangle();
                Button.Width = 30;
                Button.Height = 20;
                Button.MouseUp += OnClick;
                Button.Cursor = Cursors.Hand;

                Border = new Border();
                Border.Width = 30;
                Border.Height = 20;
                Border.BorderThickness = new Thickness(1);
                Border.BorderBrush = new SolidColorBrush(Colors.Black);
                Border.Margin = new Thickness(10, 5, 10, 5);
                Border.Child = Button;

                TextBox = new TextBox();
                TextBox.VerticalContentAlignment = VerticalAlignment.Center;
                TextBox.Width = 85;
                TextBox.Height = 20;
                TextBox.MaxLength = 9;
                TextBox.LostFocus += OnTextBoxLostFocus;
                TextBox.Margin = new Thickness(10, 5, 20, 5);

                SetColor(color);
            }

            private void SetColor(Color color)
            {
                Color = color;
                Button.Fill = new SolidColorBrush(color);
                TextBox.Text = Newtonsoft.Json.JsonConvert.SerializeObject(color).Replace("\"", "");
            }

            private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
            {
                string text = TextBox.Text;
                object color = null;
                try
                {
                    color = Newtonsoft.Json.JsonConvert.DeserializeObject<Color>('"' + text + '"');
                }
                catch
                {
                    color = null;
                }
                if(color == null)
                {
                    MessageBox.Show(StringAdapter.GetResource("Invalid_Value") + "\r\n" + text);
                }
                else
                {
                    if (!Color.Equals((Color)color))
                    {
                        SetColor((Color)color);
                        Callback((Color)color);
                    }
                }
            }

            private void OnClick(object sender, RoutedEventArgs e)
            {
                var colorDialog = new System.Windows.Forms.ColorDialog();
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                colorDialog.ShowHelp = true;
                colorDialog.Color = System.Drawing.Color.White;
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var oriColor = colorDialog.Color;
                    Color newColor = Color.FromArgb(oriColor.A, oriColor.R, oriColor.G, oriColor.B);
                    if (!newColor.Equals(Color))
                    {
                        Callback(newColor);
                        SetColor(newColor);
                    }
                }
            }

            public void OnClosing()
            {
                OnTextBoxLostFocus(null, null);
            }
        }


        List<ColorRow> colors;

        public WindowColors()
        {
            InitializeComponent();
            FontSize = 14;

            colors = new List<ColorRow>();
            var AddColor = new Action<string, object, Color, Action<Color>>((key, o, color, action) =>
            {
                string content;
                if (o is L4D2MM.ModState)
                    content = o.GetString();
                else
                    content = StringAdapter.GetResource(o.ToString());
                colors.Add(new ColorRow(StringAdapter.GetResource(key) + '.' + content, color, action));
            });

            AddColor("Indicator", "Normal", Configure.View.IndicatorNormal, c => Configure.View.IndicatorNormal = c);
            AddColor("Indicator", "Ignored", Configure.View.IndicatorIgnore, c => Configure.View.IndicatorIgnore = c);
            AddColor("Indicator", "Conflicted", Configure.View.IndicatorCollision, c => Configure.View.IndicatorCollision = c);
            AddColor("State", L4D2MM.ModState.Unregisted, Configure.View.StateUnregisted, c => Configure.View.StateUnregisted = c);
            AddColor("State", L4D2MM.ModState.Unsubscribed, Configure.View.StateUnsubscribed, c => Configure.View.StateUnsubscribed = c);
            AddColor("State", L4D2MM.ModState.Miss, Configure.View.StateMiss, c => Configure.View.StateMiss = c);
            AddColor("State", L4D2MM.ModState.Off, Configure.View.StateOff, c => Configure.View.StateOff = c);
            AddColor("State", L4D2MM.ModState.On, Configure.View.StateOn, c => Configure.View.StateOn = c);

            int Col = 3;
            int Row = colors.Count;
            while (Col-- > 0)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength();
                ctlGrid.ColumnDefinitions.Add(cd);
            }
            while (Row-- > 0)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength();
                ctlGrid.RowDefinitions.Add(rd);
            }

            var GirdAction = new Action<UIElement, int, int>((ctl, col, row) =>
            {
                ctlGrid.Children.Add(ctl);
                Grid.SetColumn(ctl, col);
                Grid.SetRow(ctl, row);
            });
            foreach (var c in colors)
            {
                var row = colors.IndexOf(c);
                GirdAction(c.Label, 0, row);
                GirdAction(c.Border, 1, row);
                GirdAction(c.TextBox, 2, row);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var c in colors)
                c.OnClosing();
        }
    }
}
