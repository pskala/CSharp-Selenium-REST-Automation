using mySpaceName.Helpers.Api;

namespace mySpaceName.Pages
{
    abstract class BasePage
    {
        protected readonly Context context;

        protected BasePage(Context context)
        {
            this.context = context;
        }
    }
}
