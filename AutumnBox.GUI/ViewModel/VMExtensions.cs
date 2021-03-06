﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/15 22:45:41 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Basic.Device;
using AutumnBox.GUI.Model;
using AutumnBox.GUI.MVVM;
using AutumnBox.GUI.Properties;
using AutumnBox.GUI.Util.Bus;
using AutumnBox.GUI.Util.Debugging;
using AutumnBox.GUI.Util.I18N;
using AutumnBox.Logging;
using AutumnBox.OpenFramework.Extension;
using AutumnBox.OpenFramework.Fast;
using AutumnBox.OpenFramework.Management;
using AutumnBox.OpenFramework.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutumnBox.GUI.ViewModel
{
    class VMExtensions : ViewModelBase
    {

        #region MVVM
        public IEnumerable<ExtensionWrapperDock> Docks
        {
            get => _docks; set
            {
                _docks = value;
                RaisePropertyChanged();
            }
        }
        private IEnumerable<ExtensionWrapperDock> _docks;

        public bool ExtPanelIsEnabled
        {
            get => _extPanelIsEnabled; set
            {
                _extPanelIsEnabled = value;
                RaisePropertyChanged();
            }
        }
        private bool _extPanelIsEnabled = false;

        public IEnumerable<IExtensionWrapper> Extensions
        {
            get => _extensions; set
            {
                _extensions = value;
                Docks = value?.ToDocks();
                RaisePropertyChanged();
            }
        }
        private IEnumerable<IExtensionWrapper> _extensions;

        public ICommand ClickItem
        {
            get => _clickItem; private set
            {
                _clickItem = value;
                RaisePropertyChanged();
            }
        }
        private ICommand _clickItem;

        public ICommand DoubleClickItem
        {
            get => _doubleClickItem; private set
            {
                _doubleClickItem = value;
                RaisePropertyChanged();
            }
        }
        private ICommand _doubleClickItem;

        #endregion

        #region Device
        public void OnSelectNoDevice(object sender, EventArgs e)
        {
            ExtPanelIsEnabled = false;
        }

        public void OnSelectDevice(object sender, EventArgs e)
        {
            ExtPanelIsEnabled = (targetState & DeviceSelectionObserver.Instance.CurrentDevice.State) != 0;
        }

        #endregion
        private DeviceState targetState;


        public VMExtensions()
        {

        }

        internal void Load(DeviceState state)
        {
            targetState = state;
            ComObserver();
            ClickItem = new FlexiableCommand((p) =>
            {
                if (!Settings.Default.DoubleClickRunExt)
                {
                    (p as ExtensionWrapperDock)?.Wrapper?.GetThread().Start();
                }
            });
            DoubleClickItem = new FlexiableCommand((p) =>
            {
                if (Settings.Default.DoubleClickRunExt)
                {
                    (p as ExtensionWrapperDock)?.Wrapper?.GetThread().Start();
                }
            });
        }
        private void ComObserver()
        {
            OpenFxEventBus.AfterOpenFxLoaded(() =>
            {
                LoadExtensions();
            });
            LanguageManager.Instance.LanguageChanged += (s, e) =>
            {
                LoadExtensions();
            };
            ExtensionViewRefresher.Instance.Refreshing += (s, e) =>
            {
                LoadExtensions();
            };
            if (targetState == AutumnBoxExtension.NoMatter)
            {
                ExtPanelIsEnabled = true;
                return;
            }
            else
            {
                DeviceSelectionObserver.Instance.SelectedDevice += OnSelectDevice;
                DeviceSelectionObserver.Instance.SelectedNoDevice += OnSelectNoDevice;
            }
        }
        public void LoadExtensions()
        {
            try
            {
                IEnumerable<IExtensionWrapper> filted =
                    OpenFx.LibsManager.Wrappers().
                    State(targetState).Region(LanguageManager.Instance.Current.LanCode).Hide().Dev(Settings.Default.DeveloperMode);
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (filted.Count() == 0)
                    {
                        Extensions = null;
                    }
                    else
                    {
                        Extensions = filted;
                    }
                });
            }
            catch (Exception e)
            {
                SLogger<VMExtensions>.Warn("can not refresh extensions", e);
            }

        }
    }
}
