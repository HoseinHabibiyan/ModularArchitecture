﻿using Microsoft.AspNetCore.Mvc.Localization;
using System;
using Microsoft.Extensions.Localization;

namespace ModularArchitecture.Localization
{
    public class JsonHtmlLocalizer : HtmlLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public JsonHtmlLocalizer(IStringLocalizer localizer) : base(localizer)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public override LocalizedHtmlString this[string name]
            => ToHtmlString(_localizer[name]);

        public override LocalizedHtmlString this[string name, params object[] arguments]
            => ToHtmlString(_localizer[name], arguments);
    }
}
