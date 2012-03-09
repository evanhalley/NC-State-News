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
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;

namespace NCSU_News
{
    public class RssItemDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/RssItems.sdf";

        // Pass the connection string to the base class.
        public RssItemDataContext(string connectionString) : base(connectionString) { }

        // Specify a single table for the to-do items.
        public Table<RssItem> RssItems;
    }

    /// <summary>
    /// Model for RSS item
    /// </summary>
    [Table]
    public class RssItem : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public enum RssItemType
        {
            News = 0,
            Sports,
            Twitter
        }

        private int _rssItemId;
        private String _title;
        private String _summary;
        private String _plainSummary;
        private String _publishedDate;
        private String _url;
        private RssItemType _itemType;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int RssItemId
        {
            get
            {
                return _rssItemId;
            }
            set
            {
                if (_rssItemId != value)
                {
                    NotifyPropertyChanging("RssItemId");
                    _rssItemId = value;
                    NotifyPropertyChanged("RssItemId");
                }
            }
        }

        [Column]
        public RssItemType ItemType
        {
            get
            {
                return _itemType;
            }
            set 
            {
                if (_itemType != value)
                {
                    NotifyPropertyChanging("ItemType");
                    _itemType = value;
                    NotifyPropertyChanged("ItemType");
                }
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Column]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    NotifyPropertyChanging("Title");
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        [Column]
        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                if (_summary != value)
                {
                    NotifyPropertyChanging("Summary");
                    _summary = value;
                    NotifyPropertyChanged("Summary");
                }
            }
        }

        /// <summary>
        /// Gets or sets the published date.
        /// </summary>
        /// <value>The published date.</value>
        [Column]
        public string PublishedDate
        {
            get
            {
                return _publishedDate;
            }
            set
            {
                if (_publishedDate != value)
                {
                    NotifyPropertyChanging("PublishedDate");
                    _publishedDate = value;
                    NotifyPropertyChanged("PublishedDate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [Column]
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (_url != value)
                {
                    NotifyPropertyChanging("Url");
                    _url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }

        /// <summary>
        /// Gets or sets the plain summary.
        /// </summary>
        /// <value>The plain summary.</value>
        [Column]
        public string PlainSummary
        {
            get
            {
                return _plainSummary;
            }
            set
            {
                if (_plainSummary != value)
                {
                    NotifyPropertyChanging("PlainSummary");
                    _plainSummary = value;
                    NotifyPropertyChanged("PlainSummary");
                }
            }
        }

        public RssItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RssItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="summary">The summary.</param>
        /// <param name="publishedDate">The published date.</param>
        /// <param name="url">The URL.</param>
        public RssItem(string title, string summary, string publishedDate, string url, RssItemType type)
        {
            _title = title;
            _summary = summary;
            _publishedDate = publishedDate;
            _url = url;
            _itemType = type;
            // Get plain text from html
            _plainSummary = HttpUtility.HtmlDecode(Regex.Replace(summary, "<[^>]+?>", ""));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

        
    }
}
