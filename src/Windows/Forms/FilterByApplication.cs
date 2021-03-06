﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Harvester.Core;
using Harvester.Core.Filters;
using Harvester.Properties;

/* Copyright (c) 2012-2013 CBaxter
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE. 
 */

namespace Harvester.Forms
{
    internal partial class FilterByApplication : FormBase
    {
        private readonly DynamicFilterExpression filter;

        public Boolean FilterEnabled { get { return filter.ApplicationFilters.Any(); } }

        public FilterByApplication(DynamicFilterExpression dynamicFilter)
        {
            Verify.NotNull(dynamicFilter, "dynamicFilter");

            InitializeComponent();

            filter = dynamicFilter;
            resetButton.Click += (sender, e) => HandleEvent(ClearChecked);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Font = SystemEventProperties.Default.Font;
            HandleEvent(PopulateApplications);
        }

        private void PopulateApplications()
        {
            var filteredApplications = new HashSet<String>(filter.ApplicationFilters, StringComparer.OrdinalIgnoreCase);
            var uniqueApplications = new HashSet<String>();

            foreach (var process in Process.GetProcesses().OrderBy(process => process.ProcessName))
            {
                if (uniqueApplications.Add(process.ProcessName))
                    applications.Items.Add(process.ProcessName, filteredApplications.Contains(process.ProcessName));

                process.Dispose();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (DialogResult == DialogResult.OK)
                HandleEvent(ApplyFilterChanges);
        }

        private void ApplyFilterChanges()
        {
            filter.ApplicationFilters = applications.CheckedItems.Cast<String>().ToList();
            filter.Update();
        }

        private void ClearChecked()
        {
            applications.ClearSelected();

            foreach (Int32 index in applications.CheckedIndices)
                applications.SetItemChecked(index, false);
        }
    }
}
