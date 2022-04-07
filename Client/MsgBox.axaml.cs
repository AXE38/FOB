using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Client
{
    public partial class MsgBox : Window
    {
        public MsgBox() : this("")
        {

        }
        public MsgBox(string msg)
        {
            Validate(this, new RoutedEventArgs());
            MsgLbl.Text = msg;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void Validate(object sender, RoutedEventArgs e)
        {
            var p_info = (sender as Control).Name;
            if (p_info == this.Name)
            {
                InitializeComponent();

            }
            else if (p_info == "OK")
            {
                this.Close("OK");
            }
            else if (p_info == "CANCEL")
            {
                this.Close("CANCEL");
            }
            e.Handled = true;
        }
    }
}
