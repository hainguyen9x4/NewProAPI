﻿using FileManager;
using Pro.Common;
using Pro.Common.Const;
using Pro.Model;

namespace Pro.Service.SubScanDataService.Implements
{
    public class PrepareService : IPrepareService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly AppBuildDataSetting _setting;
        private readonly IAppSettingData _settingData;

        public PrepareService(IApplicationSettingService applicationSettingService,
            IAppSettingData appSettingData)
        {
            _applicationSettingService = applicationSettingService;
            _settingData = appSettingData;
            var settings = _applicationSettingService.GetValueGetScan(ApplicationSettingKey.AppsettingsScanGet, useOtherSetting: _settingData.UseSettingGetSetNumber);
            _setting = JsonManager.StringJson2Object<AppBuildDataSetting>(settings);
        }
        public NewestChapModel PrepareNewestChapDatas()
        {
            var homeLinkWithSub = _applicationSettingService.GetValue(ApplicationSettingKey.HomePage) + _applicationSettingService.GetValue(ApplicationSettingKey.SubDataForHomePage);
            var folder = _setting.FolderSaveData + _setting.FolderNewestData;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            DirectoryInfo directory = new DirectoryInfo(folder);

            FileInfo? fileNewestData = directory.GetFiles(_setting.NewestDataFile + "*.json").OrderBy(f => f.Name).FirstOrDefault();

            var resultDatas = new NewestChapModel();
            if (fileNewestData != null && !string.IsNullOrEmpty(fileNewestData.FullName))
            {

                var dataNewstList = FileReader.ReadListDataFromFile<NewestChapModel>(fileNewestData.FullName).FirstOrDefault();

                if (dataNewstList != null && dataNewstList.ChapLinks.Any())
                {
                    dataNewstList.ChapLinks = FileReader.AddStoryNameToUrlLink(dataNewstList.ChapLinks, dataNewstList.StoryName);
                    dataNewstList.ChapLinks = FileReader.AddHomeUrlLink(dataNewstList.ChapLinks, homeLinkWithSub);

                    dataNewstList.StoryLink = homeLinkWithSub + dataNewstList.StoryLink;

                    dataNewstList.FileDataNewestPathLocal = fileNewestData;

                    resultDatas = dataNewstList;
                }
            }
            return resultDatas;
        }
    }
}