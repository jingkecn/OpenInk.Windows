using System;
using System.Numerics;
using Microsoft.Graphics.Canvas.Geometry;
using MyScript.IInk.Graphics;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.InteractiveInk.UI.Implementations
{
    public sealed partial class Path
    {
        [CanBeNull] public CanvasPathBuilder PathBuilder { get; set; }

        [CanBeNull]
        public CanvasGeometry Geometry
        {
            get
            {
                if (PathBuilder == null)
                {
                    return null;
                }

                if (!IsInFigure)
                {
                    return CanvasGeometry.CreatePath(PathBuilder);
                }

                PathBuilder.EndFigure(CanvasFigureLoop.Open);
                IsInFigure = false;

                return CanvasGeometry.CreatePath(PathBuilder);
            }
        }

        private bool IsInFigure { get; set; }
    }

    /// <summary>
    ///     Implements <see cref="IPath" />.
    ///     <inheritdoc cref="IPath" />
    /// </summary>
    public sealed partial class Path : IPath
    {
        public void MoveTo(float x, float y)
        {
            if (IsInFigure)
            {
                PathBuilder?.EndFigure(CanvasFigureLoop.Open);
            }

            PathBuilder?.BeginFigure(x, y);
            IsInFigure = true;
        }

        public void LineTo(float x, float y)
        {
            PathBuilder?.AddLine(x, y);
        }

        public void CurveTo(float x1, float y1, float x2, float y2, float x, float y)
        {
            PathBuilder?.AddCubicBezier(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x, y));
        }

        public void QuadTo(float x1, float y1, float x, float y)
        {
            PathBuilder?.AddQuadraticBezier(new Vector2(x1, y1), new Vector2(x, y));
        }

        public void ArcTo(float rx, float ry, float phi, bool fA, bool fS, float x, float y)
        {
            throw new NotImplementedException();
        }

        public void ClosePath()
        {
            PathBuilder?.EndFigure(CanvasFigureLoop.Closed);
            IsInFigure = false;
        }

        public uint UnsupportedOperations => (uint)PathOperation.ARC_OPS;
    }
}
