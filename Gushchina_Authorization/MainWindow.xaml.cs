using Gushchina_Authorization.Pages;
using System;
using System.Windows;

namespace Gushchina_Authorization
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrmMain.Navigate(new Autho());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (FrmMain.CanGoBack)
            {
                FrmMain.GoBack();
            }
        }

        private void FrmMain_ContentRendered(object sender, EventArgs e)
        {
            if (FrmMain.CanGoBack)
            {
                BtnBack.Visibility = Visibility.Visible;
            }
            else
            {
                BtnBack.Visibility = Visibility.Hidden;
            }
        }
    }
}
