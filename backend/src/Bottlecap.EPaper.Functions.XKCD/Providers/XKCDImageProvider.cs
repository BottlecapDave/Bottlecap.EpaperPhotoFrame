﻿
using Bottlecap.EPaper.Services.ImageProviders;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Bottlecap.EPaper.Functions.XKCD.Providers
{
    public class XKCDImageProvider : IImageProvider
    {
        private const string FEED_URL = "https://xkcd.com/rss.xml";

        public XKCDImageProvider()
        {
        }

        public async Task<Stream> GetImageAsync()
        {
            using (var client = new HttpClient())
            {
                var itemsResponse = await client.GetStringAsync(new Uri(FEED_URL));
                using (var stringReader = new StringReader(itemsResponse))
                {
                    using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings() { Async = true }))
                    {
                        var feedReader = new RssFeedReader(xmlReader);

                        while (await feedReader.Read())
                        {
                            if (feedReader.ElementType == SyndicationElementType.Item)
                            {
                                var item = await feedReader.ReadItem();
                                var regex = new Regex("src=\"([^\"]+)\"");
                                var match = regex.Match(item.Description);
                                if (match.Groups.Count == 2)
                                {
                                    return await client.GetStreamAsync(match.Groups[1].Value);
                                }
                            }
                        }
                    }
                }

                return new MemoryStream();
            }
        }
    }
}
