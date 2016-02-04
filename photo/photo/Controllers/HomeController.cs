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

        /// <summary>
        /// gets information for each photo in PhotographsDBEntities db, and locates the address
        /// that each photo is located at 
        /// </summary>
        /// <returns>returns list of all the image info </returns>
        [HttpGet]
        public string GetImgs()
        {
            List<ImageInfo> stuff = new List<ImageInfo>();          

            foreach (PhotoInfo i in db.PhotoInfoes)
            {
                ImageInfo ii = new ImageInfo();
                ii.FileName = i.FileName;
                ii.Id = i.Id;
                ii.Info = i.Info;
                ii.Name = i.Name;
                ii.FileLocation = GetImgFolder() + i.FileName;
                stuff.Add(ii);
            }           
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(stuff);
            return json;

        }


        /// <summary>
        /// find the address that the images are at
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>string of address for images</returns>
        private string GetImgFolder()
        {
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            string auth = originalUri.PathAndQuery;
            string path = originalUri.AbsoluteUri;
            string root = path.Replace(auth, "");
            if (auth == "/") { root = ""; }
            else { root += "/"; }
            return root + "Images/";
        }

        /// <summary>
        /// finds the address for the image with the file name passed
        /// </summary>
        /// <param name="fileName">filename of image</param>
        /// <returns>the address that the file is located at</returns>
        [HttpGet]
        public string GetImgAddress(string fileName)        
        {
            List<string> stuff = new List<string>();
            DirectoryInfo directory = new DirectoryInfo(Server.MapPath(@"~\Images"));
            var imgs = directory.GetFiles().ToList();
            foreach (FileInfo i in imgs)
            {
                if (fileName == i.ToString())
                {
                    stuff.Add(GetImgFolder() + i.ToString());
                }
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(stuff);
            return json;
        }


        /// <summary>
        /// returns PhotoInfo in dp with the file name passed
        /// </summary>
        /// <param name="imgInfo">string of image info</param>
        /// <returns>returns json string of Photoinfo</returns>
        [HttpGet]
        public string FindImageByFileName(string imgInfo)
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


        /// <summary>
        /// find the PhotoInfo in dp with the same name as in the string passed
        /// </summary>
        /// <param name="imgInfo"></param>
        /// <returns>json string of photoInfo</returns>
        [HttpGet]
        public string FindImageByName(string imgInfo)
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


        /// <summary>
        /// gets acces token and creates apiContext for Paypal api
        /// </summary>
        /// <returns>ApiContext</returns>
        private APIContext GetAPIContext()
        {
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            sdkConfig.Add("mode", "sandbox");
            string accessToken = new OAuthTokenCredential("Ae2ZWMxCl_ueuNy87vcg52hTjX9aVWfnvLQSMjDuTn2sj0crrWYIWwPseO_6H4nLpXKcHE9_DjtrmDEC", "EEmZr7iiuNCksXtPh5NjcVcguVGic0TwCW-f7GFmgfmrG8wBUhn_UJj53OxraTkKijC4UYQHv-fzlH7z", sdkConfig).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);
            return apiContext;
        }


        /// <summary>
        /// creates a transaction for photo with photoname at the size and cost passed to be viewed and payed for in paypal
        /// </summary>
        /// <param name="photoname">name of photo being bought</param>
        /// <param name="size">size of photo</param>
        /// <param name="cost">cost of photo</param>
        /// <returns>address to be redirected to paypal to view and pay for the photo</returns>
        [HttpPost]
        public string Purchase(string photoname, string size, string cost)
        {
            //Dictionary<string, string> sdkConfig = new Dictionary<string, string>();                      
            //sdkConfig.Add("mode", "sandbox");
            //string accessToken = new OAuthTokenCredential("Ae2ZWMxCl_ueuNy87vcg52hTjX9aVWfnvLQSMjDuTn2sj0crrWYIWwPseO_6H4nLpXKcHE9_DjtrmDEC", "EEmZr7iiuNCksXtPh5NjcVcguVGic0TwCW-f7GFmgfmrG8wBUhn_UJj53OxraTkKijC4UYQHv-fzlH7z", sdkConfig).GetAccessToken();
            //APIContext apiContext = new APIContext(accessToken);
            APIContext apiContext = GetAPIContext();
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
            redirUrls.cancel_url = Request.Url + ""; 
            string tempurl = "http://" + Request.Url.Authority + "/Home/Confirm";
            redirUrls.return_url = tempurl;

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
         


        /// <summary>
        /// parses the paypal paymentID and PayerId from address and finds the payment information to be displayed int the 
        /// view for the user to confirm or cancel the purchase
        /// </summary>
        /// <returns>View(Payment)</returns>
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

        /// <summary>
        /// Executes the payment and returns view with purchase information 
        /// </summary>
        /// <param name="mod">model containing payment information</param>
        /// <returns>View(model)</returns>
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