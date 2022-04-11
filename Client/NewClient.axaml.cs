using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;

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
            Debug.Print(p_info);
            if (p_info == this.Name)
            {
                InitializeComponent();
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
            }
            e.Handled = true;
        }

        private void Execute()
        {
            if (!isEdit)
            {
                client.fullname = this.tbFullname.Text;
                client.pass = this.tbPass.Text;
                client.phone = this.tbPhone.Text;
                Handler.AddItem("Client", this.client);
                this.Close();
            } else
            {
                client.fullname = this.tbFullname.Text;
                client.pass = this.tbPass.Text;
                client.phone = this.tbPhone.Text;
                Handler.EditItem("Client", this.client);
                this.Close();
            }
        }
    }
}
