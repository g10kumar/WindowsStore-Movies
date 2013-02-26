// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace ShareAll.Common
{
    class SettingsFlyout
    {
        private const int _width = 400;
        private Popup _popup;

        public void ShowFlyout(UserControl control)
        {
            _popup = new Popup();
            _popup.Closed += OnPopupClosed;
            Window.Current.Activated += OnWindowActivated;
            _popup.IsLightDismissEnabled = true;
            _popup.Width = _width;
            _popup.Height = Window.Current.Bounds.Height;

            control.Width = _width;
            control.Height = Window.Current.Bounds.Height;

            _popup.Child = control;
            _popup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - _width);
            _popup.SetValue(Canvas.TopProperty, 0);
            _popup.IsOpen = true;
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _popup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
        }
    }
}