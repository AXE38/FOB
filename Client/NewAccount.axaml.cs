using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;

namespace Client
{
    public partial class NewAccount : Window
    {
        private DB_Cred cred;
        private bool isEdit = false;
        public NewAccount() : this(null)
        {

        }
        public NewAccount(DB_Cred cred)
        {
            this.cred = cred;
            Validate(this, new RoutedEventArgs());
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private async void Validate(object sender, RoutedEventArgs e)
        {
            var p_info = (sender as Control).Name;
            if (p_info == this.Name)
            {
                InitializeComponent();
                if (cred == null || cred.id == null)
                {
                    this.Title = "Добавить";
                    if (cred == null)
                        this.cred = new DB_Cred();
                    this.tbClientName.Text = cred.client_name;
                    this.dpCreateDate.SelectedDate = DateTime.Now;
                    this.tpCreateDate.SelectedTime = DateTime.Now.TimeOfDay;
                }
                else
                {
                    this.Title = "Изменить";
                    this.tbClientName.Text = cred.client_name;
                    this.tbNum.Text = cred.num;
                    this.tbIssS.Text = cred.iss_s.ToString();
                    this.tbS.Text = cred.s.ToString();
                    this.isEdit = true;
                    this.dpCreateDate.SelectedDate = cred.create_date;
                    this.tpCreateDate.SelectedTime = cred.create_date.TimeOfDay;
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
        }

        private async void Execute()
        {
            cred.num = this.tbNum.Text;
            cred.create_date = dpCreateDate.SelectedDate.Value.Date;
            cred.create_date = cred.create_date.Add((TimeSpan)tpCreateDate.SelectedTime);
            bool isValid = true;
            if (double.TryParse(this.tbIssS.Text.Replace(',', '.'), out double iss_s))
            {
                cred.iss_s = iss_s;
            }
            else
            {
                isValid = false;
                var msgBox = new MsgBox("Неверное число");
                await msgBox.ShowDialog(this);
                this.tbIssS.Text = "";
            }
            if (double.TryParse(this.tbS.Text.Replace(',', '.'), out double s))
            {
                cred.s = s;
            }
            else
            {
                isValid = false;
                var msgBox = new MsgBox("Неверное число");
                await msgBox.ShowDialog(this);
                this.tbS.Text = "";
            }

            if (isValid)
            {
                if (!isEdit)
                {
                    Handler.AddItem("Account", this.cred);
                    this.Close();
                }
                else
                {
                    Handler.EditItem("Account", this.cred);
                    this.Close();
                }
            }
        }
    }
}
