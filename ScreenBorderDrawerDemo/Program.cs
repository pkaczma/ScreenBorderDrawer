using ScreenBorderDrawer;

public static class ScreenBorderDemo
{
    public static ScreenBorderController controller = new();
    public static bool isDemoRunning = true;
    public static void Main(string[] args)
    {
        _ = DemoLabelAsync();
        _ = DemoSizeAsync();
        _ = DemoColorAsync();
        _ = DemoWidthAsync();
        _ = DemoPingPongAsync();
        Console.ReadLine();
    }

    public async static Task DemoLabelAsync()
    {
        string demoText = "Label Demo Text ";
        int dir = -1;
        int at = demoText.Length-1;
        Guid borderOverlay = await controller.CreateBorderAsync(new(50,50), new(150,150), Color.Red, 3, "Label");
        while (isDemoRunning)
        {
            if(at == demoText.Length || at == 0) dir *= -1;
            at+=dir;
            await controller.SetBorderLabelAsync(borderOverlay,demoText.Substring(0,at)??"");
            await Task.Delay(200);
        }
    }
    public async static Task DemoSizeAsync()
    {
        Size maxSize = new(200, 200);
        Size minSize = new(150, 150);
        Size currentSize = minSize;
        int dir = 1;
        Guid borderOverlay = await controller.CreateBorderAsync(new(250,50), new(150,150), Color.Green, 3, "Size Demo");
        while (isDemoRunning)
        {
            if(currentSize.Width > maxSize.Width || currentSize.Width < minSize.Width) dir*=-1;
            currentSize = new(currentSize.Width+dir,currentSize.Height+dir);
            await controller.SetBorderSizeAsync(borderOverlay, currentSize);
            await Task.Delay(10);
        }
    }
    public async static Task DemoColorAsync()
    {
        double step = 0;
        Guid borderOverlay = await controller.CreateBorderAsync(new(500,50), new(150,150), Color.Red, 3, "Color Demo");
        while (isDemoRunning)
        {
            double frequency = 0.1; 

            int r = (int)(Math.Sin(frequency * step + 0) * 127 + 128);
            int g = (int)(Math.Sin(frequency * step + 2 * Math.PI / 3) * 127 + 128);
            int b = (int)(Math.Sin(frequency * step + 4 * Math.PI / 3) * 127 + 128);

            Color color = Color.FromArgb(255, r, g, b);

            step++;

            await controller.SetBorderColorAsync(borderOverlay, color);
            await Task.Delay(33);
        }
    }
    public async static Task DemoWidthAsync()
    {
        int minWidth = 2;
        int maxWidth = 9;
        int currentWidth = minWidth;
        int dir = 1;
        
        Guid borderOverlay = await controller.CreateBorderAsync(new(750, 50), new(150, 150), Color.Yellow, currentWidth, "Width Demo");
        
        while (isDemoRunning)
        {
            try{
                if (currentWidth > maxWidth || currentWidth < minWidth) dir *= -1;
                currentWidth += dir;

                await controller.SetBorderWidthAsync(borderOverlay, currentWidth);
                await Task.Delay(50);
            }catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }

    public async static Task DemoPingPongAsync()
    {
        int minX = 50;
        int maxX = 750;
        int currentX = minX;
        int dirX = 2;
        int dirY = 1;
        int minY = 250;
        int maxY = 350;
        int currentY = minY;
        
        Guid borderOverlay = await controller.CreateBorderAsync(new(currentX, currentY), new(150, 150), Color.Cyan, 3, "Ping Pong");
        
        while (isDemoRunning)
        {
            currentX += dirX;
            currentY += dirY;
            if (currentX >= maxX || currentX <= minX) dirX *= -1;
            if (currentY >= maxY || currentY <= minY) dirY *= -1;
            
            await controller.SetBorderLocationAsync(borderOverlay, new Point(currentX, currentY));
            
            await Task.Delay(5); 
        }
    }
}