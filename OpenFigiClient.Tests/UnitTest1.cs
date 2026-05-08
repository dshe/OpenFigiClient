using System.Text.Json.Nodes;
namespace OpenFigiClient.Tests;

public class UnitTest1(ITestOutputHelper output)
{
    private readonly ITestOutputHelper Output = output;
    protected void Write(string format, params object[] args) =>
        Output.WriteLine(string.Format(format, args));

    [Fact]
    public async Task Test0()
    {
        var openFigiClient = new Client();
        JsonArray arr = await openFigiClient.GetMappingAsync("ID_ISIN", "IE00BKM4GZ66", TestContext.Current.CancellationToken);
        foreach (JsonNode? node in arr)
        {
            if (node == null)
                break;

            Write(Client.PrintAllValues(node));

            //string currency = node?["currency"]?.ToString() ?? "(missing)"; // currency is not returned!!!
            //string figi = node?["figi"]?.ToString() ?? "";
            //string ticker = node?["ticker"]?.ToString() ?? "";
            //string exchCode = node?["exchCode"]?.ToString() ?? "";
            //Write($"FIGI: {figi}, Ticker: {ticker}, Exchange: {exchCode}");
        }
    }

}
/// 
/// Object : "{"data":
/// [{"figi":"BBG006B70NW2","name":"ISHARES CORE EM IMI ACC","ticker":"EMIM","exchCode":"LN","compositeFIGI":"BBG006B70NV3","securityType":"ETP","marketSector":"Equity","shareClassFIGI":"BBG006B70NT6","securityType2":"Mutual Fund","securityDescription":"EMIM"},
/// {"figi":"BBG006B8QQL7","name":"ISHARES CORE EM IMI ACC","ticker":"EIMI","exchCode":"LN","compositeFIGI":"BBG006B8QQK8","securityType":"ETP","marketSector":"Equity","shareClassFIGI":"BBG006B70NT6","securityType2":"Mutual Fund","securityDescription":"EIMI"},
/// {"figi":"BBG006KZCX19","name":"ISHARES CORE EM IMI ACC","ticker":"EMIM","exchCode":"NA","compositeFIGI":"BBG006KZCX00","securityType":"ETP","marketSector":"Equity","shareClassFIGI":"BBG006B70NT6","securityType2":"Mutual Fund","securityDescription":"EMIM"},
/// {"figi":"BBG006KZCY35","name":"ISHARES CORE EM IMI ACC","ticker":"IS3N","exchCode":"GR","compositeFIGI":"BBG006KZCY35","securityType":"ETP","marketSector":"Equity","shareClassFIGI":"BBG006B70NT6","securityType2":"Mutual Fund"...