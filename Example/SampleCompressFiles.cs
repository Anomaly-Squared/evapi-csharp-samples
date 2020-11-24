using System;
using ExaVault.Api;
using ExaVault.Client;
using ExaVault.Model;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace Example
{
    class SampleCompressFiles
    {
        /**
         * sample_compress_files.php - Use the Resources API to compress files 
         */
        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleCompressFiles()
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

        public void CompressFiles()
        {
            try
            {
                // We are demonstrating the use of the ResourcesApi, which is used for file operations (upload, download, delete, etc)
                //
                // For this demo, we'll create a new folder and upload some files into it. Then we'll compress some of the files into
                // a new zip file in the folder

                // We have to override the default configuration of the API object with an updated host URL so that our code
                //  will reach the correct URL for the api. We have to override this setting for each of the API classes we use
                var resourcesApi = new ResourcesApi(evAccountUrl); // string | Account URL required for the request

                // We will create a new folder tree for the demo. The top-level folder will have a different name each time you run this script
                var parentFolder = "sample_compress_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                // We are uploading a sample file provided along with this script.
                // It will have a different name in the account each time it is uploaded
                var fileName = "../../files/dog.jpg";
                byte[] file = System.IO.File.ReadAllBytes(fileName);

                int targetSize = file.Length;


                // We'll store the IDs, which we'll grab from the responses from new resource uploads, that we want to compress
                List<string> compressResources = new List<string>();
                //string[] compressResources = new string[6];

                for (int i = 1; i < 6; i++)
                {
                    // We're  uploading the same file under different names to make sure we have multiple files in our target folder
                    var targetFilename = "/" + parentFolder + "/dog" + i + ".jpg";


                    // The uploadFile method of the ResourcesApi class will let us upload a file to our account
                    // See https://www.exavault.com/developer/api-docs/#operation/uploadFile for the details of this method
                    //

                    ResourceResponse resultUpload = resourcesApi.UploadFile(evApiKey, evAccessToken, targetFilename, targetSize, file);

                    // We want to make an archive that contains the files we've uploaded
                    // The ResourcesApi::uploadFile method returns a \Swagger\Client\Model\ResourceResponse object
                    // The ResourceResponse::getData method will give us a Resource object
                    // The Resource::getId method will give us the resource ID of the newly uploaded file
                    compressResources.Add("id:" + resultUpload.Data.Id);
                }

                Console.WriteLine("Uploaded starting files to {0} " , parentFolder);

                // We stored the resource IDs of all the files we want to compress into the compressResources[] array

                // API methods that take a JSON body, such as the compressFiles method, require us to submit an object with the 
                // parameters we want to send to the API. 
                // See https://www.exavault.com/developer/api-docs/#operation/compressFiles for the request body schema
                //
                // This will overwrite an existing zip file with a new one
                var requestBody = new CompressFilesRequestBody(compressResources, "/", "zipped_files.zip");
                ResourceResponse result = resourcesApi.CompressFiles(evApiKey, evAccessToken, requestBody);

                // The ResourcesApi::compressFiles method returns a \Swagger\Client\Model\ResourceResponse object
                Console.WriteLine("Created archive at {0}", result.Data.Attributes.Path);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // If there was a problem, such as our credentials not being correct, or the URL not working, 
                // there will be an exception thrown 
                Console.WriteLine("Exception when calling resourcesApi: " + ex.Message);
                Console.ReadLine();
            }
        }
    }


}
