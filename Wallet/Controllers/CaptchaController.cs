using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wallet.Models;

namespace Wallet.Controllers
{
    public class CaptchaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ValidGoogleReCaptcha(string recaptchaResponse)
        {
            //public key 6LeM0WEUAAAAACazkj0iQm-4OKPdrJdTyadTEuud
            //secret key 6LeM0WEUAAAAAHSBqeLjZzw_9FYsz25vohLVDxWr
            const string secret = "6LeM0WEUAAAAAHSBqeLjZzw_9FYsz25vohLVDxWr";
            string path = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
            var client = new WebClient();
            var reply = client.DownloadString(string.Format(path, secret, recaptchaResponse));
            var captchaResponse = JsonConvert.DeserializeObject<CaptchaGoogle>(reply);            
            if (!captchaResponse.Success)
                return Json(captchaResponse.ErrorCodes);
            else
                return Json("ReCaptcha is valid.");
        }
    }
}