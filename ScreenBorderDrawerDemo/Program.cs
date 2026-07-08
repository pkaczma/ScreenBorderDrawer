using ScreenBorderDrawer;

Console.WriteLine("Hello, World!");
ScreenBorderController screenBorderFormService = new();
_ = screenBorderFormService.CreateBorderAsync(new(30,30), new(100,100), Color.Black, 5);
Console.ReadLine();