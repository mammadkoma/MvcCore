using System;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Utils
{
    /// <summary>
    /// Ye Ke Model Binder Extensions
    /// </summary>
    public static class YeKeModelBinderExtensions
    {
        /// <summary>
        /// Inserts YeKeModelBinderProvider at the top of the MvcOptions.ModelBinderProviders list.
        /// </summary>
        public static MvcOptions UseYeKeModelBinder(this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ModelBinderProviders.Insert(0, new YeKeModelBinderProvider());
            return options;
        }
    }

    /// <summary>
    /// Persian Ye Ke Model Binder Provider
    /// </summary>
    public class YeKeModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Creates an IModelBinder based on ModelBinderProviderContext.
        /// </summary>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(DataSourceLoadOptions))
            {
                return new DataSourceLoadOptionsModelBinder();
            }

            if (context.Metadata.IsComplexType)
            {
                return null;
            }

            if (context.Metadata.ModelType == typeof(string))
            {
                return new YeKeModelBinder();
            }

#if !NETSTANDARD1_6
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
#else
            return new SimpleTypeModelBinder(context.Metadata.ModelType);
#endif
        }
    }

    /// <summary>
    /// Persian Ye Ke Model Binder
    /// </summary>
    public class YeKeModelBinder : IModelBinder
    {

        /// <summary>
        /// Attempts to bind a model.
        /// </summary>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

#if !NETSTANDARD1_6
            var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType, logger);
#else
            var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType);
#endif

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var valueAsString = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }

            var model = valueAsString.Replace((char)1610, (char)1740).Replace((char)1603, (char)1705);
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }

    public class DataSourceLoadOptionsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var loadOptions = new DataSourceLoadOptions();
            DataSourceLoadOptionsParser.Parse(loadOptions, key => bindingContext.ValueProvider.GetValue(key).FirstOrDefault());
            if (loadOptions.Filter != null)
            {
                for (int i = 0; i < loadOptions.Filter.Count; i++)
                {
                    if (loadOptions.Filter[i] != null)
                    {
                        var Type = Regex.Unescape(loadOptions.Filter[i].GetType().ToString());
                        // SearchPanel
                        if (Type == "Newtonsoft.Json.Linq.JArray")
                        {
                            JArray Temp = JArray.Parse(loadOptions.Filter[i].ToString());
                            Temp = JArray.Parse(Temp.ToString().Replace("ي", "ی").Replace("ك", "ک"));
                            loadOptions.Filter[i] = Temp;
                        }
                        // FilterRow
                        else
                        {
                            if (loadOptions.Filter[i].ToString().Contains("ي"))
                                loadOptions.Filter[i] = loadOptions.Filter[i].ToString().Replace("ي", "ی");
                            if (loadOptions.Filter[i].ToString().Contains("ك"))
                                loadOptions.Filter[i] = loadOptions.Filter[i].ToString().Replace("ك", "ک");
                        }
                    }
                }
            }
            bindingContext.Result = ModelBindingResult.Success(loadOptions);
            return Task.CompletedTask;
        }
    }
}