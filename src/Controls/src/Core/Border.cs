﻿#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	[ContentProperty(nameof(Content))]
	public class Border : View, IContentView, IBorderView, IPaddingElement
	{
		ReadOnlyCollection<Element>? _logicalChildren;

		internal ObservableCollection<Element> InternalChildren { get; } = new();

		internal override IReadOnlyList<Element> LogicalChildrenInternal =>
			_logicalChildren ??= new ReadOnlyCollection<Element>(InternalChildren);

		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View),
			typeof(Border), null, propertyChanged: ContentChanged);

		public static readonly BindableProperty PaddingProperty = PaddingElement.PaddingProperty;

		public View? Content
		{
			get { return (View?)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public Thickness Padding
		{
			get => (Thickness)GetValue(PaddingElement.PaddingProperty);
			set => SetValue(PaddingElement.PaddingProperty, value);
		}

		public static readonly BindableProperty StrokeShapeProperty =
			BindableProperty.Create(nameof(StrokeShape), typeof(IShape), typeof(Layout), null);

		public static readonly BindableProperty StrokeProperty =
			BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(Layout), null);

		public static readonly BindableProperty StrokeThicknessProperty =
			BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(Layout), 1.0, propertyChanged: StrokeThicknessChanged);

		public static readonly BindableProperty StrokeDashArrayProperty =
			BindableProperty.Create(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(Layout), null,
				defaultValueCreator: bindable => new DoubleCollection());

		public static readonly BindableProperty StrokeDashOffsetProperty =
			BindableProperty.Create(nameof(StrokeDashOffset), typeof(double), typeof(Layout), 0.0);

		public static readonly BindableProperty StrokeLineCapProperty =
			BindableProperty.Create(nameof(StrokeLineCap), typeof(PenLineCap), typeof(Layout), PenLineCap.Flat);

		public static readonly BindableProperty StrokeLineJoinProperty =
			BindableProperty.Create(nameof(StrokeLineJoin), typeof(PenLineJoin), typeof(Layout), PenLineJoin.Miter);

		public static readonly BindableProperty StrokeMiterLimitProperty =
			BindableProperty.Create(nameof(StrokeMiterLimit), typeof(double), typeof(Layout), 10.0);

		[System.ComponentModel.TypeConverter(typeof(StrokeShapeTypeConverter))]
		public IShape? StrokeShape
		{
			set { SetValue(StrokeShapeProperty, value); }
			get { return (IShape?)GetValue(StrokeShapeProperty); }
		}

		public Brush? Stroke
		{
			set { SetValue(StrokeProperty, value); }
			get { return (Brush?)GetValue(StrokeProperty); }
		}

		public double StrokeThickness
		{
			set { SetValue(StrokeThicknessProperty, value); }
			get { return (double)GetValue(StrokeThicknessProperty); }
		}

		public DoubleCollection? StrokeDashArray
		{
			set { SetValue(StrokeDashArrayProperty, value); }
			get { return (DoubleCollection?)GetValue(StrokeDashArrayProperty); }
		}

		public double StrokeDashOffset
		{
			set { SetValue(StrokeDashOffsetProperty, value); }
			get { return (double)GetValue(StrokeDashOffsetProperty); }
		}

		public PenLineCap StrokeLineCap
		{
			set { SetValue(StrokeLineCapProperty, value); }
			get { return (PenLineCap)GetValue(StrokeLineCapProperty); }
		}

		public PenLineJoin StrokeLineJoin
		{
			set { SetValue(StrokeLineJoinProperty, value); }
			get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
		}

		public double StrokeMiterLimit
		{
			set { SetValue(StrokeMiterLimitProperty, value); }
			get { return (double)GetValue(StrokeMiterLimitProperty); }
		}

		IShape? IBorderStroke.Shape => StrokeShape;

		Paint? IStroke.Stroke => Stroke;

		LineCap IStroke.StrokeLineCap =>
			StrokeLineCap switch
			{
				PenLineCap.Flat => LineCap.Butt,
				PenLineCap.Round => LineCap.Round,
				PenLineCap.Square => LineCap.Square,
				_ => LineCap.Butt
			};

		LineJoin IStroke.StrokeLineJoin =>
			StrokeLineJoin switch
			{
				PenLineJoin.Round => LineJoin.Round,
				PenLineJoin.Bevel => LineJoin.Bevel,
				PenLineJoin.Miter => LineJoin.Miter,
				_ => LineJoin.Round
			};

		public float[]? StrokeDashPattern => StrokeDashArray?.ToFloatArray();

		float IStroke.StrokeDashOffset => (float)StrokeDashOffset;

		float IStroke.StrokeMiterLimit => (float)StrokeMiterLimit;


		object? IContentView.Content => Content;

		IView? IContentView.PresentedContent => Content;

		public Size CrossPlatformArrange(Graphics.Rect bounds)
		{
			bounds = bounds.Inset(StrokeThickness / 2);
			this.ArrangeContent(bounds);
			return bounds.Size;
		}

		public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
		{
			var inset = Padding + (StrokeThickness / 2);
			return this.MeasureContent(inset, widthConstraint, heightConstraint);
		}

		public static void ContentChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is Border border)
			{
				if (oldValue is Element oldElement)
					border.InternalChildren.Remove(oldElement);
				if (newValue is Element newElement)
					border.InternalChildren.Add(newElement);
			}

			((IBorderView)bindable).InvalidateMeasure();
		}

		public static void StrokeThicknessChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((IBorderView)bindable).InvalidateMeasure();
		}

		public void OnPaddingPropertyChanged(Thickness oldValue, Thickness newValue)
		{
			(this as IBorderView).InvalidateMeasure();
		}

		public Thickness PaddingDefaultValueCreator()
		{
			return Thickness.Zero;
		}
	}
}