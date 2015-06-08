﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Net;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using GuestBook_Data;

namespace GuestBook_WebRole
{
    public partial class _Default : System.Web.UI.Page
    {
        private static bool storageInitialized = false;
        private static CloudBlobClient blobStorage;
        private static CloudQueueClient queueStorage;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Timer1.Enabled = true;
            }
        }

        protected void SignButton_Click(object sender, EventArgs e)
        {
            if (this.FileUpload1.HasFile)
            {
                this.InitializeStorage();

                string uniqueBlobName = string.Format("image_{0}{1}",
                    Guid.NewGuid().ToString(),
                    Path.GetExtension(this.FileUpload1.FileName));
                CloudBlockBlob blob =
                    blobStorage.GetContainerReference("guestbookpics")
                    .GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType =
                    this.FileUpload1.PostedFile.ContentType;
                blob.UploadFromStream(this.FileUpload1.FileContent);

                GuestBookEntry entry = new GuestBookEntry()
                {
                    GuestName = this.NameTextBox.Text,
                    Message = this.MessageTextBox.Text,
                    PhotoUrl = blob.Uri.ToString(),
                    ThumbnailUrl = blob.Uri.ToString()
                };
                GuestBookDataSource ds = new GuestBookDataSource();
                ds.AddGuestBookEntry(entry);
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            this.DataList1.DataBind();
        }

        private void InitializeStorage()
        {
            if (!storageInitialized)
            {
                var storageAccount = CloudStorageAccount.Parse(
                        CloudConfigurationManager.GetSetting("DataConnectionString"));
                blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference(
                    "guestbookpics");
                container.CreateIfNotExists();

                var permissions = container.GetPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);

                storageInitialized = true;
            }
        }
    }
}
