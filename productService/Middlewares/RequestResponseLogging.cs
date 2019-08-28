using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using productService.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace productService.Middlewares
{
    public class RequestResponseLogging
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestResponseLogging> logger;
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        private const int ReadChunkBufferLength = 4096;

        public RequestResponseLogging(RequestDelegate next, ILogger<RequestResponseLogging> logger)
        {
            this.next = next;
            this.logger = logger;
            this.recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            var request = await FormatRequest(context.Request);
            var response = "";

            logger.LogInformation(Utils.preLog(context) + request);

            var originalBodyStream = context.Response.Body;

            using (var responseBody = recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = responseBody;

                await next(context);

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                responseBody.Seek(0, SeekOrigin.Begin);

                response = await FormatResponse(context.Response, responseBody);
            }
            logger.LogInformation(Utils.preLog(context, true) + response);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var headers = FormatHeaders(request.Headers);

            var preStr = $"{request.Method} {request.Scheme}://{request.Host}{request.Path}{request.QueryString}  Header: {headers} ";

            if (request.HasFormContentType)
            {
                return preStr;
            }

            var body = "";

            request.EnableBuffering();
            request.EnableRewind();

            using (var stream = recyclableMemoryStreamManager.GetStream())
            {
                await request.Body.CopyToAsync(stream);
                request.Body.Seek(0, SeekOrigin.Begin);

                body = await ReadStreamInChunksAsync(stream);
            }

            return preStr + $"Body: {body}";
        }

        private async Task<string> FormatResponse(HttpResponse response, Stream stream)
        {
            var headers = FormatHeaders(response.Headers);
            string body = await ReadStreamInChunksAsync(stream);

            return $"HttpCode: {response.StatusCode}  Header: {headers} Body: {body}";
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("[");
            foreach (var (key, value) in headers)
            {
                stringBuilder.Append($"{key}: {value}; ");
            }
            stringBuilder.AppendLine("]");

            return stringBuilder.ToString();
        }

        private static async Task<string> ReadStreamInChunksAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;

            using (var stringWriter = new StringWriter())
            using (var streamReader = new StreamReader(stream))
            {
                var readChunk = new char[ReadChunkBufferLength];
                int readChunkLength;

                do
                {
                    readChunkLength = await streamReader.ReadBlockAsync(readChunk, 0, ReadChunkBufferLength);
                    await stringWriter.WriteAsync(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                result = "[" + Environment.NewLine + stringWriter.ToString() + Environment.NewLine + "]";
            }

            return result;
        }
    }
}
