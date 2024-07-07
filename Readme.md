# NexusGPT
此專案為串接GPT API的後端專案，提供前端串接GPT API的功能，並能儲存對話、紀錄GPT Token、儲存照片。

## 使用技術
- Event Sourcing
- Domain Driven Design
- Clean Architecture
- Hexagonal Architecture
- Unit Test
- SignalR
- AWS S3
- Entity Framework Core From Aggregate Root
- Code First

## 功能介紹
### 聊天室:
- 建立聊天室
- 變更聊天室名稱
- 刪除聊天室
- 取得聊天室列表
- 取得聊天室內容
- 上傳聊天室
- 分享聊天室
- 搜尋聊天室

### 聊天室訊息:
- 支援一般對話
- 支援圖片訊息


## 建立Table
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project NexusGPT.Adapter.Out --startup-project NexusGPT.WebApplication
dotnet ef database update --project NexusGPT.Adapter.Out --startup-project NexusGPT.WebApplication
```
## Docker
### Build
```bash
docker build -f  NexusGPT.WebApplication/Dockerfile -t nexusgpt:0.0.1 .
```

### Run
#### Use AWS S3
```bash
docker run -d -p 8080:80 --name nexusgpt \
  -e  ConnectionString="Your ConnectionString" \
  -e  AWS_ACCESSKEY="Your AWSAccessKey" \
  -e  AWS_SECRET="Your AWSSecretKey" \
  -e  AWS_REGION="Your AWSRegion" \
  -e  AWS_BUCKET="Your AWSBucketName" \
  -e  OPENAI_API_KEY="Your OpenAI API Key" \
  nexusgpt:0.0.1
```

#### Use Local File
```bash
docker run -d -p 8080:80 --name nexusgpt \
  -e  ConnectionString="Your ConnectionString" \
  -e  OPENAI_API_KEY="Your OpenAI API Key" \
  -v  /path/to/your/local/file:/app/Images \
  nexusgpt:0.0.1
```

## 未來更新
- 支援AWS DynamoDB
- 支援MongoDB
- 加上使用者驗證
- 加上OpenTelemetry