# FaceCompare

FaceCompare is a cross-platform face recognition and matching application built with **.NET MAUI Blazor Hybrid** and **FaceAiSharp**.

---

## 🎯 What the App Does

FaceCompare allows you to upload two face images and determine whether both images show the same person. It performs fully offline, local AI-powered facial landmark alignment and feature extraction to generate high-precision facial embeddings and compute their similarity.

- **Header Title**: *"Is This  the Same Person"*
- **Image Inputs**: Two dedicated upload buttons (*"Upload the first photo"* and *"Upload the second photo"*) with reserved preview boxes to eliminate layout shifting.
- **Comparison Output**:
  - **Match**: `That's a Match`
  - **No Match**: `Sorry you Don't Look a Like`

---

## 🧠 How It Works

1. **Face Detection & Landmark Localization**:
   The application uses **SCRFD** (Sample and Computation Redistribution for Efficient Face Detection) via `FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks()` to detect faces and extract five key facial landmarks (two eyes, nose tip, two mouth corners).

2. **Facial Alignment**:
   `AlignFaceUsingLandmarks` aligns the face by rotating and cropping it into a standardized 112x112 pixel face crop where the eyes are aligned horizontally. If no face landmarks are detected, it falls back to a standard resize.

3. **ArcFace Embeddings Generation**:
   The aligned face is passed to **ArcFace** (ResNet-100) via `FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator()`, producing a 512-dimensional normalized feature embedding vector.

4. **Cosine Similarity / Dot Product**:
   Since the ArcFace embedding vectors are $L_2$-normalized, their cosine similarity equals their dot product:
   $$\text{Similarity} = \mathbf{e}_1 \cdot \mathbf{e}_2 = \sum_{i=1}^{512} e_{1,i} \cdot e_{2,i}$$
   
   - **Threshold Rule**:
     - If $\text{Similarity} \ge 0.42 \implies$ **"That's a Match"**
     - If $\text{Similarity} < 0.42 \implies$ **"Sorry you Don't Look a Like"**

5. **ONNX Runtime Inference**:
   All inference executes locally on the machine via `Microsoft.ML.OnnxRuntime`, ensuring complete privacy without sending images to any external server or cloud API.

---

## 🚀 How to Use

1. Launch the application (`FaceCompare.exe` on Windows).
2. Click **"Upload the first photo"** to select an image from your device.
3. Click **"Upload the second photo"** to select the second image.
4. Click the **"Compare"** button.
5. The result (*"That's a Match"* or *"Sorry you Don't Look a Like"*) will appear below the button.

---

## 📦 Releases & Platform Distribution

All release artifacts are organized in the `release/` folder:

```
release/
├── FaceCompare-Windows-x64.zip    # Ready-to-use zipped Windows release
├── windows/                       # Windows x64 binaries & executable
│   └── FaceCompare.exe
```

### 1. 🪟 Windows Release (`release/windows/`)
- **Executable**: `release/windows/FaceCompare.exe`
- **ZIP Package**: `release/FaceCompare-Windows-x64.zip`
- **Build Command**:
  ```powershell
  dotnet publish -f net10.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=None -o release/windows
  ```
- **Single-file EXE command**:
  ```powershell
  dotnet publish FaceCompare.csproj -f net10.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=None -p:PublishSingleFile=true -p:SelfContained=false -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:UseAppHost=true -o release/singlefile
  ```
  This keeps the same MAUI Windows app while emitting a single executable file for the Windows target instead of the large folder publish bundle.
- **Requirements**: Windows 10 (version 1809+) or Windows 11.
---

## 🛠️ Tech Stack & Dependencies

- **Framework**: .NET MAUI / ASP.NET Core Blazor Hybrid (`net10.0`)
- **Vision Models**: [FaceAiSharp.Bundle](https://github.com/georg-jung/FaceAiSharp) (`v0.6.35`)
  - SCRFD 2.5G face detection with 5-point landmark localization
  - ArcFace ResNet-100 face recognition model
- **Machine Learning Engine**: Microsoft ONNX Runtime (`v1.26.0`)
- **Image Processing**: SixLabors ImageSharp (`v3.1.12`)
