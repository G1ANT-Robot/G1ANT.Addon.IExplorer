﻿/**
*    Copyright(C) G1ANT Ltd, All rights reserved
*    Solution G1ANT.Addon, Project G1ANT.Addon.IExplorer
*    www.g1ant.com
*
*    Licensed under the G1ANT license.
*    See License.txt file in the project root for full license information.
*
*/
using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace G1ANT.Addon.IExplorer
{
    public class TabActivator
    {

        private enum OBJID : uint
        {
            OBJID_WINDOW = 0x00000000,
        }

        private const int CHILDID_SELF = 0;
        private readonly IntPtr _hWnd;
        private IAccessible _accessible;

        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(IntPtr hwnd, uint id, ref Guid iid,
                                                             [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object
                                                                 ppvObject);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
                                                  string lpszWindow);

        [DllImport("oleacc.dll")]
        private static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren,
                                                     [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] object[] rgvarChildren, out int pcObtained);

        internal TabActivator(IntPtr ieHandle)
        {
            _hWnd = ieHandle;
            AccessibleObjectFromWindow(GetDirectUIHWND(ieHandle), OBJID.OBJID_WINDOW, ref _accessible);

            CheckForAccessible();
        }

        private TabActivator(IAccessible acc)
        {
            if (acc == null)
                throw new Exception("Could not get accessible");

            _accessible = acc;
        }

        private TabActivator[] Children
        {
            get
            {
                var num = 0;
                var res = GetAccessibleChildren(_accessible, out num);

                if (res == null)
                    return new TabActivator[0];

                var list = new List<TabActivator>(res.Length);

                foreach (object obj in res)
                {
                    var acc = obj as IAccessible;

                    if (acc != null)
                        list.Add(new TabActivator(acc));
                }

                return list.ToArray();
            }
        }

        private int ChildCount
        {
            get { return _accessible.accChildCount; }
        }

        private string LocationUrl
        {
            get
            {
                var url = _accessible.accDescription[CHILDID_SELF];

                if (url.Contains(Environment.NewLine))
                    url = url.Split('\n')[1];

                return url;
            }
        }

        private string Title
        {
            get
            {
                var title = _accessible.accDescription[CHILDID_SELF];

                if (title.Contains(Environment.NewLine))
                    title = title.Split('\n')[0].Trim();

                return title;
            }
        }

        private void CheckForAccessible()
        {
            if (_accessible == null)
                throw new Exception("Could not get accessible.  Accessible can't be null");
        }

        internal void ActivateTab(string by, string phrase)
        {
            TabSearchBy searchBy;
            if (Enum.TryParse(by.CapitalizeFirstLetter(), out searchBy) == false)
            {
                throw new ApplicationException("Argument 'by' is not correct");
            }

            int tabIndexToActivate = 0;
            switch (searchBy)
            {
                case TabSearchBy.Title:
                    tabIndexToActivate = GetTabIndexToActivateByTitle(phrase);
                    break;
                case TabSearchBy.Url:
                    tabIndexToActivate = GetTabIndexToActivateByUrl(phrase);
                    break;
            }

            AccessibleObjectFromWindow(GetDirectUIHWND(_hWnd), OBJID.OBJID_WINDOW, ref _accessible);

            CheckForAccessible();

            var index = 0;
            var ieDirectUIHWND = new TabActivator(_hWnd);

            foreach (var accessor in ieDirectUIHWND.Children)
            {
                foreach (var child in accessor.Children)
                {
                    foreach (var tab in child.Children)
                    {
                        if (tabIndexToActivate >= child.ChildCount - 1)
                            return;

                        if (index == tabIndexToActivate)
                        {
                            tab.ActivateTab();
                            var activeIe = IEManager.launchedIE?.Where(ie => ie.Ie.hWnd == _hWnd).FirstOrDefault();
                            if (activeIe != null)
                            {
                                IEManager.SwitchIE(activeIe.Id);
                            }
                            return;
                        }

                        index++;
                    }
                }
            }
        }

        private void ActivateTab()
        {
            _accessible.accDoDefaultAction(CHILDID_SELF);
        }

        private int GetTabIndexToActivateByUrl(string urlPart)
        {
            AccessibleObjectFromWindow(GetDirectUIHWND(_hWnd), OBJID.OBJID_WINDOW, ref _accessible);

            CheckForAccessible();

            var index = 0;
            var ieDirectUIHWND = new TabActivator(_hWnd);

            foreach (var accessor in ieDirectUIHWND.Children)
            {
                foreach (var child in accessor.Children)
                {
                    foreach (var tab in child.Children)
                    {
                        var tabUrl = tab.LocationUrl;

                        if (!string.IsNullOrEmpty(tabUrl))
                        {
                            if (tab.LocationUrl.ToLower().Contains(urlPart.ToLower()))
                                return index;
                        }

                        index++;
                    }
                }
            }

            return -1;
        }

        private int GetTabIndexToActivateByTitle(string titlePart)
        {
            AccessibleObjectFromWindow(GetDirectUIHWND(_hWnd), OBJID.OBJID_WINDOW, ref _accessible);

            CheckForAccessible();

            var index = 0;
            var ieDirectUIHWND = new TabActivator(_hWnd);

            foreach (var accessor in ieDirectUIHWND.Children)
            {
                foreach (var child in accessor.Children)
                {
                    foreach (var tab in child.Children)
                    {
                        if (!string.IsNullOrEmpty(tab.Title))
                        {
                            if (tab.Title.ToLower().Contains(titlePart.ToLower()))
                                return index;
                        }
                        index++;
                    }
                }
            }

            return -1;
        }

        private IntPtr GetDirectUIHWND(IntPtr ieFrame)
        {
            // For IE 8:
            var directUI = FindWindowEx(ieFrame, IntPtr.Zero, "CommandBarClass", null);
            directUI = FindWindowEx(directUI, IntPtr.Zero, "ReBarWindow32", null);
            directUI = FindWindowEx(directUI, IntPtr.Zero, "TabBandClass", null);
            directUI = FindWindowEx(directUI, IntPtr.Zero, "DirectUIHWND", null);

            if (directUI == IntPtr.Zero)
            {
                // For IE 9:
                //directUI = FindWindowEx(ieFrame, IntPtr.Zero, "WorkerW", "Navigation Bar");
                directUI = FindWindowEx(ieFrame, IntPtr.Zero, "WorkerW", null);
                directUI = FindWindowEx(directUI, IntPtr.Zero, "ReBarWindow32", null);
                directUI = FindWindowEx(directUI, IntPtr.Zero, "TabBandClass", null);
                directUI = FindWindowEx(directUI, IntPtr.Zero, "DirectUIHWND", null);
            }

            return directUI;
        }

        private static int AccessibleObjectFromWindow(IntPtr hwnd, OBJID idObject, ref IAccessible acc)
        {
            var guid = new Guid("{618736e0-3c3d-11cf-810c-00aa00389b71}"); // IAccessibleobject obj = null;
            object obj = null;

            var num = AccessibleObjectFromWindow(hwnd, (uint)idObject, ref guid, ref obj);

            acc = (IAccessible)obj;

            return num;
        }

        private static object[] GetAccessibleChildren(IAccessible ao, out int childs)
        {
            childs = 0;
            object[] ret = null;
            var count = ao.accChildCount;

            if (count > 0)
            {
                ret = new object[count];
                AccessibleChildren(ao, 0, count, ret, out childs);
            }

            return ret;
        }
    }
}
