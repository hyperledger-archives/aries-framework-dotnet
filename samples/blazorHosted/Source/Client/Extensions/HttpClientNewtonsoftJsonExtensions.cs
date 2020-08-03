//#nullable enable
//namespace Hyperledger.Aries.AspNetCore.Extensions
//{
//  using System;
//  using System.Collections.Generic;
//  using System.Linq;
//  using System.Net.Http;
//  using System.Text;
//  using System.Threading.Tasks;
//  using Newtonsoft.Json;
  
//  public static partial class HttpClientNewtonsoftJsonExtensions
//  {
//    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
//    {
//      if (client == null)
//      {
//        throw new ArgumentNullException(nameof(client));
//      }

//      JsonContent content = JsonContent.Create(value, mediaType: null, options);
//      return client.PostAsync(requestUri, content, cancellationToken);
//    }

//    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
//    {
//      if (client == null)
//      {
//        throw new ArgumentNullException(nameof(client));
//      }

//      JsonContent content = JsonContent.Create(value, mediaType: null, options);
//      return client.PostAsync(requestUri, content, cancellationToken);
//    }

//    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, string? requestUri, TValue value, CancellationToken cancellationToken)
//        => client.PostAsJsonAsync(requestUri, value, options: null, cancellationToken);

//    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, CancellationToken cancellationToken)
//        => client.PostAsJsonAsync(requestUri, value, options: null, cancellationToken);
//  }

//}
