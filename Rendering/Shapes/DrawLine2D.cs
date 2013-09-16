﻿using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Responsible for rendering all kinds of 2D lines (Line2D, Circle, etc)
	/// </summary>
	public class DrawLine2D : DrawBehavior
	{
		public DrawLine2D(Drawing draw)
		{
			this.draw = draw;
			material = new Material(Shader.Position2DColor, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		public void Draw(IEnumerable<DrawableEntity> entities)
		{
			foreach (var entity in entities)
				AddVerticesFromLine(entity);
			if (vertices.Count == 0)
				return;
			draw.AddLines(material, vertices.ToArray());
			vertices.Clear();
		}

		private void AddVerticesFromLine(DrawableEntity entity)
		{
			var color = entity.Get<Color>();
			var points = entity.GetInterpolatedList<Point>();
			foreach (Point point in points)
				vertices.Add(new VertexPosition2DColor(ScreenSpace.Current.ToPixelSpaceRounded(point),
					color));
		}

		private readonly List<VertexPosition2DColor> vertices = new List<VertexPosition2DColor>();
	}
}