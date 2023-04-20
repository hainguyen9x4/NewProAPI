﻿using Pro.Common;

namespace Pro.Service
{
    public interface IUploadImageService
    {
        IResultUpload UploadImage(string filePath);
        IResultUpload UploadImage(string fileName, string filePath, string pathSave = "");
        IResultUpload UploadImage(string fileName, string url, string pathSave = "", bool isNeedConvert = false, int retryTime = 2, int sleepNextRetryTime = 30 * 1000);
        DataStoryForSave UploadLink2Store(DataStoryForSave dataStory);
    }
}