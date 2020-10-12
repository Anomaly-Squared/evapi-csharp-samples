using System;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using System.Configuration;

namespace Example
{
    class SampleGetAccountInfo
    {
        /**
        * sample_get_account.php - Use the AccountApi to return your account info and check disk usage
        */

        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleGetAccountInfo()
        {

            /**
             * To use this script, add your credentials to App.Config or Web.Config file
             * 
             * Your API key will be the EV_KEY
             * Your access token will be EV_TOKEN
             * Your account URL will be the address you should use for the API endpoint
             * 
             * To obtain your API Key and Token, you'll need to use the Developer page within the web file manager
             * See https://www.exavault.com/developer/api-docs/#section/Obtaining-Your-API-Key-and-Access-Token
             * 
             * Access tokens do not expire, so you should only need to obtain the key and token once.
             * 
             * Your account URL is determined by the name of your account. 
             * The URL that you will use is https://accountname.exavault.com/api/v2/ replacing the "accountname" part with your
             *   account name
             * See https://www.exavault.com/developer/api-docs/#section/Introduction/The-API-URL
             */

            this.evApiKey = ConfigurationManager.AppSettings["EV_KEY"];  // string | API Key required for the request
            this.evAccessToken = ConfigurationManager.AppSettings["EV_TOKEN"];  // string | Access Token for the request
            this.evAccountUrl = ConfigurationManager.AppSettings["ACCOUNT_URL"];  // string | Access Token for the request
        }

        public void GetAccountInfo()
        {
            try
            {
                // We are demonstrating the use of the AccountAPI, which can be used to manage the account settings 
                // We have to override the default configuration of the AccountApi with an updated host URL so that our code
                //  will reach the correct URL for the api.
                var apiInstance = new AccountApi(evAccountUrl); // string | Account URL required for the request


                // The GetAccount method of the AccountApi class will give us access to the current status of our account
                // See https://www.exavault.com/developer/api-docs/#operation/getAccount for the details of this method

                // We must pass in our API Key and Access Token with every call, which we retrieved from the ConfigurationManager above

                AccountResponse result = apiInstance.GetAccount(evApiKey, evAccessToken);

                // If we got this far without the program ending, our call to getAccount succeeded and returned something
                // The call returns a Swagger\Client\Model\AccountResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/getAccount for the details of the response object

                // The AccountResponse object that we got back (result) is composed of additional, nested objects
                // The Quota object will tell us how much space we've use
                var quota = result.Data.Attributes.Quota;

                // The values returned in the Quota object are given in bytes, so we convert that to GB
                var accountMaxSize = quota.DiskLimit / (1024 * 1024 * 1024);
                var accountCurrentSize = quota.DiskUsed / (1024 * 1024 * 1024);


                Console.WriteLine("Account used : {0} GB  ({1} %)", accountCurrentSize, Math.Round(Convert.ToDouble(accountCurrentSize / accountMaxSize * 100), 1));
                Console.WriteLine("Total Size   : {0} GB ", accountMaxSize);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // If there was a problem, such as our credentials not being correct, or the URL not working, 
                // there will be an exception thrown 

                Console.WriteLine("Exception when calling AccountApi.GetAccount: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
