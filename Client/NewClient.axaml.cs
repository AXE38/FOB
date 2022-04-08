using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace Client
{
    public partial class NewClient : Window
    {
        private DB_Client client;
        private bool isEdit = false;
        public NewClient() : this(null)
        {

        }
        public NewClient(DB_Client client)
        {
            this.client = client;
            Validate(this, new RoutedEventArgs());
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /*private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }*/

        private void Validate(object sender, RoutedEventArgs e)
        {
            var p_info = (sender as Control).Name;
            if (p_info == this.Name)
            {
                InitializeComponent();
                tbFullname.TextInput += (a, e) =>
                {
                    Validate(tbFullname, new RoutedEventArgs());
                };
                tbPass.TextInput += (a, e) =>
                {
                    Validate(tbPass, new RoutedEventArgs());
                };
                tbPhone.TextInput += (a, e) =>
                {
                    Validate(tbPhone, new RoutedEventArgs());
                };
                if (client == null)
                {
                    this.Title = "Добавить";
                    this.client = new DB_Client();
                } else
                {
                    this.Title = "Изменить";
                    this.tbFullname.Text = client.fullname;
                    this.tbPass.Text = client.pass;
                    this.tbPhone.Text = client.phone;
                    this.isEdit = true;
                }
            }
            else if (p_info == "OK")
            {
                Execute();
            }
            else if (p_info == "CANCEL")
            {
                this.Close();
            } else if (p_info == "tbFullname")
            {
                client.fullname = this.tbFullname.Text;
            } else if (p_info == "tbPass")
            {
                client.pass = this.tbPass.Text;
            } else if (p_info == "tbPhone")
            {
                client.phone = this.tbPhone.Text;
            }
            e.Handled = true;
        }

        private void Execute()
        {
            if (!isEdit)
            {
                Handler.AddItem("Client", this.client);
            } else
            {
                Handler.EditItem("Client", this.client);
            }
        }
    }
}
