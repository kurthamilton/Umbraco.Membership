using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace ODK.Umbraco.Mvc
{
    public abstract class OdkSurfaceControllerBase : SurfaceController
    {
        private readonly List<string> _feedbackMessages = new List<string>();
        private readonly List<bool> _feedbackSuccesses = new List<bool>();

        protected void AddFeedback(string message, bool success)
        {
            _feedbackMessages.Add(message);
            _feedbackSuccesses.Add(success);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (_feedbackMessages.Count > 0)
            {
                TempData["Feedback.Messages"] = _feedbackMessages.ToArray();
                TempData["Feedback.Successes"] = _feedbackSuccesses.ToArray();
            }

            base.OnResultExecuting(filterContext);
        }

        protected void SetModel(object model)
        {
            TempData["Model"] = model;
        }
    }
}
