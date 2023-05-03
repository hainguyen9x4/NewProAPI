using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pro.Model;
using System;

namespace Pro.Data.Repositorys.Implements
{
    public class ImageRepository : IImageRepository
    {
        private readonly IMongoCollection<ImagesOneChap> _images;
        public ImageRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringMain);
            var database = client.GetDatabase(settings.DatabaseName);
            _images = database.GetCollection<ImagesOneChap>(settings.XStorysCollectionImage);
        }
        public IMongoQueryable<ImagesOneChap> GetAll() => _images.AsQueryable().Where(image => true);
        //public List<Chap> GetAll() => _images.Find(image => true).ToList();

        public ImagesOneChap GetById(int id) => _images.Find(image => image.Id == id).FirstOrDefault();

        public ImagesOneChap Create(ImagesOneChap image)
        {
            _images.InsertOne(image);
            return image;
        }

        public List<ImagesOneChap> Creates(List<ImagesOneChap> chaps)
        {
            var Id = 1 + _images.AsQueryable().Count();
            foreach (var c in chaps)
            {
                c.Id = Id++;
            }
            _images.InsertMany(chaps);
            return chaps;
        }

        public void Update(int id, ImagesOneChap updatedStory) => _images.ReplaceOne(story => story.Id == id, updatedStory);

        public void Delete(ImagesOneChap storyForDeletion) => _images.DeleteOne(story => story.Id == storyForDeletion.Id);

        public void Delete(int id) => _images.DeleteOne(story => story.Id == id);
    }
}
