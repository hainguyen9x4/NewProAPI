using MongoDB.Driver;
using Pro.Model;
using MongoDB.Driver.Linq;

namespace Pro.Data.Repositorys.Implements
{
    public class ResultScanDataRepository : IResultScanDataRepository
    {
        private readonly IMongoCollection<ResultScanData> _resultScanData;
        public ResultScanDataRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringAppSetting);
            var database = client.GetDatabase(settings.DatabaseName);
            _resultScanData = database.GetCollection<ResultScanData>(settings.XStorysCollectionResultScanData);
        }
        public IMongoQueryable<ResultScanData> GetAll()
        {
            return _resultScanData.AsQueryable();
        }

        public ResultScanData GetById(int id) => _resultScanData.Find(ResultScanData => ResultScanData.Id == id).FirstOrDefault();

        public ResultScanData Create(ResultScanData ResultScanData)
        {
            var settingId = 1 + _resultScanData.AsQueryable().Max(s => s.Id);
            ResultScanData.Id = settingId;
            _resultScanData.InsertOne(ResultScanData);
            return ResultScanData;
        }

        public List<ResultScanData> Creates(List<ResultScanData> ResultScanDatas)
        {
            var total = _resultScanData.AsQueryable().Max(s => s.Id);
            for (int i = 1; i <= ResultScanDatas.Count; i++)
            {
                ResultScanDatas[i-1].Id = i + total;
            }
            _resultScanData.InsertMany(ResultScanDatas);
            return ResultScanDatas;
        }
        public void Update(int id, ResultScanData updatedStory) => _resultScanData.ReplaceOne(ResultScanData => ResultScanData.Id == id, updatedStory);

        public void Delete(ResultScanData storyForDeletion) => _resultScanData.DeleteOne(ResultScanData => ResultScanData.Id == storyForDeletion.Id);

        public void Delete(int id) => _resultScanData.DeleteOne(ResultScanData => ResultScanData.Id == id);

        public void Update2(int id, ResultScanData updatedentity)
        {
            throw new NotImplementedException();
        }
    }
}
