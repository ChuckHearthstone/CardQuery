using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CardQuery
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            InitDictionary();
            AddContent();
        }

        private Dictionary<string,string> dictionary;

        private void InitDictionary()
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add("QQGroup", "https://jq.qq.com/?_wv=1027&k=5ecq1Hn");
            dictionary.Add("GitHub", "https://github.com/ChuckHearthBuddy/CardQuery");
            dictionary.Add("Blog", "https://www.cnblogs.com/chucklu/category/1519356.html");
        }

        private void AddContent()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "This tool is written by Chuck Lu";
            foreach (var item in dictionary)
            {
                var link = GetHyperlink(item.Key, item.Value);
                textBlock.Inlines.Add(Environment.NewLine);
                textBlock.Inlines.Add(link);
            }
            MainStackPanel.Children.Add(textBlock);
        }

        private Hyperlink GetHyperlink(string text, string url)
        {
            Hyperlink hyperlink = new Hyperlink();
            hyperlink.NavigateUri = new Uri(url);
            hyperlink.Inlines.Add(text);
            return hyperlink;
        }
    }
}
