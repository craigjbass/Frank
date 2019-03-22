using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using FluentAssertions;
using Frank.API.WebDevelopers;
using NUnit.Framework;
using RestSharp;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace Frank.EndToEndTests
{
    public class FrankTest
    {
        private IWebApplicationBuilder _builder;
        private IWebApplication _webApplication;
        private IRestResponse _response;

        [SetUp]
        public void SetUp()
        {
            
            _builder = System.Frank.Configure();
            _builder.ListenOn("127.0.0.1", "8019");
        }

        [TearDown]
        public void TearDown()
        {
            StopFrank();
        }

        private void StartFrank()
        {
            _webApplication = _builder.Build();
            _webApplication.Start();
        }

        private void StopFrank()
        {
            _webApplication.Stop();
        }

        private void MakeGetRequest(string path)
        {
            var request = new RestRequest(path, Method.GET);
            var client = new RestClient("http://127.0.0.1:8019/");

            _response = client.Execute(request);
        }

        private IRestResponse TheResponse()
        {
            if (_response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new Exception(
                    Encoding.UTF8.GetString(_response.RawBytes)
                );
            }

            return _response;
        }

        [Test]
        public void CanServeNotFound()
        {
            StartFrank();

            MakeGetRequest("/");
            TheResponse().StatusCode.Should().Be(404);
        }

        [Test]
        public void CanServeNotFoundTwice()
        {
            StartFrank();

            MakeGetRequest("/");
            TheResponse().StatusCode.Should().Be(404);

            MakeGetRequest("/another-route");
            TheResponse().StatusCode.Should().Be(404);
        }

        [Test]
        public void CanServeOk()
        {
            _builder.WithRoutes(router => { router.Get("/success", Ok); });
            StartFrank();

            MakeGetRequest("/success");
            TheResponse().StatusCode.Should().Be(200);
        }

        [Test]
        public void CanServeOk2()
        {
            _builder.WithRoutes(router => { router.Get("/okay", Ok); });
            StartFrank();

            MakeGetRequest("/okay");
            TheResponse().StatusCode.Should().Be(200);
        }
        
        
        [Test]
        public void CanServeANoContentStatusCode()
        {
            _builder.WithRoutes(router => { router.Get("/no-content", Created); });
            StartFrank();

            MakeGetRequest("/no-content");
            TheResponse().StatusCode.Should().Be(201);
        }
    }
}