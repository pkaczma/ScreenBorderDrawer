# ScreenBorderDrawer

A thread-safe library for drawing transparent overlay borders from console applications.

<img width="933" height="444" alt="image" src="https://github.com/user-attachments/assets/e08b5044-78e3-4a6e-89c3-012dd4d9309e" />

Draw simple borders directly on top of your desktop workspace. This tool is specifically designed to be useful for highlighting windows or visually tracking UI automation elements without interfering with the applications underneath.

## Installation

The library is available as a NuGet package. You can easily add it to your project using the .NET CLI:

```bash
dotnet add package pkaczma.ScreenBorderDrawer
```

## Features

The library allows you to create floating boundaries and dynamically adjust their properties asynchronously at runtime:

*   **Position:** Move the borders anywhere on the screen coordinate system.
*   **Size:** Expand or shrink the bounding box on the fly.
*   **Color:** Change the border color to indicate different states or statuses.
*   **Width:** Adjust the pixel thickness of the lines.
*   **Labels:** Attach and update text tags to clearly identify specific regions.
