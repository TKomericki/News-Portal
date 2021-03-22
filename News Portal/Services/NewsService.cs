using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using News_Portal.Models;

namespace News_Portal.Services
{
    public class NewsService : INewsService
    {
        private readonly IMongoCollection<News> news;

        public NewsService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("database"));
            IMongoDatabase database = client.GetDatabase("nmbp");
            news = database.GetCollection<News>("news");
        }

        public List<News> getNNews(int N)
        {
            List<News> result = new List<News>();

            result = news.Find(new BsonDocument()).Sort(new BsonDocument { { "date", -1 } }).Limit(N).ToList();

            return result;
        }

        public void addComment(Guid id, string comment, DateTime date)
        {
            news.UpdateOne(new BsonDocument { { "_id", id } }, new BsonDocument { { "$push", new BsonDocument { { "comments", new BsonDocument { { "comment", comment }, { "date", date } } } } } });
        }

        public List<BsonDocument> NumOfNewsByComments()
        {
            string map = @"function(){
                if(this.comments !== undefined){
                emit(this.comments.length, {count: 1});
                }
            }";

            string reduce = @"function(key, values){
                var num = 0;
                values.forEach(function(value){
                    num += value.count;            
                });
                return key, {count: num};
            }";

            string finalize = @"function(key, reducedVal){
                return key, reducedVal.count;
            }";

            var options = new MapReduceOptions<News, BsonDocument>
            {
                OutputOptions = MapReduceOutputOptions.Inline,
                Finalize = finalize,
            };
            var results = news.MapReduce(map, reduce, options).ToList().OrderByDescending(s => s["_id"]).ToList();

            return results;
        }

        public List<BsonDocument> PercentageCommented()
        {
            string map = @"function(){
                if(this.comments !== undefined){
                    var commented = this.comments.length > 0 ? 1 : 0;
                    emit(0, {yes: commented, no: 1 - commented});
                }
            }";

            string reduce = @"function(key, values){
                var yes = 0;
                var no = 0;
                values.forEach(function(value){
                    yes += value.yes;
                    no += value.no;            
                });
                return key, {yes: yes, no: no};
            }";

            string finalize = @"function(key, reducedVal){
                var total = reducedVal.yes + reducedVal.no;
                var percent = Math.round(10000 * reducedVal.yes / total) / 10000;
                return key, percent;
            }";

            var options = new MapReduceOptions<News, BsonDocument>
            {
                OutputOptions = MapReduceOutputOptions.Inline,
                Finalize = finalize
            };
            var results = news.MapReduce(map, reduce, options);

            return results.ToList();
        }

        public List<BsonDocument> MostfrequentWords()
        {
            string map = @"function(){
                var title = this.title.split(/[?:,|. ]+/);
                var text = this.text.split(/[?:,|. ]+/);
                var words = {};
                for(var i = 0; i < title.length; i++){
                    if(title[i].length != 0){
                        if(!words.hasOwnProperty(title[i].toLowerCase())) words[title[i].toLowerCase()] = 0;
                        words[title[i].toLowerCase()]++;
                    }
                }
                for(var i = 0; i < text.length; i++){
                    if(text[i].length != 0){
                        if(!words.hasOwnProperty(text[i].toLowerCase())) words[text[i].toLowerCase()] = 0;
                        words[text[i].toLowerCase()]++;
                    }
                }
                emit(this.author, words);
            }";

            string reduce = @"function(key, values){
                var words = {};
                values.forEach(function(value){
                    for(var word in value){
                        if(!words.hasOwnProperty(word)) words[word] = 0;
                        words[word] += value[word];
                    }          
                });
                return key, words;
            }";

            string finalize = @"function(key, reducedVal){
                var words = [];
                for(var word in reducedVal){
                    words.push([word, reducedVal[word]]);
                }
                words.sort(function (a, b){
                    if(b[1] == a[1]) return a[0] > b[0];
                    else return b[1] - a[1];
                });

                limit = words.length < 10 ? words.length : 10;
                result = {}
                for(var i = 0; i < limit; i++){
                    result[words[i][0]] = words[i][1];
                }
                /*result = []
                for(var i = 0; i < limit; i++){
                    result.push(words[i][0]);
                }*/
                return key, result;
            }";

            var options = new MapReduceOptions<News, BsonDocument>
            {
                OutputOptions = MapReduceOutputOptions.Inline,
                Finalize = finalize
            };
            var results = news.MapReduce(map, reduce, options);

            return results.ToList();
        }
    }
}
