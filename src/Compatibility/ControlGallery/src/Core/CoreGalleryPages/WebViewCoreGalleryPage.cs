using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Microsoft.Maui.Essentials;

using WindowsOS = Microsoft.Maui.Controls.PlatformConfiguration.Windows;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery
{
	internal class WebViewCoreGalleryPage : CoreGalleryPage<WebView>
	{
		protected override bool SupportsFocus
		{
			get { return false; }
		}

		protected override void InitializeElement(WebView element)
		{
			element.HeightRequest = 200;

			element.Source = new UrlWebViewSource { Url = "http://xamarin.com/" };
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			var urlWebViewSourceContainer = new ViewContainer<WebView>(Test.WebView.UrlWebViewSource,
				new WebView
				{
					Source = new UrlWebViewSource { Url = "https://www.google.com/" },
					HeightRequest = 200
				}
			);

			const string html = "<!DOCTYPE html><html>" +
				"<head><meta name='viewport' content='width=device-width,initial-scale=1.0'></head>" +
				"<body><div class=\"test\"><h2>I am raw html</h2></div></body></html>";

			var htmlWebViewSourceContainer = new ViewContainer<WebView>(Test.WebView.HtmlWebViewSource,
				new WebView
				{
					Source = new HtmlWebViewSource { Html = html },
					HeightRequest = 200
				}
			);

			var htmlFileWebSourceContainer = new ViewContainer<WebView>(Test.WebView.LoadHtml,
				new WebView
				{
					Source = new HtmlWebViewSource
					{
						Html = @"<!DOCTYPE html><html>
<head>
<meta name='viewport' content='width=device-width,initial-scale=1.0'>
<link rel=""stylesheet"" href=""default.css"">
</head>
<body>
<h1>Xamarin.Forms</h1>
<p>The CSS and image are loaded from local files!</p>
<img src='WebImages/XamarinLogo.png'/>
<p><a href=""local.html"">next page</a></p>
</body>
</html>"
					},
					HeightRequest = 200
				}
			);

			// NOTE: Currently the ability to programmatically enable/disable mixed content only exists on Android
			// NOTE: Currently the ability to programmatically enable/disable zoom only exists on Android
			if (DeviceInfo.Platform == DevicePlatform.Android)
			{
				var mixedContentTestPage = "https://mixed-content-test.appspot.com/";

				var mixedContentDisallowedWebView = new WebView() { HeightRequest = 1000 };
				mixedContentDisallowedWebView.On<Android>().SetMixedContentMode(MixedContentHandling.NeverAllow);
				mixedContentDisallowedWebView.Source = new UrlWebViewSource
				{
					Url = mixedContentTestPage
				};

				var mixedContentAllowedWebView = new WebView() { HeightRequest = 1000 };
				mixedContentAllowedWebView.On<Android>().SetMixedContentMode(MixedContentHandling.AlwaysAllow);
				mixedContentAllowedWebView.Source = new UrlWebViewSource
				{
					Url = mixedContentTestPage
				};

				var enableZoomControlsWebView = new WebView() { HeightRequest = 200 };
				enableZoomControlsWebView.On<Android>().SetEnableZoomControls(true);
				enableZoomControlsWebView.On<Android>().SetDisplayZoomControls(false);
				enableZoomControlsWebView.Source = new UrlWebViewSource
				{
					Url = "https://www.xamarin.com"
				};

				var displayZoomControlsWebView = new WebView() { HeightRequest = 200 };
				displayZoomControlsWebView.On<Android>().SetEnableZoomControls(true);
				displayZoomControlsWebView.On<Android>().SetDisplayZoomControls(true);
				displayZoomControlsWebView.Source = new UrlWebViewSource
				{
					Url = "https://www.xamarin.com"
				};

				var mixedContentDisallowedContainer = new ViewContainer<WebView>(Test.WebView.MixedContentDisallowed,
					mixedContentDisallowedWebView);
				var mixedContentAllowedContainer = new ViewContainer<WebView>(Test.WebView.MixedContentAllowed,
					mixedContentAllowedWebView);

				var enableZoomControlsContainer = new ViewContainer<WebView>(Test.WebView.EnableZoomControls,
					enableZoomControlsWebView);
				var displayZoomControlsWebViewContainer = new ViewContainer<WebView>(Test.WebView.DisplayZoomControls,
					displayZoomControlsWebView);

				Add(mixedContentDisallowedContainer);
				Add(mixedContentAllowedContainer);
				Add(enableZoomControlsContainer);
				Add(displayZoomControlsWebViewContainer);
			}


			var jsAlertWebView = new WebView
			{
				Source = new HtmlWebViewSource
				{
					Html = @"<!DOCTYPE html><html>
<head>
<meta name='viewport' content='width=device-width,initial-scale=1.0'>
<link rel=""stylesheet"" href=""default.css"">
</head>
<body>
<button onclick=""window.alert('foo');"">Click</button>
</body>
</html>"
				},
				HeightRequest = 200
			};

			jsAlertWebView.On<WindowsOS>().SetIsJavaScriptAlertEnabled(true);

			var javascriptAlertWebSourceContainer = new ViewContainer<WebView>(Test.WebView.JavaScriptAlert,
				jsAlertWebView
			);

			var evaluateJsWebView = new WebView
			{
				Source = new UrlWebViewSource { Url = "https://www.google.com/" },
				HeightRequest = 50
			};
			var evaluateJsWebViewSourceContainer = new ViewContainer<WebView>(Test.WebView.EvaluateJavaScript,
				evaluateJsWebView
			);

			var resultsLabel = new Label();
			var execButton = new Button();
			execButton.Text = "Evaluate Javascript";
			execButton.Command = new Command(async () => resultsLabel.Text = await evaluateJsWebView.EvaluateJavaScriptAsync(
												"var test = function(){ return 'This string came from Javascript!'; }; test();"));

			evaluateJsWebViewSourceContainer.ContainerLayout.Children.Add(resultsLabel);
			evaluateJsWebViewSourceContainer.ContainerLayout.Children.Add(execButton);


			Add(urlWebViewSourceContainer);
			Add(htmlWebViewSourceContainer);
			Add(htmlFileWebSourceContainer);
			Add(javascriptAlertWebSourceContainer);
			Add(evaluateJsWebViewSourceContainer);
		}
	}
}