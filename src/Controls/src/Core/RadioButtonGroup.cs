using System.Collections;
using CommunityToolkit.Mvvm.Messaging;

namespace Microsoft.Maui.Controls
{
	public static class RadioButtonGroup
	{
		static readonly BindableProperty RadioButtonGroupControllerProperty =
			BindableProperty.CreateAttached("RadioButtonGroupController", typeof(RadioButtonGroupController), typeof(Compatibility.Layout<View>), default(RadioButtonGroupController),
			defaultValueCreator: (b) => new RadioButtonGroupController((Compatibility.Layout<View>)b),
			propertyChanged: (b, o, n) => OnControllerChanged(b, (RadioButtonGroupController)o, (RadioButtonGroupController)n));

		static RadioButtonGroupController GetRadioButtonGroupController(BindableObject b)
		{
			return (RadioButtonGroupController)b.GetValue(RadioButtonGroupControllerProperty);
		}

		public static readonly BindableProperty GroupNameProperty =
			BindableProperty.Create("GroupName", typeof(string), typeof(Compatibility.Layout<View>), null,
			propertyChanged: (b, o, n) => { GetRadioButtonGroupController(b).GroupName = (string)n; });

		public static string GetGroupName(BindableObject b)
		{
			return (string)b.GetValue(GroupNameProperty);
		}

		public static void SetGroupName(BindableObject bindable, string groupName)
		{
			bindable.SetValue(GroupNameProperty, groupName);
		}

		public static readonly BindableProperty SelectedValueProperty =
			BindableProperty.Create("SelectedValue", typeof(object), typeof(Compatibility.Layout<View>), null,
			defaultBindingMode: BindingMode.TwoWay,
			propertyChanged: (b, o, n) => { GetRadioButtonGroupController(b).SelectedValue = n; });

		public static object GetSelectedValue(BindableObject bindableObject)
		{
			return bindableObject.GetValue(SelectedValueProperty);
		}

		public static void SetSelectedValue(BindableObject bindable, object selectedValue)
		{
			bindable.SetValue(SelectedValueProperty, selectedValue);
		}

		internal static void UpdateRadioButtonGroup(RadioButton radioButton)
		{
			string groupName = radioButton.GroupName;

			Element scope = string.IsNullOrEmpty(groupName)
				? GroupByParent(radioButton)
				: GetVisualRoot(radioButton);

			WeakReferenceMessenger.Default.Send(new RadioButtonGroupSelectionChanged(scope, radioButton));
		}

		internal static Element GroupByParent(RadioButton radioButton)
		{
			Element parent = radioButton.Parent;

			if (parent != null)
			{
				// Traverse logical children
				IEnumerable children = ((IElementController)parent).LogicalChildren;
				IEnumerator itor = children.GetEnumerator();
				while (itor.MoveNext())
				{
					var rb = itor.Current as RadioButton;
					if (rb != null && rb != radioButton && string.IsNullOrEmpty(rb.GroupName) && (rb.IsChecked == true))
						rb.SetValueFromRenderer(RadioButton.IsCheckedProperty, false);
				}
			}

			return parent;
		}

		static void OnControllerChanged(BindableObject bindableObject, RadioButtonGroupController oldController,
			RadioButtonGroupController newController)
		{
			if (newController == null)
			{
				return;
			}

			newController.GroupName = GetGroupName(bindableObject);
			newController.SelectedValue = GetSelectedValue(bindableObject);
		}

		internal static Element GetVisualRoot(Element element)
		{
			Element parent = element.Parent;
			while (parent != null && !(parent is Page))
				parent = parent.Parent;
			return parent;
		}
	}
}