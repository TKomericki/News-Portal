using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace News_Portal.Models
{
    public class News
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("title")]
        [Required]
        public string Title { get; set; }

        [BsonElement("text")]
        [Required]
        public string Text { get; set; }

        [BsonElement("date")]
        [Required]
        public DateTime Date { get; set; }

        [BsonElement("author")]
        [Required]
        public string Author { get; set; }

        [BsonElement("image")]
        [Required]
        public byte[] Image { get; set; }

        [BsonElement("comments")]
        [Required]
        public List<BsonDocument> Comment { get; set; }
    }
}
