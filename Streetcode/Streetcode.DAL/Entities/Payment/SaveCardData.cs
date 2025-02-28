using Newtonsoft.Json;

namespace Streetcode.DAL.Entities.Payment;

public class SaveCardData
{
    [JsonProperty("saveCard")]
    public bool SaveCard { get; set; }

    [JsonProperty("walletId")]
    required public string WalletId { get; set; }
}
