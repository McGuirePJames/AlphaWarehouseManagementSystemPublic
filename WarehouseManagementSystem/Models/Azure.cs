using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Helpers;

namespace WarehouseManagementSystem.Models
{
    public class Azure
    {
        internal static string GetStorageConnectionString()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[""].ToString();

            return connectionString;
        }
        public class FunctionApp
        {

        }
        public class Blob{

            public string StorageUri { get; set; }
            public string Uri { get; set; }

            internal static Models.Response UploadImageToBlobStorage(WebImage image, string containerName)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Models.Azure.GetStorageConnectionString());
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(image.FileName);
                
                try
                {
                    byte[] imageByteArray = image.GetBytes();
                    blob.UploadFromByteArray(imageByteArray, 0, imageByteArray.Length);
                    return new Models.Response() { Success = true, Message = "Success" };
                }
                catch (Exception ex)
                {
                    return new Models.Response() { Success = false, Message = ex.Message.ToString() };
                }

            }
            internal static void RemoveBlobFromStorage(string fileName, string containerName)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Models.Azure.GetStorageConnectionString());
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);
                blockBlob.Delete();
            }
            internal static Models.Azure.Blob GetBlobByName(string blobName, string containerName)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Models.Azure.GetStorageConnectionString());
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

                foreach (IListBlobItem blob in blobContainer.ListBlobs(null, false))
                {
                    CloudBlockBlob blockBlob = (CloudBlockBlob)blob;
                    if(blockBlob.Name == blobName)
                    {
                        return new Models.Azure.Blob(){ StorageUri = blob.StorageUri.ToString(), Uri = blob.Uri.ToString() };
                    }
                }
                return new Models.Azure.Blob();
            }
        }

    }
}