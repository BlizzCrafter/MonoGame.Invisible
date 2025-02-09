![Banner](https://github.com/BlizzCrafter/MonoGame.Invisible/master/Logos/Banner.png)

# Welcome to MonoGame.Invisible!
[![Version](https://img.shields.io/nuget/v/MonoGame.Invisible?style=for-the-badge&logo=NuGet&logoColor=0289CC&logoSize=auto&label=MonoGame.Invisible&labelColor=262626&color=707070)](https://www.nuget.org/packages/MonoGo.Templates)

MonoGame.Invisible makes your MonoGame window fully transparent!

### Look for yourself:

https://github.com/user-attachments/assets/0cd45914-a175-4d84-8b2e-ff4d6d358aab

Now you can create one of those hyped "**Desktop-Games**" with the **MonoGame.Framework**... if you know what I mean :)

### How To Use It?

Creating an invisible MonoGame window with this library is as easy as:

```c#
protected override void Initialize()
{
    TransparentWindowManager.Setup(this, graphics);
    _windowManager = TransparentWindowManager.Create(this, TransparencyMode.ColorKey, new Color(1, 1, 1));
    _windowManager.Initialize();

    base.Initialize();
}

protected override void Draw(GameTime gameTime)
{
    _windowManager.BeginDraw(gameTime);
    spriteBatch.Begin();
    // Your Drawings here!
    spriteBatch.End();
    _windowManager.EndDraw(gameTime);
}
```

Only the **Windows** platform is supported!

Check out the integrated sample project, which shows the invisible MonoGame window in action.
It's much smoother than in the video. You need to see it for yourself!

### Now Have Fun with MonoGame.Invisible!

PS: If you create such a Desktop-Game with MonoGame.Invisible, then please let me know. I will be happy to add a link to your game here on this site! :)
