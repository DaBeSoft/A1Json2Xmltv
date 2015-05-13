using System.Net;

namespace A1Json2Xmltv
{
    class AndroidWebClient: WebClient
    {
        public AndroidWebClient()
        {
            Headers.Add("user-agent", "Mozilla/5.0 (Linux; U; Android 5.0.2; de-at; D5503 Build/14.5.A.0.242) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1 A1TV/3.2");
            Headers.Add("Accept", "application/json");
        }
    }
}
