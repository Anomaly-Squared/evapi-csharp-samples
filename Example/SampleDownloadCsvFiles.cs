using System;
using ExaVault.Api;
using ExaVault.Client;
using ExaVault.Model;
using System.Configuration;
using System.Collections.Generic;

namespace Example
{
    class SampleDownloadCsvFiles
    {
        /**
        * sample_download_csv_files.php - Use the ResourcesApi to download all of the CSV files found within a folder tree
        */
        private readonly string evApiKey;
        private readonly string evAccessToken;
        private readonly string evAccountUrl;

        public SampleDownloadCsvFiles()
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
        public void DownloadCsvFiles()
        {
            try
            {
                // We are demonstrating the use of the ResourcesApi, which can be used to manage files and folders in your account

                // We have to override the default configuration of the API object with an updated host URL so that our code
                //  will reach the correct URL for the api. We have to override this setting for each of the API classes we use

                var resourceApi = new ResourcesApi(evAccountUrl);

                // For this demo, we want to download all of the CSV files located within a certain folder.
                // - Your account comes pre-loaded with a folder tree named "Sample Files and Folders" which contains 
                // a folder tree containing many samples. If you have renamed, deleted or moved this folder,
                // this demo script will not work.

                // First, we'll get a list of all the CSV files within the desired folder

                ResourceCollectionResponse listResult = resourceApi.ListResources(evApiKey, evAccessToken, "/Sample Files and Folders", null, 0, null, "file", "*.csv");

                // The ResourcesApi::listResources method returns a \Swagger\Client\Model\ResourceCollectionResponse object
                // See https://www.exavault.com/developer/api-docs/#operation/listResources for the response schema

                // The ResourceCollectionResponse::getReturnedResults method will indicate how many matching files are included
                // in this response. If we didn't find any matches, there's nothing else to do
                if (listResult.ReturnedResults == 0)
                {
                    Console.WriteLine("Found no files to download");
                    return;
                }
                else
                {
                    Console.WriteLine("Found " + listResult.ReturnedResults + " CSV files to download");
                }



                // If we got this far, there are files for us to download
                // We are going to save the IDs of all the files we want to download into an array
                List<string> downloads = new List<string>();
                List<Resource> listedFiles = listResult.Data;

                foreach (var listedFile in listedFiles)
                {
                    downloads.Add("id:" + listedFile.Id);
                    Console.WriteLine(listedFile.Attributes.Path);
                }


                // Now that we used the ResourcesApi to gather all of the IDs of the resources that 
                // matched our search, we will use the DownloadApi to download multiple files
                /****************************************************************************************/
                /** NOTE - THIS IS AN UNUSUAL WORKAROUND REQUIRED BY THE AUTO-GENERATED PHP ClIENT SDK **/
                /****************************************************************************************/
                /** Ideally, we would use the ResourcesApi for all resources calls, but due to a bug in
                 * the library that creates the ResourcesApi, you cannot download multiple files at once
                 * using that API. Instead, use the DownloadApi download methods which has the same parameters and
                 * output as the ResourcesApi See https://www.exavault.com/developer/api-docs/#operation/download
                 */
                /************************************************************************************/
                byte[] downloadResult = resourceApi.Download(evApiKey, evAccessToken, downloads);

                // The body of the result is the binary content of our file(s), 
                // We write that content into a single file, named with .zip if there were multiple files 
                // downloaded or just named .csv if not (since we were storing csvs)

                var downloadFile = "";

                if (downloads.Count > 1)
                {
                    downloadFile = "../../files/download.zip";
                }
                else
                {
                    downloadFile = "../../files/download-" + +new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + ".csv";
                }

                System.IO.File.WriteAllBytes(downloadFile, downloadResult);
                Console.WriteLine("File(s) downloaded to {0}", downloadFile);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception when calling Api " + ex.Message);
            }
        }
    }
}
