using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

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

        private Dictionary<string,string> _dictionary;

        private void InitDictionary()
        {
            _dictionary = new Dictionary<string, string>
            {
                {"QQGroup", "https://jq.qq.com/?_wv=1027&k=5ecq1Hn"},
                {"GitHub", "https://github.com/ChuckHearthBuddy/CardQuery"},
                {"Blog", "https://www.cnblogs.com/chucklu/category/1519356.html"}
            };
        }

        private void AddContent()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "This tool is written by Chuck Lu";
            foreach (var item in _dictionary)
            {
                var link = GetHyperlink(item.Key, item.Value);
                textBlock.Inlines.Add(Environment.NewLine);
                textBlock.Inlines.Add(link);
            }
            MainDockPanel.Children.Add(textBlock);
            MainDockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            MainDockPanel.VerticalAlignment = VerticalAlignment.Stretch;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
        }

        private Hyperlink GetHyperlink(string text, string url)
        {
            Hyperlink hyperlink = new Hyperlink();
            hyperlink.NavigateUri = new Uri(url);
            hyperlink.Inlines.Add(text);
            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            return hyperlink;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
