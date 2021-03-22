using MongoDB.Bson;
using News_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace News_Portal.Services
{
    public interface INewsService
    {
        public List<News> getNNews(int N);
        public void addComment(Guid id, string comment, DateTime date);
        public List<BsonDocument> NumOfNewsByComments();
        public List<BsonDocument> PercentageCommented();
        public List<BsonDocument> MostfrequentWords();
    }
}
