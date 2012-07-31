using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Health;
using Microsoft.Health.ItemTypes;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;


namespace Samples.HvMvc
{
    public class HVUserImageHelper
    {
        private static HVUserImageHelper _default;
        public static HVUserImageHelper Default
        {
            get
            {
                if (_default == null)
                    _default = new HVUserImageHelper();
                return _default;
            }
        }

        // Azure blob storage keys
        private string _accountName = "sampleshvmvc";
        private string _accountKey = "+La88lUFw5B7VpP/PPi621wompDCc+oPqf5/PVn+DW27VHVjdA8uumTZ6MpBeHF5eBedVBeEyfYDWIQ9vQIqng==";
        private string _containerName = "userimages";

        private HVUserImageHelper() { }

        /// <summary>
        /// Saves a HealthVault record image to blob storage for future use
        /// </summary>
        /// <param name="record"></param>
        public void SaveImageToBlobStorage(HealthRecordInfo record)
        {
            // get the items for the health record and set the type to personImage
            var collection = record.GetItemsByType(PersonalImage.TypeId, HealthRecordItemSections.All);

            PersonalImage image = null;
            if (collection.Count != 0)
            {
                // get the first item which is the image
                image = collection[0] as PersonalImage;

                // Create a stream to read the image into
                using (Stream currentImageStream = image.ReadImage())
                {
                    // Read the image
                    byte[] imageBytes = new byte[currentImageStream.Length];
                    currentImageStream.Read(imageBytes, 0, (int)currentImageStream.Length);

                    // create vars to access blob storage account
                    var account = CloudStorageAccount.Parse(ConnectionString);
                    var client = account.CreateCloudBlobClient();
                    var container = client.GetContainerReference(_containerName);
                    // if it does not exist create it
                    if (container.CreateIfNotExist())
                    {
                        // since it's new we want to change the persmissions of the container to be public
                        var p = new BlobContainerPermissions();
                        p.PublicAccess = BlobContainerPublicAccessType.Container;
                        container.SetPermissions(p);
                    }

                    // get a block blob reference
                    var item = container.GetBlockBlobReference(record.Id.ToString() + ".jpg");

                    // set the type to image
                    item.Properties.ContentType = "image\\jpeg";

                    // upload to blob storage
                    item.UploadByteArray(imageBytes);
                }
            }
        }

        /// <summary>
        /// Gets a health records image url in Azure blob storage
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public string GetImageUrl(HealthRecordInfo record)
        {
            return GetImageUrl(record.Id);
        }

        /// <summary>
        /// Gets a health records image url in Azure blob storage
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public string GetImageUrl(Guid id)
        {
            return GetImageUrl(id.ToString());
        }

        /// <summary>
        /// Gets the image Url of a user by id in Azure blob storage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetImageUrl(string id)
        {
            return string.Format("http://{0}.blob.core.windows.net/{1}/{2}.jpg", _accountName, _containerName, id);
        }


        private string ConnectionString
        {
            get
            {
                return string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _accountName, _accountKey);
            }
        }

    }
}