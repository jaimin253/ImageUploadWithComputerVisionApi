using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace TestFileUpload.Utility
{
    public static class AzureConnection
    {
        static string account = CloudConfigurationManager.GetSetting("AzureStorageAccountName");
        static string key = CloudConfigurationManager.GetSetting("AzureStorageAccountKey");
        public static CloudStorageAccount GetConnectionString()
        {
            string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net;", account, key);
            return CloudStorageAccount.Parse(connectionString);
        }
    }
}