using Newtonsoft.Json;

namespace Streetcode.DAL.Entities.Payment;

public class BasketOrder
{
    [JsonProperty("name")]
    required public string Name { get; set; }

    [JsonProperty("qty")]
    public int Qty { get; set; }

    [JsonProperty("sum")]
    public long Sum { get; set; }

    [JsonProperty("icon")]
    required public string Icon { get; set; }

    [JsonProperty("unit")]
    required public string Unit { get; set; }

    [JsonProperty("code")]
    required public string Code { get; set; }

    [JsonProperty("barcode")]
    required public string Barcode { get; set; }

    [JsonProperty("header")]
    required public string Header { get; set; }

    [JsonProperty("footer")]
    required public string Footer { get; set; }

    [JsonProperty("tax")]
    required public List<int> Tax { get; set; }

    [JsonProperty("uktzed")]
    required public string Uktzed { get; set; }
}
