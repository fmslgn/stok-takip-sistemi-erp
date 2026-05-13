using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StokTakip.WinForms
{
    public static class ModernUi
    {
        public static readonly Color Bg = Color.FromArgb(255, 255, 255);
        public static readonly Color Surface = Color.FromArgb(248, 250, 252);
        public static readonly Color Card = Color.White;

        public static readonly Color Dark = Color.FromArgb(15, 23, 42);
        public static readonly Color Text = Color.FromArgb(30, 41, 59);
        public static readonly Color Muted = Color.FromArgb(100, 116, 139);
        public static readonly Color Border = Color.FromArgb(226, 232, 240);

        public static readonly Color Accent = Color.FromArgb(79, 70, 229);
        public static readonly Color AccentHover = Color.FromArgb(67, 56, 202);
        public static readonly Color AccentSoft = Color.FromArgb(238, 242, 255);

        public static readonly Color BlueSoft = Color.FromArgb(239, 246, 255);
        public static readonly Color Blue = Color.FromArgb(37, 99, 235);

        public static readonly Color CyanSoft = Color.FromArgb(236, 254, 255);
        public static readonly Color Cyan = Color.FromArgb(8, 145, 178);

        public static readonly Color GreenSoft = Color.FromArgb(236, 253, 245);
        public static readonly Color Green = Color.FromArgb(5, 150, 105);

        public static readonly Color AmberSoft = Color.FromArgb(255, 251, 235);
        public static readonly Color Amber = Color.FromArgb(217, 119, 6);

        public static readonly Color RedSoft = Color.FromArgb(254, 242, 242);
        public static readonly Color Red = Color.FromArgb(220, 38, 38);

        public static readonly Color PurpleSoft = Color.FromArgb(250, 245, 255);
        public static readonly Color Purple = Color.FromArgb(147, 51, 234);

        public static readonly Color SuccessBg = Color.FromArgb(220, 252, 231);
        public static readonly Color SuccessText = Color.FromArgb(22, 101, 52);

        public static readonly Color WarningBg = Color.FromArgb(254, 249, 195);
        public static readonly Color WarningText = Color.FromArgb(133, 77, 14);

        public static readonly Color DangerBg = Color.FromArgb(254, 226, 226);
        public static readonly Color DangerText = Color.FromArgb(153, 27, 27);

        public static Font UiFont(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font("Segoe UI", size, style);
        }

        public static Label Label(
            string text,
            int x,
            int y,
            int w,
            int h,
            float size = 9,
            FontStyle style = FontStyle.Regular,
            Color? color = null)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = UiFont(size, style),
                ForeColor = color ?? Text,
                BackColor = Color.Transparent,
                AutoEllipsis = true
            };
        }

        public static PremiumButton Button(string text, int x, int y, int w, int h, bool primary = false)
        {
            var btn = new PremiumButton
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = UiFont(9, FontStyle.Bold),
                IsPrimary = primary,
                IsDanger = false
            };

            btn.RefreshStyleState();
            return btn;
        }

        public static PremiumButton DangerButton(string text, int x, int y, int w, int h)
        {
            var btn = new PremiumButton
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = UiFont(9, FontStyle.Bold),
                IsPrimary = false,
                IsDanger = true
            };

            btn.RefreshStyleState();
            return btn;
        }

        public static RoundedPanel CardPanel(int x, int y, int w, int h)
        {
            return new RoundedPanel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Card,
                BorderColor = Border,
                Radius = 22
            };
        }

        public static Panel MessageBoxPanel(string title, string message, int x, int y, int w, int h, string type = "warning")
        {
            Color bg;
            Color fg;
            Color border;

            if (type == "success")
            {
                bg = SuccessBg;
                fg = SuccessText;
                border = Color.FromArgb(134, 239, 172);
            }
            else if (type == "danger")
            {
                bg = DangerBg;
                fg = DangerText;
                border = Color.FromArgb(252, 165, 165);
            }
            else
            {
                bg = WarningBg;
                fg = WarningText;
                border = Color.FromArgb(253, 224, 71);
            }

            var panel = new RoundedPanel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = bg,
                BorderColor = border,
                Radius = 16
            };

            panel.Controls.Add(Label(title, 16, 10, w - 32, 20, 8.8f, FontStyle.Bold, fg));
            panel.Controls.Add(Label(message, 16, 32, w - 32, h - 36, 8.3f, FontStyle.Regular, fg));

            return panel;
        }

        public static void ConfigureForm(Form form, string title, int w, int h)
        {
            form.Text = title;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ClientSize = new Size(w, h);
            form.BackColor = Bg;
            form.Font = UiFont(9);
        }

        public static void ConfigurePremiumGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Border;
            grid.Font = UiFont(8.8f);
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 36;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Dark;
            grid.ColumnHeadersDefaultCellStyle.Font = UiFont(8.8f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Dark;

            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = AccentSoft;
            grid.DefaultCellStyle.SelectionForeColor = Dark;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
        }
    }

    public class PremiumButton : Button
    {
        public bool IsPrimary { get; set; }
        public bool IsDanger { get; set; }
        public bool IsActive { get; set; }

        public PremiumButton()
        {
            FlatStyle = FlatStyle.Flat;
            Cursor = Cursors.Hand;
            FlatAppearance.BorderSize = 1;
            TextAlign = ContentAlignment.MiddleCenter;
            UseVisualStyleBackColor = false;
            Padding = new Padding(0);
            Margin = new Padding(0);

            MouseEnter += (_, _) =>
            {
                if (!IsActive)
                {
                    ApplyHoverState();
                }
            };

            MouseLeave += (_, _) =>
            {
                RefreshStyleState();
            };

            MouseDown += (_, _) =>
            {
                if (!IsActive)
                {
                    ApplyPressedState();
                }
            };

            MouseUp += (_, _) =>
            {
                RefreshStyleState();
            };
        }

        public void RefreshStyleState()
        {
            if (IsActive)
            {
                BackColor = ModernUi.AccentSoft;
                ForeColor = ModernUi.Accent;
                FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
                return;
            }

            if (IsDanger)
            {
                BackColor = Color.White;
                ForeColor = ModernUi.DangerText;
                FlatAppearance.BorderColor = ModernUi.Border;
                return;
            }

            if (IsPrimary)
            {
                BackColor = ModernUi.Accent;
                ForeColor = Color.White;
                FlatAppearance.BorderColor = ModernUi.Accent;
            }
            else
            {
                BackColor = Color.White;
                ForeColor = ModernUi.Text;
                FlatAppearance.BorderColor = ModernUi.Border;
            }
        }

        private void ApplyHoverState()
        {
            if (IsDanger)
            {
                BackColor = ModernUi.RedSoft;
                ForeColor = ModernUi.Red;
                FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
                return;
            }

            if (IsPrimary)
            {
                BackColor = ModernUi.AccentHover;
                ForeColor = Color.White;
                FlatAppearance.BorderColor = ModernUi.AccentHover;
            }
            else
            {
                BackColor = ModernUi.AccentSoft;
                ForeColor = ModernUi.Accent;
                FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
            }
        }

        private void ApplyPressedState()
        {
            if (IsDanger)
            {
                BackColor = Color.FromArgb(254, 226, 226);
                ForeColor = ModernUi.Red;
                FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
                return;
            }

            if (IsPrimary)
            {
                BackColor = Color.FromArgb(55, 48, 163);
                ForeColor = Color.White;
                FlatAppearance.BorderColor = Color.FromArgb(55, 48, 163);
            }
            else
            {
                BackColor = Color.FromArgb(224, 231, 255);
                ForeColor = ModernUi.Accent;
                FlatAppearance.BorderColor = Color.FromArgb(165, 180, 252);
            }
        }
    }

    public class RoundedPanel : Panel
    {
        public int Radius { get; set; } = 20;
        public Color BorderColor { get; set; } = Color.FromArgb(226, 232, 240);

        public RoundedPanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = GetRoundedRectanglePath(rect, Radius))
            using (SolidBrush bgBrush = new SolidBrush(BackColor))
            using (Pen borderPen = new Pen(BorderColor, 1))
            {
                e.Graphics.FillPath(bgBrush, path);
                e.Graphics.DrawPath(borderPen, path);
                Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}