using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client
{
    public partial class MainWindow : Window
    {

        public ObservableCollection<DB_Client> DB_Clients { get; set; }
        public ObservableCollection<DB_Cred> DB_Creds { get; set; }
        private string token { get; set; } = null;
        public MainWindow()
        {
            InitializeComponent();
            Init();
#if DEBUG
            this.AttachDevTools();
#else
            Authorize();
#endif
        }

        private void Authorize()
        {
            this.Show();
            Auth a = new Auth(token == null);
            a.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            a.ShowDialog(this);
        }

        private void G_CR_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = getColumnName(sender as DataGrid, e.PropertyName);
        }

        private void GCL_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = getColumnName(sender as DataGrid, e.PropertyName);
        }

        private void Init()
        {
            DB_Clients = new ObservableCollection<DB_Client>();
            DB_Creds = new ObservableCollection<DB_Cred>();
            gCL.Items = DB_Clients;
            gCL.AutoGeneratingColumn += GCL_AutoGeneratingColumn;

            G_CR.Items = DB_Creds;
            G_CR.AutoGeneratingColumn += G_CR_AutoGeneratingColumn;

            TC1.SelectionChanged += TC1_SelectionChanged;

            DB_Clients.Add(new DB_Client()
            {
                id = 1,
                fullname = "123",
                pass = "1245",
                phone = "567890"
            });
        }

        private void TC1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            miOpenCr.IsVisible = Tab1.IsSelected;
        }

        private async void Validate(object? sender, RoutedEventArgs e)
        {
            var p_info = (sender as Control).Name;
            if (p_info == "miExit")
            {
                ((App?)Application.Current).Exit();
            } else if (p_info == "miRelog")
            {
                Authorize();
            } else if (p_info == "miDelete")
            {
                var m = new MsgBox("Вы уверены?");
                if (await m.ShowDialog<string>(this) != "OK")
                {
                    Handler.DeleteItem(null, 0);
                }
            } else if (p_info == "miNew")
            {
                var cl = new NewClient();
                cl.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                cl.ShowDialog(this);
            } else if (p_info == "miEdit")
            {
                var cl = new NewClient((DB_Client)gCL.SelectedItem);
                cl.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                cl.ShowDialog(this);
            }
        }

        private string getColumnName(DataGrid sender, string columnName)
        {
            if (sender == gCL)
            {
                switch (columnName)
                {
                    case "id":
                        return "ID";
                    case "fullname":
                        return "ФИО";
                    case "pass":
                        return "Паспортные данные";
                    case "phone":
                        return "Номер телефона";
                }
            } else if (sender == G_CR)
            {
                switch (columnName)
                {
                    case "id":
                        return "ID";
                    case "collection_id":
                        return "ID клиента";
                    case "client_name":
                        return "ФИО";
                    case "num":
                        return "Номер";
                    case "iss_s":
                        return "Сумма выдачи";
                    case "s":
                        return "Сумма";
                    case "create_date":
                        return "Дата выдачи";
                }
            }
            throw new InvalidOperationException();
        }
    }
}
