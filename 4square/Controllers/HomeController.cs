using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _4square.Models;

namespace _4square.Controllers
{
    public class HomeController : Controller
    {
		private ReferencesDBContext db = new ReferencesDBContext();

		private const string CLIENT_ID = "IYNYRUPN34L23RN5IV10VPQ01EPWXH0ARAMN1KWWLIY4BJXB";
		private const string CLIENT_SECRET = "HFWOWGGHHZ4EGRFGPOA4APEM4EAZLPUOH0WG2HP4FAAUUG5U";
	    private const string VERSION = "20150328";

	    public HomeController()
	    {
		    ViewBag.CLIENT_ID = CLIENT_ID;
			ViewBag.CLIENT_SECRET = CLIENT_SECRET;
		    ViewBag.VERSION = VERSION;
	    }

        // GET: Home
        public ActionResult Index()
        {
			ReferenceCounter referenceCounter = new ReferenceCounter();

            return View(referenceCounter);
        }

		[HttpPost]
	    public ActionResult Index(ReferenceCounter referenceCounter, string Command)
	    {
		    if (ModelState.IsValid)
		    {
			    ViewBag.references = AddReference(referenceCounter.placeToSearch);
		    }

			ViewBag.command = Command;
			ViewBag.userLocation = referenceCounter.userLocation;

			ViewBag.encodedPlace = System.Web.HttpUtility.UrlEncode(referenceCounter.placeToSearch);
			return View(referenceCounter);
	    }

	    public ActionResult Login(string code)
	    {
		    string uri = "https://foursquare.com/oauth2/access_token" +
						"?client_id=" + CLIENT_ID +
						"&client_secret=" + CLIENT_SECRET +
						"&grant_type=authorization_code" +
						"&redirect_uri=" + Url.Action("Login", "Home", null, Request.Url.Scheme, null) +
						"&code=" + code + 
						"&v = " + VERSION;

		    HttpWebRequest request = WebRequest.CreateHttp(uri);
		    request.Method = WebRequestMethods.Http.Get;

		    string token = GetTokenFromResponse(request.GetResponse());
		    string name = GetNameByToken(token);
			
			Session["token"] = token;
		    Session["name"] = name;

			return RedirectToAction("Index");
	    }

	    public ActionResult Logout()
	    {
			Session.Remove("token");
			Session.Remove("name");

			return RedirectToAction("Index");
	    }

	    private string GetNameByToken(string token)
	    {
		    string uri = "https://api.foursquare.com/v2/users/self" +
						"?oauth_token=" + token +
						"&v=" + VERSION;

		    HttpWebRequest request = WebRequest.CreateHttp(uri);
		    request.Method = WebRequestMethods.Http.Get;

		    return GetNameFromResponse(request.GetResponse());
	    }

	    private string GetTokenFromResponse(WebResponse response)
	    {
			StreamReader reader = new StreamReader(response.GetResponseStream());
			dynamic json = System.Web.Helpers.Json.Decode(reader.ReadToEnd());

			response.Close();

		    return json.access_token;
	    }

	    private string GetNameFromResponse(WebResponse response)
	    {
		    StreamReader reader = new StreamReader(response.GetResponseStream());
		    dynamic json = System.Web.Helpers.Json.Decode(reader.ReadToEnd());

			response.Close();

			return String.Format("{0} {1}", json.response.user.firstName, json.response.user.lastName);
	    }

	    private int AddReference(string place)
	    {
		    Reference reference = db.References.SingleOrDefault(r => r.Text == place);
		    if (reference == null)
		    {
			    reference = new Reference
			    {
					ID = db.References.Count(),
				    References = 0,
				    Text = place
			    };

			    db.References.Add(reference);
		    }

		    reference.References++;
		    db.SaveChanges();

		    return reference.References;
	    }
    }
}