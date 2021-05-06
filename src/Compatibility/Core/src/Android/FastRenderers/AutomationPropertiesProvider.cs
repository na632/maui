using System;
using System.ComponentModel;
using Android.Views;
using Android.Widget;
using AView = Android.Views.View;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Android.FastRenderers
{
	internal class AutomationPropertiesProvider : IDisposable
	{
		string _defaultContentDescription;
		bool? _defaultFocusable;
		ImportantForAccessibility? _defaultImportantForAccessibility;
		string _defaultHint;
		bool _disposed;

		IVisualElementRenderer _renderer;

		public AutomationPropertiesProvider(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementPropertyChanged += OnElementPropertyChanged;
			_renderer.ElementChanged += OnElementChanged;
		}

		AView Control => _renderer?.View;

		VisualElement Element => _renderer?.Element;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (Element != null)
			{
				Element.PropertyChanged -= OnElementPropertyChanged;
			}

			if (_renderer != null)
			{
				_renderer.ElementChanged -= OnElementChanged;
				_renderer.ElementPropertyChanged -= OnElementPropertyChanged;

				_renderer = null;
			}
		}

		void SetContentDescription()
			=> Controls.Platform.AutomationPropertiesProvider.SetContentDescription(Control, Element, _defaultContentDescription, _defaultHint);

		void SetFocusable()
			=> Controls.Platform.AutomationPropertiesProvider.SetFocusable(Control, Element, ref _defaultFocusable, ref _defaultImportantForAccessibility);

		void SetLabeledBy()
			=> Controls.Platform.AutomationPropertiesProvider.SetLabeledBy(Control, Element);

		bool _defaultsSet;
		void SetupDefaults()
		{
			if (_defaultsSet || Control == null)
				return;

			_defaultsSet = true;
			Controls.Platform.AutomationPropertiesProvider.SetupDefaults(Control, ref _defaultHint, ref _defaultContentDescription);
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += OnElementPropertyChanged;
			}

			SetupDefaults();
			Controls.Platform.AutomationPropertiesProvider.AccessibilitySettingsChanged(Control, Element, _defaultHint, _defaultContentDescription, ref _defaultFocusable, ref _defaultImportantForAccessibility);
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == AutomationProperties.HelpTextProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == AutomationProperties.NameProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == AutomationProperties.IsInAccessibleTreeProperty.PropertyName)
			{
				SetFocusable();
			}
			else if (e.PropertyName == AutomationProperties.LabeledByProperty.PropertyName)
			{
				SetLabeledBy();
			}
		}
	}
}