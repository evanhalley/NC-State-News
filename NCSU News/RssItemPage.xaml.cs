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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace NCSU_News
{
    public partial class RssItemPage : PhoneApplicationPage
    {
        private string link;

        public RssItemPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string title = "";
            string summary = "";
            string url = "";
            if (NavigationContext.QueryString.TryGetValue("title", out title))
            {
                PageTitle.Text = title;
                
            }
            if (NavigationContext.QueryString.TryGetValue("summary", out summary))
            {
                webBrowserSummary.NavigateToString(summary);
            }

            if (NavigationContext.QueryString.TryGetValue("url", out url))
            {
                link = url;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(link);
            wbt.Show();
        }
    }
}