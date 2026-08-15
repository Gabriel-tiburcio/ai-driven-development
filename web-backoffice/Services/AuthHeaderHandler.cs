using System.Net.Http.Headers;

namespace AllStay.Web.Backoffice.Services;

public class AuthHeaderHandler(AuthState authState) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authState.Token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authState.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
