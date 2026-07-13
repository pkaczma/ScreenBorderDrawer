using System.Collections.Concurrent;

namespace ScreenBorderDrawer;

public class ScreenBorderController : IDisposable
{
    private readonly ConcurrentDictionary<Guid, ScreenBorderOverlay> _activeOverlays = new();
    private WindowsFormsSynchronizationContext syncContext;
    private Thread uiThread;
    private ManualResetEvent uiReady = new ManualResetEvent(false);

    public ScreenBorderController()
    {   
        syncContext = new();
        uiThread = new(_ =>
        {
            Application.EnableVisualStyles();
            ApplicationContext context = new();
            SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
            syncContext = (WindowsFormsSynchronizationContext?)SynchronizationContext.Current??throw new Exception("Sync context not created");

            uiReady.Set();
            Application.Run(context);
        })
        {
            IsBackground = true
        };
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start();

        uiReady.WaitOne();
    }
    public async Task<Guid> CreateBorderAsync(Point location, Size size, Color? borderColor = null, int width = 2, string labelText = "")
    {
        if(width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Overlay's border width must be greater than 0.");
        Guid guid = Guid.NewGuid();

        syncContext.Send(_ =>
        {
            ScreenBorderOverlay overlay = new(location, size, borderColor??Color.White, width, labelText);
            
            _activeOverlays.TryAdd(guid, overlay);

            overlay.FormClosed += (s, e) =>
            {
                ScreenBorderOverlay? useless;
                _activeOverlays.TryRemove(guid, out useless);
            };
            overlay.Show();
        }, null);
        return guid;
    }
    public void RemoveBorder(Guid borderGuid)
    {
        if (!_activeOverlays.TryRemove(borderGuid, out ScreenBorderOverlay? overlay))
        {
            throw new KeyNotFoundException($"Overlay with guid {borderGuid} doesn't exist.");
        }

        syncContext.Post(_ =>
        {
            overlay?.Close();
        }, null);
    }
    public async Task<IntPtr> GetBorderHandleAsync(Guid borderGuid)
    {
        return (await _GetBorderFormAsync(borderGuid)).Handle;
    }
    public async Task SetBorderSizeAsync(Guid borderGuid, Size size)
    {
        ScreenBorderOverlay borderForm = await _GetBorderFormAsync(borderGuid);
        syncContext.Post(_ =>
        {
            borderForm.PositionWindow(borderForm.Location, size);
        }, null);
    }
    public async Task SetBorderLocationAsync(Guid borderGuid, Point location)
    {
        ScreenBorderOverlay borderForm = await _GetBorderFormAsync(borderGuid);
        syncContext.Post(_ =>
        {
            borderForm.PositionWindow(location, borderForm.ClientSize);
        },null);
    }
    public async Task SetBorderColorAsync(Guid borderGuid, Color color)
    {
        ScreenBorderOverlay borderForm = await _GetBorderFormAsync(borderGuid);
        syncContext.Post(_ =>
        {
            borderForm.BorderColor = color;
        },null);
    }
    public async Task SetBorderWidthAsync(Guid borderGuid, int borderWidth)
    {
        if(borderWidth <= 0) throw new ArgumentOutOfRangeException(nameof(borderWidth), "Overlay's border width must be greater than 0.");
        if(borderWidth > 10) throw new ArgumentOutOfRangeException(nameof(borderWidth), "Overlay's border width must be less than 10.");
        ScreenBorderOverlay borderForm = await _GetBorderFormAsync(borderGuid);
        syncContext.Post(_ =>
        {
            borderForm.BorderWidth = borderWidth;
        },null);
    }

    public async Task SetBorderLabelAsync(Guid borderGuid, string label)
    {
        ScreenBorderOverlay borderForm = await _GetBorderFormAsync(borderGuid);
        syncContext.Post(_ =>
        {
            borderForm.LabelText = label;
        },null);
    }

    private Task<ScreenBorderOverlay> _GetBorderFormAsync(Guid guid)
    {
        ScreenBorderOverlay? borderForm;
        if(_activeOverlays.TryGetValue(guid, out borderForm) == false)
            throw new KeyNotFoundException($"ScreenBorder element with guid {guid} has not been found!");
        return Task.FromResult(borderForm);
    }

    public void Dispose()
    {
        foreach(Guid guid in _activeOverlays.Keys)
        {
            RemoveBorder(guid);
        }
        syncContext?.Send(_ => Application.ExitThread(), null);
        if(uiThread != null && uiThread.IsAlive) uiThread.Join(1000);
    }
}