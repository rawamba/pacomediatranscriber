# 📘 PacoMediaTranscriber Architecture & System Overview

---

## ⭐ Project Story / Motivation

PacoMediaTranscriber was created to solve a practical problem we see across teams: **Video content is everywhere — training sessions, operations, customer demos — but extracting meaningful text from these recordings is difficult, inconsistent, and time-consuming.**

The goal of this project is to provide:

* A clean, browser-based experience for uploading videos
* Fast, secure user authentication using Microsoft identity
* A scalable, event-driven AWS AI pipeline (Transcribe + Rekognition)
* Seamless output delivery to Google Drive
* A modern CI/CD workflow powered by Azure DevOps

This application also serves as a **reference architecture** for cross-cloud integration between Microsoft, AWS, and Google services.

---

# 🚀 Overview

**PacoMediaTranscriber** is a cloud-native Single Page Application (SPA) built using:

* **Blazor WebAssembly (.NET 8)**
* **Microsoft Identity Platform (MSAL)**
* **Microsoft Graph API**
* **AWS video AI pipeline**
* **Azure DevOps** (CI/CD)
* **Google Drive API** (final storage)

Users authenticate using Microsoft accounts, upload videos, and receive AI-generated text results stored in Google Drive.

---

# 🧩 Architecture Summary

## Frontend (This Repository)

* Blazor WebAssembly SPA
* Uses MSAL for authentication
* Calls Microsoft Graph `/me` to show user profile
* Uploads videos to AWS S3 via pre-signed URLs
* Hosted on S3 + optionally CloudFront

## AWS Backend

* S3 bucket for video ingestion
* API Gateway + Lambda (`get-upload-url`)
* Lambda for S3 event trigger (`on-video-uploaded`)
* Step Functions workflow orchestrating:

  * Amazon Transcribe (speech-to-text)
  * Amazon Rekognition (video text extraction)
* Outputs merged into a text file and uploaded to Google Drive

## CI/CD

* Source code in GitHub
* Azure DevOps builds the Blazor WASM SPA
* Deploys static files to S3 via AWS CLI

---

# 📂 Project Structure

```
PacoMediaTranscriber/
│
├── Pages/
│   ├── UserProfile.razor         
│   ├── VideoUpload.razor         
│
├── Shared/
│   ├── LoginDisplay.razor        
│   ├── MainLayout.razor
│
├── Data/
│   ├── GraphClientExtensions.cs  
│
├── Services/
│   ├── ApiClient.cs              
│
├── wwwroot/
│   ├── appsettings.json          
│
├── Program.cs                    
├── PacoMediaTranscriber.csproj
└── azure-pipelines.yml          
```

---

# 🔐 Authentication Overview

Uses the **Microsoft Identity Platform (MSAL)** with PKCE.

### Redirect URIs:

**Local:**

```
https://localhost:{port}/authentication/login-callback
```

**Production:**

```
https://{cloudfront-domain}/authentication/login-callback
```

### Graph Scopes:

```
User.Read
```

---

# 🧪 Local Development

Run the app:

```
dotnet watch run
```

Sign in via MSAL → View profile → Upload videos → Verify pipeline output.

---

# ☁️ CI/CD Pipeline (Azure DevOps → AWS S3)

The pipeline performs:

1. Restore + build
2. Publish (static WASM output)
3. Upload to S3 using AWS CLI
4. Optional CloudFront invalidation

### Required Pipeline Variables:

| Name                    | Description   |
| ----------------------- | ------------- |
| `AWS_ACCESS_KEY_ID`     | IAM key       |
| `AWS_SECRET_ACCESS_KEY` | IAM secret    |
| `AWS_REGION`            | Region        |
| `S3_BUCKET_NAME`        | Target bucket |

### Deployment Command

```
aws s3 sync ./publish/wwwroot s3://$S3_BUCKET_NAME --delete
```

---

# 🏗️ High-Level Architecture Diagram (Mermaid)

```
flowchart LR
    A[User Browser\nBlazor WebAssembly SPA] -->|Sign-in| B[Microsoft Identity Platform]
    A -->|/me| C[Microsoft Graph API]

    A -->|Get Upload URL| D[API Gateway]
    A -->|Upload Video| E[S3 Bucket]

    E -->|S3 Event| F[AWS Lambda: OnVideoUploaded]
    F -->|Start Workflow| G[Step Functions]
    G -->|AI Processing| H[Transcribe & Rekognition]
    G -->|Final Output| I[Google Drive API]
    I --> J[Google Drive Storage]
```

---

# 🔮 Backend Workflow (Detailed)

```
sequenceDiagram
    autonumber

    participant U as User
    participant SPA as Blazor WASM
    participant API as API Gateway
    participant L1 as Lambda:GetUploadUrl
    participant S3 as Amazon S3
    participant L2 as Lambda:OnVideoUploaded
    participant SF as Step Functions
    participant AI as Transcribe/Rekognition
    participant GD as Google Drive API

    U->>SPA: Select Video
    SPA->>API: POST /get-upload-url
    API->>L1: Invoke Lambda
    L1->>S3: Create Pre-signed URL
    L1-->>SPA: Return URL + JobId
    SPA->>S3: Upload Video

    S3-->>L2: S3 Event Trigger
    L2->>SF: Start Execution

    SF->>AI: Start Transcription + Text Detection
    AI-->>SF: Results
    SF->>GD: Upload Final Text File
    GD-->>SF: Success
```

---

# 📝 Backend Processing Summary

* S3 receives the uploaded video
* Lambda triggers Step Functions
* Step Functions orchestrates:

  * Amazon Transcribe
  * Amazon Rekognition
* Results merged and uploaded to Google Drive

This architecture is entirely serverless, scalable, and cost-efficient.

---

# 📜 License

GNU 

---

# 💬 Contact

**Lead Developer / Architect:** Roger Wamba
