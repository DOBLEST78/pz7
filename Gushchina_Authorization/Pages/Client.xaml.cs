using Gushchina_Authorization.Model;
using System.Windows.Controls;

namespace Gushchina_Authorization.Pages
{
    public partial class Client : Page
    {
        public Client(UserLog user, string role)
        {
            InitializeComponent();
            TxtRole.Text = "Роль: " + role;

            if (user == null || user.Client == null)
            {
                TxtWelcome.Text = "Добро пожаловать!";
                TxtDetails.Text = "Вы вошли в приложение как гость.";
                return;
            }

            TxtWelcome.Text = "Добро пожаловать, " + user.Client.FullName + "!";
            TxtDetails.Text =
                "Логин: " + user.Login + "\n" +
                "Телефон: " + user.Client.Phone + "\n" +
                "Электронная почта: " + user.Client.Email + "\n" +
                "Дата рождения: " + user.Client.BirthDate.ToString("dd.MM.yyyy") + "\n" +
                "Предпочтения: " + (string.IsNullOrWhiteSpace(user.Client.Preferences)
                    ? "не указаны"
                    : user.Client.Preferences);
        }
    }
}
