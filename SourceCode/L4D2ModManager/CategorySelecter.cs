using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace L4D2ModManager
{
    //for mainwindow
    internal class CategorySelecter
    {
        private class ComboItem
        {
            public string Name => Converter(Value);
            public object Value { get; set; }
            public object Category { get; set; }
            public Func<object, string> Converter;
            public ComboItem() { }
            public ComboItem(object value, object category, bool special = false)
            {
                Value = value;
                Category = category;
                if (!special)
                    Converter = o => o.GetString();
                else
                    Converter = o => "---" + StringAdapter.GetResource(Value as string) + "---";
            }
            private class SpecialCategory
            {
                public string Identity { get; set; }
                public SpecialCategory(string id)
                {
                    Identity = id;
                }
            }
            static private object AllCategory = new SpecialCategory("All");
            static private object UncategorizedCategory = new SpecialCategory("Uncategorized");
            static private object IgnoreListCategory = new SpecialCategory("IgnoreList");
            static private object ConflictedCategory = new SpecialCategory("Conflicted");

            static public ComboItem All = new ComboItem("All", AllCategory, true);
            static public ComboItem Uncategorized = new ComboItem("Uncategorized", UncategorizedCategory, true);
            static public ComboItem IgnoreList = new ComboItem("Ignore_List", IgnoreListCategory, true);
            static public ComboItem Conflicted = new ComboItem("Conflicted", ConflictedCategory, true);

            public bool IsAll => Category == AllCategory;
            public bool IsUncategorized => Category == UncategorizedCategory;
            public bool IsIgnoreList => Category == IgnoreListCategory;
            public bool IsConflicted => Category == ConflictedCategory;
        }

        List<ComboItem> MainComboList, SubComboList;
        ComboBox MainComboBox, SubComboBox;
        ListView ListView;
        List<ViewItem> AllViewItems, MainViewItems, SubViewItems;

        public CategorySelecter(ComboBox main, ComboBox sub, ListView listView)
        {
            MainComboList = new List<ComboItem>();
            MainComboList.Add(ComboItem.All);
            foreach (var v in Enum.GetValues(typeof(ModCategory)))
                MainComboList.Add(new ComboItem(v, v));
            MainComboList.Add(ComboItem.Uncategorized);
            MainComboList.Add(ComboItem.IgnoreList);
            MainComboList.Add(ComboItem.Conflicted);
            SubComboList = new List<ComboItem>();
            SubComboList.Add(ComboItem.All);

            MainComboBox = main;
            SubComboBox = sub;
            ListView = listView;

            MainComboBox.ItemsSource = MainComboList;
            SubComboBox.ItemsSource = SubComboList;
            MainComboBox.DisplayMemberPath = "Name";
            MainComboBox.SelectedValuePath = "Category";
            SubComboBox.DisplayMemberPath = "Name";
            SubComboBox.SelectedValuePath = "Category";
            MainComboBox.SelectedIndex = 0;
            SubComboBox.SelectedIndex = 0;
            MainComboBox.Items.Refresh();
            SubComboBox.Items.Refresh();

            MainComboBox.SelectionChanged += OnMainComboBoxSelectionChanged;
            SubComboBox.SelectionChanged += OnSubComboBoxSelectionChanged;

            AllViewItems = new List<ViewItem>();
            MainViewItems = new List<ViewItem>();
            SubViewItems = new List<ViewItem>();
            ListView.ItemsSource = SubViewItems;
        }

        private void OnMainComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems[0] as ComboItem;
            RefreshMainComboListItem(selected);
            SubComboBox.SelectedIndex = 0;
            RefreshSubComboListItem(ComboItem.All);
            ListView.Dispatcher.Invoke(new Action(() => ListView.Items.Refresh()));
            SubComboList.Clear();
            SubComboList.Add(ComboItem.All);
            if (!selected.IsAll && !selected.IsUncategorized && !selected.IsIgnoreList && !selected.IsConflicted)
            {
                var subcategory = L4D2Type.GetSubcategory((ModCategory)selected.Category);
                if (subcategory != null && subcategory.Length > 0)
                {
                    foreach (var v in subcategory)
                        SubComboList.Add(new ComboItem(v, v));
                    SubComboList.Add(ComboItem.Uncategorized);
                    SubComboList.Add(ComboItem.IgnoreList);
                    SubComboList.Add(ComboItem.Conflicted);
                }
            }
            SubComboBox.Items.Refresh();
        }

        private void OnSubComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSubComboListItem(e.AddedItems[0] as ComboItem);
            ListView.Dispatcher.Invoke(new Action(() => ListView.Items.Refresh()));
        }

        private void RefreshMainComboListItem(ComboItem selected)
        {
            MainViewItems.Clear();
            Func<ViewItem, bool> predict = (item) => true;
            if (!selected.IsAll)
            {
                if (selected.IsUncategorized)
                    predict = (item) => item.Mod.Mod.Category.Length == 0;
                else if (selected.IsIgnoreList)
                    predict = (item) => item.Mod.IsIgnoreCollision;
                else if (selected.IsConflicted)
                    predict = (item) => item.Mod.IsHaveCollision;
                else
                    predict = (item) => item.Mod.Mod.Category.Contains((ModCategory)selected.Category);
            }
            MainViewItems.AddRange(AllViewItems.Where(predict));
        }

        private void RefreshSubComboListItem(ComboItem selected)
        {
            SubViewItems.Clear();
            Func<ViewItem, bool> predict = (item) => true;
            if (!selected.IsAll)
            {
                var category = (ModCategory)(MainComboBox.SelectedItem as ComboItem).Category;
                if (selected.IsUncategorized)
                    predict = (item) =>
                    {
                        if (!item.Mod.Mod.Category.Contains(category))
                            return true;
                        var all = L4D2Type.GetSubcategory(category);
                        var have = item.Mod.Mod.SubCategory(category);
                        return all.Union(have).Count() == 0;
                    };
                else if (selected.IsIgnoreList)
                    predict = (item) => item.Mod.IsIgnoreCollision;
                else if (selected.IsConflicted)
                    predict = (item) => item.Mod.IsHaveCollision;
                else
                    predict = (item) =>
                    {
                        if (!item.Mod.Mod.Category.Contains(category))
                            return false;
                        var subcategory = item.Mod.Mod.SubCategory(category);
                        return subcategory.Contains(selected.Category);
                    };
            }
            SubViewItems.AddRange(MainViewItems.Where(predict));
        }

        public void Update(L4D2MM manager)
        {
            AllViewItems.Clear();
            foreach (var v in manager.Mods)
            {
                AllViewItems.Add(new ViewItem(v.Value));
            }
            MainComboBox.Dispatcher.Invoke(new Action(() => RefreshMainComboListItem(MainComboBox.SelectedItem as ComboItem)));
            SubComboBox.Dispatcher.Invoke(new Action(() => RefreshSubComboListItem(SubComboBox.SelectedItem as ComboItem)));
            ListView.Dispatcher.Invoke(new Action(() => ListView.Items.Refresh()));
        }

        public void RefreshLanguage()
        {
            //make sure the selected one will be refreshed at the same time... too weird...
            MainComboBox.SelectionChanged -= OnMainComboBoxSelectionChanged;
            SubComboBox.SelectionChanged -= OnSubComboBoxSelectionChanged;
            int premain = MainComboBox.SelectedIndex;
            int presub = SubComboBox.SelectedIndex;
            MainComboBox.SelectedIndex  = -1;
            SubComboBox.SelectedIndex = -1;
            MainComboBox.Items.Refresh();
            SubComboBox.Items.Refresh();
            MainComboBox.SelectedIndex = premain;
            SubComboBox.SelectedIndex = presub;
            MainComboBox.Items.Refresh();
            SubComboBox.Items.Refresh();
            MainComboBox.SelectionChanged += OnMainComboBoxSelectionChanged;
            SubComboBox.SelectionChanged += OnSubComboBoxSelectionChanged;

            ListView.Items.Refresh();
        }
    }
}
