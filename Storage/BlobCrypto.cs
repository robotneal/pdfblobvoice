using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Storage;
using Microsoft.Extensions.Configuration;

namespace InvoiceToPdf.Storage;

public interface IBlobCrypto
{
    ClientSideEncryptionOptions GetBlobEncryptionOptions();
}

internal class BlobCrypto : IBlobCrypto
{
    private readonly KeyClient _keyClient;
    private readonly string _keyName;
    private readonly ICredentials _cred;

    public BlobCrypto(IConfiguration config, ICredentials cred)
    {
        _cred = cred;
        _keyClient = new KeyClient(new(config["KeyLocation"]), cred.Get);
        _keyName = config["KeyName"];
    }

    private CryptographyClient GetCryptoClient()
    {
        var key = _keyClient.GetKey(_keyName);
        return new CryptographyClient(key.Value.Id, _cred.Get);
    }

    public ClientSideEncryptionOptions GetBlobEncryptionOptions()
    {
        return new ClientSideEncryptionOptions(ClientSideEncryptionVersion.V2_0)
        {
            KeyEncryptionKey = GetCryptoClient(),
            KeyResolver = new KeyResolver(_cred.Get),
            KeyWrapAlgorithm = "RSA-OAEP"
        };
    }
}