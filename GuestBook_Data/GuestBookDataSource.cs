using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace GuestBook_Data
{
    public class GuestBookDataSource
    {
        public static CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;

        static GuestBookDataSource()
        {
            storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("GuestBookEntry");
            table.CreateIfNotExists();
        }

        public GuestBookDataSource()
        {
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public void AddGuestBookEntry(GuestBookEntry newItem)
        {
            TableOperation operation = TableOperation.Insert(newItem);
            CloudTable table = tableClient.GetTableReference("GuestBookEntry");
            table.Execute(operation);
        }

    }
}
