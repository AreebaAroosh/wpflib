using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using WPFLib.Contracts;

namespace WPFLib
{
    public class AsyncSearchListBox : ListBox
    {
        public static readonly DependencyProperty MaxSearchResultsProperty = DependencyProperty.Register("MaxSearchResults", typeof(int), typeof(AsyncSearchListBox), new FrameworkPropertyMetadata() { DefaultValue = 30 });
        public int MaxSearchResults
        {
            get { return (int)GetValue(MaxSearchResultsProperty); }
            set { SetValue(MaxSearchResultsProperty, value); }
        }

        private static readonly DependencyPropertyKey IsTooManyResultsPropertyKey = DependencyProperty.RegisterReadOnly("IsTooManyResults", typeof(bool), typeof(AsyncSearchListBox), new FrameworkPropertyMetadata() { DefaultValue = false });
        public static readonly DependencyProperty IsTooManyResultsProperty = IsTooManyResultsPropertyKey.DependencyProperty;
        public bool IsTooManyResults
        {
            get { return (bool)GetValue(IsTooManyResultsProperty); }
            private set { SetValue(IsTooManyResultsPropertyKey, value); }
        }  

        public bool CanSearch()
        {
            return this.DataContext is IAsyncSearchListBoxController;
        }

        //static AsyncSearchListBox()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(AsyncSearchListBox), new FrameworkPropertyMetadata(typeof(AsyncSearchListBox)));
        //}

        public static readonly DependencyProperty SearchQueryProperty = DependencyProperty.Register("SearchQuery", typeof(string), typeof(AsyncSearchListBox), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnSearchQueryChanged) });
        public string SearchQuery
        {
            get { return (string)GetValue(SearchQueryProperty); }
            set { SetValue(SearchQueryProperty, value); }
        }

        #region OnSearchQueryChanged
        private static void OnSearchQueryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AsyncSearchListBox control = o as AsyncSearchListBox;
            if (control != null)
                control.OnSearchQueryChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnSearchQueryChanged(string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(newValue))
            {
                StopSearch();
            }
            else
            {
                PerformSearch(newValue, MaxSearchResults).Run();
            }
        }
        #endregion 

        Task<IEnumerable<object>> currentSearchTask;
        CancellationTokenSource currentToken;

        private static readonly DependencyPropertyKey IsSearchingPropertyKey = DependencyProperty.RegisterReadOnly("IsSearching", typeof(bool), typeof(AsyncSearchListBox), new UIPropertyMetadata());
        public static readonly DependencyProperty IsSearchingProperty = IsSearchingPropertyKey.DependencyProperty;
        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            private set { SetValue(IsSearchingPropertyKey, value); }
        }

        void StopSearch()
        {
            lock (this)
            {
                this.ItemsSource = null;
                if (currentToken != null && !currentToken.IsCancellationRequested)
                {
                    currentToken.Cancel();
                }
                currentToken = null;
                IsSearching = false;
            }
        }

        IEnumerable<IAsync> PerformSearch(string query, int maxSearchResults)
        {
            IsSearching = true;
            this.ItemsSource = null;

            var token = new CancellationTokenSource();
            lock (this)
            {
                if (currentToken != null && !currentToken.IsCancellationRequested)
                {
                    currentToken.Cancel();
                }
                currentToken = token;
            }

            var searchTask = Controller.GetResults(currentToken, query, maxSearchResults + 1);
            currentSearchTask = searchTask;

            yield return searchTask;

            if (token.IsCancellationRequested)
                yield break;
            
            if (currentSearchTask == searchTask)
            {
                lock (this)
                {
                    // Новых поисков не запускалось, значит все
                    IsSearching = false;
                    currentSearchTask = null;
                    currentToken = null;
                }
            }
            if (token.IsCancellationRequested)
                yield break;
            var res = searchTask.Result.Take(maxSearchResults + 1).ToList();
            this.ItemsSource = res.Take(maxSearchResults);

            IsTooManyResults = (res.Count > maxSearchResults);

            yield break;
        }

        IAsyncSearchListBoxController Controller
        {
            get
            {
                return this.DataContext as IAsyncSearchListBoxController;
            }
        }
    }
}
