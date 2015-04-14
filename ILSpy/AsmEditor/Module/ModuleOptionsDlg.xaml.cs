﻿/*
    Copyright (C) 2014-2015 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ICSharpCode.ILSpy.AsmEditor.Module
{
	/// <summary>
	/// Interaction logic for ModuleOptionsDlg.xaml
	/// </summary>
	public partial class ModuleOptionsDlg : Window
	{
		public ModuleOptionsDlg()
		{
			InitializeComponent();
			SourceInitialized += (s, e) => this.HideMinimizeAndMaximizeButtons();
		}

		void Ok_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !HasError;
		}

		void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}

		public bool HasError {
			get {
				var data = (ModuleOptionsVM)DataContext;
				if (data != null && data.HasError)
					return true;

				return this.HasError();
			}
		}
	}
}
