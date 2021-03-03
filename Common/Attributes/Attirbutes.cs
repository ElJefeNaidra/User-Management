namespace UserManagement.Common.Attributes
{
    /// <summary>
    /// A decorator telling the developer that the controller method is an API rather than
    /// an functional method i.e. data insertion, retrieval etc
    /// </summary>
    internal class APIAttribute : Attribute
    {
    }

    /// <summary>
    /// A decorator telling the developer that the controller method is to appear on a menu tree
    /// as an item.
    /// </summary>
    internal class NavigationAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    internal class AuthorisationFriendlyNameAttribute : Attribute
    {
        public string FriendlyName { get; private set; }

        public AuthorisationFriendlyNameAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }
    }

    internal class AuthorisationIsMenuItemAttribute : Attribute
    {
    }

    internal class AuthorisationIsParentAttribute : Attribute
    {
    }

    internal class AuthorisationIsControllerActionAttribute : Attribute
    {
    }

}