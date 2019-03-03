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
    /// WindowClassify.xaml
    /// </summary>
    public partial class WindowClassify : Window
    {
        private class RegexItem
        {
            public Pair<string, string> Regex { get; private set; }
            public string Key
            {
                get => Regex.Key;
                set { if (value.Length > 0) Regex.Key = value; }
            }
            public string Pattern
            {
                get => Regex.Value;
                set => Regex.Value = value;
            }
            public RegexItem(Pair<string, string> regex)
            {
                Regex = regex;
            }
        }

        private class RuleItem
        {
            public Pair<string, string> Classify { get; private set; }
            public string Rule
            {
                get => Classify.Key;
                set => Classify.Key = value;
            }
            public string Target
            {
                get => Classify.Value;
                set => Classify.Value = value;
            }
            public RuleItem(Pair<string, string> classify)
            {
                Classify = classify;
            }
        }

        List<RegexItem> m_regexes;
        List<RuleItem> m_rules;

        public WindowClassify()
        {
            InitializeComponent();

            m_regexes = new List<RegexItem>();
            m_rules = new List<RuleItem>();
            LoadRegexes();
            LoadRules();
            ctlListViewRegexes.ItemsSource = m_regexes;
            ctlListViewRules.ItemsSource = m_rules;
        }

        private void LoadRegexes()
        {
            m_regexes.Clear();
            foreach (var regex in L4D2Type.CustomContentInstance.CustomRegex)
                m_regexes.Add(new RegexItem(regex));
        }

        private void LoadRules()
        {
            m_rules.Clear();
            foreach (var rule in L4D2Type.CustomContentInstance.CustomClassify)
                m_rules.Add(new RuleItem(rule));
        }

        private void OnButtonAddRegexClick(object sender, RoutedEventArgs e)
        {
            L4D2Type.CustomContentInstance.CustomRegex.Add(new Pair<string, string>("NewRegex", ""));
            LoadRegexes();
            ctlListViewRegexes.Items.Refresh();
        }

        private void OnButtonDeleteRegexClick(object sender, RoutedEventArgs e)
        {
            if (CtlExtension.ConfirmBox(StringAdapter.GetInfo("ConfirmDelete")))
            {
                var item = (sender as Button).DataContext as RegexItem;
                L4D2Type.CustomContentInstance.CustomRegex.Remove(item.Regex);
                m_regexes.Remove(item);
                ctlListViewRegexes.Items.Refresh();
            }
        }

        private void OnButtonAddRuleClick(object sender, RoutedEventArgs e)
        {
            L4D2Type.CustomContentInstance.CustomClassify.Add(new Pair<string, string>("NewLogic", "Category"));
            LoadRules();
            ctlListViewRules.Items.Refresh();
        }

        private void OnButtonDeleteRuleClick(object sender, RoutedEventArgs e)
        {
            if (CtlExtension.ConfirmBox(StringAdapter.GetInfo("ConfirmDelete")))
            {
                var item = (sender as Button).DataContext as RuleItem;
                L4D2Type.CustomContentInstance.CustomClassify.Remove(item.Classify);
                m_rules.Remove(item);
                ctlListViewRules.Items.Refresh();
            }
        }
    }
}
