using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProvisioningBuildTools
{
    public static class Utility
    {
        public static bool SetSelectedItem(ComboBox comboBox, string item, bool resetIndex = true, bool checkStartWith = false)
        {
            bool success = false;

            IEnumerable<string> items = comboBox.Items?.Cast<string>() ?? Enumerable.Empty<string>();

            if (!string.IsNullOrEmpty(item) && items.Contains(item, StringComparer.InvariantCultureIgnoreCase))
            {
                comboBox.SelectedItem = items.First(str => string.Compare(str, item, true) == 0);
                success = true;
            }
            else
            {
                if (checkStartWith && !string.IsNullOrEmpty(item))
                {
                    string[] actualItems = items.Where(str => str.ToUpper().StartsWith(item.ToUpper())).ToArray();

                    if (actualItems != null && actualItems.Length == 1)
                    {
                        comboBox.SelectedItem = actualItems[0];
                        success = true;
                    }
                }
                else
                {
                    if (resetIndex)
                    {
                        if (comboBox.Items.Count > 0)
                        {
                            comboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
            return success;
        }

        public static bool SetSelectedItem(ComboBox comboBox, string item, Func<string[], string, string> findFunc)
        {
            bool success = false;

            IEnumerable<string> items = comboBox.Items?.Cast<string>() ?? Enumerable.Empty<string>();

            if (!string.IsNullOrEmpty(item) && items.Contains(item, StringComparer.InvariantCultureIgnoreCase))
            {
                comboBox.SelectedItem = items.First(str => string.Compare(str, item, true) == 0);
                success = true;
            }
            else
            {
                if (findFunc != null && !string.IsNullOrEmpty(item))
                {
                    string actualItem = findFunc(items.ToArray(), item.ToUpper());

                    if (!string.IsNullOrEmpty(actualItem))
                    {
                        comboBox.SelectedItem = actualItem;
                        success = true;
                    }
                }
            }
            return success;
        }

        public static void SetItems(CheckedListBox checkedListBox, string[] items, string[] lastCheckedItems, string[] usuallyUsedItems)
        {
            items = items ?? new string[0];
            lastCheckedItems = lastCheckedItems ?? new string[0];
            usuallyUsedItems = usuallyUsedItems ?? new string[0];

            items = usuallyUsedItems.Where(a => items.Contains(a, StringComparer.InvariantCultureIgnoreCase)).Concat(items).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

            checkedListBox.Items.Clear();
            checkedListBox.Items.AddRange(items);

            foreach (var item in lastCheckedItems)
            {
                int index = items.Select(str => str.ToUpper()).ToList().IndexOf(item.ToUpper());

                if (index >= 0)
                {
                    checkedListBox.SetItemChecked(index, true);
                }
            }
        }
        public static void SetItems(ListBox lsbTotal, ListBox lsbUsed, string[] items, string[] lastUsedItems, string[] usuallyUsedItems)
        {
            items = items ?? new string[0];
            lastUsedItems = lastUsedItems ?? new string[0];
            usuallyUsedItems = usuallyUsedItems ?? new string[0];

            items = usuallyUsedItems.Where(a => items.Contains(a, StringComparer.InvariantCultureIgnoreCase)).Concat(items).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

            lsbTotal.Items.Clear();
            lsbTotal.Items.AddRange(items);
            lsbUsed.Items.Clear();

            foreach (var item in lastUsedItems)
            {
                int index = items.Select(str => str.ToUpper()).ToList().IndexOf(item.ToUpper());

                if (index >= 0)
                {
                    lsbUsed.Items.Add(item);
                }
            }
        }

        public static bool IsGitProjectPath(string projectPath)
        {
            return Directory.Exists(projectPath) && Directory.GetDirectories(projectPath).Select(dir => (new DirectoryInfo(dir)).Name).Contains(".git");
        }

        public static bool SetSelectedItem(CheckedListBox checkedListBox, string item, bool checkStartWith = true)
        {
            string actualItem = GetItem(checkedListBox, item, checkStartWith);

            if (!string.IsNullOrEmpty(actualItem))
            {
                checkedListBox.SetItemChecked(checkedListBox.Items.IndexOf(actualItem), true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SetSelectedItem(ListBox lstTotal, ListBox lstSelected, string item, bool checkStartWith = true)
        {
            string actualItem = GetItem(lstTotal, item, checkStartWith);

            if (!string.IsNullOrEmpty(actualItem))
            {
                lstSelected.Items.Add(actualItem);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SetSelectedItem(CheckedListBox checkedListBox, IEnumerable<string> items, bool checkStartWith = true, bool checkedMoveFirst = true)
        {
            bool success = false;
            bool successTemp = false;

            IEnumerable<string> checkedItems = Enumerable.Empty<string>();

            if (items != null && items.Count() > 0)
            {
                success = true;

                ClearCheckedItems(checkedListBox);

                foreach (var item in items)
                {
                    successTemp = SetSelectedItem(checkedListBox, item, checkStartWith);

                    if (successTemp)
                    {
                        checkedItems = checkedItems.Append(checkedListBox.CheckedItems[checkedListBox.CheckedItems.Count - 1].ToString());
                    }

                    success = success && successTemp;
                }
            }

            if (checkedMoveFirst && checkedItems.Count() > 0)
            {
                string[] temps = checkedItems.Concat(checkedListBox.Items.Cast<string>()).Distinct().ToArray();

                checkedListBox.Items.Clear();
                checkedListBox.Items.AddRange(temps);

                for (int i = 0; i < checkedItems.Count(); i++)
                {
                    checkedListBox.SetItemChecked(i, true);
                }
            }

            return success;
        }

        public static bool SetSelectedItem(ListBox lsbTotal, ListBox lsbSelected, IEnumerable<string> items, bool checkStartWith = true, bool checkedMoveFirst = true)
        {
            bool success = false;
            bool successTemp = false;

            IEnumerable<string> checkedItems = Enumerable.Empty<string>();

            if (items != null && items.Count() > 0)
            {
                success = true;

                lsbSelected.Items.Clear();

                foreach (var item in items)
                {
                    successTemp = SetSelectedItem(lsbTotal, lsbSelected, item, checkStartWith);

                    if (successTemp)
                    {
                        checkedItems = checkedItems.Append(lsbSelected.Items[lsbSelected.Items.Count - 1].ToString());
                    }

                    success = success && successTemp;
                }
            }

            if (checkedMoveFirst && checkedItems.Count() > 0)
            {
                string[] temps = checkedItems.Concat(lsbTotal.Items.Cast<string>()).Distinct().ToArray();

                lsbTotal.Items.Clear();
                lsbTotal.Items.AddRange(temps);
            }

            return success;
        }

        public static string GetItem(CheckedListBox checkedListBox, string item, bool checkStartWith = true)
        {
            string actualItem = null;

            string[] items = checkedListBox.Items?.Cast<string>().ToArray();

            if (items != null && items.Length > 0)
            {
                actualItem = items.FirstOrDefault(str => string.Compare(str, item, true) == 0);

                if (string.IsNullOrEmpty(actualItem) && !string.IsNullOrEmpty(item) && checkStartWith)
                {
                    string[] actualItems = items.Where(str => str.ToUpper().StartsWith(item.ToUpper())).ToArray();

                    if (actualItems != null && actualItems.Length == 1)
                    {
                        actualItem = actualItems[0];
                    }
                }
            }

            return actualItem;
        }

        public static string GetItem(ListBox listBox, string item, bool checkStartWith = true)
        {
            string actualItem = null;

            string[] items = listBox.Items?.Cast<string>().ToArray();

            if (items != null && items.Length > 0)
            {
                actualItem = items.FirstOrDefault(str => string.Compare(str, item, true) == 0);

                if (string.IsNullOrEmpty(actualItem) && !string.IsNullOrEmpty(item) && checkStartWith)
                {
                    string[] actualItems = items.Where(str => str.ToUpper().StartsWith(item.ToUpper())).ToArray();

                    if (actualItems != null && actualItems.Length == 1)
                    {
                        actualItem = actualItems[0];
                    }
                }
            }

            return actualItem;
        }

        public static void ClearCheckedItems(CheckedListBox checkedListBox)
        {
            List<string> checkedItems = (checkedListBox.CheckedItems?.Cast<string>() ?? Enumerable.Empty<string>()).ToList();

            foreach (var item in checkedItems)
            {
                checkedListBox.SetItemChecked(checkedListBox.Items.IndexOf(item), false);
            }
        }

        public static void SetDoubleBuffered(object o, bool doubleBuffered)
        {
            Type type = o.GetType();
            PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo.SetValue(o, doubleBuffered);
        }
    }
}
