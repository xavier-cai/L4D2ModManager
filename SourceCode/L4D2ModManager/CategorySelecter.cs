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
                    Converter = o => (o as L4D2Type.Category).Name;
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

            public bool IsSpecial => IsAll || IsUncategorized || IsIgnoreList || IsConflicted;
            public bool IsAll => Category == AllCategory;
            public bool IsUncategorized => Category == UncategorizedCategory;
            public bool IsIgnoreList => Category == IgnoreListCategory;
            public bool IsConflicted => Category == ConflictedCategory;
        }

        List<ComboItem> MainComboList, SubComboList;
        ComboBox MainComboBox, SubComboBox;
        TextBox SearchBox;
        ListView ListView;
        List<ViewItem> AllViewItems, MainViewItems, SubViewItems, SearchViewItems, DisplayViewItems;
        Comparison<ViewItem> SortFunction = null;

        public CategorySelecter(ComboBox main, ComboBox sub, TextBox search, ListView listView)
        {
            MainComboList = new List<ComboItem>();
            SubComboList = new List<ComboItem>();

            MainComboBox = main;
            SubComboBox = sub;
            ListView = listView;
            SearchBox = search;

            MainComboBox.ItemsSource = MainComboList;
            SubComboBox.ItemsSource = SubComboList;
            MainComboBox.DisplayMemberPath = "Name";
            MainComboBox.SelectedValuePath = "Category";
            SubComboBox.DisplayMemberPath = "Name";
            SubComboBox.SelectedValuePath = "Category";
            LoadComboItem();

            MainComboBox.SelectionChanged += OnMainComboBoxSelectionChanged;
            SubComboBox.SelectionChanged += OnSubComboBoxSelectionChanged;

            SearchBox.TextChanged += OnSearchBoxDataContextChanged;

            AllViewItems = new List<ViewItem>();
            MainViewItems = new List<ViewItem>();
            SubViewItems = new List<ViewItem>();
            SearchViewItems = new List<ViewItem>();
            DisplayViewItems = new List<ViewItem>();
            ListView.ItemsSource = DisplayViewItems;
        }

        private void LoadComboItem()
        {
            MainComboList.Clear();
            MainComboList.Add(ComboItem.All);
            foreach (var v in L4D2Type.GetCategory())
                MainComboList.Add(new ComboItem(v, v));
            MainComboList.Add(ComboItem.Uncategorized);
            MainComboList.Add(ComboItem.IgnoreList);
            MainComboList.Add(ComboItem.Conflicted);
            SubComboList.Clear();
            SubComboList.Add(ComboItem.All);
            MainComboBox.SelectedIndex = 0;
            SubComboBox.SelectedIndex = 0;
            MainComboBox.Items.Refresh();
            SubComboBox.Items.Refresh();
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
            if (!selected.IsSpecial)
            {
                var subcategory = (selected.Value as L4D2Type.Category).Children;
                if (subcategory != null && subcategory.Count > 0)
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
            UpdateSearch();
            ListView.Dispatcher.Invoke(new Action(() => ListView.Items.Refresh()));
        }

        private void OnSearchBoxDataContextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSearch();
            ListView.Dispatcher.Invoke(new Action(() => ListView.Items.Refresh()));
        }

        private void RefreshMainComboListItem(ComboItem selected)
        {
            MainViewItems.Clear();
            Func<ViewItem, bool> predict = item => true;
            if (!selected.IsAll)
            {
                if (selected.IsUncategorized)
                    predict = item => item.Mod.Mod.Category.Count == 0;
                else if (selected.IsIgnoreList)
                    predict = item => item.Mod.IsIgnoreCollision;
                else if (selected.IsConflicted)
                    predict = item => item.Mod.IsHaveCollision;
                else
                    predict = item => {
                        if (item.Mod.Mod.Category.Contains(selected.Category as L4D2Type.Category))
                            return true;
                        foreach (var sub in item.Mod.Mod.Category)
                            if (sub.Parent == (selected.Category as L4D2Type.Category))
                                return true;
                        return false;
                        };
            }
            MainViewItems.AddRange(AllViewItems.Where(predict));
        }

        private void RefreshSubComboListItem(ComboItem selected)
        {
            SubViewItems.Clear();
            Func<ViewItem, bool> predict = (item) => true;
            if (!selected.IsAll)
            {
                var category = (SubComboBox.SelectedItem as ComboItem).Value as L4D2Type.Category;
                if (selected.IsUncategorized)
                    predict = item =>
                    {
                        if (!item.Mod.Mod.Category.Contains(category))
                            return true;
                        var all = category.Children;
                        var have = item.Mod.Mod.Category;
                        return all.Union(have).Count() == 0;
                    };
                else if (selected.IsIgnoreList)
                    predict = item => item.Mod.IsIgnoreCollision;
                else if (selected.IsConflicted)
                    predict = item => item.Mod.IsHaveCollision;
                else
                    predict = item => item.Mod.Mod.Category.Contains(category);
            }
            SubViewItems.AddRange(MainViewItems.Where(predict));
            UpdateSearch();
        }

        private void UpdateSearch()
        {
            SearchViewItems.Clear();
            var keyword = SearchBox.Text.ToLower();
            if (keyword.Length <= 0)
                SearchViewItems.AddRange(SubViewItems);
            else
                SearchViewItems.AddRange(SubViewItems.Where(item =>
                    item.ID.ToLower().Contains(keyword)
                    || item.Key.ToLower().Contains(keyword)
                    || item.Mod.Source.GetString().ToLower().Contains(keyword)
                    || item.Mod.Key.ToLower().Contains(keyword)
                    || item.Mod.ModState.GetString().ToLower().Contains(keyword)
                    || item.Mod.Mod.Title.Contains(keyword)
                    || item.Mod.Mod.Author.ToLower().Contains(keyword)
                    || item.Mod.Mod.Description.ToLower().Contains(keyword)
                ));
            Sort();
        }

        private void Sort()
        {
            DisplayViewItems.Clear();
            DisplayViewItems.AddRange(SearchViewItems);
            if (SortFunction != null)
                DisplayViewItems.Sort(SortFunction);
        }

        public void Sort(Comparison<ViewItem> func)
        {
            SortFunction = func;
            Sort();
            ListView.Dispatcher.Invoke(new Action(() => ListView.Items.Refresh()));
        }

        public void Refresh()
        {
            object selectedItemMain = null;
            object selectedItemSub = null;
            var selectedMain = MainComboBox.SelectedItem as ComboItem;
            if(selectedMain != null)
            {
                selectedItemMain = selectedMain.Category;
                var selectedSub = SubComboBox.SelectedItem as ComboItem;
                if (selectedSub != null)
                {
                    selectedItemSub = selectedSub.Category;
                }
            }

            LoadComboItem();

            Func<ComboBox, object, ComboItem> FindCategoryInItem = (combo, category) =>
            {
                foreach (var item in combo.Items)
                    if (item is ComboItem)
                        if ((item as ComboItem).Category == category)
                            return item as ComboItem;
                return null;
            };

            if (selectedItemMain != null)
            {
                var findMain = FindCategoryInItem(MainComboBox, selectedItemMain);
                if (findMain != null)
                {
                    MainComboBox.SelectedItem = findMain;
                    SubComboList.Clear();
                    SubComboList.Add(ComboItem.All);
                    if (!findMain.IsSpecial)
                    {
                        var subcategory = (findMain.Category as L4D2Type.Category).Children;
                        if (subcategory != null && subcategory.Count > 0)
                        {
                            foreach (var v in subcategory)
                                SubComboList.Add(new ComboItem(v, v));
                            SubComboList.Add(ComboItem.Uncategorized);
                            SubComboList.Add(ComboItem.IgnoreList);
                            SubComboList.Add(ComboItem.Conflicted);
                        }
                    }
                    if (selectedItemSub != null)
                    {
                        var findSub = FindCategoryInItem(SubComboBox, selectedItemSub);
                        if (findSub != null)
                            SubComboBox.SelectedItem = findSub;
                    }
                }
            }
            MainComboBox.Items.Refresh();
            SubComboBox.Items.Refresh();
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
            SearchBox.Dispatcher.Invoke(new Action(() => UpdateSearch()));
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
