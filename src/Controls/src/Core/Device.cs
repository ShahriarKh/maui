using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Essentials;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="Type[@FullName='Microsoft.Maui.Controls.Device']/Docs" />
	//[Obsolete]
	public static class Device
	{
		// this is just for those cases where the runtime needs to pre-load renderers
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Assembly DefaultRendererAssembly { get; set; }

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='iOS']/Docs" />
		[Obsolete("Use Essentials.DevicePlatform.iOS instead.")]
		public const string iOS = "iOS";
		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='Android']/Docs" />
		[Obsolete("Use Essentials.DevicePlatform.Android instead.")]
		public const string Android = "Android";
		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='UWP']/Docs" />
		[Obsolete("Use Essentials.DevicePlatform.UWP instead.")]
		public const string UWP = "UWP";
		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='macOS']/Docs" />
		[Obsolete("Use Essentials.DevicePlatform.macOS instead.")]
		public const string macOS = "macOS";
		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='GTK']/Docs" />
		[Obsolete("Use Essentials.DevicePlatform.GTK instead.")]
		public const string GTK = "GTK";
		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='Tizen']/Docs" />
		[Obsolete("Use Essentials.DevicePlatform.Tizen instead.")]
		public const string Tizen = "Tizen";
		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='WPF']/Docs" />
		[Obsolete("Use Essentials.DevicePlatform.WPF instead.")]
		public const string WPF = "WPF";
		[Obsolete("Use Essentials.DevicePlatform.MacCatalyst instead.")]
		public const string MacCatalyst = "MacCatalyst";
		[Obsolete("Use Essentials.DevicePlatform.tvOS instead.")]
		public const string tvOS = "tvOS";

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='Idiom']/Docs" />
		[Obsolete("Use Essentials.DeviceInfo.Idiom instead.")]
		public static TargetIdiom Idiom
		{
			get
			{
				var idiom = DeviceInfo.Idiom;
				if (idiom == DeviceIdiom.Tablet)
					return TargetIdiom.Tablet;
				if (idiom == DeviceIdiom.Phone)
					return TargetIdiom.Phone;
				if (idiom == DeviceIdiom.Desktop)
					return TargetIdiom.Desktop;
				if (idiom == DeviceIdiom.TV)
					return TargetIdiom.TV;
				if (idiom == DeviceIdiom.Watch)
					return TargetIdiom.Watch;
				return TargetIdiom.Unsupported;
			}
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='RuntimePlatform']/Docs" />
		[Obsolete("Use Essentials.DeviceInfo.Platform instead.")]
		public static string RuntimePlatform => DeviceInfo.Platform.ToString();

		// [Obsolete]
		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='FlowDirection']/Docs" />
		public static FlowDirection FlowDirection
		{
			get
			{
				return AppInfo.RequestedLayoutDirection == LayoutDirection.RightToLeft
					? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
			}
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='IsInvokeRequired']/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.IsDispatchRequired instead.")]
		public static bool IsInvokeRequired =>
			Application.Current.FindDispatcher().IsDispatchRequired;

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='BeginInvokeOnMainThread']/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.Dispatch() instead.")]
		public static void BeginInvokeOnMainThread(Action action) =>
			Application.Current.FindDispatcher().Dispatch(action);

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='InvokeOnMainThreadAsync']/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.DispatchAsync() instead.")]
		public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func) =>
			Application.Current.FindDispatcher().DispatchAsync(func);

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='InvokeOnMainThreadAsync'][0]/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.DispatchAsync() instead.")]
		public static Task InvokeOnMainThreadAsync(Action action) =>
			Application.Current.FindDispatcher().DispatchAsync(action);

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='InvokeOnMainThreadAsync']/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.DispatchAsync() instead.")]
		public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask) =>
			Application.Current.FindDispatcher().DispatchAsync(funcTask);

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='InvokeOnMainThreadAsync']/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.DispatchAsync() instead.")]
		public static Task InvokeOnMainThreadAsync(Func<Task> funcTask) =>
			Application.Current.FindDispatcher().DispatchAsync(funcTask);

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='GetMainThreadSynchronizationContextAsync']/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.GetSynchronizationContextAsync() instead.")]
		public static Task<SynchronizationContext> GetMainThreadSynchronizationContextAsync() =>
			Application.Current.FindDispatcher().GetSynchronizationContextAsync();

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='GetNamedSize'][1]/Docs" />
		public static double GetNamedSize(NamedSize size, Element targetElement)
		{
			return GetNamedSize(size, targetElement.GetType());
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='GetNamedSize'][0]/Docs" />
		public static double GetNamedSize(NamedSize size, Type targetElementType)
		{
			return GetNamedSize(size, targetElementType, false);
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='StartTimer']/Docs" />
		[Obsolete("Use BindableObject.Dispatcher.StartTimer() instead.")]
		public static void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			_ = callback ?? throw new ArgumentNullException(nameof(callback));

			var dispatcher = Application.Current.FindDispatcher();

			dispatcher.StartTimer(interval, callback);
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Device.xml" path="//Member[@MemberName='GetNamedSize'][2]/Docs" />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes) =>
			DependencyService.Get<IFontNamedSizeService>()?.GetNamedSize(size, targetElementType, useOldSizes) ??
			throw new NotImplementedException("The current platform does not implement the IFontNamedSizeService dependency service.");

		public static class Styles
		{
			public static readonly string TitleStyleKey = "TitleStyle";

			public static readonly string SubtitleStyleKey = "SubtitleStyle";

			public static readonly string BodyStyleKey = "BodyStyle";

			public static readonly string ListItemTextStyleKey = "ListItemTextStyle";

			public static readonly string ListItemDetailTextStyleKey = "ListItemDetailTextStyle";

			public static readonly string CaptionStyleKey = "CaptionStyle";

			public static readonly Style TitleStyle = new Style(typeof(Label)) { BaseResourceKey = TitleStyleKey };

			public static readonly Style SubtitleStyle = new Style(typeof(Label)) { BaseResourceKey = SubtitleStyleKey };

			public static readonly Style BodyStyle = new Style(typeof(Label)) { BaseResourceKey = BodyStyleKey };

			public static readonly Style ListItemTextStyle = new Style(typeof(Label)) { BaseResourceKey = ListItemTextStyleKey };

			public static readonly Style ListItemDetailTextStyle = new Style(typeof(Label)) { BaseResourceKey = ListItemDetailTextStyleKey };

			public static readonly Style CaptionStyle = new Style(typeof(Label)) { BaseResourceKey = CaptionStyleKey };
		}
	}
}
