using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ModularArchitecture.Localization.Localication.Extensions
{
    [HtmlTargetElement("a", Attributes = ActionAttributeName)]
    [HtmlTargetElement("a", Attributes = ControllerAttributeName)]
    [HtmlTargetElement("a", Attributes = AreaAttributeName)]
    [HtmlTargetElement("a", Attributes = PageAttributeName)]
    [HtmlTargetElement("a", Attributes = PageHandlerAttributeName)]
    [HtmlTargetElement("a", Attributes = FragmentAttributeName)]
    [HtmlTargetElement("a", Attributes = HostAttributeName)]
    [HtmlTargetElement("a", Attributes = ProtocolAttributeName)]
    [HtmlTargetElement("a", Attributes = RouteAttributeName)]
    [HtmlTargetElement("a", Attributes = RouteValuesDictionaryName)]
    [HtmlTargetElement("a", Attributes = RouteValuesPrefix + "*")]
    public class CultureAnchorTagHelper : AnchorTagHelper
    {
        public CultureAnchorTagHelper(IHttpContextAccessor contextAccessor, IHtmlGenerator generator) :
            base(generator)
        {
            _contextAccessor = contextAccessor;
        }

        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private const string FragmentAttributeName = "asp-fragment";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        private const string Href = "href";

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string defaultRequestCulture = "en-US";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Get the culture from the route values
            var culture = (string)_contextAccessor.HttpContext.Request.RouteValues["culture"];

            // Set the culture in the route values if it is not null
            if (culture != null && culture != defaultRequestCulture && (culture.Length == 2 || culture.Length == 5 && Regex.IsMatch(culture, "\\w{2}-\\w{2}")))
            {
                // Remove the 'href' just in case
                output.Attributes.RemoveAll("href");
                RouteValues["culture"] = culture;
            }

            // Call the base class for all other functionality, we've only added the culture route value.
            // Because the route has the `{culture=en-US}`, the tag helper knows what to do with that route value.
            base.Process(context, output);
        }
    }
}
