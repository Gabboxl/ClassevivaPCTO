using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassevivaPCTO.Utils
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private Func<Task<string>> _getToken;

        public AuthenticatedHttpClientHandler(Func<Task<string>> getToken)
        {
            _getToken = getToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            // Check if the request's endpoint requires authentication
            if (RequiresAuthentication(request.RequestUri))
            {
                var token = await _getToken();

                request.Headers.Add("Z-Auth-Token", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private bool RequiresAuthentication(Uri requestUri)
        {
            // Add logic to determine if the request's endpoint requires authentication

            //array of string that the request uri should not contain
            string[] notAuth = { "/rest/v1/auth/login" };



            //check if the absolute path contains a string of the array
            if (notAuth.Any(requestUri.AbsolutePath.Contains))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
