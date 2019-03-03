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
    /// WindowCategory.xaml
    /// </summary>
    public partial class WindowCategory : Window
    {
        internal class CategoryItem
        {
            public L4D2Type.Category Category { get; private set; }
            public string Name
            {
                get => Category.Name;
                set => Category.Name = value;
            }
            public bool Detect
            {
                get => Category.SingletonResource;
                set => Category.SingletonResource = value;
            }
            public string Keywords
            {
                get => Category.Keys.Count > 0 ? Category.Keys.Aggregate((a, b) => a + ';' + b) : "";
                set => Category.Keys = value.Split(';').Where(key => key.Length > 0).ToList();
            }
            internal CategoryItem(L4D2Type.Category category)
            {
                Category = category;
            }
        }

        List<CategoryItem> m_items;

        public WindowCategory()
        {
            InitializeComponent();

            m_items = new List<CategoryItem>();
            LoadCategoryItem();
            ctlListView.ItemsSource = m_items;
        }

        private void LoadCategoryItem()
        {
            m_items.Clear();
            foreach (var c in L4D2Type.CategoryRoot.Children)
            {
                m_items.Add(new CategoryItem(c));
                foreach (var sub in c.Children)
                    m_items.Add(new CategoryItem(sub));
            }
        }

        private ContextMenu CreateMenu(bool canAdd)
        {
            var menu = new ContextMenu();
            if(canAdd)
            {
                var addCategory = new MenuItem();
                addCategory.Header = StringAdapter.GetResource("Add_Category");
                addCategory.Click += OnAddCategoryClick;
                var addSubcategory = new MenuItem();
                addSubcategory.Header = StringAdapter.GetResource("Add_Subcategory");
                addSubcategory.Click += OnAddSubcategoryClick;
                menu.Items.Add(addCategory);
                menu.Items.Add(addSubcategory);
                menu.Items.Add(new Separator());
            }
            var delete = new MenuItem();
            delete.Header = StringAdapter.GetResource("Delete");
            delete.Click += OnDeleteClick;
            menu.Items.Add(delete);
            return menu;
        }

        private void ListViewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ListView;
            if(list != null)
            {
                bool canAdd = false;
                if(list.SelectedItems.Count == 1)
                {
                    var selected = list.SelectedItems[0] as CategoryItem;
                    if(selected.Category.Level.Equals(1))
                    {
                        canAdd = true;
                    }
                }
                var menu = CreateMenu(canAdd);
                ctlListView.ContextMenu = menu;
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if(CtlExtension.ConfirmBox(StringAdapter.GetInfo("ConfirmDelete")))
            {
                foreach(var v in ctlListView.SelectedItems)
                {
                    var item = v as CategoryItem;
                    if(item != null)
                    {
                        item.Category.Parent.RemoveChild(item.Category);
                        Logging.Log("Remove category : " + item.Category.ToString());
                    }
                }
                LoadCategoryItem();
                ctlListView.Items.Refresh();
            }
        }

        private void OnAddSubcategoryClick(object sender, RoutedEventArgs e)
        {
            var item = ctlListView.SelectedItem as CategoryItem;
            if(item != null && item.Category.Level.Equals(1))
            {
                var index = ctlListView.SelectedIndex;
                var newSubCategory = new L4D2Type.Category("NewSubcategory");
                item.Category.AddChild(newSubCategory);
                LoadCategoryItem();
                ctlListView.Items.Refresh();
                for (; index < ctlListView.Items.Count; index++)
                {
                    if ((ctlListView.Items[index] as CategoryItem).Category == newSubCategory)
                    {
                        ctlListView.SelectedIndex = index;
                        ctlListView.ScrollIntoView(ctlListView.Items[index]);
                    }
                }
            }
        }

        private void OnAddCategoryClick(object sender, RoutedEventArgs e)
        {
            var newCategory = new L4D2Type.Category("NewCategory");
            L4D2Type.CategoryRoot.AddChild(newCategory);
            LoadCategoryItem();
            ctlListView.Items.Refresh();
            ctlListView.SelectedIndex = ctlListView.Items.Count - 1;
            ctlListView.ScrollIntoView(ctlListView.SelectedItem);
        }

        private void OnButtonMenuClick(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as CategoryItem;
            if(item != null)
            {
                ctlListView.SelectedItem = item;
                var menu = CreateMenu(item.Category.Level.Equals(1));
                menu.IsOpen = true;
            }
        }
    }
}
