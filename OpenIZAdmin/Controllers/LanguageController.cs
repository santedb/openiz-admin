using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Extensions;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.LanguageModels;
using OpenIZAdmin.Util;

namespace OpenIZAdmin.Controllers
{
    public class LanguageController : BaseController
    {
        /// <summary>
        /// Displays the create view.
        /// </summary>
        /// <returns>Returns the create view.</returns>
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = new EditLanguageModel();

            var languages = LanguageUtil.GetLanguageList();

            //var bundle = this.ImsiClient.Query<ConceptClass>(c => c.ObsoletionTime == null);

            //bundle.Reconstitute();

            //var conceptClasses = bundle.Item.OfType<ConceptClass>();

            //model.ConceptClassList.AddRange(conceptClasses.ToSelectList().OrderBy(c => c.Text));

            model.LanguageList = languages.Select(l => new SelectListItem { Text = l.DisplayName, Value = l.TwoLetterCountryCode, Selected = l.TwoLetterCountryCode == Locale.EN }).OrderBy(l => l.Text).ToList();

            return View(model);
        }

        // GET: Language
        public ActionResult Index()
        {
            return View();
        }
    }
}