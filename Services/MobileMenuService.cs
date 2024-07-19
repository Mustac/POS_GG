using POS_OS_GG.Helpers;

namespace POS_OS_GG.Services
{
    public class MobileMenuService
    {

        public MobileNavigation MobileNavigation { get; private set; }

        public event Action<MobileNavigation> OnMobileNavClicked;

        public void MobileNavClicked(MobileNavigation mobileNavigation)
        {
            OnMobileNavClicked?.Invoke(mobileNavigation);
            MobileNavigation = mobileNavigation;
        }
    }
}
