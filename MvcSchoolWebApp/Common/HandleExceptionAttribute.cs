using System.Web.Mvc;

namespace MvcSchoolWebApp.Common
{
    public class HandleExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            //var result = new ViewResult { ViewName = "Error" };
            //var modelMetadata = new EmptyModelMetadataProvider();
            //result.ViewData = new ViewDataDictionary(
            //        modelMetadata, context.ModelState);
            //result.ViewData.Add("HandleException",
            //        context.Exception);
            //context.Result = result;
            //context.ExceptionHandled = true;
        }
    }
}