using System;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using System.Configuration;
using System.Collections.Generic;
namespace Example
{
    class SampleGetFailedLogins
    {
        /**
         * sample_get_failed_logins.php - Use the ActivityApi to retrieve the list of users who had failed logins 
         * in the last 24 hours.
         */
        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleGetFailedLogins()
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

        public void GetFailedLogins()
        {
            try
            {
                // We are demonstrating the use of the ActivityApi, which can be used to retrieve session and webhook logs
                // We have to override the default configuration of the ActivityApi object with an updated host URL so that our code
                //  will reach the correct URL for the api.

                var apiInstance = new ActivityApi(evAccountUrl);
                // The getSessionLogs method of the ActivityApi class will give us access activity logs for our account
                // See https://www.exavault.com/developer/api-docs/#operation/getSessionLogs for the details of this method

                // We must pass in our API Key and Access Token with every call, which we retrieved from the .env file above
                // This method also supports filtering parameters to limit the results returned. Check the link to 
                // our API documentation for a list of those parameters.

                var today = DateTime.Today;
                var startDate = today.AddDays(-1);
                var endDate = today;
                var type = "PASS";
                var offset = 0;
                var limit = 200;
                var sort = "-date";

                SessionActivityResponse listResult = apiInstance.GetSessionLogs(evApiKey, evAccessToken, startDate, endDate,
                                    null, null, null, type, offset, limit, sort);



                // If we got this far without the program ending, our call to getSessionLogs succeeded and returned something
                // The call returns a \Swagger\Client\Model\SessionActivityResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/getSessionLogs for the details of the response object

               // List<KeyValuePair<string,int>> failedLogins = new List<KeyValuePair<string, int>>(); // Container to hold our info

                Dictionary<string,int> failedLogins = new Dictionary<string, int>();

                // The returned activity will be an array of \Swagger\Client\Model\SessionActivityEntry objects, which we can access
                //   from the SessionActivityResponse::getData method
                var activityLogs = listResult.Data;
                // Loop over the returned items, which should include only "Connect" operations, per our filters to the getSessionLogs call
                foreach (var activity in activityLogs ) {
                    // Each SessionActivityEntry object has a getAttributes method that allows us to access the details for the 
                    // logged activity, which will be a \Swagger\Client\Model\SessionActivityEntryAttributes object

                    // The SessionActivityEntryAttributes object has accessors for username, client IP address, status, operation, etc
                    if (activity.Attributes.Status == "failed") {
                        if (!failedLogins.ContainsKey(activity.Attributes.Username))
                        {
                            failedLogins[activity.Attributes.Username] = 1;
                        } else
                        {
                            failedLogins[activity.Attributes.Username] += 1;
                        }
                    }
                }

                Console.WriteLine(failedLogins.Count + " Users with failed logins: " );
                Console.WriteLine( "Username        Count");
                Console.WriteLine("============    ============");

                foreach (var item in failedLogins)
                {
                    string key = item.Key.PadRight(12);
                    string value = item.Value.ToString().PadRight(12);

                    Console.Write(key);
                    Console.WriteLine(value);

                }

                Console.ReadLine();


            }
            catch (Exception ex)
            {

                // If there was a problem, such as our credentials not being correct, or the URL not working, 
                // there will be an exception thrown. 
                Console.WriteLine("Exception when calling ActivityApi->getSessionLogs:" + ex.Message);
                Console.ReadLine();
            }

        }
    }
}
