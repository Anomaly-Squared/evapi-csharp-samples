using System;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Example
{
    class SampleListUsers
    {
        /**
         * sample_list_users.php - Use the UsersApi to create a report of account users
         */
        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleListUsers()
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

        public void ListUsers()
        {
            try
            {


                // We are demonstrating the use of the UsersApi, which can be used to retrieve user settings and create a report
                // We have to override the default configuration of the UserApi object with an updated host URL so that our code
                //  will reach the correct URL for the api.

                var usersApi = new UsersApi(evAccountUrl);


                // The listUsers method of the UsersApi class will give us access the users defined in our account
                // See https://www.exavault.com/developer/api-docs/#operation/listUsers for the details of this method

                // We must pass in our API Key and Access Token with every call, which we retrieved from the .env file above
                // This method also supports filtering parameters to limit the results returned. Check the link to 
                // our API documentation for a list of those parameters.
                UserCollectionResponse listResult = usersApi.ListUsers(evApiKey, evAccessToken);

                // If we got this far without the program ending, our call to listUsers succeeded and returned something
                // The call returns a \Swagger\Client\Model\UserCollectionResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/listUsers for the details of the response object

                var totalUsersForAccount = listResult.TotalResults;
                var totalUsersRetrieved = listResult.ReturnedResults;

                // The returned users will be an array of \Swagger\Client\Model\User objects which we can access from the 
                //   UserCollectionResponse::getData method

                var usersRetrieved = listResult.Data;

                // We are creating a CSV file in the same directory as this script. 
                // 
                // This opens the file for writing (which removes existing data) and gives us a file handle that we 
                // can use to write the CSV data with

                // Writing the column titles to our CSV file
                var outputFilename = "../../files/users_listing.csv";

                List<string> csvColumnHeaders = new List<string>()
                        {"Id", "Username", "Nickname", "Email Address", "Home Folder", "Role", "Time Zone",
                        "Download", "Upload", "Modify", "Delete", "List", "Change Password", "Share", "Notification",
                        "View Form Data", "Delete Form Data", "Expiration", "Last Logged In", "locked", "Created", "Modified" };

                // Build the file header
                string csvHeader = "";
                csvColumnHeaders.ForEach(line =>
                {
                    csvHeader += line + ",";
                });
                csvHeader = csvHeader.Remove(csvHeader.Length - 1);
                csvHeader += Environment.NewLine;

                File.WriteAllText(outputFilename, csvHeader);


                // Looping over the users array that we got back from our listUsers call.

                foreach (var user in usersRetrieved)
                {

                    string csvLine = "";

                    // The internal ID of a user isn't visible in the web file manager. It is used by the API to 
                    //   access the user. 
                    csvLine += user.Id + ",";

                    // The detailed data about the individual user is accessed through the User::getAttributes method
                    // which returns a \Swagger\Client\Model\UserAttributes object
                    csvLine += user.Attributes.Username + ",";
                    csvLine += user.Attributes.Nickname + ",";
                    csvLine += user.Attributes.Email + ",";
                    csvLine += user.Attributes.HomeDir + ",";
                    csvLine += ((UserAttributes.RoleEnum)user.Attributes.Role).ToString() + ",";
                    csvLine += user.Attributes.TimeZone + ",";
                    csvLine += user.Attributes.Created + ",";
                    csvLine += user.Attributes.Modified + ",";
                    csvLine += user.Attributes.AccessTimestamp + ",";
                    csvLine += user.Attributes.Expiration + ",";
                    csvLine += (user.Attributes.Status == 0 ? "" : "locked") + ",";

                    // The access timestamp returns a non-standard value representing 'never'
                    csvLine += (user.Attributes.AccessTimestamp.Substring(0, 4) == "0000" ? "never" : user.Attributes.AccessTimestamp);

                    // The UserAttributes::getPermissions method returns a \Swagger\Client\Model\UserPermissions object,
                    //   which contains the true/false flags for each of the permissions available to a user
                    //   See https://www.exavault.com/docs/account/04-users/00-introduction#managing-user-roles-and-permissions
                    //
                    csvLine += (user.Attributes.Permissions.Download == true ? "download" : "") + ",";
                    csvLine += (user.Attributes.Permissions.Upload == true ? "upload" : "") +",";
                    csvLine += (user.Attributes.Permissions.Modify == true ? "moodify" : "") +",";
                    csvLine += (user.Attributes.Permissions.Delete == true ? "delete" : "") +",";
                    csvLine += (user.Attributes.Permissions.List == true ? "list" : "") +",";
                    csvLine += (user.Attributes.Permissions.ChangePassword == true ? "change password" : "") +",";
                    csvLine += (user.Attributes.Permissions.Share == true ? "share" : "") +",";
                    csvLine += (user.Attributes.Permissions.Notification == true ? "notification" : "") +",";
                    csvLine += (user.Attributes.Permissions.ViewFormData == true ? "view form data" : "") +",";
                    csvLine += (user.Attributes.Permissions.DeleteFormData == true ? "delete_form_data" : "") +",";

                    csvLine = csvLine.Remove(csvLine.Length - 1);
                    csvLine += Environment.NewLine;
                    File.AppendAllText(outputFilename, csvLine);
                }

                Console.WriteLine("Listed: " +  totalUsersRetrieved + " users to " + outputFilename);
                Console.ReadLine();
            }
            catch (Exception ex)
            {

                // If there was a problem, such as our credentials not being correct, or the URL not working, 
                // there will be an exception thrown. 
                Console.WriteLine("Exception when calling UserApi->listUsers: " + ex.Message); 
            }


        }
    }

}
