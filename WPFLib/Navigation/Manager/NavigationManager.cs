using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using System.Reflection;
using WPFLib;
using WPFLib.Collections;
using WPFLib.Configuration;

namespace WPFLib.Navigation
{
    public class NavigationManager : INavigationManager, INotifyPropertyChanged
    {
        NavigationSuspendStateChanges currentSuspendChanges = null;
        INavigationNode lastStateInSuspendSession = null;

        public INavigationSuspendStateChanges SuspendChanges(bool getLastChange)
        {
            lastStateInSuspendSession = null;
            currentSuspendChanges = new NavigationSuspendStateChanges(this, getLastChange);
            return currentSuspendChanges;
        }

        public INavigationSuspendStateChanges SuspendChanges()
        {
            return SuspendChanges(false);
        }

        internal void ResumeStateChanges(NavigationSuspendStateChanges o)
        {
            if (o == currentSuspendChanges)
            {
                bool needStateCollect = currentSuspendChanges.GetLastChange && lastStateInSuspendSession != null;
                currentSuspendChanges = null;
                if (needStateCollect)
                {
                    OnNavigationNodeStateChangedImpl(lastStateInSuspendSession);
                }
                lastStateInSuspendSession = null;
            }
        }

        public NavigationManager(IList<INavigationNodeProvider> nodeProviders)
        {
            history = new ObservableCollection<NavigationState>();
            historyView = CollectionViewSource.GetDefaultView(history);
            historyView.CurrentChanged += new EventHandler(historyView_CurrentChanged);

            _nodeProviders = nodeProviders;
            Initialize();
        }

        void historyView_CurrentChanged(object sender, EventArgs e)
        {
            if (!changingStep)
            {
                this.CurrentStep = historyView.CurrentPosition;
            }
        }

        private void Initialize()
        {
            ForwardCommand = new DelegateCommand(Forward);
            BackCommand = new DelegateCommand(Back);
            ForwardInControlCommand = new DelegateCommand(ForwardInsideControl);
            BackInControlCommand = new DelegateCommand(BackInsideControl);
        }

        public static bool IsNavigationManagerAvailable
        {
            get
            {
                return ServiceLocator.Current != null && Instance != null;
            }
        }

        public static INavigationManager Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<INavigationManager>();
            }
        }

        IList<INavigationNodeProvider> _nodeProviders;

        public IList<INavigationNodeProvider> NodeProviders
        {
            get
            {
                return _nodeProviders;
            }
        }

        //private bool isOpening = false;

        public void Open(string state)
        {
            Open(state, true);
        }

        public void Open(string state, bool useLastState)
        {
            using (this.SuspendChanges(useLastState))
            {
                if (String.IsNullOrEmpty(state))
                    return;
                //isOpening = true;
                //try
                //{
                    var stateList = NavigationState.FromString(state);

                    foreach (var nodeProvider in NodeProviders)
                    {
                        if (nodeProvider.Open(stateList))
                        {
                            //CurrentURIState = stateList;
                            if(!useLastState)
                                OnHistoryStateChanged(stateList);
                            break;
                        }
                    }
                //}
                //finally
                //{
                //    isOpening = false;
                //}
            }
        }

        NavigationState _currentState = null;

        /// <summary>
        /// Доступно только в момент сбора состояний узлов
        /// содержит собранные на данный момент состояния
        /// </summary>
        public NavigationState CurrentState
        {
            get
            {
                return _currentState;
            }
            private set
            {
                _currentState = value;
            }
        }

        public void OnNavigationNodeStateChanged(object sender)
        {
            //if (isOpening)
            //    return;
            if (sender is INavigationNode)
            {
				var node = (INavigationNode)sender;
                if (currentSuspendChanges != null)
                {
                    if(currentSuspendChanges.GetLastChange == true)
                        lastStateInSuspendSession = node;
                    return;
                }

                // Пытаемся собирать состояние после обновления байндинга и загрузки контролов
                Action a = () => OnNavigationNodeStateChangedImpl(node);
                Dispatcher.CurrentDispatcher.BeginInvoke(a, DispatcherPriority.Input);
                //a(); // Debug
            }
        }

		public NavigationState GetNavigationNodeState(INavigationNode node)
		{
			try
			{
                if (node is INavigationNodeProxy)
                {
                    node = (node as INavigationNodeProxy).Child;
                    if (node == null)
                    {
                        return null;
                    }
                }

				NavigationState state = new NavigationState();
				CurrentState = state;

                // Надо собирать состояние от последнего дочернего элемента вверх
                var children = GetAllChildren(new List<INavigationNode>() { node });
                children.Reverse();
                foreach (var child in children)
                {
                    var localState = child.LocalState;
                    if (localState != null)
                    {
                        state.NodesState.AddRange(localState);
                    }
                }

                //// Проход вниз
                //var children = GetNodeChildren(node);
                //while (children.Count > 0)
                //{
                //    var child = children[0];
                //    children.RemoveAt(0);
                //    children.AddRange(GetNodeChildren(child));

                //    var localState = child.LocalState;
                //    if (localState != null)
                //    {
                //        state.NodesState.AddRange(localState);
                //    }
                //}

				// Проход вверх

				var parent = node;

				while (parent != null)
				{
					var localState = parent.LocalState;
					if (localState != null)
					{
						state.NodesState.InsertRange(0, localState);
					}
					parent = FindParentNavigationNode(parent);
				}

				return state;
			}
			finally
			{
				CurrentState = null;
			}
		}

        /// <summary>
        /// Получить все дочерние элементы
        /// элементы сортируются по уровням удаленности
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        List<INavigationNode> GetAllChildren(List<INavigationNode> nodes)
        {
            List<INavigationNode> result = new List<INavigationNode>();

            List<INavigationNode> allChildren = new List<INavigationNode>();
            foreach (var child in nodes)
            {
                allChildren.AddRange(GetNodeChildren(child));
            }

            result.AddRange(allChildren);
            if (allChildren.Count > 0)
            {
                result.AddRange(GetAllChildren(allChildren));
            }
            return result;
        }

    	private void OnNavigationNodeStateChangedImpl(INavigationNode node)
        {
			var state = GetNavigationNodeState(node);
			OnHistoryStateChanged(state);
        }

        private INavigationNode FindParentNavigationNode(object node)
        {
            var parent = node;
            while (parent != null)
            {
                parent = GetParent(parent);
                if (parent is INavigationNode && !(parent is INavigationNodeProxy))
                    return parent as INavigationNode;
            }
            return null;
        }

        private object GetWinFormsAdapterParentHack(object o)
        {
            /********** WinForms ********/
            //Assembly asm = typeof(WindowsFormsHost).Assembly;
            //Type type = asm.GetType ("System.Windows.Forms.Integration.WinFormsAdapter");

            //if(o.GetType() == type)
            //{
            //    object parent = type.InvokeMember ("_host",
            //        BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance,
            //        null,
            //        o,
            //        new object[0]);
            //    WindowsFormsHost host = parent as WindowsFormsHost;
            //    return host;
            //}
            return null;
        }

        private object GetParent(object node)
        {
            if (node is FrameworkElement)
            {
                var el = node as FrameworkElement;
                if (el.Parent is System.Windows.Documents.AdornerDecorator)
                {
                    var decorator = el.Parent as System.Windows.Documents.AdornerDecorator;
                    if (decorator.Parent == null)
                    {
                        /********** WinForms ********/
                        // Это AvalonAdapter
                        //HwndSource wpfHandle = PresentationSource.FromVisual(el as Visual) as HwndSource;

                        //if (wpfHandle != null)
                        //{
                        //    System.Windows.Forms.Integration.ElementHost host = System.Windows.Forms.Control.FromChildHandle(wpfHandle.Handle) as System.Windows.Forms.Integration.ElementHost;
                        //    return host.Parent;
                        //}
                        //else
                        //{
                        //    return null;
                        //}
                    }
                }
                return (node as FrameworkElement).Parent;
            }
            /********** WinForms ********/
            //else if (node is System.Windows.Forms.Control)
            //{
            //    var ctl = node as System.Windows.Forms.Control;
            //    if (ctl.Parent == null)
            //    {
            //        return GetWinFormsAdapterParentHack(ctl);
            //    }
            //    return ctl.Parent;
            //}
            return null;
        }

        private ObservableCollection<string> _history = new ObservableCollection<string>();

        public ObservableCollection<string> History
        {
            get
            {
                return _history;
            }
        }

        public ICollectionView HistoryView
        {
            get
            {
                return historyView;
            }
        }

        private ObservableCollection<NavigationState> history;
        ICollectionView historyView;

        bool changingStep = false;

        int __currentStep = -1;
        int _currentStep
        {
            get
            {
                return __currentStep;
            }
            set
            {
                var old = __currentStep;
                __currentStep = value;
                if (old != __currentStep)
                {
                    changingStep = true;
                    try
                    {
                        historyView.MoveCurrentToPosition(__currentStep);
                    }
                    finally
                    {
                        changingStep = false;
                    }
                    OnPropertyChanged("CurrentStep");
                }
            }
        }

        public int CurrentStep
        {
            get
            {
                return _currentStep;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > History.Count - 1)
                {
                    value = History.Count - 1;
                }
                var oldValue = _currentStep;
                _currentStep = value;
                if (History.Count > 0)
                {
                    try
                    {
                        Open(History[value]);
                    }
                    catch
                    {
                        _currentStep = oldValue;
                        OnPropertyChanged("CurrentStep");
                        throw;
                    }
                }
                OnPropertyChanged("CurrentURI");
            }
        }

        private void OnHistoryStateChanged(NavigationState state)
        {
            var stringState = state.ToString();
            if (History.Count > 0 && History[CurrentStep] == stringState)
                return;
            while(CurrentStep + 1 < History.Count)
            {
                HistoryRemoveAt(CurrentStep + 1);
            }
            HistoryAdd(state);
            //CurrentURIState = state;
            _currentStep = History.Count - 1;
        }

        private void HistoryRemoveAt(int index)
        {
            History.RemoveAt(index);
            history.RemoveAt(index);
        }

        private void HistoryAdd(NavigationState state)
        {
            History.Add(state.ToString());
            history.Add(state);
        }

        WeakKeyDictionary<INavigationNode, NavigationNodeView> nodes = new WeakKeyDictionary<INavigationNode, NavigationNodeView>();

        public void OnNavigationNodeLoaded(object sender)
        {
            if (sender is INavigationNode && !(sender is INavigationNodeProxy))
            {
                var node = sender as INavigationNode;
                var parent = FindParentNavigationNode(node);
                if (parent != null)
                {
                    AddNodeChild(parent, node);
                }
            }
        }

        private List<INavigationNode> GetNodeChildren(INavigationNode node)
        {
            nodes.RemoveCollectedEntries();
            if (nodes.ContainsKey(node))
            {
                if(nodes[node].Children.Count > 0)
                    return nodes[node].Children;
            }
            return new List<INavigationNode>();
        }

        private void AddNodeChild(INavigationNode parent, INavigationNode child)
        {
            nodes.RemoveCollectedEntries();
            NavigationNodeView view = null;
            if (nodes.ContainsKey(parent))
            {
                view = nodes[parent];
            }
            else
            {
                view = new NavigationNodeView();
            }

            view.AddChild(child);

            nodes[parent] = view;
        }

        public string CurrentURI
        {
            get
            {
                if (CurrentURIState == null)
                    return null;
                return CurrentURIState.ToString();
            }
        }

        NavigationState _currentURIState;

        private NavigationState CurrentURIState
        {
            get
            {
                if (CurrentStep < 0)
                {
                    return null;
                }
                return history[CurrentStep];
            }
            //set
            //{
            //    _currentURIState = value;
            //    if (value != null && History.Contains(value.ToString()))
            //    {
            //        int index = -1;
            //        for (int i = 0; i < History.Count; i++)
            //        {
            //            if (History[i] == value.ToString())
            //            {
            //                index = Math.Max(index, i);
            //            }
            //        }
            //        _currentStep = index;
            //    }
            //    OnPropertyChanged("CurrentURI");
            //}
        }


        public void Back()
        {
            CurrentStep--;
        }

        public void Forward()
        {
            CurrentStep++;
        }

        public void BackInsideControl()
        {
            if (CurrentURIState == null || CurrentURIState.NodesState.Count == 0)
                return;
            var controlId = CurrentURIState.NodesState[0].ControlId;

            int i = CurrentStep - 1;
            while (i >= 0 && i <= history.Count - 1)
            {
                var state = history[i];
                if (state.NodesState.Count > 0 && state.NodesState[0].ControlId == controlId)
                {
                    CurrentStep = i;
                    return;
                }
                i--;
            }
        }

        public void ForwardInsideControl()
        {
            if (CurrentURIState == null || CurrentURIState.NodesState.Count == 0)
                return;
            var controlId = CurrentURIState.NodesState[0].ControlId;

            int i = CurrentStep + 1;
            while (i + 1 <= history.Count)
            {
                var state = history[i];
                if (state.NodesState.Count > 0 && state.NodesState[0].ControlId == controlId)
                {
                    CurrentStep = i;
                    return;
                }
                i++;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INavigationManager Members


        public ICommand ForwardCommand
        {
            get;
            private set;
        }

        public ICommand BackCommand
        {
            get;
            private set;
        }

        public ICommand ForwardInControlCommand
        {
            get;
            private set;
        }

        public ICommand BackInControlCommand
        {
            get;
            private set;
        }

        #endregion
    }
}
