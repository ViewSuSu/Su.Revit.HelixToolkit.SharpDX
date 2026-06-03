 [English](README.en.md) | [简体中文](README.md)

![Revit Support](https://img.shields.io/badge/Revit-2013~2026-green)
![Platform](https://img.shields.io/badge/Platform-WPF%2BSharpDX-orange)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

# 🚀 Su.Revit.HelixToolkit.SharpDX 使用说明文档

## 🌐 项目地址

**GitHub**: https://github.com/ViewSuSu/Su.Revit.HelixToolkit.SharpDX  
**Gitee**: https://gitee.com/SususuChang/su.-revit.-helix-toolkit.-sharp-dx

## 🎬 演示动画

![功能演示](HD.gif)

---

## 📦 安装方式

### 通过 NuGet 安装（推荐）

```bash
# Package Manager
Install-Package Su.Revit.HelixToolkit.SharpDX

# .NET CLI
dotnet add package Su.Revit.HelixToolkit.SharpDX
```

### 包引用 (csproj)

```xml
<PackageReference Include="Su.Revit.HelixToolkit.SharpDX" Version="1.0.0" />
```

---

## 📖 简介

Su.Revit.HelixToolkit.SharpDX 是一个专为 Revit 插件开发设计的高性能 3D 可视化工具库。基于 HelixToolkit.Wpf.SharpDX 开发，提供了简单易用的 API 来在 Revit 插件中创建功能丰富的 3D 视图窗口。

**核心特性**:
- 🚀 **高性能渲染**: 对 Solid 三角面进行了索引优化，能够承载海量三角面数据的 Solid 模型
- 🎯 **完整交互**: 支持鼠标悬停高亮、点击选择、多选、旋转、缩放、平移等完整交互功能
- 📐 **坐标系适配**: 自动处理 Revit 与 Helix 坐标系转换，无缝集成
- 🎨 **材质系统**: 支持 Revit 原生材质、自定义颜色、贴图材质等多种渲染方式
- ⚡ **内存优化**: 高效的几何数据管理和内存释放机制

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

## 🎨 GeometryObjectOptions 使用指南

### 📝 基本配置

`GeometryObjectOptions` 用于配置几何对象的渲染方式：

#### 使用 Revit 材质

```csharp
var options = new GeometryObjectOptions(
    geometryObject,    // 📐 Revit 几何对象
    revitMaterial      // 🎨 Revit 材质（可选）
);
```

#### 使用自定义颜色

```csharp
var options = new GeometryObjectOptions(
    geometryObject,           // 📐 Revit 几何对象
    Colors.Blue,              // 🔵 自定义颜色
    0.8f                      // 💧 透明度 (0-1)
);
```

#### 使用贴图材质

```csharp
var options = new GeometryObjectOptions(
    geometryObject,           // 📐 Revit 几何对象
    textureStream,            // 🖼️ 贴图流
    Colors.White,             // ⚪ 自发光颜色
    1.0f                      // 💧 透明度
);
```

### ⚙️ 渲染参数配置

```csharp
var options = new GeometryObjectOptions(geometryObject, material)
{
    LevelOfDetail = 0.8,                              // 🎯 细节等级 (0-1)
    MinAngleInTriangle = 0,                           // 📐 三角面最小角度
    MinExternalAngleBetweenTriangles = Math.PI / 4,   // 📏 相邻面最小外角
    IsDrawSolidEdges = true,                          // 📏 绘制轮廓线
    SolidEdgeThickness = 2f,                          // 🖊️ 轮廓线粗细
    SolidEdgeSmoothness = 10f                         // ✨ 轮廓线平滑度
};
```

### 🔧 参数说明

| 参数 | 说明 | 默认值 | 影响 |
|------|------|--------|------|
| `LevelOfDetail` | 渲染细节等级 | 0.5 | 值越高网格越密集，精度越高但性能消耗越大 |
| `MinAngleInTriangle` | 三角面最小夹角 | 0 | 控制网格生成时的平滑度 |
| `MinExternalAngleBetweenTriangles` | 相邻三角面最小外角 | 2π | 判断曲面平滑过渡程度 |
| `IsDrawSolidEdges` | 是否绘制轮廓线 | true | 显示边界线条 |
| `SolidEdgeThickness` | 轮廓线粗细 | 2f | 线条的像素宽度 |
| `SolidEdgeSmoothness` | 轮廓线平滑度 | 10f | 数值越大边缘越平滑 |

---

## 💡 使用技巧

### 🚀 性能优化

- ✅ 使用 `EnableSwapChainRendering` 提升渲染性能
- ✅ 合理设置 `FXAALevel` 平衡画质和性能
- ✅ 及时调用 `Clear()` 释放资源
- ✅ 根据需求调整 `LevelOfDetail`，避免不必要的细节
- ✅ 利用 Solid 三角面索引优化处理海量数据

### 🎯 最佳实践

1. **📱 响应式设计**: 视口会自动适应容器大小
2. **🔄 实时更新**: 支持动态添加/移除几何对象
3. **🎮 用户友好**: 提供直观的鼠标交互反馈
4. **🎨 视觉一致**: 保持与 Revit 相似的视觉风格
5. **⚡ 性能平衡**: 根据场景复杂度调整渲染参数
6. **💾 内存管理**: 及时清理不再使用的几何对象

### 🔄 场景管理

```csharp
// 🧹 清空场景
builder.Clear();

// 📦 重新添加对象
builder.Add(newGeometryObjects);

// 🎯 重置相机
builder.SetCamera(newView);
```

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

### ❓ 如何优化复杂模型的性能？
```csharp
var options = new GeometryObjectOptions(geometryObject, material)
{
    LevelOfDetail = 0.3,      // 🎯 降低细节等级
    IsDrawSolidEdges = false  // 📏 禁用轮廓线绘制
};
```

### ❓ 如何处理材质透明度？
```csharp
// 方法1: 使用颜色透明度
var options = new GeometryObjectOptions(geometryObject, Colors.Red, 0.5f);

// 方法2: 使用 Revit 材质的透明度
var material = document.GetElement(materialId) as Autodesk.Revit.DB.Material;
var options = new GeometryObjectOptions(geometryObject, material);
```

### ❓ 如何处理海量三角面的 Solid 模型？
```csharp
// 库已内置三角面索引优化，自动处理海量数据
// 只需正常创建 GeometryObjectOptions 即可
var options = new GeometryObjectOptions(largeSolidModel, material);
```

---

## 📞 技术支持

如果在使用过程中遇到问题，请检查：

- ✅ Revit 文档对象是否正确传递
- ✅ 几何对象集合是否包含有效数据
- ✅ 视口控件是否正确添加到 WPF 可视化树
- ✅ 事件处理程序是否正确注册和注销
- ✅ 渲染参数是否在合理范围内
- ✅ 内存使用是否正常，及时调用 Clear() 释放资源

### 📚 更多资源

- 📖 **完整源代码**: 请访问上面的 GitHub 或 Gitee 仓库
- 💡 **功能建议**: 欢迎提交 Pull Request 或功能建议
- 📋 **更新日志**: 查看仓库的 Release 页面获取最新版本信息

---

**🎉 开始使用 Su.Revit.HelixToolkit.SharpDX 创建出色的 3D 可视化体验吧！**


## 星曲线图 / Star History

[![Star History Chart](https://api.star-history.com/svg?repos=ViewSuSu/Su.Revit.HelixToolkit.SharpDX&type=Date)](https://star-history.com/#ViewSuSu/Su.Revit.HelixToolkit.SharpDX&Date)
