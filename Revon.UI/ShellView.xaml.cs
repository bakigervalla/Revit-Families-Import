using System.Windows;

namespace Revon.UI.Views
{
    /// <summary>
    /// The class of our modeless dialog.
    /// </summary>
    /// <remarks>
    /// Besides other methods, it has one method per each command button.
    /// In each of those methods nothing else is done but setting a request
    /// to be later picked up by the Idling handler. All those commands
    /// are performed on doors currently selected in the active document.
    /// </remarks>
    /// 
    public partial class ShellView : Window
    {

        public ShellView()
        {
            InitializeComponent();

        }

        //App app;

        //public ShellView(App _app)
        //{
        //    app = _app;
        //    InitializeComponent();
        //}

    }
}
