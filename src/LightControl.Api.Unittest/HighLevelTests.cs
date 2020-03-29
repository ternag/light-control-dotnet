﻿using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Xunit.Abstractions;

namespace LightControl.Api.UnitTest
{
  public class HighLevelTests
  {
    private readonly ITestOutputHelper _outputHelper;
    private readonly HttpClient _client;

    public HighLevelTests(ITestOutputHelper outputHelper)
    {
      _outputHelper = outputHelper;
      var server = new TestServer(new WebHostBuilder()
        .UseEnvironment("Development")
        .UseStartup<Startup>());
      _client = server.CreateClient();
    }

    [Fact]
    public async Task GivenATestServe_RootShouldReturnGreenState()
    {
      var response = await _client.GetAsync("/");
      JsonDocument json = await GetJsonFromContent(response);
      var actual = json.RootElement.GetProperty("state").GetString();
      actual.ToLower().Should().Be("green");
    }

    [Fact]
    public async Task FlickShouldRetureLedObject()
    {
      // Act
      var response = await _client.GetAsync("/api/led/3/_flick");
      
      // Assert
      //response.EnsureSuccessStatusCode();
      JsonDocument json = await GetJsonFromContent(response);

      JsonElement id = json.RootElement.GetProperty("id");
      JsonElement state = json.RootElement.GetProperty("state");
      id.GetInt32().Should().Be(3);
      state.GetInt32().Should().Be(1);
    }

    private async Task<JsonDocument> GetJsonFromContent(HttpResponseMessage response)
    {
      var responseString = await response.Content.ReadAsStringAsync();
      _outputHelper.WriteLine(responseString);
      return JsonDocument.Parse(responseString);
    }
  }
}