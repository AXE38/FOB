using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client
{
    public partial class NewClient : Window
    {
        public NewClient()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
