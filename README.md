# OpenTelemetrySample with .NET Core 3.1
This is a demo project to show how to use OpenTelemetry for tracing on .NET Core 3.1

# Demo scenaio
1. Website
2. WebAPI
3. Redis

![image](https://user-images.githubusercontent.com/16613047/146793401-a903f5c1-75c6-456b-b9a4-65019d2fa4a7.png)

## Step 0 : Init application
docker run -d -p 9411:9411 openzipkin/zipkin


## Step 1 : Run Website with as below 
https://localhost:44338/api/WeatherForecast

![image](https://user-images.githubusercontent.com/16613047/146460984-4421bd47-7804-4ec2-b781-58a1aea0ef0c.png)


## Step 2 : Open zipkin url as below
http://localhost:9411/

你可以依據剛剛的請求，來看在 zipkin 中是如何呈現 trace 的內容
![image](https://user-images.githubusercontent.com/16613047/146461320-555ae8e6-8d71-416d-a7fb-1cb763909c7c.png)
