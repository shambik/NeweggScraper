using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeweggScraper.View
{
    /// <summary>
    /// Interaction logic for NotifyMe.xaml
    /// </summary>
    public partial class NotifyMe : UserControl
    {
        public static string To { get; set; }
        public static string MailingList { get; set; }
        public static bool Notify { get; set; }
        public static bool NotifySound { get; set; }
        public static bool NotifyTelegram { get; set; }
        public static bool NotifyDiscord { get; set; }
        private int loopInterval;

        public NotifyMe()
        {
            InitializeComponent();
            //Instance = this;
        }
        private void AddToMailingListClickedUcBtnClick(object sender, RoutedEventArgs e)
        {
            AddToMailingList();
        }
        private void AddMailKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AddToMailingList();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void NotifyMeLoaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.SearchFiltersScroller.Visibility = Visibility.Hidden;
            MainWindow.Instance.SearchFiltersHeader.Visibility = Visibility.Hidden;
            MainWindow.Instance.SearchCategoryPanel.Visibility = Visibility.Hidden;

            if (!string.IsNullOrEmpty(MainWindow.mailListt))
                mailList.Visibility = Visibility.Visible;
            else
                mailList.Visibility = Visibility.Hidden;

            if (string.IsNullOrWhiteSpace(MainWindow.mailListt))
                clearMailBtn.IsEnabled = false;

            mailingListBlock.Text = MainWindow.mailListt;
            existingToken.Text = $"Token entered:\n{MainWindow.botToken}";
            existingChannel.Text = $"Channel Entered:\n{MainWindow.telegramChannel}";
            existingDiscordToken.Text = $"Token entered:\n{MainWindow.DiscordBotToken}";
            loopInterval = !string.IsNullOrEmpty(MainWindow.Instance.loopTime.Text) ? Int32.Parse(MainWindow.Instance.loopTime.Text) : 0;

            discordLoopDelay.Text =
                $"{MainWindow.DicordLoopDelay} loops ({(loopInterval * MainWindow.DicordLoopDelay)} seconds)";

            if (!notifyMeMail.IsChecked ?? false)
            {
                addMailBtn.IsEnabled = false;
                toMail.IsEnabled = false;
            }

            if (Notify)
            {
                addMailBtn.IsEnabled = true;
                toMail.IsEnabled = true;
                notifyMeMail.IsChecked = true;
            }

            if (NotifySound)
            {
                notifyMeSound.IsChecked = true;
            }


            if (NotifyTelegram)
            {
                BotTokenBox.IsEnabled = true;
                TelegramChannelBox.IsEnabled = true;
                BotTokenBox.Opacity = 1;
                TelegramChannelBox.Opacity = 1;
                notifyMeTelegram.IsChecked = true;
            }
            else
            {
                BotTokenBox.IsEnabled = false;
                BotTokenBox.Opacity = 0.5;
                TelegramChannelBox.Opacity = 0.5;
                TelegramChannelBox.IsEnabled = false;
                notifyMeTelegram.IsChecked = false;
            }


            if (NotifyDiscord)
            {
                DiscordTokenBox.IsEnabled = true;
                DiscordTokenBox.Opacity = 1;
                notifyMeDiscord.IsChecked = true;
                DiscordLoopDelay.IsEnabled = true;
                DiscordLoopDelay.Opacity = 1;

            }
            else
            {
                DiscordTokenBox.IsEnabled = false;
                DiscordTokenBox.Opacity = 0.5;
                notifyMeDiscord.IsChecked = false;
                DiscordLoopDelay.IsEnabled = false;
                DiscordLoopDelay.Opacity = 0.5;

            }
        }

        private void notifyChecked(object sender, RoutedEventArgs e)
        {
            addMailBtn.IsEnabled = true;
            toMail.IsEnabled = true;
            Notify = true;

            if (!string.IsNullOrWhiteSpace(MainWindow.mailListt))
                mailList.Text = "Mail will be sent To:";
        }

        private void notifyUnchecked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MainWindow.mailListt))
                mailList.Text = "Will not send emails until Notify Me is checked";
            else
                mailList.Visibility = Visibility.Collapsed;

            addMailBtn.IsEnabled = false;
            toMail.IsEnabled = false;
            Notify = false;
        }

        private void ClearMailListClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.mailListt = To = MailingList = mailingListBlock.Text = "";
            clearMailBtn.IsEnabled = false;
            mailList.Visibility = Visibility.Hidden;
        }

        private void NotifyMeSound_OnChecked(object sender, RoutedEventArgs e)
        {
            NotifySound = true;
            SoundIcon.Foreground = Brushes.DarkOrange;
        }
        private void NotifyMeSound_OnUnchecked(object sender, RoutedEventArgs e)
        {
            NotifySound = false;
            SoundIcon.Foreground = Brushes.White;
        }
        private void AddToMailingList()
        {
            To = toMail.Text;
            MailingList = toMail.Text.Replace(",", "\x0A");

            if (!mailingListBlock.Text.Contains(toMail.Text) && !string.IsNullOrEmpty(toMail.Text))
                MainWindow.mailListt = mailingListBlock.Text += $"{MailingList}\n";

            mailList.Visibility = !string.IsNullOrEmpty(mailingListBlock.Text)
                ? Visibility.Visible : Visibility.Hidden;

            clearMailBtn.IsEnabled = true;
            toMail.Text = "";
        }


        private void NotifyMeTelegram_OnChecked(object sender, RoutedEventArgs e)
        {
            NotifyTelegram = true;
            TelegramIcon.Foreground = Brushes.DodgerBlue;
            BotTokenBox.IsEnabled = true;
            TelegramChannelBox.IsEnabled = true;
            BotTokenBox.Opacity = 1;
            TelegramChannelBox.Opacity = 1;
        }

        private void NotifyMeTelegram_OnUnchecked(object sender, RoutedEventArgs e)
        {
            NotifyTelegram = false;
            TelegramIcon.Foreground = Brushes.White;
            BotTokenBox.IsEnabled = false;
            TelegramChannelBox.IsEnabled = false;
            BotTokenBox.Opacity = 0.5;
            TelegramChannelBox.Opacity = 0.5;
        }

        private void BotTokenBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            MainWindow.botToken = BotTokenBox.Text;
            existingToken.Text = $"Token entered:\n{MainWindow.botToken}";
        }

        private void TelegramChannelBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            MainWindow.telegramChannel = TelegramChannelBox.Text;
            existingChannel.Text = $"Channel Entered:\n{MainWindow.telegramChannel}";
        }

        private void DiscordBotTokenBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            MainWindow.DiscordBotToken = DiscordTokenBox.Text;
            existingDiscordToken.Text = $"Token entered:\n{MainWindow.DiscordBotToken}";
        }

        private void NotifyMeDiscord_OnChecked(object sender, RoutedEventArgs e)
        {
            NotifyDiscord = true;
            DiscordIcon.Foreground = Brushes.MediumPurple;
            DiscordTokenBox.IsEnabled = true;
            DiscordTokenBox.Opacity = 1;
            DiscordLoopDelay.IsEnabled = true;
            DiscordLoopDelay.Opacity = 1;

        }

        private void NotifyMeDiscord_OnUnchecked(object sender, RoutedEventArgs e)
        {
            NotifyDiscord = false;
            DiscordIcon.Foreground = Brushes.White;
            DiscordTokenBox.IsEnabled = false;
            DiscordLoopDelay.IsEnabled = false;
            DiscordTokenBox.Opacity = 0.5;
            DiscordLoopDelay.Opacity = 0.5;
        }


        private void DiscordLoopDelay_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow.Instance.IsAnumber(DiscordLoopDelay.Text))
            {
                if (!string.IsNullOrEmpty(DiscordLoopDelay.Text))
                    MainWindow.DicordLoopDelay = int.Parse(DiscordLoopDelay.Text);
            }

            loopInterval = !string.IsNullOrEmpty(MainWindow.Instance.loopTime.Text) ? Int32.Parse(MainWindow.Instance.loopTime.Text) : 0;
            discordLoopDelay.Text =
                $"{MainWindow.DicordLoopDelay} loops ({(loopInterval * MainWindow.DicordLoopDelay)} seconds)";
        }
    }

}
