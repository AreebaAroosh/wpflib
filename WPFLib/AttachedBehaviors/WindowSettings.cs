using System;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Markup;

namespace WPFLib
{
    /// <summary>
    /// Persists a Window's Size, Location and WindowState to UserScopeSettings 
    /// </summary>
    public class WindowSettings
    {
        public enum ModeEnum
        {
            All,
            Size,
            Location,
            State,
            None
        }

        #region WindowApplicationSettings Helper Class
        public class WindowApplicationSettings : ApplicationSettingsBase
        {
            private WindowSettings windowSettings;

            private ModeEnum Mode;

            private static string GetWindowKey(Window window)
            {
                // По идее одно окно может иметь несколько мест где его показывают
                // и где оно должно сохранять свои размеры независимо
                // для этого надо делать поле идентификатора которое байндить на дата контекст,
                // потом когда понадобится
                if (!String.IsNullOrEmpty(window.Uid))
                {
                    return window.Uid;
                }
                var type = window.GetType();
                if (type == typeof(Window))
                {
                    throw new Exception("Use custom window class or fill x:Uid for state saving");
                }
                return type.FullName;
            }

            public WindowApplicationSettings(WindowSettings windowSettings)
                : base(GetWindowKey(windowSettings.window))
            {
                this.windowSettings = windowSettings;
            }

            [UserScopedSetting]
            public Rect Location
            {
                get
                {
                    if (this["Location"] != null)
                    {
                        return ((Rect)this["Location"]);
                    }
                    return Rect.Empty;
                }
                set
                {
                    this["Location"] = value;
                }
            }

            [UserScopedSetting]
            public WindowState WindowState
            {
                get
                {
                    if (this["WindowState"] != null)
                    {
                        return (WindowState)this["WindowState"];
                    }
                    return WindowState.Normal;
                }
                set
                {
                    this["WindowState"] = value;
                }
            }

        }
        #endregion

        #region Constructor
        private Window window = null;
        private ModeEnum mode;

        public WindowSettings(Window window, ModeEnum mode)
        {
            this.mode = mode;
            this.window = window;
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Load the Window Size Location and State from the settings object
        /// </summary>
        protected virtual void LoadWindowState()
        {
            this.Settings.Reload();
            if (this.Settings.Location != Rect.Empty)
            {
                if (SaveLocation)
                {
                    this.window.Left = this.Settings.Location.Left;
                    this.window.Top = this.Settings.Location.Top;
                }
                if (SaveSize)
                {
                    this.window.Width = this.Settings.Location.Width;
                    this.window.Height = this.Settings.Location.Height;
                }
            }

            if (this.Settings.WindowState != WindowState.Maximized)
            {
                if (SaveState)
                {
                    this.window.WindowState = this.Settings.WindowState;
                }
            }
        }

        bool SaveState
        {
            get
            {
                return mode == ModeEnum.All || mode == ModeEnum.State;
            }
        }

        bool SaveLocation
        {
            get
            {
                return mode == ModeEnum.All || mode == ModeEnum.Location;
            }
        }

        bool SaveSize
        {
            get
            {
                return mode == ModeEnum.All || mode == ModeEnum.Size;
            }
        }

        /// <summary>
        /// Save the Window Size, Location and State to the settings object
        /// </summary>
        protected virtual void SaveWindowState()
        {
            if (SaveState)
            {
                this.Settings.WindowState = this.window.WindowState;
            }
            if (SaveLocation || SaveSize)
            {
                this.Settings.Location = this.window.RestoreBounds;
            }
            this.Settings.Save();
        }
        #endregion

        #region Private Methods

        internal void Attach()
        {
            if (this.window != null)
            {
                this.window.Closing += new CancelEventHandler(window_Closing);
                this.window.Initialized += new EventHandler(window_Initialized);
                this.window.Loaded += new RoutedEventHandler(window_Loaded);
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Settings.WindowState == WindowState.Maximized)
            {
                this.window.WindowState = this.Settings.WindowState;
            }
        }

        private void window_Initialized(object sender, EventArgs e)
        {
            LoadWindowState();
        }

        private void window_Closing(object sender, CancelEventArgs e)
        {
            SaveWindowState();
        }
        #endregion

        #region Settings Property Implementation
        private WindowApplicationSettings windowApplicationSettings = null;

        protected virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
        {
            return new WindowApplicationSettings(this);
        }

        [Browsable(false)]
        public WindowApplicationSettings Settings
        {
            get
            {
                if (windowApplicationSettings == null)
                {
                    this.windowApplicationSettings = CreateWindowApplicationSettingsInstance();
                }
                return this.windowApplicationSettings;
            }
        }
        #endregion
    }
}