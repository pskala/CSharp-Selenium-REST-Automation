using mySpaceName.Helpers.Api;

namespace mySpaceName.Pages
{
    class LoginPage : BasePage
    {
        public string UserName
        {
            set
            {
                context.SetValue(value, "#login_field");
            }
        }

        public string Password
        {
            set
            {
                context.SetValue(value, "#password");
            }
        }

        public LoginPage(Context context) : base(context)
        {
        }

        public void Login(string userName, string password)
        {
            context.Click(".HeaderMenu-link.no-underline.mr-3");
            UserName = userName;
            Password = password;
            SubmitButtonClick();
        }

        private void SubmitButtonClick()
        {
            context.Click(".btn.btn-primary.btn-block");
        }
    }
}