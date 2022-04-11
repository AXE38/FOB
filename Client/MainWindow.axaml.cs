using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace Client
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<DB_Client> DB_Clients { get; set; }
        public ObservableCollection<DB_Cred> DB_Creds { get; set; }
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

        private async void Authorize()
        {
            this.Show();
            Auth a = new Auth(Handler.Token == null);
            a.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            object res = await a.ShowDialog<object>(this);
            Handler.Token = (string)res;
            Handler.Init_DB_result();
            Populate_Grids();
        }

        private void G_CR_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = getColumnName(sender as DataGrid, e.PropertyName);
        }

        private void G_CL_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = getColumnName(sender as DataGrid, e.PropertyName);
        }

        private void Populate_Grids()
        {
            string entity_type = "CLIENT";
            string[] columns = { "id", "fullname", "pass", "phone" };

            var res = Handler.SelectItems(entity_type, columns, offset: 0, fetch: 200);
            DB_Clients.Clear();
            if (res.state == "200")
            {
                foreach (XmlNode n in (XmlNodeList)res.result)
                {
                    DB_Clients.Add(new DB_Client()
                    {
                        id = Convert.ToInt32(n["id"]?.InnerText),
                        fullname = n["fullname"]?.InnerText,
                        pass = n["pass"]?.InnerText,
                        phone = n["phone"]?.InnerText
                    });
                }
            }
            G_CL.SelectedIndex = 0;

            entity_type = "VW_ACCOUNT";
            columns = new string[] { "id", "collection_id", "client_name", "num", "iss_s", "s", "create_date"};
            res = Handler.SelectItems(entity_type, columns, offset: 0, fetch:200);
            DB_Creds.Clear();
            if (res.state == "200")
            {
                
                foreach (XmlNode n in (XmlNodeList)res.result)
                {
                    DB_Creds.Add(new DB_Cred()
                    {
                        id = Convert.ToInt32(n["id"]?.InnerText),
                        collection_id = Convert.ToInt32(n["collection_id"]?.InnerText),
                        client_name = n["client_name"]?.InnerText,
                        num = n["num"]?.InnerText,
                        iss_s = Convert.ToDouble(n["iss_s"]?.InnerText),
                        s = Convert.ToDouble(n["s"]?.InnerText),
                        create_date = Convert.ToDateTime(n["create_date"]?.InnerText)
                    });
                }
            }
            G_CR.SelectedIndex = 0;

        }

        private void Init()
        {
            DB_Clients = new ObservableCollection<DB_Client>();
            DB_Creds = new ObservableCollection<DB_Cred>();
            G_CL.Items = DB_Clients;
            G_CL.AutoGeneratingColumn += G_CL_AutoGeneratingColumn;
            G_CL.SelectionChanged += G_CL_SelectionChanged;

            G_CR.Items = DB_Creds;
            G_CR.AutoGeneratingColumn += G_CR_AutoGeneratingColumn;

            TC1.SelectionChanged += TC1_SelectionChanged;
        }

        private void G_CL_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (G_CL.SelectedItem == null)
            {
                miEdit.IsEnabled = false;
                miDelete.IsEnabled = false;
                miOpenCr.IsEnabled = false;
            } else
            {
                miEdit.IsEnabled = true;
                miDelete.IsEnabled = true;
                miOpenCr.IsEnabled = true;
            }
        }

        private void TC1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            miOpenCr.IsVisible = Tab1.IsSelected;
            miNew.IsEnabled = !Tab2.IsSelected;
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
                m.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (await m.ShowDialog<string>(this) == "OK")
                {
                    if (Tab1.IsSelected)
                        Handler.DeleteItem("CLIENT", ((DB_Client)G_CL.SelectedItem).id);
                    else if (Tab2.IsSelected)
                        Handler.DeleteItem("ACCOUNT", ((DB_Cred)G_CR.SelectedItem).id);
                    Populate_Grids();
                }
            } else if (p_info == "miNew")
            {
                if (TC1.SelectedItem == Tab1)
                {
                    var cl = new NewClient();
                    cl.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    await cl.ShowDialog(this);
                }
                else if (TC1.SelectedItem == Tab2)
                {
                    var ac = new NewAccount();
                    ac.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    await ac.ShowDialog(this);
                }
                Populate_Grids();
            } else if (p_info == "miEdit")
            {
                if (TC1.SelectedItem == Tab1)
                {
                    var cl = new NewClient((DB_Client)G_CL.SelectedItem);
                    cl.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    await cl.ShowDialog(this);
                } else if (TC1.SelectedItem == Tab2)
                {
                    var ac = new NewAccount((DB_Cred)G_CR.SelectedItem);
                    ac.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    await ac.ShowDialog(this);
                }
                Populate_Grids();
            } else if (p_info == "miOpenCr") {
                var client = (DB_Client)G_CL.SelectedItem;
                var cred = new DB_Cred()
                {
                    client_name = client.fullname,
                    collection_id = client.id
                };
                var ac = new NewAccount(cred);
                ac.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                await ac.ShowDialog(this);
                Populate_Grids();
            } else if (p_info == "miAbout")
            {
                var aboutStr = "АБС \"Фронт-офис\" 0.1.0\n Автор: Сергей Мельник \n 2022";
                var msgBox = new MsgBox(aboutStr);
                msgBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                await msgBox.ShowDialog(this);
            }
        }

        private string getColumnName(DataGrid sender, string columnName)
        {
            if (sender == G_CL)
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
                    default:
                        return columnName;
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
                    default:
                        return columnName;
                }
            }
            return null;
        }
    }
}
