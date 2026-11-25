using Gushchina_Authorization.Model;
using Gushchina_Authorization.Services;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Gushchina_Authorization.Pages
{
    public partial class Autho : Page
    {
        private int failedAttempts = 0;
        private int secondsLeft = 10;
        private string captchaText = "";

        private bool timerEnabled = true;

        private DispatcherTimer timer = new DispatcherTimer();

        public Autho()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            CreateHistoryTable();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = GetPassword();

            if (login == "" || password == "")
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            try
            {
                if (CaptchaArea.Visibility == Visibility.Visible &&
                    TxtCaptchaInput.Text.Trim() != captchaText)
                {
                    FailedLogin(login, "Неверная CAPTCHA");
                    return;
                }

                string hash = Hash.HashPassword(password);
                UserLog user;

                using (RestaurantDataEntities db = new RestaurantDataEntities())
                {
                    user = db.UserLogs
                        .Include(x => x.Client)
                        .FirstOrDefault(x => x.Login == login && x.PasswordHash == hash);
                }

                if (user == null)
                {
                    FailedLogin(login, "Неверный логин или пароль");
                    return;
                }

                SaveHistory(login, true, null);
                ResetForm();

                MessageBox.Show("Вы вошли под ролью: Клиент");
                NavigationService.Navigate(new Client(user, "Клиент"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка базы данных: " + ex.GetBaseException().Message);
            }
        }

        private void FailedLogin(string login, string reason)
        {
            SaveHistory(login, false, reason);

            failedAttempts++;
            ClearPassword();
            TxtCaptchaInput.Clear();

            if (failedAttempts >= 3)
            {
                MessageBox.Show("Превышено количество попыток входа.");

                if (timerEnabled)
                {
                    StartBlock();
                }
                else
                {
                    failedAttempts = 0;
                    ShowCaptcha();
                    TxtAttempts.Text = "Осталось попыток: 3";
                }

                return;
            }

            ShowCaptcha();
            TxtAttempts.Text = "Осталось попыток: " + (3 - failedAttempts);
            TxtAttempts.Visibility = Visibility.Visible;

            MessageBox.Show(reason);
        }

        private void StartBlock()
        {
            secondsLeft = 10;
            SetInputsEnabled(false);

            TxtAttempts.Visibility = Visibility.Collapsed;
            TxtTimer.Visibility = Visibility.Visible;
            TxtTimer.Text = "Повторный ввод доступен через: 10 сек.";

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            secondsLeft--;
            TxtTimer.Text = "Повторный ввод доступен через: " + secondsLeft + " сек.";

            if (secondsLeft <= 0)
            {
                timer.Stop();
                failedAttempts = 0;
                TxtTimer.Visibility = Visibility.Collapsed;
                SetInputsEnabled(true);
                HideCaptcha();
            }
        }

        private void SetInputsEnabled(bool value)
        {
            TxtLogin.IsEnabled = value;
            PswPassword.IsEnabled = value;
            TxtPasswordVisible.IsEnabled = value;
            ChkShowPassword.IsEnabled = value;
            TxtCaptchaInput.IsEnabled = value;
            BtnRefreshCaptcha.IsEnabled = value;
            BtnEnter.IsEnabled = value;
        }

        private void ShowCaptcha()
        {
            CaptchaArea.Visibility = Visibility.Visible;
            GenerateCaptcha();
        }

        private void HideCaptcha()
        {
            CaptchaArea.Visibility = Visibility.Collapsed;
            TxtAttempts.Visibility = Visibility.Collapsed;
            TxtCaptchaInput.Clear();
            TxtCaptchaText.Text = "";
            captchaText = "";
        }

        private void GenerateCaptcha()
        {
            captchaText = CaptchaGenerator.GenerateCaptchaText(6);
            TxtCaptchaText.Text = captchaText;
        }

        private void CreateHistoryTable()
        {
            using (RestaurantDataEntities db = new RestaurantDataEntities())
            {
                db.Database.ExecuteSqlCommand(
                    @"IF OBJECT_ID('dbo.LoginHistory', 'U') IS NULL
                    CREATE TABLE dbo.LoginHistory
                    (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        UserLogin NVARCHAR(50) NOT NULL,
                        AttemptTime DATETIME NOT NULL,
                        IsSuccess BIT NOT NULL,
                        FailReason NVARCHAR(200) NULL
                    )");
            }
        }

        private void SaveHistory(string login, bool success, string reason)
        {
            using (RestaurantDataEntities db = new RestaurantDataEntities())
            {
                db.Database.ExecuteSqlCommand(
                    @"INSERT INTO dbo.LoginHistory
                    (UserLogin, AttemptTime, IsSuccess, FailReason)
                    VALUES (@p0, @p1, @p2, @p3)",
                    login, DateTime.Now, success, (object)reason ?? DBNull.Value);
            }
        }

        private string GetPassword()
        {
            if (ChkShowPassword.IsChecked == true)
            {
                return TxtPasswordVisible.Text;
            }

            return PswPassword.Password;
        }

        private void ClearPassword()
        {
            PswPassword.Clear();
            TxtPasswordVisible.Clear();
        }

        private void ResetForm()
        {
            timer.Stop();
            failedAttempts = 0;
            secondsLeft = 10;
            TxtTimer.Visibility = Visibility.Collapsed;
            SetInputsEnabled(true);
            HideCaptcha();
        }

        private void BtnRefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            TxtCaptchaInput.Clear();
        }

        private void ChkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            TxtPasswordVisible.Text = PswPassword.Password;
            PswPassword.Visibility = Visibility.Collapsed;
            TxtPasswordVisible.Visibility = Visibility.Visible;
        }

        private void ChkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PswPassword.Password = TxtPasswordVisible.Text;
            TxtPasswordVisible.Visibility = Visibility.Collapsed;
            PswPassword.Visibility = Visibility.Visible;
        }

        private void BtnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, "Гость"));
        }
    }
}
