﻿using Pro.Common;
using Pro.Model;

namespace Pro.Service.SubScanDataService
{
    public interface IGetRawDataService
    {
        bool GetRawDatasForNew(NewStory newestDatas);
        bool FindNewStory(int numberPage, string homeUrl);
    }
}
