using System;
using ExaVault.Api;
using ExaVault.Client;
using ExaVault.Model;
using System.Configuration;
using System.Collections.Generic;

namespace Example
{
    class SampleAddNotifications
    {
        /**
       * sample_get_account.php - Use the AccountApi to return your account info and check disk usage
       */

        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleAddNotifications()
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

        public void AddNotifications()
        {
            var parentFolder = "";
            var uploadFolder = "";
            var downloadFolder = "";

            try
            {
                // We are demonstrating the use of the NotificationsApi, which can be used to manage notification settings 
                //  for files and folders

                // For this demo, we'll create a new folder tree and add notifications to those new folders. If you have 
                // an existing file or folder that you want to create a notification for, you won't need the step where
                // we use the ResourcesApi to create the folders first.
                //
                // We have to override the default configuration of the API object with an updated host URL so that our code
                // will reach the correct URL for the api. We have to override this setting for each of the API classes we use
                var resourcesApi = new ResourcesApi(evAccountUrl); // string | Account URL required for the request


                // We will create a new folder tree for the demo. The top-level folder will have a different name each time 
                // you run this script
                parentFolder = "sample_notifications_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                // We can actually be sneaky and add missing parent folders by passing a multi-level path
                uploadFolder = "/" + parentFolder + "/uploads";
                downloadFolder = "/" + parentFolder + "/downloads";

                // API methods that take a JSON body, such as the addFolder method, require us to submit an object with the 
                // parameters we want to send to the API. This call requires a single parameter path
                var requestBody = new AddFolderRequestBody(uploadFolder);
                // See https://github.com/ExaVault/evapi-csharp/blob/main/docs/AddFolderRequestBody.md for the details of AddFolderRequestBody model

                // We have to pass the $API_KEY and $ACCESS_TOKEN with every API call. 
                ResourceResponse result = resourcesApi.AddFolder(evApiKey, evAccessToken, requestBody);


                // The addFolder method of the ResourcesApi returns a \Swagger\Client\Model\ResourceResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/addFolder for the details of the response object
                Console.WriteLine("Created new folder {0}", result.Data.Attributes.Path);

                // Now we can add the second folder
                // See https://github.com/ExaVault/evapi-csharp/blob/main/docs/AddFolderRequestBody.md for the details of AddFolderRequestBody model
                requestBody = new AddFolderRequestBody(downloadFolder);   
                result = resourcesApi.AddFolder(evApiKey, evAccessToken, requestBody);

                // The addFolder method of the ResourcesApi returns a \Swagger\Client\Model\ResourceResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/addFolder for the details of the response object
                Console.WriteLine("Created new folder {0}", result.Data.Attributes.Path);

            }
            catch (Exception ex)
            {
                // If there was a problem, such as our credentials not being correct, or the URL not working, 
                // there will be an exception thrown 

                Console.WriteLine("Exception when calling ResourcesApi-> addFolder: " + ex.Message);
                Console.ReadLine();
            }

            try
            {
                // If we got this far without the program ending, we were able to set up our folders to create notifications,
                //   and now we can use the NotificationsApi to create those
                // We have to override the default configuration of the API object with an updated host URL so that our code
                //  will reach the correct URL for the api.
                var notificationsApi = new NotificationsApi(evAccountUrl); // string | Account URL required for the request

                // API methods that take a JSON body, such as the addFolder method, require us to submit an object with the 
                // parameters we want to send to the API. 
                // See https://www.exavault.com/developer/api-docs/#operation/addNotification for the request body schema

                // - We want to be notified by email whenever anyone downloads from our downloads folder, so we are using
                //   the constant "notice_user_all", which means anyone, including users and share recipients.
                //   See  https://www.exavault.com/developer/api-docs/#operation/addNotification  for a list of other 
                //   constants that can be used in the usernames array
                // - Note that the downloadFolder variable contains the full path to the folder that was created earlier
                // - We could also have pulled the ID for the new folder out of the ResourceResponse object and used that 
                //   as a resource identifier here. For example, if the ID of the downloads folder is 23422, we could pass
                //   id:23422 in the resource parameter of this call.

                var bodyType = AddNotificationRequestBody.TypeEnum.Folder;
                var bodyResource = downloadFolder;
                var bodyAction = AddNotificationRequestBody.ActionEnum.Download;
                var bodyUsernames = new List<string>(new string[] { "notice_user_all" });
                var bodySendEmail = true;

                var notificationRequestBody = new AddNotificationRequestBody(bodyType, bodyResource, bodyAction, bodyUsernames, bodySendEmail);
                // See https://github.com/ExaVault/evapi-csharp/blob/main/docs/AddNotificationRequestBody.md for the details of AddNotificationRequestBody model


                // We must pass in our API Key and Access Token with every call, which we retrieved from the ConfigurationManager.
                NotificationResponse result = notificationsApi.AddNotification(evApiKey, evAccessToken, notificationRequestBody);
                Console.WriteLine("Created download notification for {0}", downloadFolder);

                // - Next we will add a notification that will send a message to several addresses when a user uploads
                //   into our uploads folder. 
                // - As with the other notification, we will pass in the full path to the folder in the resource parameter
                //
                // There are some things we're doing differently:
                //   - We're using a different constant for the usernames parameter "notice_users_all_users", which means
                //   only trigger notifications when an action is done by a user account (not share recipients) 
                //   See  https://www.exavault.com/developer/api-docs/#operation/addNotification for a list of other 
                //   constants that can be used in the usernames array
                //   - We are sending the notification to a bunch of email addresses, rather than just our own
                //   - We have added an optional custom message to be included in each notification email


                notificationRequestBody.Type = AddNotificationRequestBody.TypeEnum.Folder;
                notificationRequestBody.Resource = uploadFolder;
                notificationRequestBody.Action = AddNotificationRequestBody.ActionEnum.Upload;
                notificationRequestBody.Usernames = new List<string>(new string[] { "notice_user_all" });
                notificationRequestBody.SendEmail = true;
                notificationRequestBody.Recipients = new List<string>(new string[] { "sally@example.com", "sidharth@example.com", "lgomez@example.com" });
                notificationRequestBody.Message = "Files have been uploaded into the main folder for you.";

                // We must pass in our API Key and Access Token with every call, which we retrieved from the ConfigurationManager. 
                result = notificationsApi.AddNotification(evApiKey, evAccessToken, notificationRequestBody);
                Console.WriteLine("Created upload notification for {0}", uploadFolder);
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception when calling NotificationsApi->addNotification: " + ex.Message);
                Console.ReadLine();
            }

        }
    }

}
