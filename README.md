# CH9329 键盘鼠标控制器

一个基于 C# WinForms 的 CH9329 控制工具，用于通过串口连接设备并模拟键盘/鼠标输入，适合文本发送、常用快捷键操作、键盘同步以及远程鼠标控制等场景。

## 功能特性

- 支持通过串口连接 CH9329 设备
- 支持文本快速发送与文件内容导入
- 支持多档发送速度切换
- 支持常用快捷键发送
- 支持键盘同步
- 支持远程鼠标模式
- 支持全局快捷键快速导入文本：
  - `Alt+R`：将剪贴板文本导入输入框
  - `Alt+T`：将当前选中文本导入输入框

## 主要界面能力

### 字符发送
- 输入或导入文本后发送到目标设备
- 支持“稳定 / 快速 / 极速”发送模式
- 支持清空输入框并自动回到输入焦点

### 快捷发送
- 回车、空格、退格
- 鼠标左键、鼠标右键
- Caps Lock、Num Lock、Scroll Lock

### 组合键
- `Ctrl+A`
- `Ctrl+C`
- `Ctrl+V`
- `Ctrl+X`
- `Ctrl+Z`
- `Win+D`
- `Alt+Tab`
- 方向键

### 同步与控制
- 键盘同步开关
- 远程鼠标模式开关
- 功能键保持（Ctrl / Shift / Alt）

## 运行环境

- Windows
- .NET 6.0
- CH9329 设备

## 项目结构

- [MainForm.cs](MainForm.cs)：主界面逻辑
- [MainForm.Designer.cs](MainForm.Designer.cs)：界面布局
- [CH9329Keyboard.cs](CH9329Keyboard.cs)：键盘/鼠标发送逻辑
- [CH9329HIDManager.cs](CH9329HIDManager.cs)：HID 相关支持
- [SerialPortManager.cs](SerialPortManager.cs)：串口通信封装
- [InputHook.cs](InputHook.cs)：键盘/鼠标钩子处理
- [TextTransferProtocol.cs](TextTransferProtocol.cs)：文本编码传输逻辑

## 构建方式

```bash
dotnet build KeyboardController.csproj
```

## 发布包

已提供 Windows x64 发行包，可在仓库 Release 中下载：

- 文件名：`CH9329_comkeybord_v1.0.0_win-x64.zip`
- 运行要求：安装 .NET 6 Runtime
- 使用方式：解压后运行 `KeyboardController.exe`

## 使用说明

1. 连接 CH9329 设备
2. 选择串口与波特率
3. 点击“连接”
4. 在主界面中选择需要的发送或同步功能

## 说明

当前仓库仅保留项目本体代码，不包含外部参考工具、PDF 资料和临时构建产物。
