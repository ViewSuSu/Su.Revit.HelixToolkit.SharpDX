# 🚀 HelixViewport3DBuilder 使用说明文档

## 📖 简介

HelixViewport3DBuilder 是一个专门用于在 Revit 插件中显示和交互 3D 模型的工具类。它基于 HelixToolkit.Wpf.SharpDX 开发，提供了简单易用的 API 来创建功能丰富的 3D 视图窗口。

---

## 🎯 快速开始

### ⚡ 基本使用

```csharp
// 1. 📦 初始化构建器
var builder = HelixViewport3DBuilder.Init(
    revitDocument, 
    geometryObjects, 
    new Viewport3DXOptions()
);

// 2. 🖥️ 获取 3D 视口控件
Viewport3DX viewport = builder.Viewport;

// 3. 📝 将 viewport 添加到你的 WPF 窗口中
```

### 🔥 完整示例

```csharp
// 准备要显示的几何对象
var geometryObjects = new List<GeometryObjectOptions>
{
    // 添加你的几何对象...
};

// 🎨 配置视口选项
var visualOptions = new Viewport3DXOptions
{
    BackgroundColor = System.Windows.Media.Colors.LightGray,
    FXAALevel = 4 // 抗锯齿等级
};

// 🏗️ 创建构建器
var builder = HelixViewport3DBuilder.Init(
    document, 
    geometryObjects, 
    visualOptions
);

// 📐 设置相机视图
builder.SetCamera(revitView);

// ✨ 启用交互功能
builder.SetHoverHighlightEnabled(true)
       .SetClickHighlightEnabled(true);
```

---

## 🎮 交互功能

### 🖱️ 鼠标操作

| 操作 | 功能 | 图标 |
|------|------|------|
| 🖱️ 中键双击 | 缩放至视图范围 | 🔍 |
| 🖱️ 中键拖动 | 平移视图 | 👐 |
| 🖱️ Shift + 右键 | 旋转视角 | 🔄 |
| 🖱️ 鼠标悬停 | 半透明高亮显示 | 👆 |
| 🖱️ 左键点击 | 选中模型 | ✅ |
| 🖱️ Ctrl + 点击 | 多选模型 | 📋 |

### 🎨 高亮功能

```csharp
// 🌈 设置高亮颜色
builder.SetHighlightColor(Colors.Red, 0.8f);  // 红色高亮

// 💫 启用闪烁效果
builder.SetHighlightBlinking(true, 100);  // 100ms 闪烁间隔

// 🔧 程序化高亮特定对象
builder.HighlightGeometryObject(specificGeometry);
```

---

## 📊 视图控制

### 🎥 相机设置

```csharp
// 方法1: 使用 Revit 视图
builder.SetCamera(revitView);

// 方法2: 自定义相机
builder.SetCamera(
    new XYZ(0, 0, 10),     // 📍 相机位置
    new XYZ(0, 0, -1),     // 👀 观察方向
    new XYZ(0, 1, 0)       // ⬆️ 上方向
);
```

### 🧭 导航控件

- ✅ **视图立方体**: 显示在右上角，点击快速切换视角
- ✅ **自动缩放**: 载入时自动调整到合适视图范围
- ✅ **抗锯齿**: 可配置的图形质量设置

---

## 🛠️ 高级功能

### 📡 事件监听

```csharp
// 👂 监听模型选中事件
builder.OnModelSelected += (sender, args) => 
{
    var selectedModel = args.SelectedModel;
    var geometryObject = args.GeometryObject;
    var hitPoint = args.HitPoint;
    
    // 🎯 处理选中逻辑
    Console.WriteLine($"选中了模型: {geometryObject}");
};

// 👂 监听取消选中事件
builder.OnModelDeselected += (sender, args) => 
{
    // 🗑️ 清除选中状态
};
```

### 🔍 选择管理

```csharp
// 📋 获取当前选中的模型
var selectedModels = builder.GetSelectedModels();

// 📋 获取当前选中的几何对象
var selectedGeometry = builder.GetSelectedGeometryObjects();

// 🧹 清除所有选择
builder.ClearHighlight();
```

---

## ⚙️ 配置选项

### 🎨 视觉配置

```csharp
var options = new Viewport3DXOptions
{
    BackgroundColor = Colors.Black,      // 🎨 背景颜色
    FXAALevel = 8,                       // 🔍 抗锯齿等级 (0-8)
    EnableRenderFrustum = true          // 🎯 视锥体裁剪
};
```

### 🔧 功能开关

```csharp
// 启用/禁用悬停高亮
builder.SetHoverHighlightEnabled(true);

// 启用/禁用点击高亮  
builder.SetClickHighlightEnabled(true);
```

---

## 💡 使用技巧

### 🚀 性能优化

- ✅ 使用 `EnableSwapChainRendering` 提升渲染性能
- ✅ 合理设置 `FXAALevel` 平衡画质和性能
- ✅ 及时调用 `Clear()` 释放资源

### 🎯 最佳实践

1. **📱 响应式设计**: 视口会自动适应容器大小
2. **🔄 实时更新**: 支持动态添加/移除几何对象
3. **🎮 用户友好**: 提供直观的鼠标交互反馈
4. **🎨 视觉一致**: 保持与 Revit 相似的视觉风格

---

## ❓ 常见问题

### ❓ 如何更改高亮颜色？
```csharp
builder.SetHighlightColor(Colors.Blue, 0.7f);  // 🔵 蓝色高亮
```

### ❓ 如何禁用所有交互？
```csharp
builder.SetHoverHighlightEnabled(false)
       .SetClickHighlightEnabled(false);
```

### ❓ 如何获取点击位置的世界坐标？
```csharp
builder.OnModelSelected += (sender, args) => 
{
    var worldPosition = args.HitPoint;  // 🌍 世界坐标
};
```

---


**🎉 开始使用 HelixViewport3DBuilder 创建出色的 3D 可视化体验吧！**