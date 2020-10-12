using System;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using System.Configuration;

namespace Example
{
    class SampleAddUser
    {
        /**
        * sample_add_user.php - Use the UsersApi to create a new user with a home directory
        */
        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleAddUser()
        {
            /**
            *To use this script, add your credentials to App.Config or Web.Config file
            *
            *Your API key will be the EV_KEY
            *Your access token will be EV_TOKEN
            * Your account URL will be the address you should use for the API endpoint
            *
            *To obtain your API Key and Token, you'll need to use the Developer page within the web file manager
            * See https://www.exavault.com/developer/api-docs/#section/Obtaining-Your-API-Key-and-Access-Token
            *
            *Access tokens do not expire, so you should only need to obtain the key and token once.
            *
            *Your account URL is determined by the name of your account.

            * The URL that you will use is https://accountname.exavault.com/api/v2/ replacing the "accountname" part with your
            *account name
            * See https://www.exavault.com/developer/api-docs/#section/Introduction/The-API-URL
            */

            this.evApiKey = ConfigurationManager.AppSettings["EV_KEY"];  // string | API Key required for the request
            this.evAccessToken = ConfigurationManager.AppSettings["EV_TOKEN"];  // string | Access Token for the request
            this.evAccountUrl = ConfigurationManager.AppSettings["ACCOUNT_URL"];  // string | Access Token for the request

        }

        public void AddUser()
        {
            try
            {
                // We are demonstrating the use of the UsersApi, which is used to create, update and remove users in your account.
                //
                // We have to override the default configuration of the API object with an updated host URL so that our code
                // will reach the correct URL for the api.
                var usersApi = new UsersApi(evAccountUrl); // string | Account URL required for the request

                // API methods that take a JSON body, such as the addUser method, require us to submit an object with the 
                // parameters we want to send to the API. 
                // See https://www.exavault.com/developer/api-docs/#operation/addUser for the request body schema
                //
                // We're going to use our API Key and a timestamp as the username because usernames must be unique
                var newUsername = evApiKey + "-" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                var homeResource = "/Home directory for API Users";
                var email = "test@example.com";
                var password = "99drowssaP";
                var role = Body5.RoleEnum.User;
                var permission = "download,upload,modify,list,changePassword,share,notification,delete";
                var timeZone = "UTC";
                var nickname = "Created via the API";
                var welcomeEmail = true;

                var requestBody = new Body5(newUsername, nickname, homeResource, email, password, role, permission, 
                                            timeZone, null, null, welcomeEmail);

                // We must pass in our API Key and Access Token with every call, which we retrieved from the ConfigurationManager.
                UserResponse result = usersApi.AddUser(evApiKey, evAccessToken, requestBody);

                // The UsersApi::addUser method returns a \Swagger\Client\Model\UserResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/addUser for the response body schema
                var userid = result.Data.Id;

                Console.WriteLine("Created new user {0} as ID #{1}", newUsername, userid);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // If there was a problem, such as our credentials not being correct, or the URL not working, 
                // there will be an exception thrown 
                Console.WriteLine("Exception when calling UsersApi->addUser: " + ex.Message);
                Console.ReadLine();
            }
        }

    }
}
