using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace InvoiceToPdf.Storage;


public interface ICredentials
{
    Azure.Core.TokenCredential Get { get; }
}

internal class Credentials : ICredentials
{
    private readonly Azure.Core.TokenCredential _credentials;
    public Credentials(IConfiguration config)
    {
        _credentials = new ClientSecretCredential(
            config["azTenant"],
            config["azClientId"],
            config["azSecret"]);
    }

    public Azure.Core.TokenCredential Get => _credentials;
}
