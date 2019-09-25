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

namespace MvcCore.Data.Controllers
{
    // hi 4
    [Route("api/[controller]/[action]")]
    public class BlogsController : Controller
    {
        private mvccoreContext _context;

        public BlogsController(mvccoreContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var blogs = _context.Blogs.Select(i => new
            {
                i.Id,
                i.Name,
                MaxDate = i.MaxDate.ToShamsiDate(), ///////////////////
                i.MaxPostCount,
                i.CategoryId,
                InsertDate = i.InsertDate.ToShamsiDate()
            }).ToList().OrderByDescending(o => o.Id);
            return Json(DataSourceLoader.Load(blogs, loadOptions));
        }

        [HttpPost]
        public IActionResult Post(string values)
        {
            var model = new Blog();
            var _values = JsonConvert.DeserializeObject<IDictionary>(values);

            try ///////////////////
            {
                PopulateModel(model, _values);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.StackTrace.Contains("System.Globalization.PersianCalendar") ? "تاریخ معتبر نیست" : ex.Message);
            }

            if (model.MaxDate == new DateTime(1, 1, 1)) ///////////////////
                return BadRequest("تاریخ معتبر نیست");

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            model.InsertDate = model.UpdateDate = DateTime.Now; ///////////////////
            var result = _context.Blogs.Add(model);
            _context.SaveChanges();


            return Json(result.Entity.Id);
        }

        [HttpPut]
        public IActionResult Put(int key, string values)
        {
            var model = _context.Blogs.FirstOrDefault(item => item.Id == key);
            if (model == null)
                return StatusCode(409, "یافت نشد");

            var _values = JsonConvert.DeserializeObject<IDictionary>(values);

            try ///////////////////
            {
                PopulateModel(model, _values);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.StackTrace.Contains("System.Globalization.PersianCalendar") ? "تاریخ معتبر نیست" : ex.Message);
            }

            if (model.MaxDate == new DateTime(1, 1, 1)) ///////////////////
                return BadRequest("تاریخ معتبر نیست");

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            model.UpdateDate = DateTime.Now; ///////////////////
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public void Delete(int key)
        {
            var model = _context.Blogs.FirstOrDefault(item => item.Id == key);

            _context.Blogs.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Blog model, IDictionary values)
        {
            string BLOG_ID = nameof(Blog.Id);
            string CategoryId = nameof(Blog.CategoryId);
            string NAME = nameof(Blog.Name);
            string MaxDate = nameof(Blog.MaxDate);
            string MaxPostCount = nameof(Blog.MaxPostCount);

            if (values.Contains(BLOG_ID))
            {
                model.Id = Convert.ToInt32(values[BLOG_ID]);
            }
            if (values.Contains(CategoryId))
            {
                model.CategoryId = Convert.ToInt32(values[CategoryId]);
            }
            if (values.Contains(NAME))
            {
                model.Name = Convert.ToString(values[NAME]);
            }
            if (values.Contains(MaxDate))
            {
                model.MaxDate = Convert.ToDateTime(values[MaxDate].ToString().ToMiladiDate()); ///////////////////
            }
            if (values.Contains(MaxPostCount))
            {
                model.MaxPostCount = Convert.ToInt32(values[MaxPostCount]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}