using System.ComponentModel;
using ScreenBorderDrawer.Internal;

namespace ScreenBorderDrawer;

internal class ScreenBorderOverlay : Form
{
    private const int FormPadding = 10;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int BorderWidth {get {return borderWidth;} set => SetBorderWidth(value); }
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal Color BorderColor { get {return borderColor;} set => SetBorderColor(value); }
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal string LabelText { get {return labelText;} set => SetBorderLabel(value); }
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal Font LabelFont { get => label.Font; set => SetBorderLabelFont(value); }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal new Point Location { get => GetLocation(); set => PositionWindow(value, Size);}
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal new Size ClientSize { get => GetClientSize(); set => PositionWindow(Location, value);}

    private int borderWidth;
    private Color borderColor;
    private string labelText;
    private Font labelFont;
    
    private Panel borderPanel;
    private Panel innerPanel;
    private PixelPerfectLabel label;

    internal ScreenBorderOverlay(Point location, Size size, Color borderColor, int borderWidth, string labelText)
    {
        this.borderColor = borderColor;
        this.borderWidth = borderWidth;
        this.labelText = labelText;
        labelFont = this.Font;
        
        borderPanel = new();
        innerPanel = new();
        label = new();
        
        InitializeComponent();
        
        SetBorderColor(borderColor);
        SetBorderWidth(borderWidth);
        SetBorderLabel(labelText);
        PositionWindow(location,size);
    }
    internal void PositionWindow(Point location, Size size)
    {
        base.Location = new(location.X-FormPadding,location.Y-FormPadding);
        base.ClientSize = new(size.Width+FormPadding*2, size.Height+FormPadding*2);
    }
    
    private Point GetLocation()
    {
        return new(base.Location.X+FormPadding, base.Location.Y+FormPadding);
    }
    private Size GetClientSize()
    {
        return new(base.ClientSize.Width-(FormPadding*2), base.ClientSize.Height-(FormPadding*2));
    }
    private void SetBorderColor(Color color)
    {
        Color invertedColor = color.Invert();
        
        this.borderColor = color;

        TransparencyKey = invertedColor;
        BackColor = invertedColor;
        borderPanel.BackColor = color;
        innerPanel.BackColor = invertedColor;
        label.BackColor = invertedColor;
        label.ForeColor = color;
    }
    private void SetBorderLabel(string labelText)
    {
        labelText = labelText.Trim();
        label.Text = labelText;
        this.labelText = labelText;
        if(String.IsNullOrEmpty(labelText)) TryHideLabel();
        else TryShowLabel();
    }
    private void SetBorderLabelFont(Font font)
    {
        labelFont = font;
        label.Font = font;
    }
    private void SetBorderWidth(int value)
    {
        borderWidth = value;
        borderPanel.Padding = new Padding(value);
        label.Location = new(FormPadding+borderWidth+10,FormPadding-8+borderWidth/2);
    }
    private void TryHideLabel()
    {
        if(!label.Visible) return;
        label.Visible = false;
    }
    private void TryShowLabel()
    {
        if(label.Visible) return;
        label.Visible = true;
    }
    private void InitializeComponent()
    {
        ResizeRedraw = true;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        AllowTransparency = true;
        TopMost = true;
        ShowInTaskbar = false;
        Padding = new(FormPadding);

        borderPanel = new Panel()
        {
            Location = new (borderWidth,borderWidth),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Dock = DockStyle.Fill
        };
        innerPanel = new Panel()
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Dock = DockStyle.Fill
        };
        label = new PixelPerfectLabel()
        {
            Location = new(FormPadding+borderWidth*3,FormPadding-10+borderWidth),
            Text = "",
            AutoSize = true,
            Font = new(LabelFont, FontStyle.Bold),
            Dock = DockStyle.None
        };
        borderPanel.Controls.Add(innerPanel);
        this.Controls.Add(label);
        this.Controls.Add(borderPanel);
    }

    protected class PixelPerfectLabel : Label
    {
        public PixelPerfectLabel()
        {
            UseCompatibleTextRendering = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            base.OnPaint(e);
        }
    }
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            const int WS_EX_LAYERED = 0x80000;
            const int WS_EX_TRANSPARENT = 0x20;
            cp.ExStyle |= (WS_EX_LAYERED | WS_EX_TRANSPARENT);
            return cp;
        }
    }
}
