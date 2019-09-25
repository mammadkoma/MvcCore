using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MvcCore.Data;

namespace MvcCore.Data.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CategoriesController : Controller
    {
        private mvccoreContext _context;

        public CategoriesController(mvccoreContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var Categories = _context.Categories.Select(i => new
            {
                i.Id,
                i.Name,
            }).ToList().OrderByDescending(o => o.Id);
            return Json(DataSourceLoader.Load(Categories, loadOptions));
        }

        [HttpGet]
        public object CategoriesLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Categories
                         orderby i.Name
                         select new
                         {
                             Value = i.Id,
                             Text = i.Name
                         };

            return DataSourceLoader.Load(lookup, loadOptions);
        }
    }
}