using System.Collections.Generic;

namespace ModularArchitecture.Identity.Core.Results
{
    public class BadRequestErrorResult
    {
        public string Message { get; set; }
        public Dictionary<string, List<string>> ModelState { get; set; }
    }
}
