using System.ComponentModel;
using ScreenBorderDrawer.Internal;

namespace ScreenBorderDrawer;

internal class ScreenBorderOverlay : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int BorderWidth {get {return Padding.All;} set{Padding = new Padding(value);}}
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal Color BorderColor { get {return BackColor;} set {BackColor = value;}}
    internal IntPtr hWnd {get; private set;}
    
    private Panel _Panel;

    internal ScreenBorderOverlay(Point location, Size size, Color borderColor, int borderWidth)
    {
        this.BorderWidth = borderWidth;
        this.BorderColor = borderColor;
        _Panel = new();
        InitializeForm();
        hWnd = Handle;
        PositionWindow(location,size);
    }
    internal void PositionWindow(Point location, Size size)
    {
        Location = location;
        ClientSize = size;
    }
    private void InitializeForm()
    {
        ResizeRedraw = true;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        AllowTransparency = true;
        TransparencyKey = BorderColor.Invert();
        TopMost = true;
        _Panel = new Panel()
        {
            Location = new (BorderWidth,BorderWidth),
            BackColor = TransparencyKey,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Dock = DockStyle.Fill
        };
        this.Controls.Add(_Panel);
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
