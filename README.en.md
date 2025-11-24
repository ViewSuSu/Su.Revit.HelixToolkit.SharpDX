# 🚀 HelixViewport3DBuilder User Guide

## 📖 Introduction

HelixViewport3DBuilder is a specialized tool class for displaying and interacting with 3D models in Revit plugins. Developed based on HelixToolkit.Wpf.SharpDX, it provides simple and easy-to-use APIs to create feature-rich 3D viewport windows.

---

## 🎯 Quick Start

### ⚡ Basic Usage

```csharp
// 1. 📦 Initialize the builder
var builder = HelixViewport3DBuilder.Init(
    revitDocument, 
    geometryObjects, 
    new Viewport3DXOptions()
);

// 2. 🖥️ Get the 3D viewport control
Viewport3DX viewport = builder.Viewport;

// 3. 📝 Add viewport to your WPF window
```

### 🔥 Complete Example

```csharp
// Prepare geometry objects to display
var geometryObjects = new List<GeometryObjectOptions>
{
    // Add your geometry objects...
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

// ✨ Enable interaction features
builder.SetHoverHighlightEnabled(true)
       .SetClickHighlightEnabled(true);
```

---

## 🎮 Interaction Features

### 🖱️ Mouse Operations

| Operation | Function | Icon |
|-----------|----------|------|
| 🖱️ Middle Double-Click | Zoom to extent | 🔍 |
| 🖱️ Middle Drag | Pan view | 👐 |
| 🖱️ Shift + Right Click | Rotate view | 🔄 |
| 🖱️ Mouse Hover | Semi-transparent highlight | 👆 |
| 🖱️ Left Click | Select model | ✅ |
| 🖱️ Ctrl + Click | Multi-select models | 📋 |

### 🎨 Highlight Features

```csharp
// 🌈 Set highlight color
builder.SetHighlightColor(Colors.Red, 0.8f);  // Red highlight

// 💫 Enable blinking effect
builder.SetHighlightBlinking(true, 100);  // 100ms blink interval

// 🔧 Programmatically highlight specific objects
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

- ✅ **View Cube**: Displayed at top-right, click for quick view switching
- ✅ **Auto Zoom**: Automatically adjusts to suitable view range on load
- ✅ **Anti-aliasing**: Configurable graphics quality settings

---

## 🛠️ Advanced Features

### 📡 Event Listening

```csharp
// 👂 Listen to model selection events
builder.OnModelSelected += (sender, args) => 
{
    var selectedModel = args.SelectedModel;
    var geometryObject = args.GeometryObject;
    var hitPoint = args.HitPoint;
    
    // 🎯 Handle selection logic
    Console.WriteLine($"Selected model: {geometryObject}");
};

// 👂 Listen to deselection events
builder.OnModelDeselected += (sender, args) => 
{
    // 🗑️ Clear selection state
};
```

### 🔍 Selection Management

```csharp
// 📋 Get currently selected models
var selectedModels = builder.GetSelectedModels();

// 📋 Get currently selected geometry objects
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
    EnableRenderFrustum = true          // 🎯 Frustum culling
};
```

### 🔧 Feature Toggles

```csharp
// Enable/disable hover highlight
builder.SetHoverHighlightEnabled(true);

// Enable/disable click highlight  
builder.SetClickHighlightEnabled(true);
```

---

## 💡 Usage Tips

### 🚀 Performance Optimization

- ✅ Use `EnableSwapChainRendering` to improve rendering performance
- ✅ Set appropriate `FXAALevel` to balance quality and performance
- ✅ Call `Clear()` promptly to release resources

### 🎯 Best Practices

1. **📱 Responsive Design**: Viewport automatically adapts to container size
2. **🔄 Real-time Updates**: Support dynamic add/remove of geometry objects
3. **🎮 User Friendly**: Provide intuitive mouse interaction feedback
4. **🎨 Visual Consistency**: Maintain visual style similar to Revit

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

---

**🎉 Start using HelixViewport3DBuilder to create outstanding 3D visualization experiences!**