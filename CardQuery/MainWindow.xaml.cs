using System.Collections.Generic;
using System.Linq;
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
            var result = text?.Replace("\n", " ");
            result = result?.Replace("<b>", string.Empty);
            result = result?.Replace("</b>", string.Empty);
            result = result?.Replace("<i>", string.Empty);
            result = result?.Replace("</i>", string.Empty);
            result = result?.Replace("[x]", string.Empty);
            result = result?.Replace("(", string.Empty);
            result = result?.Replace(")", string.Empty);
            result = result?.Replace("（", string.Empty);
            result = result?.Replace("）", string.Empty);
            result = result?.Replace("$", string.Empty);
            result = result?.Replace("#", string.Empty);
            return result;
        }

        private string GetCardInfo(Card card)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(card.Id);

            var nameInEnglish = card.GetLocName(Locale.enUS);
            var nameInChinese = card.GetLocName(Locale.zhCN);
            stringBuilder.AppendLine($"{nameInChinese}({nameInEnglish})");

            stringBuilder.AppendLine(nameInEnglish);
            stringBuilder.AppendLine(nameInChinese);

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

            stringBuilder.AppendLine($"{nameof(card.Cost)}:{card.Cost}");
            stringBuilder.AppendLine($"{nameof(card.Attack)}:{card.Attack}");
            stringBuilder.AppendLine($"{nameof(card.Health)}:{card.Health}");
            stringBuilder.AppendLine($"{nameof(card.Set)}:{card.Set}");
            stringBuilder.AppendLine($"{nameof(card.Class)}:{card.Class}");

            stringBuilder.AppendLine("===split line===");
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

            var prefix = "Sim_";
            if (id.StartsWith(prefix))
            {
                id = id.Replace(prefix, string.Empty);
            }

            if (!Cards.All.ContainsKey(id))
            {
                MessageBox.Show($"Can not find the card with id {id}");
                return;
            }

            Card card = Cards.All[id];
            AddCardToListView(new List<Card> {card});
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

            var dictionary = new Dictionary<string, Card>();
            var cardList = Cards.GetFromFuzzyName(name, Locale.zhCN, false);
            foreach (var item in cardList)
            {
                AddItemToDictionary(dictionary, item);
            }
            cardList = Cards.GetFromFuzzyName(name, Locale.enUS, false);
            foreach (var item in cardList)
            {
                AddItemToDictionary(dictionary, item);
            }

            if (dictionary.Count == 0)
            {
                MessageBox.Show($"Can not find the card with name {name} in English(en-US) or Chinese(zh-CN)");
                return;
            }

            cardList = dictionary.Values.ToList();
            AddCardToListView(cardList);
        }

        private void AddItemToDictionary(Dictionary<string, Card> dictionary, Card card)
        {
            var cardId = card.Id;
            if (!dictionary.ContainsKey(cardId))
            {
                dictionary.Add(cardId, card);
            }
        }

        private void AddCardToListView(List<Card> tempCards)
        {
            var cards = tempCards.OrderBy(x => x.Cost).ThenBy(x => x.Name);
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
                ListViewCardList_PreviewMouseLeftButtonUp(null, null);
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

        private void TextBoxCardText_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            TextBoxCardInfo.Clear();
            ListViewCardList.Items.Clear();

            string text = TextBoxCardText.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Card text should not be empty.");
                return;
            }

            var dictionary = new Dictionary<string, Card>();
            var cardList = Cards.GetFromFuzzyText(text, Locale.zhCN, false);
            foreach (var item in cardList)
            {
                AddItemToDictionary(dictionary, item);
            }
            cardList = Cards.GetFromFuzzyText(text, Locale.enUS, false);
            foreach (var item in cardList)
            {
                AddItemToDictionary(dictionary, item);
            }

            if (dictionary.Count == 0)
            {
                MessageBox.Show($"Can not find the card with text {text} in English(en-US) or Chinese(zh-CN)");
                return;
            }

            cardList = dictionary.Values.ToList();
            AddCardToListView(cardList);
        }
    }
}
