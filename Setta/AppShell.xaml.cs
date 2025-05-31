using Setta.Pages;

namespace Setta
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddWorkoutPage), typeof(AddWorkoutPage));
        }
    }
}
