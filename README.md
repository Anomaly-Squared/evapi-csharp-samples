# ExaVault C# API Sample Code - v2.0

## Introduction
Welcome to the sample code for ExaVault's C# code library, which demonstrates how to use various aspects of our API with your ExaVault account. The C# code library is available [on Github](https://github.com/ExaVault/evapi-csharp). The library is generated from our API's [public swagger YAML file](https://www.exavault.com/api/docs/evapi_2.0_public.yaml)

## Requirements
To run these scripts, you'll need Visual Studio installed as well as .Net Framework 4.0 or later.

You will also need an ExaVault account as well as and an API key and access token.

Some of the sample scripts will assume your account contains the **ExaVault Quick Start.pdf** file and the **Sample Files and Folders** folder, which come pre-loaded with a new account. You may need to make changes to `SampleUploadFiles.cs` and `SampleDownloadCsvFiles` if those files are not present.

## Running Your First Sample
**Step 1 - Installation**

- Clone to local development

  ```bash
  $ git clone git@github.com:ExaVault/evapi-csharp-samples.git
  ```

*Alternatively, if you want to run samples in a new .Net project, all you need to do for this is copying `bin\Release` folder onto your local computer and add all Dlls in this folder as reference into your project. You need to edit your App.config or Web.config file like explained below. Then, you can add the related class and test it.*

**Step 2 - Get your API Credentials**

The next step is to generate an API key and token from your ExaVault account. You'll need to log into the ExaVault web file manager, as an admin-level user, to get the API key and access token. See [our API reference documentation](https://www.exavault.com/developer/api-docs/v2/#section/Obtaining-Your-API-Key-and-Access-Token) for the step-by-step guide to create your key and/or token.

If you are not an admin-level user of an ExaVault account, you'll need someone with admin-level access to follow the steps and give you the API key and access token.

**Step 3 - Add Your API Credentials to the sample code**

You need to add your API credentials into the App.Config file. Edit this file like below.

```bash
  <appSettings>
    <add key="ACCOUNT_URL" value="https://your_account_name.exavault.com/api/v2"/>
    <add key="EV_KEY" value="your_key_here"/>
    <add key="EV_TOKEN" value="your_access_token"/>
  </appSettings>
```

- replace **your\_key\_here** with your API key. Don't add any extra spaces or punctuation
- replace **your\_token\_here** with your access token.
- replace **your\_account_name** with the name of your ExaVault account

And save the file.

**Step 4 - Run the sample script**

In `Program.cs` file, you will see all samples as commented. All you need to test, uncomment the method you want.

```bash
    //SampleGetAccountInfo sampleGetAccountInfo = new SampleGetAccountInfo();
    //sampleGetAccountInfo.GetAccountInfo();

    //SampleAddNotifications sampleAddNotifications = new SampleAddNotifications();
    //sampleAddNotifications.AddNotifications();

    //SampleAddUser sampleAddUser = new SampleAddUser();
    //sampleAddUser.AddUser();

    //SampleCompressFiles sampleCompressFiles = new SampleCompressFiles();
    //sampleCompressFiles.CompressFiles();

    //SampleDownloadCsvFiles sampleDownloadCsvFiles = new SampleDownloadCsvFiles();
    //sampleDownloadCsvFiles.DownloadCsvFiles();

    //SampleGetFailedLogins sampleGetFailedLogins = new SampleGetFailedLogins();
    //sampleGetFailedLogins.GetFailedLogins();

    //SampleListUsers sampleListUsers = new SampleListUsers();
    //sampleListUsers.ListUsers();

    //SampleSharedFolder sampleSharedFolder = new SampleSharedFolder();
    //sampleSharedFolder.SharedFolders();

    //SampleUploadFiles sampleUploadFiles = new SampleUploadFiles();
    //sampleUploadFiles.UploadFiles();
```
Now you're ready to run your first sample. Try SampleGetAccountInfo first

Just remove the comment from `SampleGetAccountInfo`

```bash
    SampleGetAccountInfo sampleGetAccountInfo = new SampleGetAccountInfo();
    sampleGetAccountInfo.GetAccountInfo();
```

If everything worked, the sample code will run and connect to your account. You'll see output on the console similar to this:

```bash
Account used: 40GB (11.4%)
Total size: 350GB
```

## Running Other Sample Files

There are several other sample classes that you can now run. You won't need to repeat the steps to set up the App.Config each time - the same environment information is used for all of the sample scripts.
Some of the sample scripts will make changes to your account (uploading, creating shares or notifications, etc). Those are marked with an asterisk below:

Script                        | Purpose    \*=Makes changes to your account when run                                   | APIs Used                      |
------------------------------|----------------------------------------------------------------------------------------|--------------------------------|
SampleGetAccountInfo.cs       | List the amount of available space for your account                                    | AccountApi                     |
SampleAddNotifications.cs     | Add upload and download notifications<br/>_\*adds folders to your account_             | ResourcesApi, NotificationsApi |
SampleAddUser.cs              | Add a new user with a home directory <br/>_\*adds a user and a folder to your account_ | UsersApi                       |
SampleCompressFiles.cs        | Compress several files into a zip file <br/>_\*adds files and folders to your account_ | ResourcesApi                   |
SampleDownloadCsvFiles.cs     | Search for files matching a certain extension, then download them.                     | ResourcesApi                   |
SampleGetFailedLogins.cs      | List usernames who had a failed login in the last 24 hours                             | ActivityApi                    |
SampleListUsers.cs            | Generate a report of users in your account                                             | UsersApi                       |
SampleSharedFolder.cs         | Create a new shared folder with a password<br />_\*adds a folder to your account_      | ResourcesApi, SharesApi        |
SampleUploadFiles.cs          | Upload a file to your account.<br />_\*uploads sample PDFS to your account_            | ResourcesApi                   |

## If Something Goes Wrong

**Problem - The type or namespace name '???' does not exist in the namespace **

An assembly could be missing. Please check your references to see if there is any yellow marked reference. If so, the package may not be the latest version. We recommend using NuGet to obtain the latest version of the packages.

```bash
Install-Package RestSharp
Install-Package Newtonsoft.Json
Install-Package JsonSubTypes
```

**Problem - 401 Unauthorized Response**

If running the sample script returns a 401 Unauthorized error like the one shown below, there is a problem with your API credentials. Double-check that the .env file exists and contains the correct values. If all else fails, you may need to log into the ExaVault web file manager and re-issue your access token.

```bash
Exception when calling AccountApi->getAccount: [401] Client error: `GET https://exavaultsupport.exavault.com/api/v2/account` resulted in a `401 Unauthorized` response:
{"responseStatus":401,"errors":[{"code":"ERROR_INVALID_CREDENTIALS","detail":"HTTP_UNAUTHORIZED"}]}
```

**Other problems with sample code**

If you encounter any other issues running this sample code, you can contact ExaVault support for help at support@exavault.com.

## Writing Your Own Code

When you're ready to write your own code, you can use our sample code as examples. You'll need to:

1. Install our code packages by downloading from  `bin\Release` folder or [applying steps our C# API reference documentation](https://github.com/ExaVault/evapi-csharp)
2. You can use the App.config or Web.config depending on your project just like our sample scripts do, or just set variables within your scripts for your API key and access token
3. Whenever you instantiate an Api object (ResourcesApi, UsersApi, etc.), override the configuration to point the code at the correct API URL:
```C#
var evAccountUrl  = "https://YOUR_ACCOUNT_NAME_HERE.exavault.com/api/v2/";
var accountApi = new AccountApi(evAccountUrl);
```
```C#
var evAccountUrl  = "https://YOUR_ACCOUNT_NAME_HERE.exavault.com/api/v2/";
var resourcesApi = new ResourcesApi(evAccountUrl);
```
```C#
var evAccountUrl  = "https://YOUR_ACCOUNT_NAME_HERE.exavault.com/api/v2/";
var usersApi = new UsersApi(evAccountUrl);
);
```

## Author

support@exavault.com

