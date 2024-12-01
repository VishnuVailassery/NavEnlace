using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Tokens
{
    public class TokenInfo
    {
        public TokenInfo(string token, string tokenType, int siteId, DateTime? expiry)
        {
            Token = token;
            TokenType = tokenType;
            SiteId = siteId;
            Expiry = expiry;
        }

        public string TokenType { get; set; }
        public string Token {  get; set; }
        public DateTime ExpiresOn { get; set; }

        public int SiteId { get; set; }
        public DateTime? Expiry { get; }
    }
}
