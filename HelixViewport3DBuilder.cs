using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using HelixToolkit.Wpf.SharpDX.Controls;
using GeometryModel3D = HelixToolkit.Wpf.SharpDX.GeometryModel3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using PhongMaterial = HelixToolkit.Wpf.SharpDX.PhongMaterial;

namespace Su.Revit.HelixToolkit.SharpDX
{
    /// <summary>
    /// HelixViewport3D 构建工具类（SharpDX 版本）
    /// <para>
    /// 该类用于简化 Revit GeometryObject 在 HelixToolkit.Wpf.SharpDX 中的渲染与显示，
    /// 提供快速初始化视口、加载几何对象、设置相机等功能。
    /// </para>
    /// </summary>
    public sealed class HelixViewport3DBuilder
    {
        /// <summary>
        /// 坐标系转换矩阵：交换 Y 和 Z 轴。
        /// </summary>
        private static readonly MatrixTransform3D swapTransform = new MatrixTransform3D(
            new Matrix3D(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1)
        );

        private readonly Document document;

        /// <summary>
        /// 存储原始材质的字典
        /// </summary>
        private readonly Dictionary<GeometryModel3D, Material> _originalMaterials;

        /// <summary>
        /// 存储模型到 GeometryObjectOptions 的映射
        /// </summary>
        private readonly Dictionary<
            GeometryModel3D,
            GeometryObjectOptions
        > _modelToGeometryObjectMap;

        /// <summary>
        /// 当前高亮的模型和对应的控制器
        /// </summary>
        private readonly Dictionary<
            MaterialGeometryModel3D,
            HighlightController
        > _highlightedModels;

        /// <summary>
        /// 高亮材质
        /// </summary>
        private Material _highlightMaterial;

        /// <summary>
        /// 悬停材质（高亮颜色的半透明版本）
        /// </summary>
        private Material _hoverMaterial;

        // 闪烁相关字段
        private bool _isHighlightBlinking;
        private TimeSpan _blinkInterval;

        // 高亮功能开关
        private bool _enableHoverHighlight;
        private bool _enableClickHighlight;

        /// <summary>
        /// 当前悬停的模型
        /// </summary>
        private MaterialGeometryModel3D _currentHoveredModel;

        /// <summary>
        /// 模型选中事件
        /// </summary>
        public event EventHandler<ModelSelectedEventArgs> OnModelSelected;

        /// <summary>
        /// 模型取消选中事件
        /// </summary>
        public event EventHandler OnModelDeselected;

        /// <summary>
        /// 当前使用的 Helix Viewport 实例。
        /// </summary>
        public Viewport3DX Viewport { get; }

        /// <summary>
        /// 渲染参数设置，例如抗锯齿、背景色等。
        /// </summary>
        public Viewport3DXOptions VisualOptions { get; set; }

        /// <summary>
        /// 构造函数（私有）。
        /// 使用 <see cref="Init"/> 方法进行初始化。
        /// </summary>
        /// <param name="document">Revit 文档对象。</param>
        /// <param name="visualOptions">视口渲染选项。</param>
        private HelixViewport3DBuilder(
            Document document,
            IEnumerable<GeometryObjectOptions> geometryObjects,
            Viewport3DXOptions visualOptions
        )
        {
            this.document = document;
            this.VisualOptions = visualOptions ?? new Viewport3DXOptions();

            // 初始化材质存储和模型映射
            _originalMaterials = new Dictionary<GeometryModel3D, Material>();
            _modelToGeometryObjectMap = new Dictionary<GeometryModel3D, GeometryObjectOptions>();
            _highlightedModels = new Dictionary<MaterialGeometryModel3D, HighlightController>();
            _highlightMaterial = CreateHighlightMaterial();
            _hoverMaterial = CreateHoverMaterial(_highlightMaterial);

            // 初始化闪烁相关字段
            _isHighlightBlinking = false;
            _blinkInterval = TimeSpan.FromMilliseconds(50);

            // 当前悬停的模型
            _currentHoveredModel = null;

            // 高亮功能开关
            _enableHoverHighlight = true;
            _enableClickHighlight = true;

            // 创建 Helix Viewport 对象
            this.Viewport = new Viewport3DX
            {
                ZoomExtentsWhenLoaded = true, // 载入时自动缩放视图
                ShowViewCube = true, // 显示右上角视图立方体
                IsViewCubeEdgeClicksEnabled = true, // 启用视图立方体边缘点击
                IsViewCubeMoverEnabled = false,
                EnableSwapChainRendering = true, // 启用交换链渲染（性能优化）
                EnableRenderFrustum = VisualOptions.EnableRenderFrustum, // 是否开启场景外渲染擦除
                IsZoomEnabled = true, // 启用缩放
                FXAALevel = VisualOptions.FXAALevel, // 抗锯齿等级
                ViewCubeVerticalPosition = 1, // 视图立方体垂直位置（右上）
                ViewCubeHorizontalPosition = 1, // 视图立方体水平位置（右上）
                EnableMouseButtonHitTest = true, // 启用鼠标点击检测
                IsHitTestVisible = true, // 启用命中测试
                BelongsToParentWindow = true, // 视口属于宿主窗口
                BackgroundColor = visualOptions.BackgroundColor, // 背景颜色
            };

            // 创建默认特效管理器（控制渲染效果）
            var effectsManager = new DefaultEffectsManager();
            Viewport.EffectsManager = effectsManager;
            Viewport.Items.Add(new AmbientLight3D() { Color = Colors.Gray });

            // 设置视图立方体的六个面（前后左右上下）的文字与颜色
            Viewport.ViewCubeTexture = BitmapExtensions.CreateViewBoxTexture(
                effectsManager,
                "前",
                "后",
                "左",
                "右",
                "上",
                "下",
                Colors.Red.ToColor4(),
                Colors.Red.ToColor4(),
                Colors.Red.ToColor4(),
                Colors.Red.ToColor4(),
                Colors.Red.ToColor4(),
                Colors.Red.ToColor4(),
                Colors.White.ToColor4(),
                Colors.White.ToColor4(),
                Colors.White.ToColor4(),
                Colors.White.ToColor4(),
                Colors.White.ToColor4(),
                Colors.White.ToColor4()
            );

            // 鼠标操作绑定
            Viewport.InputBindings.Add(
                new MouseBinding(
                    ViewportCommands.ZoomExtents,
                    new MouseGesture(MouseAction.MiddleDoubleClick)
                )
            ); // 中键双击 → 缩放视图
            Viewport.InputBindings.Add(
                new MouseBinding(ViewportCommands.Pan, new MouseGesture(MouseAction.MiddleClick))
            ); // 中键单击拖动 → 平移
            Viewport.InputBindings.Add(
                new MouseBinding(
                    ViewportCommands.Rotate,
                    new MouseGesture(MouseAction.RightClick, ModifierKeys.Shift)
                )
            ); // Shift + 右键 → 旋转视角

            // 鼠标事件处理
            Viewport.MouseLeftButtonDown += OnViewportMouseLeftButtonDown;
            Viewport.FormMouseMove += OnViewportMouseMove;
            Viewport.MouseLeave += OnViewportMouseLeave;
            Add(geometryObjects);
        }

        /// <summary>
        /// 初始化视口构建器。
        /// </summary>
        /// <param name="document">Revit 文档对象。</param>
        /// <param name="visualOptions">视口视觉选项，可选。</param>
        /// <returns>新的 <see cref="HelixViewport3DBuilder"/> 实例。</returns>
        public static HelixViewport3DBuilder Init(
            Document document,
            IEnumerable<GeometryObjectOptions> geometryObjects,
            Viewport3DXOptions visualOptions = default
        )
        {
            return new HelixViewport3DBuilder(document, geometryObjects, visualOptions);
        }

        /// <summary>
        /// 清空场景并保留视口。
        /// </summary>
        /// <returns>当前构建器实例。</returns>
        public HelixViewport3DBuilder Clear()
        {
            // 清空前清除高亮状态
            ClearHighlight();
            ClearHoverHighlight();
            _originalMaterials.Clear();
            _modelToGeometryObjectMap.Clear();

            Viewport.Items.Clear();
            return this;
        }

        /// <summary>
        /// 向视口中添加单个模型对象。
        /// </summary>
        /// <param name="model">要添加的 3D 模型。</param>
        /// <param name="geometryObject">对应的几何对象选项</param>
        private HelixViewport3DBuilder Add(
            GeometryModel3D model,
            GeometryObjectOptions geometryObject = null
        )
        {
            if (model != null)
            {
                model.Transform = swapTransform; // 坐标系转换
                model.ToolTip = "123";
                Viewport.Items.Add(model);

                // 如果提供了 GeometryObjectOptions，则建立映射关系
                if (geometryObject != null)
                {
                    _modelToGeometryObjectMap[model] = geometryObject;
                }
            }
            return this;
        }

        /// <summary>
        /// 添加单个 GeometryObject。
        /// </summary>
        /// <param name="geometryObject">包含渲染信息的几何对象选项。</param>
        private HelixViewport3DBuilder Add(GeometryObjectOptions geometryObject)
        {
            if (geometryObject != null)
            {
                List<GeometryModel3D> geometryModel3Ds = geometryObject.ToGeometryModel3Ds(
                    document
                );
                foreach (var geometryModel3D in geometryModel3Ds)
                {
                    Add(geometryModel3D, geometryObject);
                }
            }
            return this;
        }

        /// <summary>
        /// 批量添加多个 GeometryObject。
        /// </summary>
        /// <param name="geometryObjects">几何对象选项集合。</param>
        private HelixViewport3DBuilder Add(IEnumerable<GeometryObjectOptions> geometryObjects)
        {
            if (geometryObjects != null)
            {
                foreach (var obj in geometryObjects)
                    Add(obj);
            }
            return this;
        }

        /// <summary>
        /// 根据 Revit 的 <see cref="View"/> 设置相机。
        /// <para>
        /// 自动将 Revit 的坐标系（Z-up）转换为 Helix 的坐标系（Y-up）。
        /// </para>
        /// </summary>
        /// <param name="view">Revit 视图对象。</param>
        public HelixViewport3DBuilder SetCamera(View view)
        {
            if (view != null)
            {
                // 坐标转换（Revit → WPF）
                var axis = new Vector3D(
                    view.UpDirection.X,
                    view.UpDirection.Z,
                    -view.UpDirection.Y
                );

                // 绕 UpDirection 旋转 180 度，保证朝向一致
                AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, 180);
                RotateTransform3D transform = new RotateTransform3D(rotation);

                // 计算视图方向与上方向
                var rotatedLookDirection = transform.Transform(
                    new Vector3D(view.ViewDirection.X, view.ViewDirection.Z, -view.ViewDirection.Y)
                );
                var rotatedUpDirection = transform.Transform(
                    new Vector3D(view.UpDirection.X, view.UpDirection.Z, -view.UpDirection.Y)
                );
                rotatedLookDirection.Normalize();
                rotatedUpDirection.Normalize();

                // 创建正交相机
                var camera = new OrthographicCamera()
                {
                    Position = new Point3D(view.Origin.X, view.Origin.Z, -view.Origin.Y),
                    LookDirection = rotatedLookDirection,
                    UpDirection = rotatedUpDirection,
                    NearPlaneDistance = 0.00001,
                    FarPlaneDistance = double.MaxValue,
                };
                Viewport.Camera = camera;
            }
            return this;
        }

        /// <summary>
        /// 根据坐标与方向设置相机。
        /// <para>
        /// 用于自定义相机位置与朝向，而不依赖 Revit 的 <see cref="View"/>。
        /// </para>
        /// </summary>
        /// <param name="position">相机位置。</param>
        /// <param name="viewDirection">观察方向。</param>
        /// <param name="upDirection">相机上方向。</param>
        public HelixViewport3DBuilder SetCamera(XYZ position, XYZ viewDirection, XYZ upDirection)
        {
            // 坐标转换：Revit(Y-up, Z-forward) → WPF(Z-up, Y-forward)
            var transformedPosition = new Point3D(position.X, position.Z, -position.Y);
            var transformedViewDir = new Vector3D(
                viewDirection.X,
                viewDirection.Z,
                -viewDirection.Y
            );
            var transformedUpDir = new Vector3D(upDirection.X, upDirection.Z, -upDirection.Y);

            // 绕上方向旋转 180 度（保证方向一致）
            var rotation = new AxisAngleRotation3D(transformedUpDir, 180);
            var transform = new RotateTransform3D(rotation);

            var rotatedLookDirection = transform.Transform(transformedViewDir);
            var rotatedUpDirection = transform.Transform(transformedUpDir);

            // 创建正交相机
            var camera = new OrthographicCamera
            {
                Position = transformedPosition,
                LookDirection = rotatedLookDirection,
                UpDirection = rotatedUpDirection,
            };

            Viewport.Camera = camera;
            return this;
        }

        /// <summary>
        /// 设置是否启用鼠标悬停高亮
        /// </summary>
        /// <param name="enable">是否启用</param>
        /// <returns>当前构建器实例</returns>
        public HelixViewport3DBuilder SetHoverHighlightEnabled(bool enable)
        {
            _enableHoverHighlight = enable;
            if (!enable)
            {
                ClearHoverHighlight();
            }
            return this;
        }

        /// <summary>
        /// 设置是否启用鼠标点击高亮
        /// </summary>
        /// <param name="enable">是否启用</param>
        /// <returns>当前构建器实例</returns>
        public HelixViewport3DBuilder SetClickHighlightEnabled(bool enable)
        {
            _enableClickHighlight = enable;
            if (!enable)
            {
                ClearHighlight();
            }
            return this;
        }

        /// <summary>
        /// 获取鼠标悬停高亮是否启用
        /// </summary>
        /// <returns>是否启用</returns>
        public bool IsHoverHighlightEnabled()
        {
            return _enableHoverHighlight;
        }

        /// <summary>
        /// 获取鼠标点击高亮是否启用
        /// </summary>
        /// <returns>是否启用</returns>
        public bool IsClickHighlightEnabled()
        {
            return _enableClickHighlight;
        }

        /// <summary>
        /// 设置高亮是否闪烁
        /// </summary>
        /// <param name="blinking">是否启用闪烁效果</param>
        /// <param name="blinkInterval">闪烁间隔时间（毫秒），可选</param>
        public HelixViewport3DBuilder SetHighlightBlinking(bool blinking, int blinkInterval = 50)
        {
            _isHighlightBlinking = blinking;
            _blinkInterval = TimeSpan.FromMilliseconds(blinkInterval);

            // 更新所有已高亮模型的闪烁状态
            foreach (var controller in _highlightedModels.Values)
            {
                if (blinking)
                {
                    controller.StartBlinking(_blinkInterval);
                }
                else
                {
                    controller.StopBlinking();
                    controller.ApplyStaticHighlight();
                }
            }
            return this;
        }

        /// <summary>
        /// 设置自定义高亮材质
        /// </summary>
        /// <param name="highlightMaterial">高亮材质</param>
        public HelixViewport3DBuilder SetHighlightColor(
            Autodesk.Revit.DB.Material highlightMaterial
        )
        {
            var newHighlightMaterial = highlightMaterial.ToPhongMaterial();
            _highlightMaterial = newHighlightMaterial;
            UpdateHoverMaterial(); // 同时更新悬停材质

            // 更新所有高亮控制器的高亮材质
            foreach (var controller in _highlightedModels.Values)
            {
                controller.UpdateHighlightMaterial(newHighlightMaterial);
            }
            return this;
        }

        /// <summary>
        /// 设置自定义高亮材质
        /// </summary>
        /// <param name="color"> 高亮颜色。</param>
        /// <param name="alpha"> 高亮透明度。</param>
        /// <returns></returns>
        public HelixViewport3DBuilder SetHighlightColor(
            System.Windows.Media.Color color,
            float alpha = 0.5f
        )
        {
            var material = color.ToColor4(alpha).ToPhongMaterial();
            _highlightMaterial = material;
            UpdateHoverMaterial(); // 同时更新悬停材质

            // 更新所有高亮控制器的高亮材质
            foreach (var controller in _highlightedModels.Values)
            {
                controller.UpdateHighlightMaterial(material);
            }
            return this;
        }

        /// <summary>
        /// 从外部传入 GeometryObject 集合并高亮显示它们
        /// </summary>
        /// <param name="geometryObjects">要高亮的几何对象集合</param>
        public HelixViewport3DBuilder SetHighlightGeometryObjects(
            IEnumerable<GeometryObject> geometryObjects
        )
        {
            if (geometryObjects == null)
                return this;

            // 清除之前的高亮
            ClearHighlight();

            // 查找与传入的 GeometryObject 对应的模型
            var modelsToHighlight = new List<MaterialGeometryModel3D>();

            foreach (var geometryObject in geometryObjects)
            {
                // 通过映射关系查找对应的模型
                var matchingModels = _modelToGeometryObjectMap
                    .Where(kvp => kvp.Value.GeometryObject == geometryObject)
                    .Select(kvp => kvp.Key)
                    .OfType<MaterialGeometryModel3D>()
                    .Where(model => model != null);

                modelsToHighlight.AddRange(matchingModels);
            }

            if (modelsToHighlight.Any())
            {
                // 为所有找到的模型添加高亮
                foreach (var model in modelsToHighlight)
                {
                    // 这里应该直接添加高亮，不受点击高亮开关影响
                    // 因为这是程序主动调用的高亮，不是用户点击
                    AddHighlightToModel(model);
                }
            }
            return this;
        }

        /// <summary>
        /// 高亮单个 GeometryObject
        /// </summary>
        /// <param name="geometryObject">要高亮的几何对象</param>
        public HelixViewport3DBuilder HighlightGeometryObject(GeometryObject geometryObject)
        {
            if (geometryObject != null)
            {
                SetHighlightGeometryObjects(new[] { geometryObject });
            }
            return this;
        }

        /// <summary>
        /// 获取当前选中的模型
        /// </summary>
        /// <returns>当前选中的模型集合</returns>
        public IEnumerable<MaterialGeometryModel3D> GetSelectedModels()
        {
            return _highlightedModels.Keys;
        }

        /// <summary>
        /// 获取当前选中的 GeometryObject
        /// </summary>
        /// <returns>当前选中的几何对象集合</returns>
        public IEnumerable<GeometryObject> GetSelectedGeometryObjects()
        {
            return _highlightedModels
                .Keys.Where(model => _modelToGeometryObjectMap.ContainsKey(model))
                .Select(model => _modelToGeometryObjectMap[model].GeometryObject);
        }

        /// <summary>
        /// 清除所有高亮显示，恢复原始材质
        /// </summary>
        public void ClearHighlight()
        {
            foreach (var model in _highlightedModels.Keys.ToList())
            {
                RemoveHighlightFromModel(model);
            }
        }

        /// <summary>
        /// 鼠标左键点击事件处理
        /// </summary>
        private void OnViewportMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_enableClickHighlight)
                return;

            var viewport = sender as Viewport3DX;
            var hitPoint = e.GetPosition(viewport).ToVector2();

            // 查找最近的模型
            var findResult = viewport.FindNearest(
                hitPoint,
                out Vector3 vector2,
                out Vector3 normal,
                out object model
            );

            if (findResult && model != null && model is MaterialGeometryModel3D hitModel)
            {
                // 清除悬停高亮
                ClearHoverHighlight();

                // 检查是否按住Ctrl键进行多选
                bool isMultiSelect =
                    (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

                if (!isMultiSelect)
                {
                    // 单选模式：清除之前的高亮
                    ClearHighlight();
                }

                // 切换选中状态
                if (_highlightedModels.ContainsKey(hitModel))
                {
                    // 取消选中
                    RemoveHighlightFromModel(hitModel);
                }
                else
                {
                    // 选中模型
                    AddHighlightToModel(hitModel);
                }

                // 获取对应的 GeometryObjectOptions
                GeometryObjectOptions geometryObject = null;
                if (_modelToGeometryObjectMap.TryGetValue(hitModel, out var geoObj))
                {
                    geometryObject = geoObj;
                }

                // 触发选中事件
                OnModelSelected?.Invoke(
                    this,
                    new ModelSelectedEventArgs(hitModel, vector2, geometryObject?.GeometryObject)
                );
            }
            else
            {
                // 没有选中任何模型，清除高亮
                ClearHighlight();
                OnModelDeselected?.Invoke(this, EventArgs.Empty);
            }

            e.Handled = true;
        }

        /// <summary>
        /// 鼠标移动事件处理 - 处理悬停高亮
        /// </summary>
        private void OnViewportMouseMove(object sender, WinformHostExtend.FormMouseMoveEventArgs e)
        {
            if (!_enableHoverHighlight)
                return;

            var viewport = sender as Viewport3DX;
            var hitPoint = e.Location.ToVector2();

            // 查找最近的模型
            var findResult = viewport.FindNearest(hitPoint, out _, out _, out object model);

            if (findResult && model != null && model is MaterialGeometryModel3D hoveredModel)
            {
                // 设置手型光标
                viewport.Cursor = Cursors.Hand;

                // 如果悬停的模型发生变化
                if (_currentHoveredModel != hoveredModel)
                {
                    // 清除之前的悬停高亮
                    ClearHoverHighlight();

                    // 如果新悬停的模型没有被选中，则应用悬停高亮
                    if (!_highlightedModels.ContainsKey(hoveredModel))
                    {
                        ApplyHoverHighlight(hoveredModel);
                    }

                    _currentHoveredModel = hoveredModel;
                }
            }
            else
            {
                // 没有悬停在模型上
                viewport.Cursor = null;
                ClearHoverHighlight();
            }
        }

        /// <summary>
        /// 鼠标离开视口事件处理
        /// </summary>
        private void OnViewportMouseLeave(object sender, EventArgs e)
        {
            ClearHoverHighlight();
        }

        /// <summary>
        /// 为单个模型添加高亮
        /// </summary>
        private void AddHighlightToModel(MaterialGeometryModel3D model)
        {
            if (model == null)
                return;

            // 如果之前没有存储过原始材质，则存储它
            if (!_originalMaterials.ContainsKey(model))
            {
                _originalMaterials[model] = model.Material;
            }

            // 创建高亮控制器
            var controller = new HighlightController(
                model,
                _originalMaterials[model],
                _highlightMaterial
            );
            _highlightedModels[model] = controller;

            // 应用高亮（根据闪烁设置）
            if (_isHighlightBlinking)
            {
                controller.StartBlinking(_blinkInterval);
            }
            else
            {
                // 非闪烁模式：直接应用高亮材质
                controller.ApplyStaticHighlight();
            }
        }

        /// <summary>
        /// 从单个模型移除高亮
        /// </summary>
        private void RemoveHighlightFromModel(MaterialGeometryModel3D model)
        {
            if (model == null || !_highlightedModels.ContainsKey(model))
                return;

            // 停止闪烁并恢复原始材质
            var controller = _highlightedModels[model];
            controller.StopBlinking();
            controller.RestoreOriginalMaterial();

            _highlightedModels.Remove(model);
        }

        /// <summary>
        /// 应用悬停高亮
        /// </summary>
        private void ApplyHoverHighlight(MaterialGeometryModel3D model)
        {
            if (model == null || _highlightedModels.ContainsKey(model))
                return;

            // 存储原始材质（如果还没有存储）
            if (!_originalMaterials.ContainsKey(model))
            {
                _originalMaterials[model] = model.Material;
            }

            // 应用悬停材质
            model.Material = _hoverMaterial;
        }

        /// <summary>
        /// 清除悬停高亮
        /// </summary>
        private void ClearHoverHighlight()
        {
            if (
                _currentHoveredModel != null
                && !_highlightedModels.ContainsKey(_currentHoveredModel)
            )
            {
                // 恢复原始材质
                if (_originalMaterials.TryGetValue(_currentHoveredModel, out var originalMaterial))
                {
                    _currentHoveredModel.Material = originalMaterial;
                }
            }
            _currentHoveredModel = null;
        }

        /// <summary>
        /// 创建高亮材质
        /// </summary>
        /// <returns>高亮材质实例</returns>
        private Material CreateHighlightMaterial()
        {
            return new PhongMaterial
            {
                Name = "HighlightMaterial",
                DiffuseColor = new Color4(1.0f, 0f, 0.0f, 0.8f), // 红色漫反射
                EmissiveColor = new Color4(
                    1.0f,
                    0f,
                    0.0f,
                    0.8f
                ) // 轻微红色自发光
                ,
            };
        }

        /// <summary>
        /// 创建悬停材质（高亮颜色的半透明版本）
        /// </summary>
        private Material CreateHoverMaterial(Material highlightMaterial)
        {
            if (highlightMaterial is PhongMaterial phongHighlight)
            {
                return new PhongMaterial
                {
                    Name = "HoverMaterial",
                    // 使用高亮颜色的半透明版本
                    DiffuseColor = new Color4(
                        phongHighlight.DiffuseColor.Red,
                        phongHighlight.DiffuseColor.Green,
                        phongHighlight.DiffuseColor.Blue,
                        phongHighlight.DiffuseColor.Alpha * 0.5f
                    ),
                    EmissiveColor = new Color4(
                        phongHighlight.DiffuseColor.Red,
                        phongHighlight.DiffuseColor.Green,
                        phongHighlight.DiffuseColor.Blue,
                        phongHighlight.DiffuseColor.Alpha * 0.5f
                    ),
                    SpecularColor = phongHighlight.SpecularColor,
                    SpecularShininess = phongHighlight.SpecularShininess,
                };
            }

            // 默认悬停材质（红色半透明）
            return new PhongMaterial
            {
                Name = "HoverMaterial",
                DiffuseColor = new Color4(1.0f, 0f, 0.0f, 0.5f), // 红色半透明
                EmissiveColor = new Color4(
                    0.5f,
                    0f,
                    0.0f,
                    0.3f
                ) // 轻微自发光
                ,
            };
        }

        /// <summary>
        /// 更新悬停材质（当高亮材质改变时）
        /// </summary>
        private void UpdateHoverMaterial()
        {
            _hoverMaterial = CreateHoverMaterial(_highlightMaterial);
        }
    }

    /// <summary>
    /// 高亮控制器 - 负责单个模型的高亮和闪烁控制
    /// </summary>
    internal class HighlightController
    {
        private readonly MaterialGeometryModel3D _model;
        private readonly Material _originalMaterial;
        private Material _highlightMaterial;
        private DispatcherTimer _blinkTimer;
        private double _blinkPhase;

        public HighlightController(
            MaterialGeometryModel3D model,
            Material originalMaterial,
            Material highlightMaterial
        )
        {
            _model = model;
            _originalMaterial = originalMaterial;
            _highlightMaterial = highlightMaterial;
            _blinkTimer = new DispatcherTimer();
            _blinkTimer.Tick += OnBlinkTimerTick;
            _blinkPhase = 0;
        }

        /// <summary>
        /// 开始闪烁
        /// </summary>
        public void StartBlinking(TimeSpan interval)
        {
            _blinkTimer.Interval = interval;
            _blinkTimer.Start();
        }

        /// <summary>
        /// 停止闪烁
        /// </summary>
        public void StopBlinking()
        {
            _blinkTimer.Stop();
        }

        /// <summary>
        /// 应用静态高亮
        /// </summary>
        public void ApplyStaticHighlight()
        {
            _model.Material = _highlightMaterial;
        }

        /// <summary>
        /// 恢复原始材质
        /// </summary>
        public void RestoreOriginalMaterial()
        {
            _model.Material = _originalMaterial;
        }

        /// <summary>
        /// 更新高亮材质
        /// </summary>
        public void UpdateHighlightMaterial(Material newHighlightMaterial)
        {
            _highlightMaterial = newHighlightMaterial;

            // 如果当前没有闪烁，立即应用新的高亮材质
            if (!_blinkTimer.IsEnabled)
            {
                ApplyStaticHighlight();
            }
        }

        private void OnBlinkTimerTick(object sender, EventArgs e)
        {
            // 更新闪烁相位
            _blinkPhase += 0.1;
            if (_blinkPhase > 1.0)
                _blinkPhase = 0.0;

            // 计算插值因子
            double t = (Math.Sin(_blinkPhase * Math.PI * 2 - Math.PI / 2) + 1) / 2;

            // 应用混合材质
            var blendedMaterial = BlendMaterials(_originalMaterial, _highlightMaterial, t);
            _model.Material = blendedMaterial;
        }

        private Material BlendMaterials(Material material1, Material material2, double t)
        {
            if (material1 is PhongMaterial phong1 && material2 is PhongMaterial phong2)
            {
                return new PhongMaterial
                {
                    Name = "BlendedMaterial",
                    AmbientColor = BlendColor4(phong1.AmbientColor, phong2.AmbientColor, t),
                    DiffuseColor = BlendColor4(phong1.DiffuseColor, phong2.DiffuseColor, t),
                    SpecularColor = BlendColor4(phong1.SpecularColor, phong2.SpecularColor, t),
                    EmissiveColor = BlendColor4(phong1.EmissiveColor, phong2.EmissiveColor, t),
                    SpecularShininess = (float)(
                        phong1.SpecularShininess * (1 - t) + phong2.SpecularShininess * t
                    ),
                };
            }
            return t > 0.5 ? material2 : material1;
        }

        private Color4 BlendColor4(Color4 color1, Color4 color2, double t)
        {
            return new Color4(
                (float)(color1.Red * (1 - t) + color2.Red * t),
                (float)(color1.Green * (1 - t) + color2.Green * t),
                (float)(color1.Blue * (1 - t) + color2.Blue * t),
                (float)(color1.Alpha * (1 - t) + color2.Alpha * t)
            );
        }
    }

    /// <summary>
    /// 模型选中事件参数
    /// </summary>
    public sealed class ModelSelectedEventArgs : EventArgs
    {
        internal ModelSelectedEventArgs(
            MaterialGeometryModel3D selectedModel,
            Vector3 hitPoint,
            GeometryObject geometryObject
        )
        {
            SelectedModel = selectedModel;
            HitPoint = hitPoint;
            GeometryObject = geometryObject;
        }

        private ModelSelectedEventArgs() { }

        /// <summary>
        /// 选中的模型
        /// </summary>
        public MaterialGeometryModel3D SelectedModel { get; }

        /// <summary>
        /// 点击位置的世界坐标
        /// </summary>
        public Vector3 HitPoint { get; }

        /// <summary>
        /// 对应的几何对象
        /// </summary>
        public GeometryObject GeometryObject { get; }
    }
}
