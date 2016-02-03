using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;
using photo.Models;
using global::PayPal;
using global::PayPal.Api.Payments;
using System.Text.RegularExpressions;
namespace photo.Controllers
{
    public class HomeController : Controller
    {
        private static OAuthTokenCredential oauth;
        private PhotographsDBEntities db = new PhotographsDBEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult test()
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

        [HttpGet]
        public string getImgs()
        {
            List<ImageInfo> stuff = new List<ImageInfo>();
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            string auth = Request.Url.LocalPath;
            string path = originalUri.AbsoluteUri;
            string root = path.Replace(auth, "");
            if (auth == "/") { root = ""; }
            else { root += "/"; }
            string a = @"~\Images";
            DirectoryInfo directory = new DirectoryInfo(Server.MapPath(@"~\Images"));
            var imgs = directory.GetFiles().ToList();

            foreach (PhotoInfo i in db.PhotoInfoes)
            {
                ImageInfo ii = new ImageInfo();
                ii.FileName = i.FileName;
                ii.Id = i.Id;
                ii.Info = i.Info;
                ii.Name = i.Name;
                ii.FileLocation = root + "Images/" + i.FileName;
                stuff.Add(ii);
            }

           // foreach (FileInfo i in imgs)
           // {
          //      stuff.Add(root + "Images/" + i.ToString());
          //  }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(stuff);

            return json;

        }


        [HttpGet]
        public string getImgAddress(string fileName)
        {
            List<string> stuff = new List<string>();
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            string auth = originalUri.PathAndQuery;
            string path = originalUri.AbsoluteUri;
            string root = path.Replace(auth, "");
            if (auth == "/") { root = ""; }
            else { root += "/"; }
            string a = @"~\Images";
            DirectoryInfo directory = new DirectoryInfo(Server.MapPath(@"~\Images"));
            var imgs = directory.GetFiles().ToList();
            foreach (FileInfo i in imgs)
            {
                if (fileName == i.ToString())
                {
                    stuff.Add(root + "Images/" + i.ToString());
                }
            }


            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(stuff);

            return json;

        }


        [HttpGet]
        public string getImgInfo(string imgInfo)
        {

            string fn = Regex.Replace(imgInfo, @"Images/", "");
            PhotoInfo pi = new PhotoInfo();
            foreach (PhotoInfo i in db.PhotoInfoes)
            {
                if (i.FileName == fn)
                {
                    pi = i;
                    break;
                }
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(pi);

            return json;
        }

        [HttpGet]
        public string findImgInfo(string imgInfo)
        {

            string fn = Regex.Replace(imgInfo, @"Images/", "");
            PhotoInfo pi = new PhotoInfo();
            foreach (PhotoInfo i in db.PhotoInfoes)
            {
                if (i.Name == fn)
                {
                    pi = i;
                    break;
                }
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(pi);

            return json;
        }
        private APIContext GetAPIContext()
        {
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            sdkConfig.Add("mode", "sandbox");
            string accessToken = new OAuthTokenCredential("Ae2ZWMxCl_ueuNy87vcg52hTjX9aVWfnvLQSMjDuTn2sj0crrWYIWwPseO_6H4nLpXKcHE9_DjtrmDEC", "EEmZr7iiuNCksXtPh5NjcVcguVGic0TwCW-f7GFmgfmrG8wBUhn_UJj53OxraTkKijC4UYQHv-fzlH7z", sdkConfig).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);
            return apiContext;
        }

        [HttpPost]
        public string Purchase(string photoname, string size, string cost)
        {
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            //sdkConfig.Add("mode", "sandbox");
            //oauth = new OAuthTokenCredential("AUFotB09UUG4p70W-HOh23z_-UOXNq25AR8ybXstqYPVLvcAMX6DPEE3FtKQO1w7S953NxNAVCVGQzf7", "EBSCyGJf9V2Wxb1QYILx8pGoQqZaAsK1y9cOTwq9c33-t7Ym4IL1vE3tXsaBd7CtZIX-Ei1rrVtQqlDT", sdkConfig);

            //string accessToken = oauth.GetAccessToken();
            //Dictionary<string, string> sdkConfig2 = new Dictionary<string, string>();
            //sdkConfig2.Add("mode", "sandbox");
            //APIContext apiContext = new APIContext(accessToken);
            //apiContext.Config = sdkConfig2;
          
            sdkConfig.Add("mode", "sandbox");
            string accessToken = new OAuthTokenCredential("Ae2ZWMxCl_ueuNy87vcg52hTjX9aVWfnvLQSMjDuTn2sj0crrWYIWwPseO_6H4nLpXKcHE9_DjtrmDEC", "EEmZr7iiuNCksXtPh5NjcVcguVGic0TwCW-f7GFmgfmrG8wBUhn_UJj53OxraTkKijC4UYQHv-fzlH7z", sdkConfig).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);

            Amount amnt = new Amount();
            amnt.currency = "USD";
            amnt.total = cost;

            List<Transaction> transactionList = new List<Transaction>();
            Transaction tran = new Transaction();
            tran.amount = amnt;
            tran.description = "Photo";
            tran.description = "creating a payment";
            tran.item_list = new ItemList();
            tran.item_list.items = new List<Item>();
            tran.item_list.items.Add(new Item());

            Item item = new Item();

            tran.item_list.items[0].currency = "USD";
            tran.item_list.items[0].description = size;
            tran.item_list.items[0].name = photoname;
            string price = cost;
            tran.item_list.items[0].price = price;// "12";
            tran.item_list.items[0].quantity = 1.ToString();// "1";
            tran.item_list.items[0].sku = photoname + " " + size;
            tran.item_list.items[0].tax = "0";
            transactionList.Add(tran);

            Payer payr = new Payer();
            payr.payment_method = "paypal";
            payr.payer_info = new PayerInfo();
            RedirectUrls redirUrls = new RedirectUrls();
            redirUrls.cancel_url = Request.Url + ""; //"https:devtools-paypal.com/guide/pay_paypal/dotnet?cancel=true";
            string tempurl = "http://" + Request.Url.Authority + "/Home/Confirm";
            redirUrls.return_url = tempurl;// Request.Url.Host + "/Donate/complete";//Html.ActionLink("completed", "complete")//Request.Url + "/complete"; //"https:devtools-paypal.com/guide/pay_paypal/dotnet?success=true";

            Payment pymnt = new Payment();
            pymnt.intent = "sale";
            pymnt.payer = payr;
            pymnt.transactions = transactionList;
            pymnt.redirect_urls = redirUrls;
            Payment createdPayment = pymnt.Create(apiContext);

            Links approvalURL = new Links();
            for (int i = 0; i < createdPayment.links.Count; ++i)
            {
                if (createdPayment.links[i].rel == "approval_url")
                {
                    approvalURL = createdPayment.links[i];
                    i = createdPayment.links.Count;
                }
            }


            return approvalURL.href;

        }       
         


        
        public ActionResult Confirm()
        {
            Purchase model = new Purchase();
            if (ModelState.IsValid)
            {
                string paymentId = Request.QueryString["paymentId"];
                string PayerID = Request.QueryString["PayerID"];
                model.PayerID = PayerID;
                model.PaymentId = paymentId;

                APIContext apiContext = GetAPIContext();
                Payment pymnt = Payment.Get(apiContext, paymentId);
                //model.City = pymnt.transactions[0].item_list.shipping_address.city;
                //model.State = pymnt.transactions[0].item_list.shipping_address.state;
                //model.Line1 = pymnt.transactions[0].item_list.shipping_address.line1;
                //model.Line2 = pymnt.transactions[0].item_list.shipping_address.line2;
                //model.postalCode = pymnt.transactions[0].item_list.shipping_address.postal_code;
                model.FirstName = pymnt.payer.payer_info.first_name;
                model.LastName = pymnt.payer.payer_info.last_name;
                model.size = pymnt.transactions[0].item_list.items[0].description;
                model.photoName = pymnt.transactions[0].item_list.items[0].name;
                model.Price = Convert.ToDecimal(pymnt.transactions[0].amount.total);
                model.PaymentMethod = pymnt.payer.payment_method;
                model.City = pymnt.payer.payer_info.billing_address.city;
                model.State = pymnt.payer.payer_info.billing_address.state;
                model.Line1 = pymnt.payer.payer_info.billing_address.line1;
                model.Line2 = pymnt.payer.payer_info.billing_address.line2;
                model.postalCode = pymnt.payer.payer_info.billing_address.postal_code;
            }
            return View(model);
        }

        public ActionResult Complete(Purchase mod)
        {
            Purchase model = mod;
            APIContext apiContext = GetAPIContext();
            Payment pymnt = Payment.Get(apiContext, model.PaymentId);
            PaymentExecution pymntExecution = new PaymentExecution();
            pymntExecution.payer_id = model.PayerID;
            Payment executedPayment = pymnt.Execute(apiContext, pymntExecution);
            return View(model);
        }

    }
}