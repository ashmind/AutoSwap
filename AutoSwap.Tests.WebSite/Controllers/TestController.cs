using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoSwap.Tests.WebSite.Models;

namespace AutoSwap.Tests.WebSite.Controllers {
    public class TestController : Controller {
        private readonly IVersionService versionService;

        public TestController(IVersionService versionService) {
            this.versionService = versionService;
        }

        public ActionResult Index() {
            return Content("VersionService: " + this.versionService.GetVersion());
        }
    }
}
