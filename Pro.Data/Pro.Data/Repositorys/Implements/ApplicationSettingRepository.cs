using MongoDB.Driver;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class ApplicationSettingRepository : IApplicationSettingRepository
    {
        private readonly IMongoCollection<ApplicationSetting> _applicationSettings;
        public ApplicationSettingRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _applicationSettings = database.GetCollection<ApplicationSetting>(settings.XStorysCollectionAppSetting);
        }
        public IQueryable<ApplicationSetting> GetAll()
        {
            return _applicationSettings.AsQueryable().Where(applicationSetting => true);
        }

        public ApplicationSetting GetById(int id) => _applicationSettings.Find(applicationSetting => applicationSetting.AppSettingId == id).FirstOrDefault();

        public ApplicationSetting Create(ApplicationSetting applicationSetting)
        {
            _applicationSettings.InsertOne(applicationSetting);
            return applicationSetting;
        }

        public List<ApplicationSetting> Creates(List<ApplicationSetting> applicationSettings)
        {
            _applicationSettings.InsertMany(applicationSettings);
            return applicationSettings;
        }
        public void Update(int id, ApplicationSetting updatedStory) => _applicationSettings.ReplaceOne(applicationSetting => applicationSetting.AppSettingId == id, updatedStory);

        public void Delete(ApplicationSetting storyForDeletion) => _applicationSettings.DeleteOne(applicationSetting => applicationSetting.AppSettingId == storyForDeletion.AppSettingId);

        public void Delete(int id) => _applicationSettings.DeleteOne(applicationSetting => applicationSetting.AppSettingId == id);

    }
}
