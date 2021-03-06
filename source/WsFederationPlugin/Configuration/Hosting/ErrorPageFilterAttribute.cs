﻿/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Resources;
using IdentityServer3.Core.Results;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.ViewModels;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace IdentityServer3.WsFederation.Hosting
{
    internal class ErrorPageFilterAttribute : ExceptionFilterAttribute
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();
        
        public override async System.Threading.Tasks.Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, System.Threading.CancellationToken cancellationToken)
        {
            var env = actionExecutedContext.ActionContext.Request.GetOwinEnvironment();
            var options = env.ResolveDependency<IdentityServerOptions>();
            var viewSvc = env.ResolveDependency<IViewService>();
            var localization = env.ResolveDependency<ILocalizationService>();
            var errorModel = new ErrorViewModel
            {
                RequestId = env.GetRequestId(),
                SiteName = options.SiteName,
                SiteUrl = env.GetIdentityServerBaseUrl(),
                ErrorMessage = localization.GetMessage(MessageIds.UnexpectedError),
                CurrentUser = env.GetCurrentUserDisplayName(),
                LogoutUrl = env.GetIdentityServerLogoutUrl(),
            };

            var stream = await viewSvc.Error(errorModel);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html")
            {
                CharSet = Encoding.UTF8.WebName
            };

            var response = new HttpResponseMessage
            {
                Content = content
            };
            actionExecutedContext.Response = response;
        }
    }
}