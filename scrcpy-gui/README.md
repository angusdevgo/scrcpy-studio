# Scrcpy Studio (现代化 GUI 增强版)

基于 [Genymobile/scrcpy](https://github.com/Genymobile/scrcpy) 进行二次开发的 Android 投屏控制台。

## ✨ 特性与二改亮点

- 🎨 **极客暗黑 Studio 控制面板**：采用 Linear / Raycast 设计系统，高质感深色主题与微光动效。
- ⚡ **双击零黑框**：彻底隐藏启动时的 CMD 黑框窗口，纯净原生体验。
- 🖥️ **一键投屏与管理**：
  - 自动发现已连接的 USB/无线 ADB 设备。
  - 支持快捷配置分辨率上限、120 FPS 高刷、码率及硬件编解码器（H.264 / H.265 / AV1）。
  - 支持快捷切换息屏投屏、窗口置顶、保持常亮、无边框模式等。
- 🛠️ **轻量免依赖**：GUI 部分采用纯 C# 编写，内置一键批处理脚本，依托 Windows 原生 .NET Framework 直接编译，无需安装庞大的 IDE 环境。

## 🚀 编译与运行

### 1. 编译 GUI 控制台
进入 `scrcpy-gui` 目录，双击运行 `build_gui.bat` 即可生成 `ScrcpyGUI.exe`。

### 2. 部署与使用
将生成的 `ScrcpyGUI.exe` 与官方编译的 `scrcpy.exe`、`adb.exe`、`scrcpy-server` 置于同一目录下即可直接运行。
