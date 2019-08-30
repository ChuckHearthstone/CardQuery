using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using HearthDb;
using HearthDb.Enums;
using Newtonsoft.Json;

namespace CardQuery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string GetCardInfo(Card card)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var json = JsonConvert.SerializeObject(card, Formatting.Indented);
            stringBuilder.AppendLine(card.GetLocName(Locale.enUS));
            stringBuilder.AppendLine(card.GetLocName(Locale.zhCN));
            stringBuilder.AppendLine(json);
            return stringBuilder.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBoxCardInfo.Clear();
            string id = TextBoxCardId.Text.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("Card Id should not be empty.");
                return;
            }

            if (!Cards.All.ContainsKey(id))
            {
                MessageBox.Show($"Can not find the card with id {id}");
                return;
            }

            Card card = Cards.All[id];
            TextBoxCardInfo.Text = GetCardInfo(card);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("https://github.com/ChuckHearthBuddy/CardQuery");
        }

        private void ButtonCardName_Click(object sender, RoutedEventArgs e)
        {
            TextBoxCardInfo.Clear();
            ListViewCardList.Items.Clear();

            string name = TextBoxCardName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Card Name should not be empty.");
                return;
            }

            var cardList = Cards.GetFromFuzzyName(name, Locale.zhCN, false);
            if (cardList.Count == 0)
            {
                cardList = Cards.GetFromFuzzyName(name, Locale.enUS, false);
            }

            if (cardList == null || cardList.Count == 0)
            {
                MessageBox.Show($"Can not find the card with name {name} in English(en-US) or Chinese(zh-CN)");
                return;
            }

            foreach (var item in cardList)
            {
                CardName cardName = new CardName
                {
                    zhCN = item.GetLocName(Locale.zhCN),
                    enUS = item.GetLocName(Locale.enUS),
                    DbfId = item.DbfId,
                    CardId = item.Id
                };
                ListViewCardList.Items.Add(cardName);
            }
        }

        private void ListViewCardList_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ListViewCardList.SelectedItem;
            if (item is CardName cardName)
            {
                var card = Cards.GetFromDbfId(cardName.DbfId, false);
                if (card == null)
                {
                    MessageBox.Show($"Can not find card with name {cardName.zhCN}");
                    return;
                }
                TextBoxCardInfo.Text = GetCardInfo(card);
            }
        }
    }
}
