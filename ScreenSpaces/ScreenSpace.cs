﻿using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.ScreenSpaces
{
	/// <summary>
	/// Converts to and from some kind of screen space like Quadratic, Pixel, etc.
	/// See http://deltaengine.net/learn/screenspace
	/// </summary>
	public abstract class ScreenSpace : IDisposable
	{
		public static ScreenSpace Current
		{
			get
			{
				if (current != null)
					return current;
				return current = resolver.ResolveScreenSpace<QuadraticScreenSpace>();
			}
		}

		private static ScreenSpace current;
		internal static ScreenSpaceResolver resolver;

		public static bool IsInitialized
		{
			get { return current != null; }
		}

		protected ScreenSpace(Window window)
		{
			this.window = window;
			viewportPixelSize = window.ViewportPixelSize;
			window.ViewportSizeChanged += Update;
			window.OrientationChanged += UpdateOrientationChanged;
			current = this;
		}

		private readonly Window window;
		protected Size viewportPixelSize;

		protected virtual void Update(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			if (ViewportSizeChanged != null)
				ViewportSizeChanged();
		}

		public event Action ViewportSizeChanged;

		private void UpdateOrientationChanged(Orientation orientation)
		{
			Update(viewportPixelSize);
		}

		public void Dispose()
		{
			window.ViewportSizeChanged -= Update;
			window.OrientationChanged -= UpdateOrientationChanged;
			current = null;
		}

		/// <summary>
		/// The rounded version of ToPixelSpace is used for lines, boxes and fonts where it matters to
		/// actually render at exact pixel positions to get sharp lines, boxes or font rendering.
		/// </summary>
		public Point ToPixelSpaceRounded(Point quadraticPos)
		{
			Point pixelPos = ToPixelSpace(quadraticPos);
			return new Point((float)Math.Round(pixelPos.X + Epsilon),
				(float)Math.Round(pixelPos.Y + Epsilon));
		}

		/// <summary>
		/// Small value to make sure we always round up in ToPixelSpaceRounded for 0.5f or 0.499999f.
		/// </summary>
		private const float Epsilon = 0.001f;

		public abstract Point ToPixelSpace(Point currentScreenSpacePos);

		public abstract Size ToPixelSpace(Size currentScreenSpaceSize);

		public Rectangle ToPixelSpace(Rectangle quadraticRect)
		{
			return new Rectangle(ToPixelSpace(quadraticRect.TopLeft), ToPixelSpace(quadraticRect.Size));
		}

		public abstract Point FromPixelSpace(Point pixelPosition);

		public abstract Size FromPixelSpace(Size pixelSize);

		public Rectangle FromPixelSpace(Rectangle quadraticRect)
		{
			return new Rectangle(FromPixelSpace(quadraticRect.TopLeft),
				FromPixelSpace(quadraticRect.Size));
		}

		public abstract Point TopLeft { get; }

		public abstract Point BottomRight { get; }

		public abstract float Left { get; }

		public abstract float Top { get; }

		public abstract float Right { get; }

		public abstract float Bottom { get; }

		public Rectangle Viewport
		{
			get { return new Rectangle(Left, Top, Right - Left, Bottom - Top); }
		}

		public float AspectRatio
		{
			get { return (Bottom - Top) / (Right - Left); }
		}

		public abstract Point GetInnerPoint(Point relativePoint);
	}
}