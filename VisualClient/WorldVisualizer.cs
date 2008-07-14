using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ICFP08
{
    public partial class WorldVisualizer : Control
    {
        private struct Line
        {
            public Line(Vector2d start, Vector2d end, Pen pen)
            {
                this.start = start;
                this.end = end;
                this.pen = pen;
            }

            public Vector2d start;
            public Vector2d end;
            public Pen pen;
        }

        private struct Ellipse
        {
            public Ellipse(Rectangle rect, Brush brush)
            {
                this.rect = rect;
                this.brush = brush;
            }

            public Rectangle rect;
            public Brush brush;
        }

        private WorldState m_state = null;
        private Brush m_backBrush = null;
        private SolidBrush m_boulderBrush = new SolidBrush(Color.LightGray);
        private SolidBrush m_craterBrush = new SolidBrush(Color.Red);
        private SolidBrush m_homeBrush = new SolidBrush(Color.Green);
        private SolidBrush m_roverBrush = new SolidBrush(Color.Blue);
        private SolidBrush m_martianBrush = new SolidBrush(Color.Purple); // makes them less threatening

        private Pen m_gridPen = new Pen(Color.Black);
        private Pen m_borderPen = new Pen(Color.Black);
        private int m_gridSizeX = 20;
        private int m_gridSizeY = 20;
        private Bitmap m_staticBG;
        private Rectangle m_roverRect;
        private List<Rectangle> m_seenRects;
        private PointF m_offset;
        private SizeF m_scale;
        private List<Ellipse> m_debugEllipses = new List<Ellipse>();
        private List<Line> m_debugLines = new List<Line>();

        public delegate void MapClickedHandler(Vector2d position);
        public event MapClickedHandler MapClicked;

        public WorldState State
        {
            set
            {
                m_state = value;
                m_state.WorldChanged += new WorldState.WorldHandler(m_state_WorldChanged);
                m_state.BoulderFound += new WorldState.BoulderHandler(m_state_BoulderFound);
                m_state.CraterFound += new WorldState.CraterHandler(m_state_CraterFound);
                m_state.HomeFound += new WorldState.HomeHandler(m_state_HomeFound);
                m_roverRect = Rectangle.Empty;
                ComputeScale();
                UpdateBackground();
            }
        }

        void m_state_HomeFound(WorldState world, Home h)
        {
            AddHome(h);
        }

        void m_state_CraterFound(WorldState world, Crater c)
        {
            AddCrater(c);
        }

        void m_state_BoulderFound(WorldState world, Boulder b)
        {
            AddBoulder(b);
        }

        private void ComputeScale()
        {
            if (m_state != null)
            {
                m_offset = new PointF((float)m_state.Size.Width / 2.0f, (float)m_state.Size.Height / 2.0f);
                m_scale = new SizeF((float)Width / (float)m_state.Size.Width, (float)Height / (float)m_state.Size.Height);
            }
        }

        void m_state_WorldChanged(WorldState world)
        {
            this.Invalidate();
        }

        public WorldVisualizer()
        {
            InitializeComponent();
            m_backBrush = new SolidBrush(BackColor);
            m_gridPen = new Pen(GridColor);
            //m_staticBG = new Bitmap(Width, Height);
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                m_backBrush.Dispose();
                m_backBrush = new SolidBrush(value);
                UpdateBackground();
            }
        }

        [Category("Grid")]
        public Color GridColor
        {
            get
            {
                return m_gridPen.Color;
            }
            set
            {
                m_gridPen.Dispose();
                m_gridPen = new Pen(value);
                UpdateBackground();
            }
        }
        [Category("Grid")]
        public Size GridSize
        {
            get
            {
                return new Size(m_gridSizeX, m_gridSizeY);
            }
            set
            {
                m_gridSizeX = value.Width;
                m_gridSizeY = value.Height;
                UpdateBackground();
            }
        }
        [Category("Objects")]
        public Color BoulderColor
        {
            get
            {
                return m_boulderBrush.Color;
            }
            set
            {
                m_boulderBrush.Dispose();
                m_boulderBrush = new SolidBrush(value);
                UpdateBackground();
            }
        }
        [Category("Objects")]
        public Color CraterColor
        {
            get
            {
                return m_craterBrush.Color;
            }
            set
            {
                m_craterBrush.Dispose();
                m_craterBrush = new SolidBrush(value);
                UpdateBackground();
            }
        }
        [Category("Objects")]
        public Color HomeColor
        {
            get
            {
                return m_homeBrush.Color;
            }
            set
            {
                m_homeBrush.Dispose();
                m_homeBrush = new SolidBrush(value);
                UpdateBackground();
            }
        }
        [Category("Objects")]
        public Color RoverColor
        {
            get
            {
                return m_roverBrush.Color;
            }
            set
            {
                m_roverBrush.Dispose();
                m_roverBrush = new SolidBrush(value);
            }
        }
        [Category("Objects")]
        public Color MartianColor
        {
            get
            {
                return m_martianBrush.Color;
            }
            set
            {
                m_martianBrush.Dispose();
                m_martianBrush = new SolidBrush(value);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.DrawImageUnscaled(m_staticBG, 0, 0);
            if(m_state != null)
            {
                DrawRover(m_state.Rover, pe.Graphics);
                foreach (Martian m in m_state.Martians)
                    DrawMartian(m, pe.Graphics);
                foreach (Ellipse e in m_debugEllipses)
                {
                    pe.Graphics.FillEllipse(e.brush, e.rect);
                    pe.Graphics.DrawEllipse(m_borderPen, e.rect);
                }
                foreach (Line l in m_debugLines)
                    pe.Graphics.DrawLine(l.pen, l.start, l.end);
                m_debugLines.Clear();
                m_debugEllipses.Clear();
            }
            //base.OnPaint(pe);            
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if(m_staticBG != null)
                m_staticBG.Dispose();
            m_staticBG = new Bitmap(Width, Height);
            ComputeScale();
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            if (m_staticBG == null)
                return;

            using (Graphics g = Graphics.FromImage(m_staticBG))
            {
                g.FillRectangle(m_backBrush, 0, 0, Width, Height);
                g.DrawRectangle(m_gridPen, 0, 0, Width - 1, Height - 1);
                for (int i = 0; i < Width; i += m_gridSizeX)
                    g.DrawLine(m_gridPen, i, 0, i, Height);
                for (int i = 0; i < Height; i += m_gridSizeY)
                    g.DrawLine(m_gridPen, 0, i, Width, i);
            }
            if (m_state != null)
            {
                foreach (Boulder b in m_state.Boulders)
                    AddBoulder(b);
                foreach (Crater c in m_state.Craters)
                    AddCrater(c);
                if (m_state.Home != null)
                    AddHome(m_state.Home);
            }
            this.Invalidate();
        }

        private void AddHome(Home home)
        {
            using (Graphics g = Graphics.FromImage(m_staticBG))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                Rectangle rect = GetObjectRect(home);
                g.FillEllipse(m_homeBrush, rect);
                g.DrawEllipse(m_borderPen, rect);
            }
        }

        private void AddCrater(Crater c)
        {
            using (Graphics g = Graphics.FromImage(m_staticBG))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                Rectangle rect = GetObjectRect(c);
                g.FillEllipse(m_craterBrush, rect);
                g.DrawEllipse(m_borderPen, rect);
            }
        }

        private Vector2d PointToMap(Point p)
        {
            return new Vector2d(
                ((float)p.X / m_scale.Width) -  m_offset.X,
                -(((float)p.Y / m_scale.Height) -  m_offset.Y));
        }

        private PointF MapToPoint(Vector2d v)
        {
            return new PointF(
                (v.x + m_offset.X) * m_scale.Width,
                (-v.y + m_offset.Y) * m_scale.Height);
        }

        private Rectangle GetObjectRect(MarsObject obj)
        {
            PointF center = new PointF(
                (obj.Position.x + m_offset.X) * m_scale.Width, 
                ((obj.Position.y * -1.0f) + m_offset.Y) * m_scale.Height);

            Rectangle r = new Rectangle(
                (int)(center.X - (obj.Radius * m_scale.Width)), 
                (int)(center.Y - (obj.Radius * m_scale.Height)),
                (int)((obj.Radius * m_scale.Width) * 2), 
                (int)((obj.Radius * m_scale.Height) * 2));

            // make sure it is at least 1 px by 1px
            if (r.Width == 0)
                r.Width = 1;
            if (r.Height == 0)
                r.Height = 1;

            return r;
        }

        private Vector2d WorldToClient(Vector2d world)
        {
            return new Vector2d(
                (world.x + m_offset.X) * m_scale.Width,
                (-world.y + m_offset.Y) * m_scale.Height);
        }

        private void AddBoulder(Boulder b)
        {
            using (Graphics g = Graphics.FromImage(m_staticBG))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                Rectangle rect = GetObjectRect(b);
                g.FillEllipse(m_boulderBrush, rect);
                g.DrawEllipse(m_borderPen, rect);
            }
        }

        private void DrawRover(Rover r, Graphics g)
        {
            // cheat and draw the rover twice as large as it should be
            MarsObject obj = new MarsObject(r.Position, r.Radius * 4);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Rectangle rect = GetObjectRect(obj);
            g.FillEllipse(m_roverBrush, rect);
            g.DrawEllipse(m_borderPen, rect);
        }

        private void DrawMartian(Martian m, Graphics g)
        {
            Rectangle rect = GetObjectRect(m);
            g.FillEllipse(m_martianBrush, rect);
            g.DrawEllipse(m_borderPen, rect);
        }

        internal void DrawLine(Vector2d start, Vector2d end, Pen p)
        {
            start = WorldToClient(start);
            end = WorldToClient(end);

            if (p == Pens.Transparent)
            {
                using (Graphics g = Graphics.FromImage(m_staticBG))
                {
                    g.DrawLine(Pens.Blue, start, end);
                }
            }
            else
                m_debugLines.Add(new Line(start, end, p));
        }

        public void DrawEllipse(MarsObject obj, Brush b)
        {
            m_debugEllipses.Add(new Ellipse(GetObjectRect(obj), b));
        }

        private void WorldVisualizer_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if(MapClicked != null)
                    MapClicked(PointToMap(e.Location));
            }
        }
    }
}
