using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace L4D2ModManager
{
    /// <summary>
    /// MainWindow.xaml : interact logic
    /// </summary>
    public partial class MainWindow : Window
    {
        private L4D2MM m_manager;
        private string m_displayedKey = "";
        private CategorySelecter m_categorySelected = null;
        //private bool m_enable = true;

        private Dictionary<string, bool> ReflectionCache = new Dictionary<string, bool>();
        private bool CheckModInfoReflection(string reflection)
        {
            if (reflection == null || reflection == "")
                return false;
            if (ReflectionCache.ContainsKey(reflection))
                return ReflectionCache[reflection];
            var ret = typeof(L4D2Mod).GetMembers().FirstOrDefault(member => member.Name.Equals(reflection) && ((member.MemberType & (System.Reflection.MemberTypes.Property)) != 0)) != null;
            ReflectionCache.Add(reflection, ret);
            Logging.Log("Reflection check for [" + reflection + "] is " + ret.ToString());
            return ret;
        }

        private Dictionary<GridViewColumn, string> SortReflectionMap = new Dictionary<GridViewColumn, string>(); 
        public MainWindow()
        {
            InitializeComponent();

            //UI
            this.FontSize = Configure.View.FontSize;
            ctlMenu.FontSize = Configure.View.FontSize;
            this.SetSize(Configure.View.WindowSize);

            foreach (var col in CustomInformation.Instance.ViewLists)
            {
                if (CheckModInfoReflection(col.Reflection))
                {
                    var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetValue(TextElement.ForegroundProperty, Brushes.Black);
                    textBlockFactory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Mod") { Converter = new StringDisplayConverter(), ConverterParameter = "Mod." + col.Reflection });
                    textBlockFactory.SetValue(TextBlock.TextAlignmentProperty, col.TextAlignment);

                    GridViewColumn column = new GridViewColumn();
                    column.Header = col.TryTranslate ? StringAdapter.GetResource(col.Header) : col.Header;
                    column.Width = col.Width;
                    column.CellTemplate = new DataTemplate { VisualTree = textBlockFactory };
                    ctlGridView.Columns.Add(column);

                    SortReflectionMap.Add(column, col.Reflection);
                }
            }

            //new
            m_manager = new L4D2MM();
            m_categorySelected = new CategorySelecter(ctlCategoryList, ctlSubcategoryList, ctlTextSearch, ctlListView);

            //initialize property
            ctlPrintText.AppendText(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " [version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ']');
            ctlProgressBar.SetValue(ProgressBar.ValueProperty, 100.0);
            InitializeListViewMenu();
            InitializeWindowMenu();
        }

        private void ProcessBarOperation(int index, int total, string info)
        {
            ctlProgressBar.Dispatcher.Invoke(new Action<DependencyProperty, object>(ctlProgressBar.SetValue), System.Windows.Threading.DispatcherPriority.Background, ProgressBar.ValueProperty, index * 100.0 / total);
            ctlProgressText.Dispatcher.Invoke(new Action<DependencyProperty, object>(ctlProgressText.SetValue), System.Windows.Threading.DispatcherPriority.Background, TextBox.TextProperty, info);
        }

        private void PrintOperation(string info)
        {
            Logging.Log(info, "Print");
            ctlPrintText.Dispatcher.Invoke(new Action<string>(ctlPrintText.AppendAndScroll), System.Windows.Threading.DispatcherPriority.Background
                , DateTime.Now.ToString("\r\n[HH:mm:ss] ") + info);
        }

        Dictionary<string, bool> _Enables = new Dictionary<string, bool>();
        bool _Enable = true;
        private void EnableOperation(string key, bool enable)
        {
            this.Dispatcher.Invoke(() => {
                bool old = true;
                if (_Enables.ContainsKey(key))
                    old = _Enables[key];
                else
                    _Enables.Add(key, enable);
                if (old != enable)
                {
                    _Enables[key] = enable;
                    if (_Enable != enable && !_Enables.ContainsValue(!enable))
                    {
                        _Enable = enable;
                        this.SetValue(MainWindow.IsEnabledProperty, enable);
                    }
                }
            }
            , System.Windows.Threading.DispatcherPriority.Send);
        }

        private void NotifyUpdate(string key, object obj)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (obj is L4D2MM.ModInfo && key == m_displayedKey)
                    LoadModInfo(obj as L4D2MM.ModInfo);
            }
            , System.Windows.Threading.DispatcherPriority.Background);
        }

        class DelegateResult
        {
            public string Result { get; set; }
            public string[] Mods { get; set; }
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Logging.Log("window load success");
            //this.Height += ctlProgressText.ActualHeight;

            //set up
            WindowCallbacks.SetProcessCallback(ProcessBarOperation);
            WindowCallbacks.SetPrintCallback(PrintOperation);
            WindowCallbacks.SetOperationEnableCallback(EnableOperation);
            WindowCallbacks.SetNotifyUpdateCallback(NotifyUpdate);
#if DEBUG
#else
            LoadModManager();
#endif
        }

        private void Exit()
        {
            Configure.SaveConfigure();
            m_manager.SaveIgnoreList();
            m_manager.SaveModState();
            L4D2Type.CustomContentInstance.SaveCustomContent();
            CustomInformation.Instance.Save();
            Logging.Log("good bye & see u");
            System.Environment.Exit(0);
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Exit();
        }

        private void LoadModManager()
        {
            new Thread(new ThreadStart(() =>
            {
                WindowCallbacks.OperationEnable(this.GetType().ToString(), false);
                if (!m_manager.LoadConfig())
                {
                    WindowCallbacks.Print(StringAdapter.GetInfo("CheckPath"));
                    WindowCallbacks.OperationEnable(this.GetType().ToString(), true);
                    return;
                }
                if (!m_manager.SetEnableSteam(Configure.EnableSteam))
                {
                    WindowCallbacks.Print(StringAdapter.GetInfo("NeedSteam"));
                    WindowCallbacks.OperationEnable(this.GetType().ToString(), true);
                    return;
                }
                m_manager.Initialize();
                Logging.Log("mod manager initialize success");
                m_categorySelected.Update(m_manager);
                WindowCallbacks.OperationEnable(this.GetType().ToString(), true);
            })).Start();
        }

        private L4D2MM.ModInfo LoadedModInfo = null;
        private void LoadModInfo(L4D2MM.ModInfo mod)
        {
            if (mod == null)
                return;
            LoadedModInfo = mod;
            var fontSize = this.FontSize;
            m_displayedKey = mod.Key;
            ctlFrameText.Document.Blocks.Clear();
            if (mod.Mod.ImageMemoryStream != null)
                ctlImage.SetSource(mod.Mod.GetAndResetImageMemoryStream());
            else
                ctlImage.SetSource(null);//or show the default image

            if (mod.Mod.Title.Length > 0)
            {
                Run run = new Run(mod.Mod.Title);
                run.FontSize = fontSize + 4;
                run.Foreground = new SolidColorBrush(Colors.Black);
                run.FontWeight = FontWeights.Bold;
                ctlFrameText.Document.Blocks.Add(new Paragraph(run));
            }
            if(mod.Mod.Category.Count > 0)
            {
                Run run = new Run(StringAdapter.GetResource("Category") + " : " + mod.Mod.Category.Aggregate("", (s, c) => s += ", " + c.ToString()).Substring(2));
                run.FontSize = fontSize;
                run.Foreground = new SolidColorBrush(Colors.Gray);
                run.FontWeight = FontWeights.Normal;
                ctlFrameText.Document.Blocks.Add(new Paragraph(run));
            }

            foreach (var box in CustomInformation.Instance.ViewBoxes)
            {
                if (CheckModInfoReflection(box.Reflection))
                {
                    string content = (box.TryTranslate ? StringAdapter.GetResource(box.Header) : box.Header) + " : \r\n";
                    string reflection = typeof(L4D2Mod).InvokeMember(box.Reflection, System.Reflection.BindingFlags.GetProperty, null, mod.Mod, null).Display();
                    if (reflection.Length > 0)
                    {
                        Run run = new Run(content + reflection);
                        run.FontSize = fontSize;
                        run.Foreground = new SolidColorBrush(box.Color);
                        run.FontWeight = FontWeights.Normal;
                        ctlFrameText.Document.Blocks.Add(new Paragraph(run));
                    }
                }
            }

            if (ctlFrameText.Document.Blocks.Count <= 0)
            {
                Run run = new Run('(' + StringAdapter.GetResource("No_Information") + ')');
                run.FontSize = fontSize;
                run.Foreground = new SolidColorBrush(Colors.Gray);
                run.FontWeight = FontWeights.Normal;
                ctlFrameText.Document.Blocks.Add(new Paragraph(run));
            }
        }

        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                ViewItem item = e.AddedItems[0] as ViewItem;
                if(m_manager.Mods.ContainsKey(item.Key))
                {
                    Logging.Log("view list select item : " + item.Key);
                    L4D2MM.ModInfo mod = m_manager.Mods[item.Key];
                    LoadModInfo(mod);
                }
             }
        }

        Func<string, MenuItem> FindListViewMenuItem;
        private void ListViewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ListView;
            var items = list.SelectedItems;
            if (items != null && items.Count > 0)
            {
                bool isMultiSelected = items.Count > 1;
                var selectedItem = items[0] as ViewItem;
                Action<string, bool> UpdateEnable = (name, enable) =>
                {
                    FindListViewMenuItem(name).IsEnabled = isMultiSelected || enable;
                };
                var mod = m_manager.Mods[selectedItem.Key];
                UpdateEnable("On", mod.CanSetOn);
                UpdateEnable("Off", mod.CanSetOff);
                UpdateEnable("Subscribe", mod.CanSubcribe);
                UpdateEnable("Unsubscribe", mod.CanUnsubscribe);
                UpdateEnable("Ignore_Collision", !mod.IsIgnoreCollision);
                UpdateEnable("Detect_Collision", mod.IsIgnoreCollision);
            }
        }

        int SortMethodId = 1;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                Comparison<ViewItem> method = null;
                if (headerClicked.Content.Equals(StringAdapter.GetResource("Col_ModId")))
                    method = (a, b) => a.ID.CompareTo(b.ID);
                else if (headerClicked.Content.Equals(StringAdapter.GetResource("Col_ModSource")))
                    method = (a, b) => a.Source.CompareTo(b.Source);
                else if (headerClicked.Content.Equals(StringAdapter.GetResource("Col_ModState")))
                    method = (a, b) => a.State.CompareTo(b.State);
                else if (headerClicked.Content.Equals(StringAdapter.GetResource("Col_ModSize")))
                    method = (a, b) => a.Mod.Mod.FileSize.CompareTo(b.Mod.Mod.FileSize);
                else if (SortReflectionMap.ContainsKey(headerClicked.Column))
                    method = (a, b) =>
                    {
                        var reflection = SortReflectionMap[headerClicked.Column];
                        var oa = typeof(L4D2Mod).InvokeMember(reflection, System.Reflection.BindingFlags.GetProperty, null, a.Mod.Mod, null);
                        var ob = typeof(L4D2Mod).InvokeMember(reflection, System.Reflection.BindingFlags.GetProperty, null, b.Mod.Mod, null);
                        if (oa is IComparable && ob is IComparable)
                            return (oa as IComparable).CompareTo(ob);
                        return oa.Display().CompareTo(ob.Display());
                    };
                else
                    return;

                if (method == null)
                    return;
                else
                {
                    if (SortMethodId.Equals(0))
                        m_categorySelected.Sort(null);
                    else
                        m_categorySelected.Sort((a, b) => SortMethodId * method(a, b));
                }
                SortMethodId += 1;
                if (SortMethodId > 1) SortMethodId = -1;
            }
        }

        private void InitializeListViewMenu()
        {
            ContextMenu menu = new ContextMenu();

            var menuItemConfirm = new Dictionary<string, Func<bool>>();
            var menuItemClick = new Dictionary<string, Func<L4D2MM.ModInfo, bool>>();
            RoutedEventHandler OnMenuItemClick = (o, e) =>
            {
                var name = (o as MenuItem).Name;
                if (!menuItemClick.ContainsKey(name))
                    return;
                if (menuItemConfirm.ContainsKey(name))
                    if (!menuItemConfirm[name]())
                        return;
                var items = ctlListView.SelectedItems;
                if (items == null || items.Count <= 0)
                    return;
                var func = menuItemClick[name];
                int count = 0;
                object[] copy = new object[items.Count];
                items.CopyTo(copy, 0);
                string firstName = (items[0] as ViewItem).Key;
                foreach (var item in copy)
                {
                    if (func((item as ViewItem).Mod))
                        count++;
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(StringAdapter.GetResource("Operation") + " : " + (o as MenuItem).Header as string + " : ");
                if (copy.Length > 1)
                    sb.Append(StringAdapter.GetResource("Success") + ' ' + count.ToString() + " / " + StringAdapter.GetResource("Selected") + ' ' + copy.Length.ToString());
                else
                    sb.Append(firstName);
                WindowCallbacks.Print(sb.ToString());
                ctlListView.Items.Refresh();
                m_manager.SaveModState();
            };

            var menuItemMap = new Dictionary<string, MenuItem>();
            Func<string, Func<bool>, Func<L4D2MM.ModInfo, bool>, MenuItem> GenerateMenuItem = (name, confirm, func) =>
            {
                MenuItem btn = new MenuItem();
                btn.Name = name;
                btn.Click += OnMenuItemClick;
                btn.Header = StringAdapter.GetResource(name);
                menuItemConfirm.Add(name, confirm);
                menuItemClick.Add(name, func);
                menuItemMap.Add(name, btn);
                return btn;
            };

            Func<string, Func<L4D2MM.ModInfo, bool>, MenuItem> GenerateMenuItemWithoutConfirm = (name, func) =>
            {
                return GenerateMenuItem(name, () => { return true; }, func);
            };

            menu.Items.Add(GenerateMenuItemWithoutConfirm("On", o => { return o.SetOn(); }));
            menu.Items.Add(GenerateMenuItemWithoutConfirm("Off", o => { return o.SetOff(); }));
            menu.Items.Add(new Separator());
            menu.Items.Add(GenerateMenuItemWithoutConfirm("Subscribe", o => { return o.Subscribe(); }));
            menu.Items.Add(GenerateMenuItemWithoutConfirm("Unsubscribe", o => { return o.Unsubscribe(); }));
            menu.Items.Add(new Separator());
            menu.Items.Add(GenerateMenuItemWithoutConfirm("Ignore_Collision", o => { return o.IgnoreCollision(); }));
            menu.Items.Add(GenerateMenuItemWithoutConfirm("Detect_Collision", o => { return o.DetectCollision(); }));
            menu.Items.Add(new Separator());
            menu.Items.Add(GenerateMenuItem("Delete", () => { return CtlExtension.ConfirmBox(StringAdapter.GetInfo("ConfirmDeleteFile")); }, o => { bool ret = o.Delete(); m_categorySelected.Update(m_manager); return ret; }));

            ctlListView.ContextMenu = menu;

            FindListViewMenuItem = name => menuItemMap[name];
        }

        void InitializeWindowMenu()
        {
            foreach(var v in ctlMenuModsource.Items)
            {
                var item = v as MenuItem;
                var source = (ModSource)item.DataContext;
                switch(source)
                {
                    case ModSource.Workshop: item.IsChecked = true; break;
                    case ModSource.Player: item.IsChecked = Configure.EnableAddons; break;
                }
            }

            ctlMenuEnableSteam.IsChecked = Configure.EnableSteam;
            ctlMenuEnableVpk.IsChecked = Configure.EnableReadVpk;

            int count = 0;
            foreach(var v in ctlMenuLanguage.Items)
            {
                var language = (v as MenuItem).DataContext as string;
                bool match = language.Equals(Configure.Language);
                (v as MenuItem).IsChecked = match;
                count += match ? 1 : 0;
            }
            Logging.Assert(count == 1);
        }

        private bool SetGamePath()
        {
            //require [Windows API Code Pack]
            //in Visual Studio, open [Tool->Package Manager Consol]
            //input the command [Install-Package WindowsAPICodePack-Shell]
            var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult result = dialog.ShowDialog();
            if (result != Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                return false;
            Configure.InstallPath = dialog.FileName;
            WindowCallbacks.Print(StringAdapter.GetResource("Menu_SetPath") + " : " + dialog.FileName);
            return true;
        }

        private void MenuItemSetPathClick(object sender, RoutedEventArgs e)
        {
            SetGamePath();
        }

        private void MenuItemReloadManagerClick(object sender, RoutedEventArgs e)
        {
            if(Configure.InstallPath == "")
            {
                if (!SetGamePath())
                    return;
            }
            LoadModManager();
        }

        private void MenuItemExitClick(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void MenuItemCustomCategoryClick(object sender, RoutedEventArgs e)
        {
            var window = new WindowCategory();
            window.Title = StringAdapter.GetResource("Menu_CustomCategory");
            window.FontSize = FontSize;
            window.Owner = this;
            window.ShowDialog();
            m_categorySelected.Refresh();
            LoadModInfo(LoadedModInfo);
            foreach (var mod in m_manager.Mods.Values)
                mod.RefreshResources();
        }

        private void MenuItemLocalModClassificationRuleClick(object sender, RoutedEventArgs e)
        {
            var window = new WindowClassify();
            window.Title = StringAdapter.GetResource("Menu_LocalModClassificationRule");
            window.FontSize = FontSize;
            window.Owner = this;
            window.ShowDialog();
        }

        private void MenuItemFontSizeClick(object sender, RoutedEventArgs e)
        {
            double oldFontSize = FontSize;
            var window = new WindowSetValue();
            window.Owner = this;
            window.Initialize(FontSize, StringAdapter.GetResource("Menu_FontSize"), StringAdapter.GetResource("Font_Size"));
            window.Verify = o => (double)o >= 6 && (double)o <= 30;
            window.VerifyErrorMsg = "-> [6.0, 30.0]";
            window.FontSize = FontSize;
            window.ShowDialog();
            if (window.InputValue != null)
            {
                this.FontSize = (double)window.InputValue;
                ctlMenu.FontSize = this.FontSize;
                Configure.View.FontSize = this.FontSize;
            }
        }

        private void MenuItemColorsClick(object sender, RoutedEventArgs e)
        {
            var window = new WindowColors();
            window.Owner = this;
            //window.FontSize = FontSize;
            window.ShowDialog();
            ctlListView.Items.Refresh();
        }

        private void MenuItemModSourceClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var source = (ModSource)item.DataContext;
            switch(source)
            {
                case ModSource.Workshop: Logging.Assert(false); break;
                case ModSource.Player: Configure.EnableAddons = item.IsChecked; break;
            }
        }

        private void MenuItemEnableSteamClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            Configure.EnableSteam = item.IsChecked;
        }

        private void MenuItemEnableVpkClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            Configure.EnableReadVpk = item.IsChecked;
        }

        private void MenuItemLanguageClick(object sender, RoutedEventArgs e)
        {
            var newlanguage = (sender as MenuItem).DataContext as string;
            if(Configure.Language != newlanguage)
            {
                LanguageHelper.LoadLanguageFile(newlanguage);
                Configure.Language = newlanguage;
                WindowCallbacks.Print(StringAdapter.GetInfo("ChangeLanguage"));
                m_categorySelected.RefreshLanguage();
                InitializeListViewMenu();
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(newlanguage);
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(newlanguage);
                Logging.Log("change language to " + newlanguage);

                //refresh check box
                int count = 0;
                foreach (var v in ctlMenuLanguage.Items)
                {
                    var language = (v as MenuItem).DataContext as string;
                    bool match = language.Equals(Configure.Language);
                    (v as MenuItem).IsChecked = match;
                    count += match ? 1 : 0;
                }
                Logging.Assert(count == 1);
            }
        }

        private void MenuItemAboutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                StringAdapter.GetInfo("About") + '.'
                + "\r\n" + StringAdapter.GetResource("License") + " : MIT"
                + "\r\n" + StringAdapter.GetResource("Project") + " : https://github.com/XavierCai1996/L4D2ModManager"
                + "\r\n" + StringAdapter.GetResource("Contact") + " : cxw39@foxmail.com"
                , StringAdapter.GetResource("About"));
        }

        private void AdaptWindowSize(Size size)
        {
            double imageWidth = Math.Min(500, size.Width * 0.36);
            double imageHeight = 36.0 / 64.0 * imageWidth;
            ctlImage.SetSize(imageWidth, imageHeight);
            ctlFrameText.Width = imageWidth;
            ctlPrintText.Height = size.Height * 0.13;
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdaptWindowSize(e.NewSize);
            Configure.View.WindowSize.Width = e.NewSize.Width;
            Configure.View.WindowSize.Height = e.NewSize.Height;
        }
    }
}
