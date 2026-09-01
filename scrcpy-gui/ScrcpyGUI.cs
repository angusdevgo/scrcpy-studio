using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScrcpyStudio
{
    #region 全局设计规范 (Linear Pro Studio & Cyber Dark Aesthetic)
    public static class UITheme
    {
        // 核心背景色系 (沉浸式深空冷灰)
        public static readonly Color WindowBg       = Color.FromArgb(13, 15, 20);   // #0D0F14 主工作区背景
        public static readonly Color SidebarBg      = Color.FromArgb(9, 10, 14);    // #090A0E 沉浸侧边栏
        public static readonly Color TitleBarBg     = Color.FromArgb(9, 10, 14);    // 标题栏背景
        public static readonly Color CardBg         = Color.FromArgb(19, 22, 29);   // #13161D 大气浮动卡片
        public static readonly Color CardHoverBg    = Color.FromArgb(25, 29, 38);   // 卡片轻悬停
        public static readonly Color CardBorder     = Color.FromArgb(32, 37, 48);   // #202530 极细微光描边
        public static readonly Color InnerBorder    = Color.FromArgb(24, 27, 36);   // 分割线
        
        // 控件输入与小区块
        public static readonly Color InputBg        = Color.FromArgb(14, 16, 22);   // #0E1016 输入框底色
        public static readonly Color InputBorder    = Color.FromArgb(38, 44, 58);   // #262C3A 边框
        public static readonly Color InputFocus     = Color.FromArgb(0, 225, 170);  // 聚焦高光

        // 核心品牌点缀（赛博极光青 / 霓虹绿）
        public static readonly Color Accent         = Color.FromArgb(0, 225, 170);  // #00E1AA 极光青
        public static readonly Color AccentHover    = Color.FromArgb(40, 245, 195);
        public static readonly Color AccentActive   = Color.FromArgb(0, 190, 140);
        public static readonly Color PrimaryText    = Color.FromArgb(8, 16, 12);

        // 状态功能色
        public static readonly Color Success        = Color.FromArgb(16, 217, 137); // #10D989
        public static readonly Color Danger         = Color.FromArgb(244, 63, 94);  // #F43F5E 霓虹玫瑰红
        public static readonly Color DangerHover    = Color.FromArgb(251, 113, 133);
        public static readonly Color Warning        = Color.FromArgb(245, 158, 11);

        // 文字排版
        public static readonly Color TextWhite      = Color.FromArgb(255, 255, 255);
        public static readonly Color TextPrimary    = Color.FromArgb(240, 243, 248);
        public static readonly Color TextSecondary  = Color.FromArgb(150, 160, 175);
        public static readonly Color TextMuted      = Color.FromArgb(95, 105, 120);
        public static readonly Color TextDisabled   = Color.FromArgb(65, 72, 85);

        // 字体层次规范
        public static Font FontPageTitle   = new Font("Segoe UI", 15F, FontStyle.Bold);
        public static Font FontPageSub     = new Font("Microsoft YaHei UI", 9.5F, FontStyle.Regular);
        public static Font FontCardHeader  = new Font("Microsoft YaHei UI", 11.5F, FontStyle.Bold);
        public static Font FontItemTitle   = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
        public static Font FontItemSub     = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);
        public static Font FontBody        = new Font("Microsoft YaHei UI", 9.5F, FontStyle.Regular);
        public static Font FontBodyBold    = new Font("Microsoft YaHei UI", 9.5F, FontStyle.Bold);
        public static Font FontMono        = new Font("Consolas", 10F, FontStyle.Regular);
        public static Font FontSmall       = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular);
    }
    #endregion

    #region 高清自绘组件库

    // 1. 宽幅沉浸圆角卡片
    public class StudioCard : Panel
    {
        public int Radius { get; set; }
        public Color BorderColor { get; set; }

        public StudioCard()
        {
            Radius = 10;
            BorderColor = UITheme.CardBorder;
            BackColor = UITheme.CardBg;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath path = Geometry.RoundedRect(rect, Radius))
                using (SolidBrush brush = new SolidBrush(BackColor))
                using (Pen pen = new Pen(BorderColor, 1.2f))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
            }
            catch { }
        }
    }

    // 2. 自绘暗黑下拉框 StudioComboBox
    public class StudioComboBox : Control
    {
        private List<string> _items = new List<string>();
        private int _selectedIndex = -1;
        private bool _isHovered = false;
        private bool _isDropped = false;
        private Form _dropForm = null;
        private ListBox _dropListBox = null;

        public event EventHandler SelectedIndexChanged;

        public List<string> Items { get { return _items; } }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value && value >= -1 && value < _items.Count)
                {
                    _selectedIndex = value;
                    Invalidate();
                    if (SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty);
                }
            }
        }

        public string SelectedItem
        {
            get { return (_selectedIndex >= 0 && _selectedIndex < _items.Count) ? _items[_selectedIndex] : null; }
            set
            {
                int idx = _items.IndexOf(value);
                if (idx >= 0) SelectedIndex = idx;
            }
        }

        public StudioComboBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Size = new Size(340, 38);
            Font = UITheme.FontBody;
            Cursor = Cursors.Hand;
            BackColor = UITheme.InputBg;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            ToggleDropdown();
        }

        private void ToggleDropdown()
        {
            if (_isDropped)
            {
                CloseDropdown();
                return;
            }

            if (_items.Count == 0) return;

            _dropForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                BackColor = UITheme.CardBorder,
                Padding = new Padding(1)
            };

            int itemHeight = 34;
            int dropHeight = Math.Min(_items.Count * itemHeight + 6, 240);
            Point screenPt = PointToScreen(new Point(0, Height + 2));

            _dropListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.CardBg,
                ForeColor = UITheme.TextPrimary,
                Font = this.Font,
                BorderStyle = BorderStyle.None,
                ItemHeight = itemHeight,
                DrawMode = DrawMode.OwnerDrawFixed,
                IntegralHeight = false
            };

            foreach (var it in _items) _dropListBox.Items.Add(it);
            if (_selectedIndex >= 0 && _selectedIndex < _dropListBox.Items.Count)
                _dropListBox.SelectedIndex = _selectedIndex;

            _dropListBox.DrawItem += (s, e) =>
            {
                if (e.Index < 0 || e.Index >= _items.Count) return;
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                bool isSel = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                Color bg = isSel ? Color.FromArgb(28, 34, 46) : UITheme.CardBg;
                Color fg = isSel ? UITheme.Accent : UITheme.TextPrimary;

                using (SolidBrush b = new SolidBrush(bg))
                    g.FillRectangle(b, e.Bounds);

                if (isSel)
                {
                    using (SolidBrush barB = new SolidBrush(UITheme.Accent))
                        g.FillRectangle(barB, e.Bounds.X + 2, e.Bounds.Y + 4, 3, e.Bounds.Height - 8);
                }

                Rectangle textRect = new Rectangle(e.Bounds.X + 14, e.Bounds.Y, e.Bounds.Width - 18, e.Bounds.Height);
                TextRenderer.DrawText(g, _items[e.Index], this.Font, textRect, fg, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            };

            _dropListBox.Click += (s, e) =>
            {
                if (_dropListBox.SelectedIndex >= 0)
                {
                    int chosen = _dropListBox.SelectedIndex;
                    CloseDropdown();
                    this.SelectedIndex = chosen;
                }
                else
                {
                    CloseDropdown();
                }
            };

            _dropForm.Deactivate += (s, e) =>
            {
                // 延迟关闭，避免与 Click 事件重入造成空指针
                this.BeginInvoke(new Action(() => CloseDropdown()));
            };

            _dropForm.Controls.Add(_dropListBox);
            _dropForm.Size = new Size(this.Width, dropHeight);
            _dropForm.Location = screenPt;

            _isDropped = true;
            _dropForm.Show();
            _dropListBox.Focus();
        }

        private void CloseDropdown()
        {
            try
            {
                if (_dropForm != null)
                {
                    Form f = _dropForm;
                    _dropForm = null;
                    _dropListBox = null;
                    if (!f.IsDisposed)
                    {
                        f.Close();
                        f.Dispose();
                    }
                }
            }
            catch { }
            finally
            {
                _isDropped = false;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                Color border = (_isDropped || _isHovered) ? UITheme.Accent : UITheme.InputBorder;
                Color bg = _isHovered ? Color.FromArgb(20, 24, 32) : UITheme.InputBg;

                using (GraphicsPath path = Geometry.RoundedRect(rect, 6))
                using (SolidBrush brush = new SolidBrush(bg))
                using (Pen pen = new Pen(border, 1.2f))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }

                string displayText = (_selectedIndex >= 0 && _selectedIndex < _items.Count) ? _items[_selectedIndex] : "请选择...";
                Rectangle textRect = new Rectangle(14, 0, Width - 36, Height);
                TextRenderer.DrawText(g, displayText, Font, textRect, UITheme.TextPrimary, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);

                int arrowX = Width - 20;
                int arrowY = Height / 2 - 2;
                using (Pen arrowPen = new Pen(_isHovered ? UITheme.Accent : UITheme.TextSecondary, 1.6f))
                {
                    arrowPen.StartCap = LineCap.Round;
                    arrowPen.EndCap = LineCap.Round;
                    if (_isDropped)
                    {
                        g.DrawLine(arrowPen, arrowX, arrowY + 4, arrowX + 4, arrowY);
                        g.DrawLine(arrowPen, arrowX + 4, arrowY, arrowX + 8, arrowY + 4);
                    }
                    else
                    {
                        g.DrawLine(arrowPen, arrowX, arrowY, arrowX + 4, arrowY + 4);
                        g.DrawLine(arrowPen, arrowX + 4, arrowY + 4, arrowX + 8, arrowY);
                    }
                }
            }
            catch { }
        }
    }

    // 3. 现代交互按钮 StudioButton
    public class StudioButton : Control
    {
        public enum ButtonStyle { Accent, Danger, Secondary, Ghost }

        private bool _isHovered = false;
        private bool _isPressed = false;

        public ButtonStyle Style { get; set; }
        public int Radius { get; set; }

        public StudioButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
            Font = UITheme.FontBodyBold;
            ForeColor = UITheme.TextWhite;
            Radius = 6;
            Style = ButtonStyle.Accent;
            Size = new Size(130, 40);
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                Color bg = UITheme.Accent;
                Color fg = UITheme.PrimaryText;
                Color border = Color.Transparent;

                if (!Enabled)
                {
                    bg = Color.FromArgb(24, 28, 36);
                    fg = UITheme.TextDisabled;
                    border = Color.FromArgb(34, 38, 48);
                }
                else
                {
                    switch (Style)
                    {
                        case ButtonStyle.Accent:
                            bg = _isPressed ? UITheme.AccentActive : (_isHovered ? UITheme.AccentHover : UITheme.Accent);
                            fg = Color.FromArgb(8, 16, 12);
                            break;
                        case ButtonStyle.Danger:
                            bg = _isPressed ? Color.FromArgb(200, 30, 60) : (_isHovered ? UITheme.DangerHover : UITheme.Danger);
                            fg = Color.White;
                            break;
                        case ButtonStyle.Secondary:
                            bg = _isPressed ? Color.FromArgb(28, 32, 42) : (_isHovered ? Color.FromArgb(24, 28, 38) : Color.FromArgb(16, 19, 26));
                            border = _isHovered ? UITheme.Accent : UITheme.InputBorder;
                            fg = _isHovered ? UITheme.TextWhite : UITheme.TextPrimary;
                            break;
                        case ButtonStyle.Ghost:
                            bg = _isPressed ? Color.FromArgb(30, 36, 48) : (_isHovered ? Color.FromArgb(22, 26, 34) : Color.Transparent);
                            fg = _isHovered ? UITheme.TextWhite : UITheme.TextSecondary;
                            break;
                    }
                }

                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath path = Geometry.RoundedRect(rect, Radius))
                {
                    using (SolidBrush brush = new SolidBrush(bg))
                        g.FillPath(brush, path);

                    if (border != Color.Transparent)
                    {
                        using (Pen pen = new Pen(border, 1.2f))
                            g.DrawPath(pen, path);
                    }
                }

                TextRenderer.DrawText(g, Text, Font, ClientRectangle, fg, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            catch { }
        }
    }

    // 4. 极简青光胶囊开关 StudioToggle
    public class StudioToggle : Control
    {
        private bool _checked = false;
        public bool Checked
        {
            get { return _checked; }
            set { if (_checked != value) { _checked = value; Invalidate(); if (CheckedChanged != null) CheckedChanged(this, EventArgs.Empty); } }
        }

        public event EventHandler CheckedChanged;

        public StudioToggle()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(44, 24);
            Cursor = Cursors.Hand;
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                Color bg = _checked ? UITheme.Accent : Color.FromArgb(28, 33, 44);
                Color border = _checked ? UITheme.AccentHover : Color.FromArgb(46, 54, 70);

                using (GraphicsPath path = Geometry.Pill(rect))
                using (SolidBrush brush = new SolidBrush(bg))
                using (Pen pen = new Pen(border, 1.2f))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }

                int thumbSize = Height - 6;
                int thumbX = _checked ? (Width - thumbSize - 3) : 3;
                Rectangle thumbRect = new Rectangle(thumbX, 3, thumbSize, thumbSize);

                Color thumbColor = _checked ? Color.FromArgb(8, 16, 12) : Color.FromArgb(195, 202, 215);
                using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
                {
                    g.FillEllipse(thumbBrush, thumbRect);
                }
            }
            catch { }
        }
    }

    // 5. 侧边栏导航 Tab
    public class StudioNavTab : Control
    {
        public enum TabIconType { Device, Video, Tools, Engine }

        private bool _isSelected = false;
        private bool _isHovered = false;

        public string TabName { get; set; }
        public TabIconType IconType { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; Invalidate(); }
        }

        public StudioNavTab(TabIconType iconType, string name)
        {
            IconType = iconType;
            TabName = name;
            Size = new Size(216, 48);
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                Color bg = Color.Transparent;
                Color fg = UITheme.TextSecondary;
                Color iconColor = UITheme.TextSecondary;

                if (_isSelected)
                {
                    bg = Color.FromArgb(20, 24, 34);
                    fg = UITheme.TextWhite;
                    iconColor = UITheme.Accent;
                }
                else if (_isHovered)
                {
                    bg = Color.FromArgb(14, 17, 24);
                    fg = UITheme.TextPrimary;
                    iconColor = UITheme.TextPrimary;
                }

                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath path = Geometry.RoundedRect(rect, 8))
                using (SolidBrush brush = new SolidBrush(bg))
                {
                    g.FillPath(brush, path);
                }

                if (_isSelected)
                {
                    using (GraphicsPath barPath = Geometry.RoundedRect(new Rectangle(3, 10, 4, Height - 20), 1))
                    using (SolidBrush barBrush = new SolidBrush(UITheme.Accent))
                    {
                        g.FillPath(barBrush, barPath);
                    }
                }

                DrawVectorIcon(g, IconType, 18, 15, iconColor);

                Rectangle textRect = new Rectangle(52, 0, Width - 56, Height);
                TextRenderer.DrawText(g, TabName, _isSelected ? UITheme.FontBodyBold : UITheme.FontBody, textRect, fg, 
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
            catch { }
        }

        private void DrawVectorIcon(Graphics g, TabIconType type, int x, int y, Color color)
        {
            using (Pen pen = new Pen(color, 1.8f))
            using (SolidBrush brush = new SolidBrush(color))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                switch (type)
                {
                    case TabIconType.Device:
                        using (GraphicsPath p = Geometry.RoundedRect(new Rectangle(x + 2, y, 12, 17), 2))
                            g.DrawPath(pen, p);
                        g.FillEllipse(brush, x + 7f, y + 13.5f, 2, 2);
                        break;

                    case TabIconType.Video:
                        using (GraphicsPath p = Geometry.RoundedRect(new Rectangle(x, y + 1, 18, 12), 2))
                            g.DrawPath(pen, p);
                        g.DrawLine(pen, x + 5, y + 17, x + 13, y + 17);
                        g.DrawLine(pen, x + 9, y + 13, x + 9, y + 17);
                        break;

                    case TabIconType.Tools:
                        using (GraphicsPath p = Geometry.RoundedRect(new Rectangle(x + 1, y + 4, 16, 12), 2))
                            g.DrawPath(pen, p);
                        using (GraphicsPath h = Geometry.RoundedRect(new Rectangle(x + 6, y + 1, 6, 4), 1))
                            g.DrawPath(pen, h);
                        break;

                    case TabIconType.Engine:
                        g.DrawEllipse(pen, x + 2, y + 2, 14, 14);
                        g.FillEllipse(brush, x + 7.5f, y + 7.5f, 3, 3);
                        g.DrawLine(pen, x + 9, y, x + 9, y + 3);
                        g.DrawLine(pen, x + 9, y + 15, x + 9, y + 18);
                        g.DrawLine(pen, x, y + 9, x + 3, y + 9);
                        g.DrawLine(pen, x + 15, y + 9, x + 18, y + 9);
                        break;
                }
            }
        }
    }

    public static class Geometry
    {
        public static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            if (radius < 1) radius = 1;
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static GraphicsPath Pill(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int d = rect.Height;
            if (d < 1) d = 1;
            path.AddArc(rect.X, rect.Y, d, d, 90, 180);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 180);
            path.CloseFigure();
            return path;
        }
    }

    #endregion

    #region 主窗体 (1180×740 旗舰级宽屏 Studio)

    public class MainForm : Form
    {
        private string scrcpyExePath = "";
        private string adbExePath = "";
        private Process scrcpyProcess = null;

        private Panel customTitleBar;
        private Panel sidebarPanel;
        private Panel contentWrapper;
        private List<StudioNavTab> navTabs = new List<StudioNavTab>();

        private Panel pageDashboard;
        private Panel pageDisplay;

        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Label lblDeviceBadge;
        private Label lblDeviceSerial;
        private Label lblDeviceSubInfo;
        private StudioComboBox comboDevices;
        private StudioButton btnRefreshDev;
        private TextBox txtWirelessIp;
        private StudioButton btnWirelessConnect;

        private StudioToggle togScreenOff;
        private StudioToggle togAlwaysOnTop;
        private StudioToggle togStayAwake;
        private StudioToggle togAudio;
        private StudioToggle togBorderless;
        private StudioToggle togTouches;

        private StudioButton btnLaunchPrimary;
        private StudioButton btnStopPrimary;

        private StudioComboBox comboResolution;
        private StudioComboBox comboFps;
        private StudioComboBox comboBitrate;
        private StudioComboBox comboCodec;



        private Label lblGlobalStatus;
        private Label lblStatusDot;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;



        public MainForm()
        {
            InitWindowArchitecture();
            DetectScrcpyEngine();
            BuildCustomTitleBar();
            BuildSidebar();
            BuildContentPages();
            SelectNavTab(0);
            RefreshDeviceList();
        }

        private void InitWindowArchitecture()
        {
            AutoScaleMode = AutoScaleMode.None;
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(1180, 740);
            MinimumSize = new Size(1180, 740);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = UITheme.WindowBg;
            Font = UITheme.FontBody;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void DetectScrcpyEngine()
        {
            string cfgFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scrcpy_gui.cfg");
            if (File.Exists(cfgFile))
            {
                try
                {
                    string saved = File.ReadAllText(cfgFile).Trim();
                    if (File.Exists(saved)) { SetCorePath(saved); return; }
                }
                catch { }
            }

            string[] candidates = new string[]
            {
                @"D:\Data\Scrcpy\scrcpy.exe",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scrcpy.exe"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\scrcpy.exe"),
                @"C:\Data\Scrcpy\scrcpy.exe",
                @"C:\scrcpy\scrcpy.exe"
            };

            foreach (string p in candidates)
            {
                if (File.Exists(p)) { SetCorePath(Path.GetFullPath(p)); return; }
            }
        }

        private void SetCorePath(string path)
        {
            scrcpyExePath = path;
            string dir = Path.GetDirectoryName(path);
            string adb = Path.Combine(dir, "adb.exe");
            adbExePath = File.Exists(adb) ? adb : "adb";
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scrcpy_gui.cfg"), path);
            }
            catch { }
        }

        private void BuildCustomTitleBar()
        {
            customTitleBar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1180, 42),
                BackColor = UITheme.TitleBarBg
            };
            customTitleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            Label title = new Label
            {
                Text = "Scrcpy Studio · 极光现代化控制工作台",
                Font = UITheme.FontBodyBold,
                ForeColor = UITheme.TextSecondary,
                BackColor = UITheme.TitleBarBg,
                Location = new Point(20, 11),
                AutoSize = true
            };
            title.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            StudioButton btnMin = new StudioButton
            {
                Text = "—",
                Style = StudioButton.ButtonStyle.Ghost,
                Location = new Point(1088, 4),
                Size = new Size(42, 34),
                Radius = 4
            };
            btnMin.Click += (s, e) => WindowState = FormWindowState.Minimized;

            StudioButton btnClose = new StudioButton
            {
                Text = "✕",
                Style = StudioButton.ButtonStyle.Ghost,
                Location = new Point(1132, 4),
                Size = new Size(42, 34),
                Radius = 4
            };
            btnClose.Click += (s, e) => Application.Exit();

            customTitleBar.Controls.AddRange(new Control[] { title, btnMin, btnClose });
            Controls.Add(customTitleBar);
        }

        private void BuildSidebar()
        {
            sidebarPanel = new Panel
            {
                Location = new Point(0, 42),
                Size = new Size(240, 698),
                BackColor = UITheme.SidebarBg
            };

            Panel brand = new Panel { Location = new Point(16, 16), Size = new Size(208, 54), BackColor = UITheme.SidebarBg };
            Label lblLogo = new Label
            {
                Text = "SCRCPY PRO",
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                ForeColor = UITheme.TextWhite,
                BackColor = UITheme.SidebarBg,
                Location = new Point(2, 2),
                AutoSize = true
            };
            Label lblVer = new Label
            {
                Text = "STUDIO ENTERPRISE EDITION",
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                ForeColor = UITheme.Accent,
                BackColor = UITheme.SidebarBg,
                Location = new Point(3, 28),
                AutoSize = true
            };
            brand.Controls.AddRange(new Control[] { lblLogo, lblVer });
            sidebarPanel.Controls.Add(brand);

            Panel line = new Panel { Location = new Point(16, 78), Size = new Size(208, 1), BackColor = UITheme.InnerBorder };
            sidebarPanel.Controls.Add(line);

            int startY = 94;
            StudioNavTab.TabIconType[] icons = new StudioNavTab.TabIconType[] {
                StudioNavTab.TabIconType.Device,
                StudioNavTab.TabIconType.Video
            };
            string[] names = new string[] { "投屏工作台", "画质与帧率" };

            for (int i = 0; i < 2; i++)
            {
                int index = i;
                StudioNavTab tab = new StudioNavTab(icons[i], names[i])
                {
                    Location = new Point(12, startY),
                    Width = 216,
                    Height = 48
                };
                tab.Click += (s, e) => SelectNavTab(index);
                sidebarPanel.Controls.Add(tab);
                navTabs.Add(tab);
                startY += 56;
            }

            Panel bottomStatus = new Panel { Location = new Point(16, 642), Size = new Size(208, 40), BackColor = UITheme.SidebarBg };
            lblStatusDot = new Label
            {
                Text = "●",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = UITheme.Success,
                BackColor = UITheme.SidebarBg,
                Location = new Point(4, 10),
                AutoSize = true
            };
            lblGlobalStatus = new Label
            {
                Text = "工作台就绪",
                Font = UITheme.FontItemSub,
                ForeColor = UITheme.TextSecondary,
                BackColor = UITheme.SidebarBg,
                Location = new Point(20, 10),
                AutoSize = true
            };
            bottomStatus.Controls.AddRange(new Control[] { lblStatusDot, lblGlobalStatus });
            sidebarPanel.Controls.Add(bottomStatus);

            Controls.Add(sidebarPanel);
        }

        private void BuildContentPages()
        {
            contentWrapper = new Panel
            {
                Location = new Point(240, 42),
                Size = new Size(940, 698),
                BackColor = UITheme.WindowBg
            };

            Panel headerPnl = new Panel { Location = new Point(32, 18), Size = new Size(876, 56), BackColor = UITheme.WindowBg };
            lblHeaderTitle = new Label
            {
                Text = "投屏控制工作台",
                Font = UITheme.FontPageTitle,
                ForeColor = UITheme.TextWhite,
                BackColor = UITheme.WindowBg,
                Location = new Point(0, 0),
                AutoSize = true
            };
            lblHeaderSubtitle = new Label
            {
                Text = "实时监控已连接的 Android 调试设备并启动零延迟超清投屏",
                Font = UITheme.FontPageSub,
                ForeColor = UITheme.TextSecondary,
                BackColor = UITheme.WindowBg,
                Location = new Point(2, 32),
                AutoSize = true
            };
            headerPnl.Controls.AddRange(new Control[] { lblHeaderTitle, lblHeaderSubtitle });
            contentWrapper.Controls.Add(headerPnl);

            pageDashboard = CreatePageDashboard();
            pageDisplay = CreatePageDisplay();

            contentWrapper.Controls.AddRange(new Control[] { pageDashboard, pageDisplay });
            Controls.Add(contentWrapper);
        }

        private void SelectNavTab(int index)
        {
            for (int i = 0; i < navTabs.Count; i++)
                navTabs[i].IsSelected = (i == index);

            pageDashboard.Visible = (index == 0);
            pageDisplay.Visible = (index == 1);

            string[] titles = { "投屏控制工作台", "画质与性能调校" };
            string[] subs = {
                "实时监控已连接的 Android 调试设备并启动零延迟超清投屏",
                "自定义最高分辨率、极限 120 FPS 帧率以及下一代硬件编解码方案"
            };

            lblHeaderTitle.Text = titles[index];
            lblHeaderSubtitle.Text = subs[index];
        }

        private Panel CreatePageDashboard()
        {
            Panel pnl = new Panel { Location = new Point(32, 82), Size = new Size(876, 595), BackColor = UITheme.WindowBg };

            // 1. 设备连接状态卡片
            StudioCard cardDev = new StudioCard { Location = new Point(0, 0), Size = new Size(876, 130) };
            
            lblDeviceBadge = new Label
            {
                Text = "● 状态检测",
                Font = UITheme.FontSmall,
                ForeColor = UITheme.Accent,
                BackColor = UITheme.CardBg,
                Location = new Point(22, 16),
                AutoSize = true
            };
            lblDeviceSerial = new Label
            {
                Text = "正在扫描设备...",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = UITheme.TextWhite,
                BackColor = UITheme.CardBg,
                Location = new Point(22, 40),
                AutoSize = true
            };
            lblDeviceSubInfo = new Label
            {
                Text = "支持 USB 线连多设备切换及 TCP/IP 无线配对",
                Font = UITheme.FontItemSub,
                ForeColor = UITheme.TextMuted,
                BackColor = UITheme.CardBg,
                Location = new Point(22, 80),
                AutoSize = true
            };

            comboDevices = new StudioComboBox
            {
                Location = new Point(380, 24),
                Width = 310
            };

            btnRefreshDev = new StudioButton
            {
                Text = "刷新设备",
                Location = new Point(706, 23),
                Size = new Size(146, 38),
                Style = StudioButton.ButtonStyle.Secondary
            };
            btnRefreshDev.Click += (s, e) => RefreshDeviceList();

            Label lblIp = new Label { Text = "无线 IP 地址:", Location = new Point(380, 79), AutoSize = true, ForeColor = UITheme.TextSecondary, BackColor = UITheme.CardBg, Font = UITheme.FontBody };
            txtWirelessIp = new TextBox
            {
                Location = new Point(480, 76),
                Width = 210,
                Text = "192.168.1.",
                Font = UITheme.FontMono,
                BackColor = UITheme.InputBg,
                ForeColor = UITheme.TextWhite,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnWirelessConnect = new StudioButton
            {
                Text = "无线配对",
                Location = new Point(706, 73),
                Size = new Size(146, 38),
                Style = StudioButton.ButtonStyle.Accent
            };
            btnWirelessConnect.Click += BtnWirelessConnect_Click;

            cardDev.Controls.AddRange(new Control[] { lblDeviceBadge, lblDeviceSerial, lblDeviceSubInfo, comboDevices, btnRefreshDev, lblIp, txtWirelessIp, btnWirelessConnect });
            pnl.Controls.Add(cardDev);

            // 2. 快捷开关卡片 (双列舒展布局)
            StudioCard cardSw = new StudioCard { Location = new Point(0, 146), Size = new Size(876, 260) };
            Label lblSwTitle = new Label
            {
                Text = "快捷特性与渲染策略 (Quick Features)",
                Font = UITheme.FontCardHeader,
                ForeColor = UITheme.TextWhite,
                BackColor = UITheme.CardBg,
                Location = new Point(22, 18),
                AutoSize = true
            };
            cardSw.Controls.Add(lblSwTitle);

            int col1 = 22, col2 = 460;
            int rY1 = 60, rY2 = 124, rY3 = 188;

            AddSwitchItem(cardSw, col1, rY1, "投屏时自动息屏", "关闭手机屏幕降低发热并保护隐私", out togScreenOff, false);
            AddSwitchItem(cardSw, col1, rY2, "窗口置顶显示", "保持投屏窗口在所有应用程序最前端", out togAlwaysOnTop, false);
            AddSwitchItem(cardSw, col1, rY3, "无边框模式 (隐藏原生标题栏)", "隐藏顶部标题栏；可随时在【工具】页快捷重置窗口位置", out togBorderless, false);

            AddSwitchItem(cardSw, col2, rY1, "保持手机常亮", "投屏期间阻止移动设备自动休眠锁屏", out togStayAwake, true);
            AddSwitchItem(cardSw, col2, rY2, "音频实时转发", "将设备内部音频流低延迟同步至电脑", out togAudio, true);
            AddSwitchItem(cardSw, col2, rY3, "绘制物理触控点", "在屏幕上可视化呈现手指滑动轨迹", out togTouches, false);

            pnl.Controls.Add(cardSw);

            // 3. 底部大按钮
            btnLaunchPrimary = new StudioButton
            {
                Text = "启动超清投屏 (无控制台黑框模式)",
                Location = new Point(0, 422),
                Size = new Size(610, 60),
                Radius = 10,
                Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold),
                Style = StudioButton.ButtonStyle.Accent
            };
            btnLaunchPrimary.Click += BtnLaunchPrimary_Click;

            btnStopPrimary = new StudioButton
            {
                Text = "结束投屏",
                Location = new Point(626, 422),
                Size = new Size(250, 60),
                Radius = 10,
                Font = new Font("Microsoft YaHei UI", 11.5F, FontStyle.Bold),
                Style = StudioButton.ButtonStyle.Danger,
                Enabled = false
            };
            btnStopPrimary.Click += BtnStopPrimary_Click;

            pnl.Controls.AddRange(new Control[] { btnLaunchPrimary, btnStopPrimary });
            return pnl;
        }

        private void AddSwitchItem(Control parent, int x, int y, string title, string subtitle, out StudioToggle toggle, bool defState)
        {
            Label lblT = new Label
            {
                Text = title,
                Font = UITheme.FontItemTitle,
                ForeColor = UITheme.TextPrimary,
                BackColor = UITheme.CardBg,
                Location = new Point(x, y),
                Size = new Size(295, 20),
                AutoEllipsis = true
            };
            Label lblS = new Label
            {
                Text = subtitle,
                Font = UITheme.FontItemSub,
                ForeColor = UITheme.TextMuted,
                BackColor = UITheme.CardBg,
                Location = new Point(x, y + 24),
                Size = new Size(295, 18),
                AutoEllipsis = true
            };
            toggle = new StudioToggle
            {
                Location = new Point(x + 330, y + 8),
                Checked = defState
            };
            parent.Controls.AddRange(new Control[] { lblT, lblS, toggle });
        }

        private Panel CreatePageDisplay()
        {
            Panel pnl = new Panel { Location = new Point(32, 82), Size = new Size(876, 595), BackColor = UITheme.WindowBg };

            StudioCard card = new StudioCard { Location = new Point(0, 0), Size = new Size(876, 510) };

            int startY = 32, step = 114;

            comboResolution = new StudioComboBox { Location = new Point(480, startY + 8), Width = 360 };
            comboResolution.Items.AddRange(new string[] { "跟随设备原生分辨率 (默认最高清)", "1080P (1920×1080 均衡高清)", "720P (1280×720 极速流畅)", "480P (超低带宽模式)" });
            comboResolution.SelectedIndex = 0;
            AddSettingRow(card, 26, startY, "最大分辨率限制 (Max Size):", "降低投屏像素可极大程度减轻低配置 PC 与 Wi-Fi 的渲染负载", comboResolution);

            startY += step;
            comboFps = new StudioComboBox { Location = new Point(480, startY + 8), Width = 360 };
            comboFps.Items.AddRange(new string[] { "原生无限制 (推荐原汁原味流畅)", "120 FPS (强制限制 120)", "90 FPS (平滑高帧)", "60 FPS (标准限制)", "30 FPS (节能降耗)" });
            comboFps.SelectedIndex = 0;
            AddSettingRow(card, 26, startY, "显示刷新率上限 (Max FPS):", "默认不加参数跟随手机原生最高刷新率，避免硬件锁帧冲突", comboFps);

            startY += step;
            comboBitrate = new StudioComboBox { Location = new Point(480, startY + 8), Width = 360 };
            comboBitrate.Items.AddRange(new string[] { "原生默认码率 (8 Mbps 均衡)", "16 Mbps (超清推荐)", "32 Mbps (发烧级无损细节)", "48 Mbps (极客极限原画)", "4 Mbps (低网络占用)" });
            comboBitrate.SelectedIndex = 0;
            AddSettingRow(card, 26, startY, "视频传输码率 (Video Bitrate):", "若追求原生纯净体验保持默认即可，需要更高画质可提升码率", comboBitrate);

            startY += step;
            comboCodec = new StudioComboBox { Location = new Point(480, startY + 8), Width = 360 };
            comboCodec.Items.AddRange(new string[] { "原生默认编码器 (Auto / H.264)", "H.265 / HEVC (高画质低带宽推荐)", "AV1 (下一代开放标准)" });
            comboCodec.SelectedIndex = 0;
            AddSettingRow(card, 26, startY, "视频硬件编码器 (Video Codec):", "默认由 scrcpy 自动选择手机最稳定、延迟最低的硬件编码器", comboCodec);

            pnl.Controls.Add(card);
            return pnl;
        }

        private void AddSettingRow(Control parent, int x, int y, string title, string sub, Control control)
        {
            Label lblT = new Label
            {
                Text = title,
                Font = UITheme.FontItemTitle,
                ForeColor = UITheme.TextPrimary,
                BackColor = UITheme.CardBg,
                Location = new Point(x, y),
                AutoSize = true
            };
            Label lblS = new Label
            {
                Text = sub,
                Font = UITheme.FontItemSub,
                ForeColor = UITheme.TextMuted,
                BackColor = UITheme.CardBg,
                Location = new Point(x, y + 26),
                AutoSize = true
            };

            parent.Controls.AddRange(new Control[] { lblT, lblS, control });
        }



        private void RefreshDeviceList()
        {
            comboDevices.Items.Clear();
            if (string.IsNullOrEmpty(adbExePath))
            {
                UpdateGlobalStatus(false, "未检测到 ADB 路径");
                return;
            }

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(adbExePath, "devices")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                if (File.Exists(scrcpyExePath))
                    psi.WorkingDirectory = Path.GetDirectoryName(scrcpyExePath);

                using (Process p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("List") || string.IsNullOrWhiteSpace(line))
                            continue;
                        string[] parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 0)
                            comboDevices.Items.Add(parts[0]);
                    }
                }

                if (comboDevices.Items.Count > 0)
                {
                    comboDevices.SelectedIndex = 0;
                    lblDeviceSerial.Text = comboDevices.SelectedItem;
                    lblDeviceBadge.Text = string.Format("● 在线设备 ({0})", comboDevices.Items.Count);
                    lblDeviceBadge.ForeColor = UITheme.Accent;
                    UpdateGlobalStatus(true, "设备在线就绪");
                }
                else
                {
                    lblDeviceSerial.Text = "无设备连接";
                    lblDeviceBadge.Text = "● 未检测到设备";
                    lblDeviceBadge.ForeColor = UITheme.Danger;
                    UpdateGlobalStatus(false, "等待设备插入");
                }
            }
            catch (Exception ex)
            {
                UpdateGlobalStatus(false, "ADB 异常: " + ex.Message);
            }
        }

        private void BtnWirelessConnect_Click(object sender, EventArgs e)
        {
            string ip = txtWirelessIp.Text.Trim();
            if (string.IsNullOrEmpty(ip)) return;
            if (!ip.Contains(":")) ip += ":5555";

            UpdateGlobalStatus(false, "配对中...");
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(adbExePath, "connect " + ip)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                if (File.Exists(scrcpyExePath))
                    psi.WorkingDirectory = Path.GetDirectoryName(scrcpyExePath);

                using (Process p = Process.Start(psi))
                {
                    string outStr = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    MessageBox.Show(outStr, "无线配对结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshDeviceList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void BtnLaunchPrimary_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(scrcpyExePath) || !File.Exists(scrcpyExePath))
            {
                MessageBox.Show("未找到核心程序 scrcpy.exe，请在【引擎与环境】指定路径！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (scrcpyProcess != null && !scrcpyProcess.HasExited)
            {
                MessageBox.Show("投屏引擎已在运行中。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string args = "";

            if (comboDevices.SelectedItem != null)
                args += string.Format("-s {0} ", comboDevices.SelectedItem);

            if (comboResolution.SelectedIndex == 1) args += "--max-size 1080 ";
            else if (comboResolution.SelectedIndex == 2) args += "--max-size 720 ";
            else if (comboResolution.SelectedIndex == 3) args += "--max-size 480 ";

            if (comboFps.SelectedIndex == 1) args += "--max-fps 120 ";
            else if (comboFps.SelectedIndex == 2) args += "--max-fps 90 ";
            else if (comboFps.SelectedIndex == 3) args += "--max-fps 60 ";
            else if (comboFps.SelectedIndex == 4) args += "--max-fps 30 ";

            if (comboBitrate.SelectedIndex == 1) args += "--video-bit-rate 16M ";
            else if (comboBitrate.SelectedIndex == 2) args += "--video-bit-rate 32M ";
            else if (comboBitrate.SelectedIndex == 3) args += "--video-bit-rate 48M ";
            else if (comboBitrate.SelectedIndex == 4) args += "--video-bit-rate 4M ";

            if (comboCodec.SelectedIndex == 1) args += "--video-codec=h265 ";
            else if (comboCodec.SelectedIndex == 2) args += "--video-codec=av1 ";

            if (togScreenOff.Checked) args += "--turn-screen-off ";
            if (togAlwaysOnTop.Checked) args += "--always-on-top ";
            if (togStayAwake.Checked) args += "--stay-awake ";
            if (togBorderless.Checked) args += "--window-borderless ";
            if (togTouches.Checked) args += "--show-touches ";
            if (!togAudio.Checked) args += "--no-audio ";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(scrcpyExePath, args.Trim())
                {
                    WorkingDirectory = Path.GetDirectoryName(scrcpyExePath),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                scrcpyProcess = Process.Start(psi);
                scrcpyProcess.BeginOutputReadLine();
                scrcpyProcess.BeginErrorReadLine();
                scrcpyProcess.EnableRaisingEvents = true;

                scrcpyProcess.Exited += (s, ev) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        btnLaunchPrimary.Enabled = true;
                        btnStopPrimary.Enabled = false;
                        UpdateGlobalStatus(true, "工作台就绪");
                        scrcpyProcess = null;
                    }));
                };

                btnLaunchPrimary.Enabled = false;
                btnStopPrimary.Enabled = true;
                UpdateGlobalStatus(true, "投屏渲染中 (无黑框)");
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateGlobalStatus(false, "启动失败");
            }
        }

        private void BtnStopPrimary_Click(object sender, EventArgs e)
        {
            if (scrcpyProcess != null && !scrcpyProcess.HasExited)
            {
                try
                {
                    scrcpyProcess.Kill();
                    UpdateGlobalStatus(true, "已终止投屏");
                }
                catch { }
            }
        }

        private void UpdateGlobalStatus(bool ok, string text)
        {
            lblStatusDot.ForeColor = ok ? UITheme.Success : UITheme.Danger;
            lblGlobalStatus.Text = text;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
    #endregion
}
