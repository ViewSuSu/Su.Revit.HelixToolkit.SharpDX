 [English](README.en.md) | [简体中文](README.md)

![Revit Support](https://img.shields.io/badge/Revit-2013~2026-green)
![Platform](https://img.shields.io/badge/Platform-WPF%2BSharpDX-orange)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

# 🚀 Su.Revit.HelixToolkit.SharpDX User Documentation

## 🌐 Project Addresses

**GitHub**: https://github.com/ViewSuSu/Su.Revit.HelixToolkit.SharpDX  
**Gitee**: https://gitee.com/SususuChang/su.-revit.-helix-toolkit.-sharp-dx

## 🎬 Demo Animation

![Feature Demo](HD.gif)

---

## 📦 Installation Methods

### Install via NuGet (Recommended)

```bash
# Package Manager
Install-Package Su.Revit.HelixToolkit.SharpDX

# .NET CLI
dotnet add package Su.Revit.HelixToolkit.SharpDX
```

### Package Reference (csproj)

```xml
<PackageReference Include="Su.Revit.HelixToolkit.SharpDX" Version="1.0.0" />
```

---

## 📖 Introduction

Su.Revit.HelixToolkit.SharpDX is a high-performance 3D visualization tool library specifically designed for Revit plugin development. Built on HelixToolkit.Wpf.SharpDX, it provides simple and easy-to-use APIs to create feature-rich 3D view windows in Revit plugins.

**Core Features**:
- 🚀 **High-Performance Rendering**: Index optimization for Solid triangle faces, capable of handling Solid models with massive triangle data
- 🎯 **Complete Interaction**: Supports mouse hover highlighting, click selection, multi-selection, rotation, zoom, pan, and other complete interaction functions
- 📐 **Coordinate System Adaptation**: Automatically handles Revit and Helix coordinate system conversion for seamless integration
- 🎨 **Material System**: Supports Revit native materials, custom colors, texture materials, and various rendering methods
- ⚡ **Memory Optimization**: Efficient geometric data management and memory release mechanism

---

## 🎯 Quick Start

### ⚡ Basic Usage

```csharp
// 1. 📦 Initialize builder
var builder = HelixViewport3DBuilder.Init(
    revitDocument, 
    geometryObjects, 
    new Viewport3DXOptions()
);

// 2. 🖥️ Get 3D viewport control
Viewport3DX viewport = builder.Viewport;

// 3. 📝 Add viewport to your WPF window
```

### 🔥 Complete Example

```csharp
// Prepare geometric objects to display
var geometryObjects = new List<GeometryObjectOptions>
{
    // Add your geometric objects...
};

// 🎨 Configure viewport options
var visualOptions = new Viewport3DXOptions
{
    BackgroundColor = System.Windows.Media.Colors.LightGray,
    FXAALevel = 4 // Anti-aliasing level
};

// 🏗️ Create builder
var builder = HelixViewport3DBuilder.Init(
    document, 
    geometryObjects, 
    visualOptions
);

// 📐 Set camera view
builder.SetCamera(revitView);

// ✨ Enable interaction functions
builder.SetHoverHighlightEnabled(true)
       .SetClickHighlightEnabled(true);
```

---

## 🎮 Interaction Features

### 🖱️ Mouse Operations

| Operation | Function | Icon |
|-----------|----------|------|
| 🖱️ Middle Double Click | Zoom to view extent | 🔍 |
| 🖱️ Middle Drag | Pan view | 👐 |
| 🖱️ Shift + Right Click | Rotate view | 🔄 |
| 🖱️ Mouse Hover | Semi-transparent highlight | 👆 |
| 🖱️ Left Click | Select model | ✅ |
| 🖱️ Ctrl + Click | Multi-select models | 📋 |

### 🎨 Highlight Function

```csharp
// 🌈 Set highlight color
builder.SetHighlightColor(Colors.Red, 0.8f);  // Red highlight

// 💫 Enable blinking effect
builder.SetHighlightBlinking(true, 100);  // 100ms blink interval

// 🔧 Programmatically highlight specific object
builder.HighlightGeometryObject(specificGeometry);
```

---

## 📊 View Control

### 🎥 Camera Settings

```csharp
// Method 1: Use Revit view
builder.SetCamera(revitView);

// Method 2: Custom camera
builder.SetCamera(
    new XYZ(0, 0, 10),     // 📍 Camera position
    new XYZ(0, 0, -1),     // 👀 Look direction
    new XYZ(0, 1, 0)       // ⬆️ Up direction
);
```

### 🧭 Navigation Controls

- ✅ **View Cube**: Displayed in upper right corner, click to quickly switch views
- ✅ **Auto Zoom**: Automatically adjusts to appropriate view extent when loading
- ✅ **Anti-Aliasing**: Configurable graphics quality settings

---

## 🛠️ Advanced Features

### 📡 Event Listening

```csharp
// 👂 Listen to model selection event
builder.OnModelSelected += (sender, args) => 
{
    var selectedModel = args.SelectedModel;
    var geometryObject = args.GeometryObject;
    var hitPoint = args.HitPoint;
    
    // 🎯 Handle selection logic
    Console.WriteLine($"Selected model: {geometryObject}");
};

// 👂 Listen to deselection event
builder.OnModelDeselected += (sender, args) => 
{
    // 🗑️ Clear selection state
};
```

### 🔍 Selection Management

```csharp
// 📋 Get currently selected models
var selectedModels = builder.GetSelectedModels();

// 📋 Get currently selected geometric objects
var selectedGeometry = builder.GetSelectedGeometryObjects();

// 🧹 Clear all selections
builder.ClearHighlight();
```

---

## ⚙️ Configuration Options

### 🎨 Visual Configuration

```csharp
var options = new Viewport3DXOptions
{
    BackgroundColor = Colors.Black,      // 🎨 Background color
    FXAALevel = 8,                       // 🔍 Anti-aliasing level (0-8)
    EnableRenderFrustum = true          // 🎯 View frustum culling
};
```

### 🔧 Function Switches

```csharp
// Enable/disable hover highlight
builder.SetHoverHighlightEnabled(true);

// Enable/disable click highlight  
builder.SetClickHighlightEnabled(true);
```

---

## 🎨 GeometryObjectOptions Usage Guide

### 📝 Basic Configuration

`GeometryObjectOptions` is used to configure the rendering method of geometric objects:

#### Using Revit Materials

```csharp
var options = new GeometryObjectOptions(
    geometryObject,    // 📐 Revit geometric object
    revitMaterial      // 🎨 Revit material (optional)
);
```

#### Using Custom Colors

```csharp
var options = new GeometryObjectOptions(
    geometryObject,           // 📐 Revit geometric object
    Colors.Blue,              // 🔵 Custom color
    0.8f                      // 💧 Transparency (0-1)
);
```

#### Using Texture Materials

```csharp
var options = new GeometryObjectOptions(
    geometryObject,           // 📐 Revit geometric object
    textureStream,            // 🖼️ Texture stream
    Colors.White,             // ⚪ Emissive color
    1.0f                      // 💧 Transparency
);
```

### ⚙️ Rendering Parameter Configuration

```csharp
var options = new GeometryObjectOptions(geometryObject, material)
{
    LevelOfDetail = 0.8,                              // 🎯 Detail level (0-1)
    MinAngleInTriangle = 0,                           // 📐 Minimum angle in triangle
    MinExternalAngleBetweenTriangles = Math.PI / 4,   // 📏 Minimum external angle between adjacent faces
    IsDrawSolidEdges = true,                          // 📏 Draw outline edges
    SolidEdgeThickness = 2f,                          // 🖊️ Outline edge thickness
    SolidEdgeSmoothness = 10f                         // ✨ Outline edge smoothness
};
```

### 🔧 Parameter Description

| Parameter | Description | Default | Impact |
|-----------|-------------|---------|--------|
| `LevelOfDetail` | Rendering detail level | 0.5 | Higher values create denser meshes, higher precision but more performance consumption |
| `MinAngleInTriangle` | Minimum angle in triangle | 0 | Controls smoothness during mesh generation |
| `MinExternalAngleBetweenTriangles` | Minimum external angle between adjacent triangles | 2π | Determines smooth transition degree of curved surfaces |
| `IsDrawSolidEdges` | Whether to draw outline edges | true | Display boundary lines |
| `SolidEdgeThickness` | Outline edge thickness | 2f | Line pixel width |
| `SolidEdgeSmoothness` | Outline edge smoothness | 10f | Higher values create smoother edges |

---

## 💡 Usage Tips

### 🚀 Performance Optimization

- ✅ Use `EnableSwapChainRendering` to improve rendering performance
- ✅ Reasonably set `FXAALevel` to balance quality and performance
- ✅ Timely call `Clear()` to release resources
- ✅ Adjust `LevelOfDetail` according to needs, avoid unnecessary details
- ✅ Utilize Solid triangle face index optimization to handle massive data

### 🎯 Best Practices

1. **📱 Responsive Design**: Viewport automatically adapts to container size
2. **🔄 Real-time Updates**: Supports dynamic addition/removal of geometric objects
3. **🎮 User-Friendly**: Provides intuitive mouse interaction feedback
4. **🎨 Visual Consistency**: Maintains visual style similar to Revit
5. **⚡ Performance Balance**: Adjust rendering parameters according to scene complexity
6. **💾 Memory Management**: Timely clean up unused geometric objects

### 🔄 Scene Management

```csharp
// 🧹 Clear scene
builder.Clear();

// 📦 Re-add objects
builder.Add(newGeometryObjects);

// 🎯 Reset camera
builder.SetCamera(newView);
```

---

## ❓ Frequently Asked Questions

### ❓ How to change highlight color?
```csharp
builder.SetHighlightColor(Colors.Blue, 0.7f);  // 🔵 Blue highlight
```

### ❓ How to disable all interactions?
```csharp
builder.SetHoverHighlightEnabled(false)
       .SetClickHighlightEnabled(false);
```

### ❓ How to get world coordinates of click position?
```csharp
builder.OnModelSelected += (sender, args) => 
{
    var worldPosition = args.HitPoint;  // 🌍 World coordinates
};
```

### ❓ How to optimize performance for complex models?
```csharp
var options = new GeometryObjectOptions(geometryObject, material)
{
    LevelOfDetail = 0.3,      // 🎯 Reduce detail level
    IsDrawSolidEdges = false  // 📏 Disable outline edge drawing
};
```

### ❓ How to handle material transparency?
```csharp
// Method 1: Use color transparency
var options = new GeometryObjectOptions(geometryObject, Colors.Red, 0.5f);

// Method 2: Use Revit material transparency
var material = document.GetElement(materialId) as Autodesk.Revit.DB.Material;
var options = new GeometryObjectOptions(geometryObject, material);
```

### ❓ How to handle Solid models with massive triangle faces?
```csharp
// The library has built-in triangle face index optimization, automatically handles massive data
// Just create GeometryObjectOptions normally
var options = new GeometryObjectOptions(largeSolidModel, material);
```

---

## 📞 Technical Support

If you encounter problems during use, please check:

- ✅ Whether Revit document object is correctly passed
- ✅ Whether geometric object collection contains valid data
- ✅ Whether viewport control is correctly added to WPF visual tree
- ✅ Whether event handlers are correctly registered and unregistered
- ✅ Whether rendering parameters are within reasonable range
- ✅ Whether memory usage is normal, timely call Clear() to release resources

### 📚 More Resources

- 📖 **Complete Source Code**: Please visit the GitHub or Gitee repositories above
- 💡 **Feature Suggestions**: Welcome to submit Pull Requests or feature suggestions
- 📋 **Update Log**: Check the repository's Release page for latest version information

---

**🎉 Start using Su.Revit.HelixToolkit.SharpDX to create outstanding 3D visualization experiences!**


## 星曲线图 / Star History

[![Star History Chart](https://api.star-history.com/svg?repos=ViewSuSu/Su.Revit.HelixToolkit.SharpDX&type=Date)](https://star-history.com/#ViewSuSu/Su.Revit.HelixToolkit.SharpDX&Date)
