using System;
using ExaVault.Api;
using ExaVault.Client;
using ExaVault.Model;
using System.Configuration;
using System.Collections.Generic;

namespace Example.files
{
    class SampleSharedFolder
    {
        /**
         * sample_shared_folder.php - Use the SharesApi to create a shared folder with a password
         */
        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleSharedFolder()
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

        public void SharedFolders()
        {
            var folderPath = "";

            try
            {
                // We are demonstrating the use of the SharesApi, which is used for managing shared folders and receives,
                // as well as for sending files. See our Sharing 101 documentation at
                // https://www.exavault.com/docs/account/05-file-sharing/00-file-sharing-101

                // For this demo, we'll create a share for a new folders. If you have an existing file or folder that you want to use 
                // for the share, you won't need this step where we use the ResourcesApi to create the folders first.
                //
                // We have to override the default configuration of the API object with an updated host URL so that our code
                //  will reach the correct URL for the api. We have to override this setting for each of the API classes we use
                var resourcesApi = new ResourcesApi(evAccountUrl);


                // We will create a new folder for the demo. The folder will have a different name each time you run this script
                folderPath = "/sample_share_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                // API methods that take a JSON body, such as the addFolder method, require us to submit an object with the 
                // parameters we want to send to the API. This call requires a single parameter path
                var requestBody = new AddFolderRequestBody(folderPath);

                //// We have to pass the $API_KEY and $ACCESS_TOKEN with every API call. 
                ResourceResponse result = resourcesApi.AddFolder(evApiKey, evAccessToken, requestBody);

                // The addFolder method of the ResourcesApi returns a \Swagger\Client\Model\ResourceResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/addFolder for the details of the response object
                Console.WriteLine("Created new folder {0}", result.Data.Attributes.Path);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception when calling ResourcesApi->addFolder: " + ex.Message);
                Console.ReadLine();
            }

            try
            {
                // If we got this far without the program ending, we were able to set up our folder
                // and now we can use the SharesApi to share the folder.
                //
                // We have to override the default configuration of the API object with an updated host URL so that our code
                // will reach the correct URL for the api.

                var sharesApi = new SharesApi(evAccountUrl);

                // API methods that take a JSON body, such as the addShare method, require us to submit an object with the 
                // parameters we want to send to the API. 
                // See https://www.exavault.com/developer/api-docs/#operation/addShare for the request body schema

                // - We want to add a password to our folder
                // - We are also going to allow visitors to upload and download
                // - Note that the $folder_path variable contains the full path to the folder that was created earlier
                // - We could also have pulled the ID for the new folder out of the ResourceResponse object and used that 
                //   as a resource identifier here. For example, if the ID of the new folder is 23422, we could pass
                //   id:23422 in the resource parameter of this call.


                var type = AddShareRequestBody.TypeEnum.Sharedfolder;
                var name = "Share";
                List<string> resources = new List<string>();
                resources.Add(folderPath); 
                
                List<AddShareRequestBody.AccessModeEnum> accessMode = new List<AddShareRequestBody.AccessModeEnum>();
                accessMode.Add(AddShareRequestBody.AccessModeEnum.Download);
                accessMode.Add(AddShareRequestBody.AccessModeEnum.Upload);
                var password = "99drowssaP?";

                var requestBody = new AddShareRequestBody(type, name, resources, accessMode, null, null, null, null, null, null, null, password);

                // We have to pass the $API_KEY and $ACCESS_TOKEN with every API call. 
                ShareResponse result = sharesApi.AddShare(evApiKey, evAccessToken, requestBody);
                // The SharesApi::addShare method returns a \Swagger\Client\Model\RegularShareResponse object
                //  See https://www.exavault.com/developer/api-docs/#operation/addShare for the response schema

                Console.WriteLine("Created shared folder {0} for {1} ", result.Data.Attributes.Hash, folderPath);
                Console.WriteLine("Password to access the folder is 99drowssaP?");
                Console.ReadLine();

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception when calling SharesApi->addShare:" + ex.Message);
                Console.ReadLine();
            }

        }
    }
}
