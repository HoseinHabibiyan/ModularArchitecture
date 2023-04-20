using System.Collections.Generic;

namespace  ModularArchitecture.Identity
{
    public class BadRequestErrorResult
    {
        public string Message { get; set; }
        public Dictionary<string, List<string>> ModelState { get; set; }
    }
}
