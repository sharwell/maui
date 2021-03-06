using System;
using ElmSharp;
using ESize = ElmSharp.Size;
using TSpan = System.Maui.Platform.Tizen.Native.Span;

namespace System.Maui.Platform.Tizen
{
	public class RadioButtonRenderer : ViewRenderer<RadioButton, Radio>
	{
		readonly TSpan _span = new TSpan();
		public RadioButtonRenderer()
		{
			RegisterPropertyHandler(RadioButton.IsCheckedProperty, UpdateIsChecked);
			RegisterPropertyHandler(RadioButton.TextProperty, UpdateText);
			RegisterPropertyHandler(RadioButton.TextColorProperty, UpdateTextColor);
			RegisterPropertyHandler(RadioButton.FontProperty, UpdateFont);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
		{
			if (Control == null)
			{
				SetNativeControl(new Radio(System.Maui.Maui.NativeParent) { StateValue = 1 });
				Control.ValueChanged += OnValueChanged;
			}
			base.OnElementChanged(e);
			ApplyTextAndStyle();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Control != null)
				{
					Control.ValueChanged -= OnValueChanged;
				}
			}
			base.Dispose(disposing);
		}

		protected override Size MinimumSize()
		{
			return Measure(Control.MinimumWidth, Control.MinimumHeight).ToDP();
		}

		protected override ESize Measure(int availableWidth, int availableHeight)
		{
			var size = Control.Geometry;
			Control.Resize(availableWidth, size.Height);
			var formattedSize = Control.EdjeObject["elm.text"].TextBlockFormattedSize;
			Control.Resize(size.Width, size.Height);
			return new ESize()
			{
				Width = Control.MinimumWidth + formattedSize.Width,
				Height = Math.Max(Control.MinimumHeight, formattedSize.Height),
			};
		}

		void OnValueChanged(object sender, EventArgs e)
		{
			Element.SetValueFromRenderer(RadioButton.IsCheckedProperty, Control.GroupValue == 1 ? true : false);
		}
		
		void UpdateIsChecked()
		{
			Control.GroupValue = Element.IsChecked ? 1 : 0;
		}

		void UpdateText(bool isInitialized)
		{
			_span.Text = Element.Text;
			if (!isInitialized)
				ApplyTextAndStyle();
		}

		void UpdateTextColor(bool isInitialized)
		{
			_span.ForegroundColor = Element.TextColor.ToNative();
			if (!isInitialized)
				ApplyTextAndStyle();
		}

		void UpdateFont(bool isInitialized)
		{
			_span.FontSize = Element.FontSize;
			_span.FontAttributes = Element.FontAttributes;
			_span.FontFamily = Element.FontFamily;
			if (!isInitialized)
				ApplyTextAndStyle();
		}

		void ApplyTextAndStyle()
		{
			SetInternalTextAndStyle(_span.GetDecoratedText(), _span.GetStyle());
		}

		void SetInternalTextAndStyle(string formattedText, string textStyle)
		{
			string emission = "elm,state,text,visible";
			if (string.IsNullOrEmpty(formattedText))
			{
				formattedText = null;
				textStyle = null;
				emission = "elm,state,text,hidden";
			}
			Control.Text = formattedText;

			var textblock = Control.EdjeObject["elm.text"];
			if (textblock != null)
			{
				textblock.TextStyle = textStyle;
			}
			Control.EdjeObject.EmitSignal(emission, "elm");
		}
	}
}
