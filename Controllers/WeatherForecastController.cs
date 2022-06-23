using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace dotnet_redis.Controllers {
  [ApiController]
  [Route ("api/[controller]")]
  public class WeatherForecastController : ControllerBase {
    private readonly IDistributedCache _cache;

    public WeatherForecastController (IDistributedCache cache) {
      _cache = cache;
    }

    [HttpGet ("GetStringAsync")]
    public async Task<IActionResult> GetStringAsync () {
      var result = await _cache.GetStringAsync ("abc");

      if (result is null) {
        return NoContent ();
      }

      var response = JsonSerializer.Deserialize<WeatherForecast> (result);
      return Ok (response);

    }

    [HttpPost ("SetStringAsync")]
    public async Task<IActionResult> SetStringAsync () {
      var key = "abc";
      var obj = new WeatherForecast { Date = DateTime.Now, TemperatureC = 1, Summary = "ABC ทดสอบ" };
      var data = JsonSerializer.Serialize (obj);
      var options = new DistributedCacheEntryOptions ();
      options.SetSlidingExpiration (TimeSpan.FromSeconds (10));

      Console.WriteLine ($"SetStringAsync Byte Count====> {data.Length}");
      await _cache.SetStringAsync (key, data, options);
      return Created (String.Empty, obj);
    }

    [HttpPost ("SetAsync")]
    public async Task<IActionResult> SetAsync () {
      var key = "abc";
      var obj = new WeatherForecast { Date = DateTime.Now, TemperatureC = 1, Summary = "ABC ทดสอบ <script>alert(1)</script>" };
      // var data = JsonSerializer.Serialize (obj);
      // var byteData = Encoding.UTF8.GetBytes (data);
      var byteData = JsonSerializer.SerializeToUtf8Bytes (obj);
      var options = new DistributedCacheEntryOptions ();
      options.SetSlidingExpiration (TimeSpan.FromSeconds (10));

      Console.WriteLine ($"SetAsync Byte Count====> {byteData.Length}");
      await _cache.SetAsync (key, byteData, options);
      return Created (String.Empty, obj);

    }

    [HttpGet ("GetAsync")]
    public async Task<IActionResult> GetAsync () {
      var result = await _cache.GetAsync ("abc");

      if (result is null) {
        return NoContent ();
      }

      var jsonString = Encoding.UTF8.GetString (result);
      Console.WriteLine ($"abc ===> {jsonString}");
      var response = JsonSerializer.Deserialize<WeatherForecast> (jsonString);
      return Ok (response);

    }

    [HttpDelete ("RemoveAsync")]
    public async Task<IActionResult> RemoveAsync () {
      await _cache.RemoveAsync ("abc");
      return NoContent ();
    }

  }
}