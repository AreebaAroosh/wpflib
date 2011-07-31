using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Windows;
using System.Reflection;
using WPFLib.Misc;
using WPFLib.Navigation;
using WPFLib.Configuration;
using System.Windows.Media;
using WPFLib.Dialogs;
using WPFLib.Contracts;
using System.ComponentModel.Composition;

namespace WPFLib.MainWindow
{
    public class MainWindowController : PropertyChangedHelper, IMainWindowController
    {
        public enum MainWindowStates
        {
            Normal,
            Restoring
        }

        public static readonly PropertyChangedEventArgs MainWindowStateArgs = PropertyChangedHelper.CreateArgs<MainWindowController>(c => c.MainWindowState);
        private MainWindowStates _MainWindowState = MainWindowStates.Normal;

        public MainWindowStates MainWindowState
        {
            get
            {
                return _MainWindowState;
            }
            private set
            {
                var oldValue = _MainWindowState;
                _MainWindowState = value;
                if (oldValue != value)
                {
                    OnPropertyChanged(MainWindowStateArgs);
                }
            }
        }


        public enum ConnectionStates
        {
            Connected,
            Disconnected
        }

        public static readonly PropertyChangedEventArgs ConnectionStateArgs = PropertyChangedHelper.CreateArgs<MainWindowController>(c => c.ConnectionState);
        private ConnectionStates _ConnectionState;

        public ConnectionStates ConnectionState
        {
            get
            {
                return _ConnectionState;
            }
            private set
            {
                var oldValue = _ConnectionState;
                _ConnectionState = value;
                if (oldValue != value)
                {
                    OnConnectionStateChanged(oldValue, value);
                    OnPropertyChanged(ConnectionStateArgs);
                }
            }
        }

        protected virtual void OnConnectionStateChanged(ConnectionStates oldValue, ConnectionStates newValue)
        {
        }


        public INavigationManager Navigation
        {
            get
            {
                return NavigationManager.Instance;
            }
        }

        #region OnCloseCommand
        public static readonly string OnCloseCommandProperty = "OnCloseCommand";
        private DelegateCommand _OnCloseCommand;

        public DelegateCommand OnCloseCommand
        {
            get
            {
                if (_OnCloseCommand == null)
                {
                    _OnCloseCommand = new DelegateCommand(OnOnCloseCommandExecuted, OnOnCloseCommandCanExecute);
                }
                return _OnCloseCommand;
            }
        }

        protected virtual bool OnOnCloseCommandCanExecute()
        {
            return true;
        }

        protected virtual void OnOnCloseCommandExecuted()
        {
        }
        #endregion

        #region OnLoadedCommand
        public static readonly string OnLoadedCommandProperty = "OnLoadedCommand";
        private DelegateCommand _OnLoadedCommand;

        public DelegateCommand OnLoadedCommand
        {
            get
            {
                if (_OnLoadedCommand == null)
                {
                    _OnLoadedCommand = new DelegateCommand(OnOnLoadedCommandExecuted, OnOnLoadedCommandCanExecute);
                }
                return _OnLoadedCommand;
            }
        }

        protected virtual bool OnOnLoadedCommandCanExecute()
        {
            return true;
        }

        protected virtual void OnOnLoadedCommandExecuted()
        {
        }
        #endregion

        public MainWindowController()
        {
        }

        //bool OnClosing()
        //{
        //    //try
        //    //{
        //    //    if (!AppMgr.App.Disconnect())
        //    //    {
        //    //        return false;
        //    //    }
        //    //    return true;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    DialogService.ErrorMessage(ex);
        //    //    return true;
        //    //}
        //    return true;
        //}

        //bool loaded = false;
        //void OnLoaded()
        //{
        //    //loaded = true;

        //    //try
        //    //{
        //    //    //if (!AppMgr.App.IsConnected && !AppMgr.App.TryConnect())
        //    //    //    refreshMainMenu();
        //    //    LoadSearchState();
        //    //    if (loadStateOnShown)
        //    //    {
        //    //        loadDockState();
        //    //    }
        //    //}
        //    //catch (Exception error)
        //    //{
        //    //    error = error.Untwine();
        //    //    //refreshMainMenu();
        //    //    Splasher.Instance.Close();
        //    //    if (InitializationError == null)
        //    //        InitializationError = error;
        //    //    else if (error.Message != InitializationError.Message)
        //    //        InitializationError = new CompositeException(InitializationError, error);
        //    //}
        //    //if (InitializationError != null)
        //    //{
        //    //    SHARTShellMgr.ErrorMessage(InitializationError);
        //    //    InitializationError = null;
        //    //}
        //}

        public static readonly PropertyChangedEventArgs TitleArgs = PropertyChangedHelper.CreateArgs<MainWindowController>(c => c.Title);
        private string _Title;

        public string Title
        {
            get
            {
                return _Title;
            }
            protected set
            {
                var oldValue = _Title;
                _Title = value;
                if (oldValue != value)
                {
                    OnPropertyChanged(TitleArgs);
                }
            }
        }

        bool saveState = false;
        bool loadStateOnShown = false;

        List<string> localState = null;

        //void OnDisconnecting(object sender, DisconnectingEventArgs e)
        //{
            //if (!e.AsFact)
            //{
            //    CollectState();

            //    if (localState != null && localState.Count < 2 && (Properties.Settings.Default.RestoreClientState || Properties.Settings.Default.AskToSaveClientState))
            //    {
            //        saveState = true;
            //    }
            //    else if (Properties.Settings.Default.RestoreClientState && !Properties.Settings.Default.AskToSaveClientState)
            //    {
            //        saveState = true;
            //    }
            //    else if (localState != null && localState.Count > 0 && Properties.Settings.Default.AskToSaveClientState)
            //    {
            //        var dlg = new SaveStateDlg();
            //        dlg.OpenTabs = Properties.Settings.Default.RestoreClientState;
            //        var res = dlg.ShowDialog(AppMgr.App.MainWindow);
            //        if (res != Forms.DialogResult.OK)
            //        {
            //            e.Cancel = true;
            //            return;
            //        }
            //        else if (res == Forms.DialogResult.OK)
            //        {
            //            Properties.Settings.Default.RestoreClientState = dlg.OpenTabs;
            //            Properties.Settings.Default.AskToSaveClientState = !dlg.DoNotAsk;
            //            Properties.Settings.Default.Save();
            //            if (Properties.Settings.Default.RestoreClientState)
            //            {
            //                saveState = true;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // Сохраним пустое
            //        saveState = true;
            //    }
            //}
            //if (!CloseAll())
            //{
            //    e.Cancel = true;
            //}
        //}

        public void CollectState()
        {
            //try
            //{
            //    localState = ControlManager.SaveState();
            //}
            //catch (Exception ex)
            //{
            //    localState = null;
            //    SHARTShellMgr.ErrorMessage("ClientState.SaveError".Localize(), ex);
            //}
        }

        //void OnDisconnect(object sender, DisconnectEventArgs e)
        //{
            //ConnectionState = ConnectionStates.Disconnected;
            //SearchState = SearchStates.Closed;
            //try
            //{
            //    DataCacheManager.Instance.ClearAll();
            //    _userName = null;
            //    AdjustCaption();
            //    if (!e.AsFact)
            //        SaveSearchList();
            //    if (!e.AsFact && saveState)
            //    {
            //        saveDockState();
            //        saveState = false;
            //    }
            //    ControlManager.CloseAll();

            //    MainMenu = GenerateMenu();

            //    GC.Collect();
            //}
            //finally
            //{
            //}
        //}

        void OnConnect(object sender, EventArgs e)
        {
            //ConnectionState = ConnectionStates.Connected;

            //try
            //{
            //    DataCacheManager.Instance.ClearAll();

            //    MainMenu = GenerateMenu();
            //    LoadSearchState();
            //    if (Properties.Settings.Default.RestoreClientState)
            //    {
            //        if (loaded)
            //        {
            //            loadDockState();
            //        }
            //        else
            //        {
            //            loadStateOnShown = true;
            //        }
            //    }
            //    AdjustCaption();
            //}
            //finally
            //{
            //}
        }

        //string _userName;
        //private string CurrentUserName
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_userName))
        //            _userName = ServiceScopeHelper<IModelService>.Execute<string>(
        //                delegate(IModelService srv) { return srv.GetCurrentUserName(); });
        //        return _userName;
        //    }
        //}

        //string _dbName;
        private string CurrentDbName
        {
            get
            {
                //if (string.IsNullOrEmpty(_dbName))
                //    _dbName = ServiceScopeHelper<IModelService>.Execute<string>(
                //        delegate(IModelService srv)
                //        {
                //            return srv.GetCurrentDatabaseName();
                //        });
                //return _dbName;
                return null;
            }
        }

        private void AdjustCaption()
        {
            //if (AppMgr.App.IsConnected)
            //{
            //    this.Title = "MainForm.Caption".Localize(_sourceCaption, CurrentUserName, AppMgr.App.Settings.ServerHostName, CurrentDbName);
            //}
            //else
            //{
            //    this.Title = "MainForm.CaptionDisconnected".Localize(_sourceCaption);
            //}
        }

        public void saveDockState()
        {
            //try
            //{
            //    var coll = new StringCollection();
            //    if (localState != null)
            //    {
            //        foreach (var s in localState)
            //        {
            //            coll.Add(s);
            //        }
            //    }
            //    Properties.Settings.Default.ClientState = coll;
            //    Properties.Settings.Default.Save();
            //}
            //catch (Exception ex)
            //{
            //    SHARTShellMgr.ErrorMessage("ClientState.SaveError".Localize(), ex);
            //}
        }

        public void SaveSearchList()
        {
            //try
            //{
            //    var coll = new StringCollection();
            //    if (SearchController.SavedQueryList != null)
            //    {
            //        int skipNum = SearchController.SavedQueryList.Count - 10;
            //        foreach (var s in SearchController.SavedQueryList.Skip(skipNum))
            //        {
            //            coll.Add(s);
            //        }
            //    }
            //    Properties.Settings.Default.SearchState = coll;
            //    Properties.Settings.Default.Save();
            //}
            //catch (Exception ex)
            //{
            //    SHARTShellMgr.ErrorMessage("ClientState.SaveError".Localize(), ex);
            //}
        }

        public void loadDockState()
        {
            //try
            //{
            //    MainWindowState = MainWindowStates.Restoring;

            //    if (Properties.Settings.Default.ClientState != null)
            //    {
            //        Splasher.Instance = new LongOpSplasher(() =>
            //        {
            //            using (new LengthyOperation())
            //            {
            //                var state = new List<string>();
            //                Properties.Settings.Default.ClientState.ForEachUnsafe<string>(state.Add);
            //                if (state.Count > 0)
            //                {
            //                    ControlManager.RestoreState(state);
            //                }
            //            }
            //        });
            //        Splasher.Instance.Status = "ClientState.Restoring".Localize();
            //        Splasher.Instance.Show(null);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    var state = (Properties.Settings.Default.ClientState ?? new StringCollection() { "<null>" }).Cast<string>().ToArray();

            //    Properties.Settings.Default.ClientState = null;
            //    Properties.Settings.Default.Save();

            //    SHARTShellMgr.ErrorMessage("ClinetState.RestoreError".Localize(), "ClientState.ClientStatePref".Localize() + String.Join(Environment.NewLine, state) + Environment.NewLine + Environment.NewLine + ex.ToString());
            //}
            //finally
            //{
            //    if (Splasher.Instance != null)
            //    {
            //        Splasher.Instance.Close();
            //    }
            //    MainWindowState = MainWindowStates.Normal;
            //}
        }

        public void LoadSearchState()
        {
            //try
            //{
            //    if (Properties.Settings.Default.SearchState != null)
            //    {

            //        using (new LengthyOperation())
            //        {
            //            var state = new List<string>();
            //            Properties.Settings.Default.SearchState.ForEachUnsafe<string>(state.Add);
            //            if (state.Count > 0)
            //            {
            //                SearchController.SavedQueryList.Clear();
            //                state.ForEach(SearchController.SavedQueryList.Add);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    var state = (Properties.Settings.Default.SearchState ?? new StringCollection() { "<null>" }).Cast<string>().ToArray();

            //    Properties.Settings.Default.SearchState = null;
            //    Properties.Settings.Default.Save();

            //    DialogService.ErrorMessage("ClinetState.RestoreError".Localize(), "ClientState.ClientStatePref".Localize() + String.Join(Environment.NewLine, state) + Environment.NewLine + Environment.NewLine + ex.ToString());
            //}
        }

        public Exception InitializationError
        {
            get;
            set;
        }

        public string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return path;
            }
        }

        public ImageSource Icon
        {
            get
            {
                return null;
            }
        }

        public static readonly PropertyChangedEventArgs ShowInTaskbarArgs = PropertyChangedHelper.CreateArgs<MainWindowController>(c => c.ShowInTaskbar);
        private bool _ShowInTaskbar;

        public bool ShowInTaskbar
        {
            get
            {
                return _ShowInTaskbar;
            }
            set
            {
                var oldValue = _ShowInTaskbar;
                _ShowInTaskbar = value;
                if (oldValue != value)
                {
                    OnPropertyChanged(ShowInTaskbarArgs);
                }
            }
        }


        #region IControlHostManager Members

        private const string ActiveStatePrefix = "!!!";

        public List<string> SaveState()
        {
            //if (NavigationManager.IsNavigationManagerAvailable)
            //{
            //    List<string> state = new List<string>();
            //    foreach (var item in WorkspaceItems)
            //    {
            //        if (item.ItemContent is INavigationNode)
            //        {
            //            var s = NavigationManager.Instance.GetNavigationNodeState(item.ItemContent as INavigationNode);
            //            if (s != null)
            //            {
            //                var stringState = s.ToString();
            //                if (!String.IsNullOrEmpty(stringState))
            //                {
            //                    state.Add(stringState);
            //                }
            //            }
            //        }
            //    }
            //    if (state.Contains(NavigationManager.Instance.CurrentURI))
            //    {
            //        var i = state.IndexOf(NavigationManager.Instance.CurrentURI);
            //        state[i] = ActiveStatePrefix + state[i];
            //    }
            //    if (state.Count > 0)
            //        return state;
            //}
            return null;
        }

        public void RestoreState(List<string> state)
        {
            //if (NavigationManager.IsNavigationManagerAvailable)
            //{
            //    int activeCount = -1;
            //    foreach (var s in state)
            //    {
            //        var statestring = s;
            //        if (statestring.StartsWith(ActiveStatePrefix))
            //        {
            //            statestring = statestring.Remove(0, ActiveStatePrefix.Length);
            //            activeCount = ItemsCount;
            //        }
            //        NavigationManager.Instance.Open(statestring);
            //    }
            //    if (activeCount >= 0)
            //    {
            //        this.WorkspaceItemsView.MoveCurrentToPosition(activeCount);
            //    }
            //}
        }

        public int ItemsCount
        {
            get { return WorkspaceItems.Count; }
        }

        public bool CloseAll()
        {
            foreach (var item in WorkspaceItems.ToList())
            {
                if (!Close(item))
                {
                    return false;
                }
            }
            return true;
        }

        public static readonly PropertyChangedEventArgs ActiveItemArgs = PropertyChangedHelper.CreateArgs<MainWindowController>(c => c.ActiveItem);

        public IWorkspaceItem ActiveItem
        {
            get {
                return (IWorkspaceItem)WorkspaceItemsView.CurrentItem;
            }
            set
            {
                if (WorkspaceItemsView.Contains(value))
                {
                    WorkspaceItemsView.MoveCurrentTo(value);
                }
            }
        }

        #endregion

        public static readonly PropertyChangedEventArgs WorkspaceItemsArgs = PropertyChangedHelper.CreateArgs<MainWindowController>(c => c.WorkspaceItems);
        private ObservableCollection<IWorkspaceItem> _WorkspaceItems;

        public ObservableCollection<IWorkspaceItem> WorkspaceItems
        {
            get
            {
                if (_WorkspaceItems == null)
                {
                    _WorkspaceItems = new ObservableCollection<IWorkspaceItem>();
                    var view = CollectionViewSource.GetDefaultView(_WorkspaceItems);
                    _workspaceItemsView = view;

                    WorkspaceItemsView.CurrentChanging += new CurrentChangingEventHandler(WorkspaceItemsView_CurrentChanging);
                    WorkspaceItemsView.CurrentChanged += new EventHandler(WorkspaceItemsView_CurrentChanged);
                }
                return _WorkspaceItems;
            }
            private set
            {
                var oldValue = _WorkspaceItems;
                _WorkspaceItems = value;
                if (oldValue != value)
                {
                    OnPropertyChanged(WorkspaceItemsArgs);
                }
            }
        }

        IWorkspaceItem PrevActiveControl = null;

        void WorkspaceItemsView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            PrevActiveControl = ActiveItem as IWorkspaceItem;
        }

        void WorkspaceItemsView_CurrentChanged(object sender, EventArgs e)
        {
            if (PrevActiveControl != null)
            {
                PrevActiveControl.OnDeactivate();
            }
            if (ActiveItem != null)
            {
                var item = ActiveItem as IWorkspaceItem;
                item.OnActivate();
            }
            //if (ActiveItem == null || !(ActiveItem is IToolbarWindow) || ((ActiveItem is WorkspaceItemWrapper) && (ActiveItem as WorkspaceItemWrapper).iToolbar == null))
            //{
            //    //this.DeactivateAll();
            //}
            OnPropertyChanged(ActiveItemArgs);
        }

        ICollectionView _workspaceItemsView = null;
        private ICollectionView WorkspaceItemsView
        {
            get
            {
                if (_workspaceItemsView == null)
                {
                    var r = WorkspaceItems;
                }
                return _workspaceItemsView;
            }
        }

        public bool Close(IWorkspaceItem item)
        {
            if (!WorkspaceItems.Contains(item))
            {
                return false;
            }
            if (item.CanClose())
            {
                item.OnClose();
                WorkspaceItems.Remove(item);
                return true;
            }
            return false;
        }

        public T Open<T>(T item) where T : IWorkspaceItem
        {
			foreach (var existing in WorkspaceItems)
				if (GetIdentification(existing) .Equals( GetIdentification(item)))
				{
					WorkspaceItemsView.MoveCurrentTo(existing);
					return (T)existing;
				}
			WorkspaceItems.Add(item);
			WorkspaceItemsView.MoveCurrentTo(item);
            return item;
		}

		private object GetIdentification(IWorkspaceItem item)
		{
			IItemHasId hasId = item as IItemHasId;
			if (hasId == null)
				return GetHeader(item);
			return hasId.Identificator;
		}

		private string GetHeader(IWorkspaceItem item)
		{
			var header = item.Header;
			if (header is IWorkspaceItemHeader)
			{
				header = (header as IWorkspaceItemHeader).Content;
			}
			if (header != null)
				return header.ToString();
			throw new ApplicationException("Не задан заголовок окна");
		}

        #region ExitCommand
        public static readonly string ExitCommandProperty = "ExitCommand";
        private DelegateCommand _ExitCommand;

        public DelegateCommand ExitCommand
        {
            get
            {
                if (_ExitCommand == null)
                {
                    _ExitCommand = new DelegateCommand(OnExitCommandExecuted, OnExitCommandCanExecute);
                }
                return _ExitCommand;
            }
        }

        protected virtual bool OnExitCommandCanExecute()
        {
            return true;
        }

        protected virtual void OnExitCommandExecuted()
        {
            Application.Current.MainWindow.Close();
        }
        #endregion
    }
}
