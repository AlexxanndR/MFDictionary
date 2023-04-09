using BespokeFusion;
using System;
using System.Windows;
using System.Windows.Media;

namespace MFDictionary.Core
{
    internal static class CustomMessageBox
    {
        public static MessageBoxResult ShowWarning(string message)
        {
            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
            {
                FontFamily = new FontFamily("Oswald Light"),
                TxtMessage = { Text = String.Format(message),
                               Foreground = Brushes.Black,
                               FontSize = 20,
                               HorizontalAlignment = HorizontalAlignment.Center },
                TxtTitle = { Text = "Warning", Foreground = Brushes.Black },
                BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                MainContentControl = { Background = Brushes.White },
                TitleBackgroundPanel = { Background = Brushes.MistyRose },
                BorderThickness = new Thickness(0),
                WindowStyle = WindowStyle.None
            };

            msg.Show();

            return msg.Result;
        }

        public static MessageBoxResult ShowInfo(string message)
        {
            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
            {
                FontFamily = new FontFamily("Oswald Light"),
                TxtMessage = { Text = String.Format(message),
                               Foreground = Brushes.Black,
                               FontSize = 20,
                               HorizontalAlignment = HorizontalAlignment.Center },
                TxtTitle = { Text = "Information", Foreground = Brushes.Black },
                BtnOk = { Content = "Ok", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                BtnCancel = { Content = "Cancel", Background = Brushes.Transparent, Foreground = Brushes.Black, BorderBrush = Brushes.Black },
                MainContentControl = { Background = Brushes.White },
                TitleBackgroundPanel = { Background = Brushes.LightBlue },
                BorderThickness = new Thickness(0),
                WindowStyle = WindowStyle.None
            };

            msg.Show();

            return msg.Result;
        }
    }
}
