using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;

namespace Client
{
    public partial class Auth : Window
    {
        private bool isValid = false;

        public Auth() : this(false)
        {
        }

        public Auth(bool isValid)
        {
            this.isValid = isValid;
            Validate(this, new RoutedEventArgs());
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnClose(object? sender, EventArgs e)
        {
            if (!isValid)
                ((App?)Application.Current).Exit();
            
        }

        private void Validate(object sender, RoutedEventArgs e)
        {
            var p_info = (sender as Control).Name;
            if (p_info == this.Name)
            {
                InitializeComponent();

            } else if (p_info == "OK")
            {   
                Execute();
            } else if (p_info == "CANCEL")
            {
                this.Close();
            }
            e.Handled = true;
        }

        private void Execute()
        {
            isValid = false;
            isValid = true;
            this.Close();
        }
    }
}
