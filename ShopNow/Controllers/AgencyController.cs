using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AutoMapper;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace ShopNow.Controllers
{

    public class AgencyController : Controller
    {
        private sncEntities db = new sncEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string filePath = null;
        private static readonly string bucketName = ConfigurationManager.AppSettings["BucketName"];
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];

        public AgencyController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<AgencyCreateViewModel, Agency>();
                config.CreateMap<Agency, AgencyEditViewModel>();
                config.CreateMap<AgencyEditViewModel, Agency>();
                config.CreateMap<Agency, AgencyListViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        public ActionResult List()
        {
            var model = new AgencyListViewModel();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.List = db.Agencies.Where(i => i.Status == 0).Select(i => new AgencyListViewModel.AgencyList
            {
                Id = i.Id,
                Email = i.Email,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber
            }).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        public ActionResult InActiveList()
        {
            var model = new AgencyListViewModel();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.List = db.Agencies.Where(i => i.Status == 1).Select(i => new AgencyListViewModel.AgencyList
            {
                Id = i.Id,
                Email = i.Email,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber
            }).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }
    }
}