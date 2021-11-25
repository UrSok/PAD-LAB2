using Ocelot.Cache;
using Ocelot.Request.Middleware;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class CustomCacheKeyGenerator : ICacheKeyGenerator
    {
        public string GenerateRequestCacheKey(DownstreamRequest downstreamRequest)
        {
            string hashedContent = null;
            StringBuilder downStreamUrlKeyBuilder = new StringBuilder($"{downstreamRequest.Method}-{downstreamRequest.OriginalString}-{downstreamRequest.Headers.Accept}");
            if (downstreamRequest.Content != null)
            {
                string requestContentString = Task.Run(async () => await downstreamRequest.Content.ReadAsStringAsync()).Result;
                downStreamUrlKeyBuilder.Append(requestContentString);
            }

            hashedContent = MD5Helper.GenerateMd5(downStreamUrlKeyBuilder.ToString());
            return hashedContent;
        }
    }
}