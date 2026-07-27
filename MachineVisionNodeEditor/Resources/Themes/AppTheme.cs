using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MachineVisionNodeEditor.Resources.Themes
{
    class AppTheme
    {
        public static void ChangeTheme(Uri ThemeUri)
        {
            ResourceDictionary Theme = new ResourceDictionary() { Source = ThemeUri };
            var dictionaries = App.Current.Resources.MergedDictionaries;

            if (dictionaries.Count != 0)
            {
                var dictToRemove = App.Current.Resources.MergedDictionaries
                                    .FirstOrDefault(d => d.Source != null &&
                                    d.Source.OriginalString == "Resources/Themes/DarkTheme.xaml");
                App.Current.Resources.MergedDictionaries.Remove(dictToRemove);

                dictToRemove = App.Current.Resources.MergedDictionaries
                                    .FirstOrDefault(d => d.Source != null &&
                                    d.Source.OriginalString == "Resources/Themes/LightTheme.xaml");
                App.Current.Resources.MergedDictionaries.Remove(dictToRemove);
            }

            App.Current.Resources.Clear();

            App.Current.Resources.MergedDictionaries.Add(Theme);
        }
    }
}
