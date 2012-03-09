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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace NCSU_News
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private readonly string panoramaIndex = "panoramaIndex";
        // Data context for the local database
        private RssItemDataContext rssItemDb;
        private NewsRetrievalHelper httpHelper;
        private bool isNewPageInstance = false;
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<RssItem> NewsItems { get; private set; }
        public ObservableCollection<RssItem> SportsItems { get; private set; }
        public ObservableCollection<RssItem> TwitterItems { get; private set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Connect to the database and instantiate data context.
            rssItemDb = new RssItemDataContext(RssItemDataContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            // Set the data context of the listbox control to the sample data
            //DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            this.NewsItems = new ObservableCollection<RssItem>();
            this.SportsItems = new ObservableCollection<RssItem>();
            this.TwitterItems = new ObservableCollection<RssItem>();

            isNewPageInstance = true;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            httpHelper = new NewsRetrievalHelper(this, rssItemDb);
            httpHelper.StartRetrieval();
        }

        public void WorkBeingDone(bool working)
        {
            if (working)
            {
                customIndeterminateProgressBar.Visibility = Visibility.Visible;
            }
            else
            {
                customIndeterminateProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void NavigateToNextPage(String title, string summary, String url)
        {
            NavigationService.Navigate(new Uri("/RssItemPage.xaml?title=" + Uri.EscapeDataString(title) + "&summary="
                + Uri.EscapeDataString(summary) + "&url=" + Uri.EscapeDataString(url), UriKind.Relative));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            httpHelper.StartRetrieval();
        }

        private void News_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ListBox listBox = (ListBox)sender;
           RssItem rssItem = NewsItems[listBox.SelectedIndex];
           NavigateToNextPage(rssItem.Title, rssItem.Summary, rssItem.Url);
        }

        private void Sports_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            RssItem rssItem = SportsItems[listBox.SelectedIndex];
            NavigateToNextPage(rssItem.Title, rssItem.Summary, rssItem.Url);
        }

        private void Twitter_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            RssItem rssItem = TwitterItems[listBox.SelectedIndex];
            NavigateToNextPage(rssItem.Title, rssItem.Summary, rssItem.Url);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the ViewModel variable in the page's State dictionary.
                State[panoramaIndex] = mainPanorama.SelectedIndex;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {         
            // If _isNewPageInstance is true, the page constuctor has been called, so
            // state may need to be restored
            if (isNewPageInstance)
            {
                // Define the query to gather all of the to-do items.
                var rssItemsInDb = from RssItem todo in rssItemDb.RssItems
                                   select todo;

                // Execute the query and place the results into a collection.
                ObservableCollection<RssItem> items = new ObservableCollection<RssItem>(rssItemsInDb);
                for (int i = 0; i < items.Count; i++)
                {
                    switch (items[i].ItemType)
                    {
                        case RssItem.RssItemType.News:
                            NewsItems.Add(items[i]);
                            break;
                        case RssItem.RssItemType.Sports:
                            SportsItems.Add(items[i]);
                            break;
                        case RssItem.RssItemType.Twitter:
                            TwitterItems.Add(items[i]);
                            break;
                    }
                }
                if (State.Count > 0)
                {
                    object defaultItem = mainPanorama.Items[(int)State[panoramaIndex]];
                    mainPanorama.SetValue(Panorama.SelectedItemProperty, defaultItem);
                }
                else
                {
                    mainPanorama.DefaultItem = 0;
                }
            }
            // Call the base method.
            base.OnNavigatedTo(e);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}