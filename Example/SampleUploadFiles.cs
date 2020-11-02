using System;
using ExaVault.Api;
using ExaVault.Client;
using ExaVault.Model;
using System.Configuration;

namespace Example
{
    class SampleUploadFiles
    {
        /**
         * sample_upload_files.php - Use the ResourcesApi to upload a file to your account
         */
        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleUploadFiles()
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

        public void UploadFiles()
        {
            try
            {
                // We are demonstrating the use of the ResourcesApi, which can be used to manage files and folders in your account

                // For this demo, we'll upload a file found in the same folder as this sample code.
                //
                // We are going to upload the file as a different name each time so that it is obvious that the file is being upload
                // There are parameters to control whether files can be overwritten by repeated uploads
                //
                // We have to override the default configuration of the API object with an updated host URL so that our code
                //  will reach the correct URL for the api. We have to override this setting for each of the API classes we use

                var resourcesApi = new ResourcesApi(evAccountUrl);
                // We are uploading a sample file provided along with this script.
                // It will have a different name in the account each time it is uploaded
                var fileName = "../../files/dog.jpg";
                byte[] file = System.IO.File.ReadAllBytes(fileName);
                var targetFilename = "dog" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + ".jpg";

                int targetSize = file.Length;


                // The uploadFile method of the ResourcesApi class will let us upload a file to our account
                // See https://www.exavault.com/developer/api-docs/#operation/uploadFile for the details of this method
                //
                ResourceResponse result = resourcesApi.UploadFile(evApiKey, evAccessToken, targetFilename, targetSize, file);


                // The uploadFile method of the ResourcesApi returns a \Swagger\Client\Model\ResourceResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/uploadFile for the details of the response object

                // Verify that the uploaded file's reported size matches what we expected.
                // The getAttributes method of the ResourceResponse object will let us get the details of the file
                var sizeUploaded = result.Data.Attributes.Size;

                //    $result->getData()->getAttributes()->getSize();

                if (sizeUploaded != targetSize)
                {
                    Console.WriteLine("Uploaded file does not match expected size. Should be {0} but is {1}", targetSize, sizeUploaded);
                }

                // Success! 
                Console.WriteLine("Uploaded {0}", result.Data.Attributes.Path);
                Console.ReadLine();


            }
            catch (Exception ex)
            {

                // If there was a problem, such as our credentials not being correct or not having upload permissions,
                // there will be an exception thrown. 
                Console.WriteLine("Exception when calling ResourcesApi->uploadFile: " + ex.Message);
            }

        }
    }
}
