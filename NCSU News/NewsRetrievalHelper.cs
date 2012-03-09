/*
 * Copyright (C) 2012 Evan Halley <http://emuneee.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.ServiceModel.Syndication;
using System.Globalization;

namespace NCSU_News
{
    public class NewsRetrievalHelper
    {
        private readonly String[] NCSU_RSS_FEED_URLS = new String[]{
            "http://news.ncsu.edu/category/releases/feed/",
            "http://www.gopack.com/headline-rss.xml", 
            "http://twitter.ncsu.edu/feed.php"};
        private int feedIndex = 0;
        private WebClient webClient; 
        private RssItemDataContext rssItemDataContext;
        private MainPage mainPage;

        public NewsRetrievalHelper(MainPage mainPage, RssItemDataContext rssItemDataContext)
        {
            this.mainPage = mainPage;
            FeedData = new List<RssItem>(NCSU_RSS_FEED_URLS.Length);
            this.rssItemDataContext = rssItemDataContext;
        }

        public List<RssItem> FeedData { get; private set; }

        public void StartRetrieval()
        {
            mainPage.WorkBeingDone(true);
            webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(WebClientOpenReadCompleted);
            webClient.OpenReadAsync(new Uri(NCSU_RSS_FEED_URLS[feedIndex]));
        }

        private void WebClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                try
                {
                    RssItem.RssItemType type = (RssItem.RssItemType) feedIndex;
                    Stream stream = e.Result;
                    XmlReader response = XmlReader.Create(stream);
                    SyndicationFeed feeds = SyndicationFeed.Load(response);
                    foreach (SyndicationItem f in feeds.Items)
                    {
                        string link;
                        if (type == RssItem.RssItemType.Sports && f.Links.Count > 1)
                        {
                            link = f.Links[1].Uri.AbsoluteUri;
                        }
                        else
                        {
                            link = f.Links[0].Uri.AbsoluteUri;
                        }
                        RssItem rssItem = new RssItem(f.Title.Text, f.Summary.Text, String.Format("{0:f}", f.PublishDate.ToLocalTime()), link, type);
                        rssItem.RssItemId = rssItem.Title.GetHashCode();
                        rssItemDataContext.RssItems.InsertOnSubmit(rssItem);
                        switch (rssItem.ItemType)
                        {
                            case RssItem.RssItemType.News:
                                mainPage.NewsItems.Add(rssItem);
                                break;
                            case RssItem.RssItemType.Sports:
                                mainPage.SportsItems.Add(rssItem);
                                break;
                            case RssItem.RssItemType.Twitter:
                                mainPage.TwitterItems.Add(rssItem);
                                break;
                        }
                    }
                    feedIndex++;
                    while (feedIndex < NCSU_RSS_FEED_URLS.Length)
                    {
                        webClient.OpenReadAsync(new Uri(NCSU_RSS_FEED_URLS[feedIndex]));
                    }
                    if (feedIndex == NCSU_RSS_FEED_URLS.Length)
                    {
                        mainPage.WorkBeingDone(false);
                        feedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    //Exception handle appropriately for your app  
                }
            }
            else
            {
                //Either cancelled or error handle appropriately for your app  
            }
        }
    }
}
