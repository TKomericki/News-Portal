using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using News_Portal.Services;
using Newtonsoft.Json;

namespace News_Portal.Controllers
{
    public class NewsController : Controller
    {
        INewsService service;
        private readonly ILogger<NewsController> _logger;

        public NewsController(ILogger<NewsController> logger, INewsService service)
        {
            _logger = logger;
            this.service = service;
        }
        public ActionResult Index()
        {
            var news = service.getNNews(15);
            ViewData["news"] = news;
            return View();
        }

        [HttpPost]
        public ActionResult addComment(object data)
        {
            string comment = Request.Query["comment"].ToString();
            Guid id = Guid.Parse(Request.Query["id"]);
            DateTime date = DateTime.Parse(Request.Query["date"]);
            service.addComment(id, comment, date);
            return Json(data);
        }

        public ActionResult MapReduce()
        {
            ViewData["MR1"] = service.NumOfNewsByComments();
            ViewData["MR2"] = service.PercentageCommented();
            ViewData["MR3"] = service.MostfrequentWords();
            return View();
        }

    }
}