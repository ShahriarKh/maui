﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Maui.Controls.Sample.Pages.ShellGalleries
{
	public partial class ShellChromeGallery
	{
		AppShell AppShell => Application.Current.MainPage as AppShell;

		public ShellChromeGallery()
		{
			InitializeComponent();

			flyoutBehavior.ItemsSource = Enum.GetNames(typeof(FlyoutBehavior));
			flyoutBehavior.SelectedIndexChanged += OnFlyoutBehaviorSelectedIndexChanged;

			if (AppShell != null)
				flyoutBehavior.SelectedIndex = (int)AppShell.FlyoutBehavior;
			else
				flyoutBehavior.SelectedIndex = 1;

			AppShell.FlyoutBackdrop = SolidColorBrush.Pink;
		}

		async void OnPushPage(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ShellChromeGallery());
		}

		async void OnPopPage(object sender, EventArgs e)
		{
			if (Navigation.NavigationStack.Count > 1)
				await Navigation.PopAsync();
		}

		void OnFlyoutBehaviorSelectedIndexChanged(object sender, EventArgs e)
		{
			AppShell.FlyoutBehavior = (FlyoutBehavior)flyoutBehavior.SelectedIndex;
		}

		protected override void OnAppearing()
		{
			AppShell.FlyoutBehavior = (FlyoutBehavior)flyoutBehavior.SelectedIndex;
		}

		void OnToggleFlyoutBackgroundColor(object sender, EventArgs e)
		{
			AppShell.RemoveBinding(Shell.FlyoutBackgroundProperty);
			if (AppShell.FlyoutBackground.IsEmpty ||
				AppShell.FlyoutBackground == SolidColorBrush.Purple)
			{
				AppShell.FlyoutBackground = SolidColorBrush.Black;
			}
			else if (AppShell.FlyoutBackground == SolidColorBrush.Black)
			{
				AppShell.FlyoutBackground = SolidColorBrush.Purple;
			}
			else
			{
				AppShell.FlyoutBackground = SolidColorBrush.Purple;
			}

			flyoutBackgroundColor.Background = AppShell.FlyoutBackground;
		}

		void OnToggleNavBarIsVisible(object sender, EventArgs e)
		{
			Shell.SetNavBarIsVisible(this, !Shell.GetNavBarIsVisible(this));
		}

		void OnToggleBackButtonIsVisible(object sender, EventArgs e)
		{
			var backButtonBehavior = Shell.GetBackButtonBehavior(this) ?? new BackButtonBehavior();
			backButtonBehavior.IsVisible = !backButtonBehavior.IsVisible;
			Shell.SetBackButtonBehavior(this, backButtonBehavior);
		}

		void OnToggleSearchHandler(object sender, EventArgs e)
		{
			var searchHandler = Shell.GetSearchHandler(this);
			if (searchHandler != null)
				RemoveSearchHandler();
			else
				AddSearchHandler("text here");
		}

		protected void AddSearchHandler(string placeholder)
		{
			var searchHandler = new CustomSearchHandler();

			searchHandler.ShowsResults = true;

			searchHandler.ClearIconName = "Clear";
			searchHandler.ClearIconHelpText = "Clears the search field text";

			searchHandler.ClearPlaceholderName = "Voice Search";
			searchHandler.ClearPlaceholderHelpText = "Start voice search";

			searchHandler.QueryIconName = "Search";
			searchHandler.QueryIconHelpText = "Press to search app";

			searchHandler.Placeholder = placeholder;

			searchHandler.ClearPlaceholderEnabled = true;
			searchHandler.ClearPlaceholderIcon = "mic.png";

			Shell.SetSearchHandler(this, searchHandler);
		}

		protected void RemoveSearchHandler()
		{
			ClearValue(Shell.SearchHandlerProperty);
		}
	}

	internal class CustomSearchHandler : SearchHandler
	{
		protected async override void OnQueryChanged(string oldValue, string newValue)
		{
			base.OnQueryChanged(oldValue, newValue);

			if (string.IsNullOrEmpty(newValue))
			{
				ItemsSource = null;
			}
			else
			{
				List<string> results = new List<string>();
				results.Add(newValue + "initial");

				ItemsSource = results;

				await Task.Delay(2000);

				results = new List<string>();

				for (int i = 0; i < 10; i++)
				{
					results.Add(newValue + i);
				}

				ItemsSource = results;
			}
		}
	}
}