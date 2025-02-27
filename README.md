![Banner](https://raw.githubusercontent.com/BlizzCrafter/MonoGame.Invisible/refs/heads/master/Logos/Banner.png)

# Welcome to MonoGame.Invisible!
[![Version](https://img.shields.io/nuget/v/MonoGame.Invisible?style=for-the-badge&logo=NuGet&logoColor=0289CC&logoSize=auto&label=MonoGame.Invisible&labelColor=262626&color=707070)](https://www.nuget.org/packages/MonoGame.Invisible)

MonoGame.Invisible makes your MonoGame window fully transparent!

### Look for yourself:

https://github.com/user-attachments/assets/0cd45914-a175-4d84-8b2e-ff4d6d358aab

Now you can create one of those hyped "**Desktop-Games**" with the **MonoGame.Framework**... if you know what I mean :)

### How To Use It?

Creating an invisible MonoGame window with this library is as easy as:

```c#
protected override void Initialize()
{
    // Setup the TransparentWindowManager
    TransparentWindowManager.Init(this, graphics);

    base.Initialize();
}

protected override void Draw(GameTime gameTime)
{
    TransparentWindowManager.Window.PrepareDraw();

    spriteBatch.Begin();
    // Your Drawings here!
    spriteBatch.End();
}
```

EASY! 😎

### Some Helpful Features:
```c#
protected override void Initialize()
{
    // The window will stay in the background - even on user interaction.
    TransparentWindowManager.Window.KeepInBackground();

    // Keeps the window in the foreground, preventing it from being moved to the back.
    TransparentWindowManager.Window.KeepInForeground();
}

protected override void Update(GameTime gameTime)
{
    // Ensure window stays in the back.
    if (TransparentWindowManager.Window.IsForegroundWindow())
    {
        TransparentWindowManager.Window.SendToBack();
    }

    if (TransparentWindowManager.Window.IsPixelOpaque(mousePoint))
    {
        // The pixel under the mouse cursor is opaque!
    }
    else
    {
        // The pixel under the mouse cursor is transparent!
    }
}
```

More helpful stuff and features inside! 🔎

> [!NOTE]
> Only works with a **MonoGame.Framework.WindowsDX** project!
> A **MonoGame.Framework.DesktopGL** project **doesn't work** - even if compiled for windows!

> [!TIP]
> Check out the **integrated sample project**, which shows the invisible MonoGame window in action.
> It's much smoother than in the video. You need to see it for yourself!

### 💖 Now Have Fun with MonoGame.Invisible! 🕸️

> [!IMPORTANT] 
> PS: If you create such a Desktop-Game with MonoGame.Invisible, then please let me know. This is not neccessary, but I would be happy to add a link to your game here on this site! :)
