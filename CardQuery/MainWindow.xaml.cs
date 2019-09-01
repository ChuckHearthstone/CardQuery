using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
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

        private string TrimCardText(string text)
        {
            var result = text?.Replace("\n", string.Empty);
            result = result?.Replace("<b>", string.Empty);
            result = result?.Replace("</b>", string.Empty);
            result = result?.Replace("<i>", string.Empty);
            result = result?.Replace("</i>", string.Empty);
            return result;
        }

        private string GetCardInfo(Card card)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(card.Id);

            stringBuilder.AppendLine(card.GetLocName(Locale.enUS));
            stringBuilder.AppendLine(card.GetLocName(Locale.zhCN));

            var textInEnglish = card.GetLocText(Locale.enUS);
            if (!string.IsNullOrWhiteSpace(textInEnglish))
            {
                stringBuilder.AppendLine(TrimCardText(textInEnglish));
            }

            var textInChinese = card.GetLocText(Locale.zhCN);
            if (!string.IsNullOrWhiteSpace(textInChinese))
            {
                stringBuilder.AppendLine(TrimCardText(textInChinese));
            }

            var json = JsonConvert.SerializeObject(card, Formatting.Indented);
            stringBuilder.AppendLine(json);
            return stringBuilder.ToString();
        }

        private void ButtonCardId_Click(object sender, RoutedEventArgs e)
        {
            TextBoxCardInfo.Clear();
            ListViewCardList.Items.Clear();

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
            AddCardToListView(new List<Card> {card});
            TextBoxCardInfo.Text = GetCardInfo(card);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            About form = new About();
            form.ShowDialog();
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

            AddCardToListView(cardList);
        }

        private void AddCardToListView(List<Card> cards)
        {
            foreach (var card in cards)
            {
                CardName cardName = new CardName
                {
                    zhCN = card.GetLocName(Locale.zhCN),
                    enUS = card.GetLocName(Locale.enUS),
                    DbfId = card.DbfId,
                    CardId = card.Id
                };
                ListViewCardList.Items.Add(cardName);
            }

            if (ListViewCardList.Items.Count > 0)
            {
                ListViewCardList.SelectedIndex = 0;
            }
        }

        private void ListViewCardList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private void TextBoxCardId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            ButtonCardId_Click(null, null);
        }

        private void TextBoxCardName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            ButtonCardName_Click(null, null);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var item = ListViewCardList.SelectedItem;
            if (item is CardName cardName)
            {
                Clipboard.SetText(cardName.CardId);
            }
            else
            {
                MessageBox.Show("Please select a card first!");
            }
        }
    }
}
